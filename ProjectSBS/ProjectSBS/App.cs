using ProjectSBS.Services.Storage;
using ProjectSBS.Services.Storage.Data;
using ProjectSBS.Services.User;
using ProjectSBS.Services.Notifications;
using ProjectSBS.Services.Interop;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.Items.Billing;
using ProjectSBS.Services.Analytics;
#if WINDOWS
using Windows.Foundation.Collections;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.UI.Composition.SystemBackdrops;
using ProjectSBS.Infrastructure.Helpers;
using WinUIEx;
#endif

namespace ProjectSBS;

public class App : Application
{
    public Window? MainWindow { get; private set; }
    public IHost? Host { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Warning);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layouter specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// RemoteControl and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                // Enable localization (see appsettings.json for supported languages)
                .UseLocalization()
                // Register Json serializers (ISerializer and ISerializer)
                .UseSerialization((context, services) => services
                    .AddContentSerializer(context))
                .UseHttp((context, services) => services
                        // Register HttpClient
#if DEBUG
                        // DelegatingHandler will be automatically injected into Refit Client
                        .AddTransient<DelegatingHandler, DebugHttpHandler>()
#endif
                        .AddSingleton<ICurrencyCache, CurrencyCache>()
                        .AddRefitClient<IApiClient>(context))
#if !__IOS__
                .UseAuthentication(auth =>
                    auth.AddMsal(name: "MsalAuthentication")
                )
#endif
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IStorageService, StorageService>();
                    services.AddSingleton<IDataService, DataService>();
                    services.AddSingleton<IUserService, MsalUser>();
                    services.AddSingleton<IItemService, ItemService>();
                    services.AddSingleton<IBillingService, BillingService>();
                    services.AddSingleton<IInteropService, InteropService>();
                    services.AddSingleton<IAnalyticsService, AnalyticsService>();

#if __ANDROID__
                    services.AddSingleton<INotificationService, AndroidNotificationService>();
#elif __IOS__
                    services.AddSingleton<INotificationService, IOSNotificationService>();
#elif __WASM__ || HAS_UNO_SKIA
                    services.AddSingleton<INotificationService, WebNotificationService>();
#elif !HAS_UNO
                    services.AddSingleton<INotificationService, WindowsNotificationService>();
#endif
                })
                .UseNavigation(RegisterRoutes)
            );

#if WINDOWS
        var manager = WindowManager.Get(builder.Window);

        manager.MinWidth = 500;
        manager.MinHeight = 500;

        var size = builder.Window.AppWindow.Size;

        builder.Window.SetWindowSize(size.Width / 1.55, size.Height / 1.1);
        builder.Window.CenterOnScreen();
        builder.Window.Title = "Project SBS";
        //builder.Window.SetIcon();

        ToastNotificationManagerCompat.OnActivated += toastArgs =>
        {
            ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
            ValueSet userInput = toastArgs.UserInput;

            //TODO work with args
            if (args.TryGetValue("action", out var action))
            {
                if (action == "openItem")
                {
                    var itemId = args["itemId"];

                    if (itemId != null)
                    {
                        var shell = builder.Window.Content;

                        if (shell != null)
                        {
                            //shell.NavigateToItem(itemId);
                        }
                    }
                }
            }
        };
#endif

        MainWindow = builder.Window;

        Host = await builder.NavigateAsync<Shell>(initialNavigate:
            async (services, navigator) =>
            {
                var auth = services.GetRequiredService<IAuthenticationService>();
                var authenticated = await auth.RefreshAsync();
                if (authenticated)
                {
                    await navigator.NavigateViewModelAsync<MainViewModel>(this, qualifier: Qualifiers.Nested);
                }
                else
                {
                    await navigator.NavigateViewModelAsync<LoginViewModel>(this, qualifier: Qualifiers.Nested);
                }
            });

#if WINDOWS
        if (MicaController.IsSupported())
        {
            MainWindow.SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.Base };
        }

        Win32.SetTitleBackgroundTransparent(MainWindow);

        //TODO Log MicaController.IsSupported()
#endif
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellViewModel)),
            new ViewMap<LoginPage, LoginViewModel>(),
            new ViewMap<MainPage, MainViewModel>(),
            new ViewMap<ItemDetailsPage, ItemDetailsViewModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
                Nested: new RouteMap[]
                {
                new RouteMap("Login", View: views.FindByViewModel<LoginViewModel>()),
                new RouteMap("Main", View: views.FindByViewModel<MainViewModel>()),
                new RouteMap("Details", View: views.FindByViewModel<ItemDetailsViewModel>()),
                }
            )
        );
    }
}