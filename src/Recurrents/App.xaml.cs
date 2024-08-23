using Uno.Resizetizer;


#if WINDOWS
using Microsoft.UI.Composition.SystemBackdrops;
using WinUIEx;
#endif

namespace Recurrents;
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    public static IServiceProvider? Services => (Current as App)!.Host?.Services;

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)
                        .CoreLogLevel(LogLevel.Warning);
                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder.EmbeddedSource<App>().Section<AppConfig>()
                )
                .UseLocalization()
                .UseSerialization((context, services) => services
                    .AddContentSerializer(context)
                    .AddJsonTypeInfo(WeatherForecastContext.Default.IImmutableListWeatherForecast))
                .UseHttp((context, services) => services
                    .AddSingleton<IWeatherCache, WeatherCache>()
                    .AddRefitClient<IApiClient>(context))
                .ConfigureServices((context, services) =>
                {
                    // Register your services here
                    // services.AddSingleton<IMyService, MyService>();
                })
                .UseNavigation(RegisterRoutes)
            );

        // Assign the Host after building
        Host = builder.Build();

        // Ensure that Services is now accessible
        if (Services == null)
        {
            throw new InvalidOperationException("Services could not be initialized.");
        }

        MainWindow = builder.Window;

#if WINDOWS
        var manager = WindowManager.Get(builder.Window);
        manager.MinWidth = 500;
        manager.MinHeight = 500;

        var size = builder.Window.AppWindow.Size;
        manager.PersistenceId = "RecurrentsMainWindow";
        builder.Window.SetWindowSize(size.Width / 1.55, size.Height / 1.1);
        builder.Window.CenterOnScreen();
        builder.Window.Title = Package.Current.DisplayName;

        if (MicaController.IsSupported())
        {
            MainWindow.SystemBackdrop = new MicaBackdrop() { Kind = MicaKind.Base };
        }
#endif

#if DEBUG
        MainWindow.EnableHotReload();
#endif
        MainWindow.SetWindowIcon();

        await builder.NavigateAsync<Shell>();
    }


    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellViewModel)),
            new DataViewMap<MainPage, MainViewModel, User>(),
            new ViewMap<LoginPage, LoginViewModel>(),
            new ViewMap<HomePage, HomeViewModel>(),
            new ViewMap<ItemDetails, ItemDetailsViewModel>(),
            new ViewMap<SettingsPage, SettingsViewModel>(),
            new DataViewMap<SecondPage, SecondViewModel, Entity>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
                Nested:
                [
                    new ("Main", View: views.FindByViewModel<MainViewModel>()),
                    new ("Login", View: views.FindByViewModel<LoginViewModel>(), IsDefault: true,
                        Nested:
                        [
                            new ("Second", View: views.FindByViewModel<SecondViewModel>()),
                            new ("Home", View: views.FindByViewModel<HomeViewModel>()),
                            new ("Items", View: views.FindByViewModel<ItemDetailsViewModel>()),
                            new ("Settings", View: views.FindByViewModel<SettingsViewModel>())
                        ]
                    ),
                ]
            )
        );
    }
}