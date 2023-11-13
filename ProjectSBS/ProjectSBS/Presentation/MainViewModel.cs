using System.Globalization;
using Windows.Globalization;

namespace ProjectSBS.Presentation;

public partial class MainViewModel : ViewModelBase
{
    //private readonly IAuthenticationService _authentication;
    private readonly IUserService _userService;
    private readonly IItemService _itemService;
    private readonly IItemFilterService _filterService;
    private readonly INavigation _navigation;

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private Type? _pageType;

    [ObservableProperty]
    private bool _isSignedIn;

    [ObservableProperty]
    public NavigationCategory _selectedCategory;

    public string? Title { get; }

    public IEnumerable<NavigationCategory> DesktopCategories => _navigation.Categories.Where(c => c.Visibility == CategoryVisibility.Desktop || c.Visibility == CategoryVisibility.Both);
    public IEnumerable<NavigationCategory> MobileCategories => _navigation.Categories.Where(c => c.Visibility == CategoryVisibility.Mobile || c.Visibility == CategoryVisibility.Both);

    public ICommand GoToSettingsCommand { get; }
    public ICommand LogoutCommand { get; }

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        IUserService userService,
        IItemService itemService,
        IItemFilterService filterService,
        INavigation navigation)
    {
        _userService = userService;
        _filterService = filterService;
        _navigation = navigation;
        _itemService = itemService;

        Title = $"{localizer["ApplicationName"]}";
#if DEBUG
        Title += $" - {appInfo?.Value?.Environment}";
#endif

        //TODO Don't use static CultureInfo
        CultureInfo ci = new("cs-CZ");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;
        ApplicationLanguages.PrimaryLanguageOverride = "cs-CZ";

        GoToSettingsCommand = new RelayCommand(GoToSettings);
        LogoutCommand = new AsyncRelayCommand(DoLogout);
    }

    public void Navigate(NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            if (PageType != typeof(SettingsPage))
            {
                GoToSettings();
                return;
            }
        }

        _navigation.NavigateNested(SelectedCategory.Page);
    }

    public async override void Load()
    {
        await _itemService.InitializeAsync();

        User = await _userService.GetUser();
        IsSignedIn = User is { };

        SelectedCategory = DesktopCategories.FirstOrDefault();
    }

    private void GoToSettings()
    {
        _navigation.NavigateNested(typeof(SettingsPage)); 
    }

    public async Task DoLogout(CancellationToken token)
    {
        //await _authentication.LogoutAsync(_dispatcher, token);
        //TODO probably will have to clean the token and User too
    }

    public override void Unload()
    {
        //throw new NotImplementedException();
    }
}