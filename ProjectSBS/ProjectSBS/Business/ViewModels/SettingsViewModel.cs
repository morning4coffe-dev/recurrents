using WS = Windows.System;

namespace ProjectSBS.Business.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ICurrencyCache _currencyCache;
    private readonly ISettingsService _settingsService;
    private readonly IItemService _itemService;
    private readonly IUserService _userService;
    private readonly IStringLocalizer _localizer;
    private readonly INavigation _navigation;
    private readonly IInteropService _interopService;

    #region Localization Strings
    public string TitleText => _localizer["Settings"];
    public string DefaultCurrencyText => _localizer["DefaultCurrency"];
    public string GoToSettingsText => _localizer["GoToSettings"];
    public string LogoutText => _localizer["Logout"];
    public string NotLoggedInText => _localizer["NotLoggedIn"];
    public string NotLoggedInDescription => _localizer["NotLoggedInDescription"];
    public string SystemLanguageAndRegionText => _localizer["SystemLanguageAndRegion"];
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

    private string _selectedCurrency = string.Empty;

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

    public SettingsViewModel(
        IStringLocalizer localizer,
        ICurrencyCache currencyCache,
        ISettingsService settingsService,
        INavigation navigation,
        INotificationService notificationService,
        IItemService itemService,
        IInteropService interopService,
        IUserService userService
    )
    {
        _localizer = localizer;
        _userService = userService;
        _currencyCache = currencyCache;
        _itemService = itemService;
        _settingsService = settingsService;
        _navigation = navigation;
        _interopService = interopService;

        _userService.OnLoggedInChanged += (s, e) =>
        {
            User = e;
            IsLoggedIn = e is { };
        };

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
    public async Task LaunchNotificationSettings() =>
        await WS.Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));

    [RelayCommand]
    public async Task LaunchLangSettings() =>
        await WS.Launcher.LaunchUriAsync(new Uri("ms-settings:regionlanguage-adddisplaylanguage"));

    public override void Unload()
    {
    }

    [RelayCommand]
    private void Login() =>
        _navigation.Navigate(typeof(LoginPage));

    [RelayCommand]
    private void Logout()
    {
        _userService.Logout();
        _navigation.Navigate(typeof(LoginPage));
    }

    [RelayCommand]
    private async Task OpenGithub() =>
        await WS.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents"));

    [RelayCommand]
    private async Task OpenRateAndReview() =>
        await _interopService.OpenStoreReviewUrlAsync();

    [RelayCommand]
    private async Task OpenPrivacyPolicy() =>
        await WS.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents/blob/ebf622cb65d60c7d353af69824f63d88fa796bde/privacy-policy.md"));

    [RelayCommand]
    private async Task OpenGithubIssues() =>
        await WS.Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents/issues/new"));
}
