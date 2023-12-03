namespace ProjectSBS.Business.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ICurrencyCache _currencyCache;
    private readonly ISettingsService _settingsService;
    private readonly IItemService _itemService;
    private readonly IUserService _userService;

    #region Localization Strings
    public string TitleText { get; init; }
    public string DefaultCurrencyText { get; init; }
    public string GoToSettingsText { get; init; }
    public string LogoutText { get; init; }
    public string NotLoggedInText { get; init; }
    public string NotLoggedInDescription { get; init; }
    public string SystemNotificationsText { get; init; }
    public string NotificationTimeText { get; init; }
    public string NotificationsDisabledText { get; init; }
    public string NotificationsDisabledDescription { get; init; }
    public string AppName { get; init; }
    public string AppVersion { get; init; }
    public string NotificationsText { get; init; }
    public string AboutText { get; init; }
    public string AboutDescription { get; init; }
    public string GitHubText { get; init; }
    public string RateAndReviewText { get; init; }
    public string PrivacyPolicyText { get; init; }
    public string ReportABugText { get; init; }
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
    IStringLocalizer localization,
    ICurrencyCache currencyCache,
    ISettingsService settingsService,
    INavigation navigation,
    INotificationService notificationService,
    IItemService itemService,
    IInteropService interopService,
    IUserService userService)
    {
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
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/project-sbs/")));
        RateAndReview = new AsyncRelayCommand(interopService.OpenStoreReviewUrlAsync);
        //TODO: Change the link to proper Privacy Policy
        PrivacyPolicy = new AsyncRelayCommand(async () =>
            await Windows.System.Launcher.LaunchUriAsync(new Uri("")));
        ReportABug = new AsyncRelayCommand(async () =>
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/project-sbs/issues/new")));

        TitleText = localization["Settings"];
        DefaultCurrencyText = localization["DefaultCurrency"];
        LogoutText = localization["Logout"];
        GoToSettingsText = localization["GoToSettings"];
        NotLoggedInText = localization["NotLoggedIn"];
        NotLoggedInDescription = localization["NotLoggedInDescription"];
        SystemNotificationsText = localization["SystemNotifications"];
        NotificationTimeText = localization["NotificationTime"];
        NotificationsDisabledText = localization["NotificationsDisabled"];
        NotificationsDisabledDescription = localization["NotificationsDisabledDescription"];
        AppName = localization["ApplicationName"];
        NotificationsText = localization["Notifications"];
        AboutText = localization["About"];
        AboutDescription = localization["AboutDescription"];
        GitHubText = localization["GitHub"];
        RateAndReviewText = localization["RateAndReview"];
        PrivacyPolicyText = localization["PrivacyPolicy"];
        ReportABugText = localization["ReportABug"];

        IsNotificationsEnabled = notificationService.IsEnabledOnDevice();

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

        SelectedCurrency = _settingsService.DefaultCurrency;
        NotificationTime = _settingsService.NotificationTime;
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
