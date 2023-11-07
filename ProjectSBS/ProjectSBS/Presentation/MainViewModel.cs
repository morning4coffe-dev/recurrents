using System.Globalization;
using Windows.Globalization;

namespace ProjectSBS.Presentation;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IUserService _userService;
    private readonly IDispatcher _dispatcher;
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

    public string? Title { get; }

    public NavigationCategory SelectedCategory
    {
        get => _navigation.SelectedCategory;
        set
        {
            if (_navigation.SelectedCategory == value)
            {
                return;
            }

            _navigation.SelectedCategory = value;

            //TODO move this to only Filtering
            //WeakReferenceMessenger.Default.Send(new CategorySelectionChanged());

            OnPropertyChanged();
        }
    }

    public List<NavigationCategory> Categories { get; }

    public ICommand GoToSettingsCommand { get; }
    public ICommand LogoutCommand { get; }

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
#if !__IOS__
        IAuthenticationService authentication,
#endif
        IUserService userService,
        IItemService itemService,
        IItemFilterService filterService,
        INavigation navigation,
        IDispatcher dispatcher)
    {
#if !__IOS__
        _authentication = authentication;
#endif
        _userService = userService;
        _filterService = filterService;
        _navigation = navigation;
        _itemService = itemService;
        _dispatcher = dispatcher;

        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        //TODO Don't use static CultureInfo
        CultureInfo ci = new("cs-CZ");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;
        ApplicationLanguages.PrimaryLanguageOverride = "cs-CZ";

        GoToSettingsCommand = new RelayCommand(GoToSettings);
        LogoutCommand = new AsyncRelayCommand(DoLogout);

        Categories = navigation.Categories;
        _navigation.NavigateNested(SelectedCategory.Page);
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

        IsSignedIn = await _authentication.IsAuthenticated();

        //HomePage is currently initialized with Categories
    }

    private void GoToSettings()
    {
        _navigation.NavigateNested(typeof(SettingsPage)); 
    }

    public async Task DoLogout(CancellationToken token)
    {
        await _authentication.LogoutAsync(_dispatcher, token);
        //TODO probably will have to clean the token and User too
    }

    public override void Unload()
    {
        //throw new NotImplementedException();
    }
}