namespace ProjectSBS.Business.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ICurrencyCache _currencyCache;
    private readonly ISettingsService _settingsService;
    private readonly IUserService _userService;
    public string TitleText { get; init; }
    public string DefaultCurrencyText { get; init; }
    public string AppName { get; init; }
    public string AppVersion { get; init; }
    public string NotificationsText { get; init; }
    public string AboutText { get; init; }

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    public bool _isLoggedIn;

    public int SelectedCurrency
    {
        set => _settingsService.DefaultCurrency = Currencies[value];
        get
        {
            for (int i = 0; i < Currencies.Count; i++)
            {
                if (Currencies[i] == _settingsService.DefaultCurrency)
                {
                    return i;
                }

            }
            return 0;
        }
    }

    public ObservableCollection<string> Currencies { get; } = [];


    public ICommand Logout { get; }
    public ICommand Login { get; }
    public SettingsViewModel(
    IStringLocalizer localization,
    ICurrencyCache currencyCache,
    ISettingsService settingsService,
    INavigation navigation,
    IUserService userService)
    {
        _userService = userService;
        _currencyCache = currencyCache;
        _settingsService = settingsService;

        _userService.OnLoggedInChanged += (s, e) =>
        {
            User = e;
            IsLoggedIn = e is { };
        };

        Logout = new RelayCommand(() =>
        {
            userService.Logout();
            navigation.Navigate(typeof(LoginPage));
        });

        Login = new RelayCommand(() => navigation.Navigate(typeof(LoginPage)));

        TitleText = localization["Settings"];
        DefaultCurrencyText = localization["DefaultCurrency"];
        AppName = localization["ApplicationName"];
        NotificationsText = localization["Notifications"];
        AboutText = localization["About"];

        Package package = Package.Current;
        PackageId packageId = package.Id;
        PackageVersion version = packageId.Version;
        AppVersion = string.Format("{0}: {1}.{2}.{3}.{4}", localization["Version"], version.Major, version.Minor, version.Build, version.Revision);
    }

    public override async void Load()
    {
        User = await _userService.GetUser();
        IsLoggedIn = User is { };

        var currency = await _currencyCache.GetCurrency(CancellationToken.None);

        if (currency?.Rates.Count == 0)
        {
            return;
        }

        //TODO sets the value on the open of the Settings
        Currencies.AddRange(currency?.Rates.Keys);
        Currencies.Add(currency?.BaseCurrency ?? "EUR");
    }

    [RelayCommand]
    public void LaunchNotificationSettings()
    {
        _ = Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));
    }

    public override void Unload()
    {
        //throw new NotImplementedException();
    }
}
