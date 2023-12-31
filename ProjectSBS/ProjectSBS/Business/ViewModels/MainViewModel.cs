namespace ProjectSBS.Business.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region Services
    private readonly IUserService _userService;
    private readonly IStringLocalizer _localizer;
    private readonly IItemService _itemService;
    private readonly INavigation _navigation;
    private readonly ICurrencyCache _currency;
    #endregion

    #region Localization Strings
    public string ClickToLoginText => _localizer["ClickToLogin"];
    public string SettingsText => _localizer["Settings"];
    public string LogoutText => _localizer["Logout"];
    #endregion

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private bool _isMobileNavigationVisible;

    [ObservableProperty]
    private Type? _pageType;

    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private MenuFlyout _menuFlyout;

    [ObservableProperty]
    private FrameworkElement _userButton;

    public NavigationCategory SelectedCategory
    {
        set
        {
            _navigation.SelectedCategory = value;
            OnPropertyChanged();
        }
        get => _navigation.SelectedCategory;
    }

    public string? Title { get; }

    public IEnumerable<NavigationCategory> DesktopCategories;

    public ICommand GoToSettings { get; }
    public ICommand Logout { get; }
    public ICommand Login { get; }

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        IUserService userService,
        IItemService itemService,
        ICurrencyCache currency,
        INavigation navigation)
    {
        _userService = userService;
        _navigation = navigation;
        _localizer = localizer;
        _itemService = itemService;
        _currency = currency;

        Title = $"{localizer["ApplicationName"]}";
#if DEBUG
        Title += $" (Dev)";
#endif

        DesktopCategories = _navigation.Categories.Where(c => c.Visibility == CategoryVisibility.Desktop || c.Visibility == CategoryVisibility.Both);

        _userService.OnLoggedInChanged += (s, e) =>
        {
            User = e;
            IsLoggedIn = e is { };
        };

        GoToSettings = new RelayCommand(() => navigation.NavigateNested(typeof(SettingsPage)));

        Logout = new RelayCommand(() =>
        {
            userService.Logout();
            navigation.Navigate(typeof(LoginPage));
        });

        Login = new RelayCommand(() =>
        {
            if (IsLoggedIn)
            {
                //TODO There is a bug in the MenuFlyout 
                //MenuFlyout.ShowAttachedFlyout(UserButton);
                return;
            }

            navigation.Navigate(typeof(LoginPage));

            _itemService.ClearItems();
        });
    }

    public async override void Load()
    {
        _navigation.NavigateNested(SelectedCategory.Page);

        User = await _userService.RetrieveUser();
        IsLoggedIn = User is { };

        _ = await _currency.GetCurrency(CancellationToken.None);

        _ = _itemService.InitializeAsync();
    }

    public void Navigate(NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            if (PageType != typeof(SettingsPage))
            {
                GoToSettings.Execute(null);
                return;
            }
        }

        _navigation.NavigateNested((args.SelectedItem as NavigationCategory)?.Page ?? SelectedCategory.Page);
    }

    public override void Unload()
    {

    }
}
