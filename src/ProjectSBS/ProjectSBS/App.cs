using CommunityToolkit.WinUI.Notifications;
using Microsoft.UI.Composition.SystemBackdrops;
using ProjectSBS.Services.FileManagement.Data;
using ProjectSBS.Services.Interop;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.Items.Billing;
using ProjectSBS.Services.Notifications;
using ProjectSBS.Services.Storage;
using Windows.Foundation.Collections;
#if WINDOWS
using ProjectSBS.Infrastructure.Helpers;
using WinUIEx;
#endif

namespace ProjectSBS;

public class App : Application
{
    public Window? MainWindow { get; private set; }

    public IHost? Host { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var startTime = DateTime.Now;

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
#if DEBUG
                        // DelegatingHandler will be automatically injected into Refit Client
                        .AddTransient<DelegatingHandler, DebugHttpHandler>()
#endif
                        .AddSingleton<ICurrencyCache, CurrencyCache>()
                        .AddRefitClient<IApiClient>(context))
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IItemService, ItemService>();
                    services.AddSingleton<IStorageService, StorageService>();
                    services.AddSingleton<IDataService, DataService>();
                    services.AddSingleton<IBillingService, BillingService>();
                    services.AddSingleton<IInteropService, InteropService>();

#if __ANDROID__
                    services.AddSingleton<INotificationService, NotificationService>();
#elif __WASM__ || HAS_UNO_SKIA
                    services.AddSingleton<INotificationService, WebNotificationService>();
#elif !HAS_UNO
                    services.AddSingleton<INotificationService, WindowsNotificationService>();
#endif
                    //ViewModels
                    services.AddTransient<ShellViewModel>();
                })
                //.UseAuthentication(builder =>
                //{
                //    builder.AddMsal();
                //})
            );

#if WINDOWS
        var manager = WindowManager.Get(builder.Window);

        //manager.PersistenceId = "SBSMainWindowPersistanceId";
        manager.MinWidth = 500;
        manager.MinHeight = 500;

        var size = builder.Window.AppWindow.Size;

        builder.Window.CenterOnScreen(size.Width / 1.55, size.Height / 1.1);
        builder.Window.Title = "Project SBS";

        ToastNotificationManagerCompat.OnActivated += toastArgs =>
        {
            // Obtain the arguments from the notification
            ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

            // Obtain any user input (text boxes, menu selections) from the notification
            ValueSet userInput = toastArgs.UserInput;

            //TODO work with args
        };
#endif

        MainWindow = builder.Window;

        Host = builder.Build();

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;
        }

#if WINDOWS
        if (MicaController.IsSupported())
        {
            MainWindow.SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.Base };
        }

        Win32.SetTitleBackgroundTransparent(MainWindow);

        //TODO Log MicaController.IsSupported()
#endif

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(ShellPage), args.Arguments);
        }
        // Ensure the current window is active
        MainWindow.Activate();

        var completeStartTime = (DateTime.Now - startTime).TotalSeconds;
    }
}