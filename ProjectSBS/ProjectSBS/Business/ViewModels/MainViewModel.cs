using Windows.Globalization;

namespace ProjectSBS.Business.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly IItemService _itemService;
    private readonly INavigation _navigation;

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
        INavigation navigation)
    {
        _userService = userService;
        _navigation = navigation;
        _itemService = itemService;

        Title = $"{localizer["ApplicationName"]}";
#if DEBUG
        Title += $" (Dev)";

        //TODO Don't use static CultureInfo
        CultureInfo ci = new("cs-CZ");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;
        ApplicationLanguages.PrimaryLanguageOverride = "cs-CZ";
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
                MenuFlyout.ShowAttachedFlyout(UserButton);
                return;
            }

            navigation.Navigate(typeof(LoginPage));

            _itemService.ClearItems();
        });
    }

    public async override void Load()
    {
        _navigation.NavigateNested(SelectedCategory.Page);

        User = await _userService.GetUser();
        IsLoggedIn = User is { };

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
