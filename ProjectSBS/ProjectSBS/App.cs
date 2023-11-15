using Microsoft.UI.Dispatching;
using ProjectSBS.Services.Analytics;
using ProjectSBS.Services.Interop;
using ProjectSBS.Services.Items.Billing;
using ProjectSBS.Services.Items.Tags;
using ProjectSBS.Services.Notifications;
using ProjectSBS.Services.Storage;
using ProjectSBS.Services.Storage.Data;
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
    private IHost? Host { get; set; }

    public static IServiceProvider? Services => (Current as App)!.Host?.Services;
    public static DispatcherQueue Dispatcher => (Current as App)!.MainWindow!.DispatcherQueue;

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
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
                    logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layouter specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    logBuilder.XamlBindingLogLevel(LogLevel.Debug);
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
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IStorageService, StorageService>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IDataService, DataService>();
                    services.AddSingleton<IUserService, MsalUser>();
                    services.AddSingleton<IItemService, ItemService>();
                    services.AddSingleton<IBillingService, BillingService>();
                    services.AddSingleton<IInteropService, InteropService>();
                    services.AddSingleton<IAnalyticsService, AnalyticsService>();
                    services.AddSingleton<IItemFilterService, ItemFilterService>();
                    services.AddSingleton<ITagService, TagService>();
                    services.AddSingleton<INavigation, NavigationService>();

                    services.AddTransient<MainViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<ItemDetailsViewModel>();
                    services.AddTransient<HomeViewModel>();
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<StatsBannerViewModel>();
                    //services.AddTransient<ConversionsViewModel>();

#if __ANDROID__
                    services.AddSingleton<INotificationService, AndroidNotificationService>();
#elif __IOS__
                    services.AddSingleton<INotificationService, IOSNotificationService>();
#elif HAS_UNO_WASM || HAS_UNO_SKIA
                    services.AddSingleton<INotificationService, WebNotificationService>();
#elif !HAS_UNO
                    services.AddSingleton<INotificationService, WindowsNotificationService>();
#endif
                })
            );

        MainWindow = builder.Window;

        Host = builder.Build();

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Set navigation context
            Services.GetRequiredService<INavigation>().RootFrame = rootFrame;

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;
        }

#if __IOS__ || __ANDROID__
        Uno.UI.FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
#endif

        if (rootFrame.Content is not { })
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(LoginPage), args.Arguments);
        }


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

        // Ensure the current window is active
        MainWindow.Activate();

#if WINDOWS
        if (MicaController.IsSupported())
        {
            MainWindow.SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.Base };
        }

        Win32.SetTitleBackgroundTransparent(MainWindow);

        //TODO Log MicaController.IsSupported()
#endif
    }
}