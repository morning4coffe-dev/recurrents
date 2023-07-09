using ProjectSBS.Services.Notifications;
using ProjectSBS.Services.Storage;

namespace ProjectSBS
{
    public class App : Application
    {
        protected Window? MainWindow { get; private set; }

        public IHost? Host { get; private set; }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
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
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton<IFileWriter, FileWriter>();

#if __ANDROID__
                        services.AddSingleton<INotificationService, NotificationService>();
#elif __WASM__
                        services.AddSingleton<INotificationService, WebNotificationService>();
#elif !HAS_UNO
                        services.AddSingleton<INotificationService, WindowsNotificationService>();
#else
                        services.AddSingleton<INotificationService, NotificationServiceBase>();
#endif
                        //ViewModels
                        services.AddTransient<ShellViewModel>();
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

                // Place the frame in the current Window
                MainWindow.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(ShellPage), args.Arguments);
            }
            // Ensure the current window is active
            MainWindow.Activate();
        }
    }
}