namespace ProjectSBS.Business.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ICurrencyCache _currencyCache;
    private readonly ISettingsService _settingsService;
    private readonly IItemService _itemService;
    private readonly IUserService _userService;
    private readonly IStringLocalizer _localizer;

    #region Localization Strings
    public string TitleText => _localizer["Settings"];
    public string DefaultCurrencyText => _localizer["DefaultCurrency"];
    public string GoToSettingsText => _localizer["GoToSettings"];
    public string LogoutText => _localizer["Logout"];
    public string NotLoggedInText => _localizer["NotLoggedIn"];
    public string NotLoggedInDescription => _localizer["NotLoggedInDescription"];
    public string SystemNotificationsText => _localizer["SystemNotifications"];
    public string NotificationTimeText => _localizer["NotificationTime"];
    public string NotificationsDisabledText => _localizer["NotificationsDisabled"];
    public string NotificationsDisabledDescription => _localizer["NotificationsDisabledDescription"];
    public string AppName => _localizer["ApplicationName"];
    public string AppVersion { get; init; }
    public string NotificationsText => _localizer["Notifications"];
    public string AboutText => _localizer["About"];
    public string AboutDescription => _localizer["AboutDescription"];
    public string GitHubText => _localizer["GitHub"];
    public string RateAndReviewText => _localizer["RateAndReview"];
    public string PrivacyPolicyText => _localizer["PrivacyPolicy"];
    public string ReportABugText => _localizer["ReportABug"];
    #endregion

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    public bool _isLoggedIn;

    public bool IsNotificationsEnabled { get; }

    private string _selectedCurrency;
    public string SelectedCurrency
    {
        get => _selectedCurrency;
        set
        {
            if (_selectedCurrency == value)
            {
                return;
            }

            _selectedCurrency = value;
            _settingsService.DefaultCurrency = value;

            OnPropertyChanged();
        }
    }

    private TimeOnly _notificationTime;
    public TimeOnly NotificationTime
    {
        get => _notificationTime;
        set
        {
            if (_notificationTime == value)
            {
                return;
            }

            _notificationTime = value;
            _settingsService.NotificationTime = value;

            _ = _itemService.GetItems().ForEach(item => item.ScheduleBilling());

            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> Currencies { get; } = [];

    public ICommand Logout { get; }
    public ICommand Login { get; }
    public ICommand GitHub { get; }
    public ICommand RateAndReview { get; }
    public ICommand PrivacyPolicy { get; }
    public ICommand ReportABug { get; }
    public SettingsViewModel(
    IStringLocalizer localizer,
    ICurrencyCache currencyCache,
    ISettingsService settingsService,
    INavigation navigation,
    INotificationService notificationService,
    IItemService itemService,
    IInteropService interopService,
    IUserService userService)
    {
        _localizer = localizer;
        _userService = userService;
        _currencyCache = currencyCache;
        _itemService = itemService;
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
        GitHub = new AsyncRelayCommand(async () =>
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents")));
        RateAndReview = new AsyncRelayCommand(interopService.OpenStoreReviewUrlAsync);
        PrivacyPolicy = new AsyncRelayCommand(async () =>
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents/blob/ebf622cb65d60c7d353af69824f63d88fa796bde/privacy-policy.md")));
        ReportABug = new AsyncRelayCommand(async () =>
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents/issues/new")));

        IsNotificationsEnabled = notificationService.IsEnabledOnDevice();

        Package package = Package.Current;
        PackageId packageId = package.Id;
        PackageVersion version = packageId.Version;
        AppVersion = string.Format("{0}: {1}.{2}.{3}.{4}", localizer["Version"], version.Major, version.Minor, version.Build, version.Revision);
    }

    public override async void Load()
    {
        User = await _userService.RetrieveUser();
        IsLoggedIn = User is { };

        var currency = await _currencyCache.GetCurrency(CancellationToken.None);

        if (currency?.Rates.Count == 0)
        {
            return;
        }

        Currencies.AddRange(currency?.Rates.Keys);

        SelectedCurrency = _settingsService.DefaultCurrency;
        NotificationTime = _settingsService.NotificationTime;
    }

    [RelayCommand]
    public void LaunchNotificationSettings() => _ = Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));

    public override void Unload()
    {
        
    }
}
