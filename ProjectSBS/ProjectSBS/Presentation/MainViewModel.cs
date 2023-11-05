namespace ProjectSBS.Presentation;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IUserService _userService;
    private readonly IDispatcher _dispatcher;
    private readonly IItemService _itemService;
    private readonly IItemFilterService _filterService;

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private Type? _pageType;

    [ObservableProperty]
    private bool _isSignedIn;

    public string? Title { get; }

    public FilterCategory SelectedCategory
    {
        get => _filterService.SelectedCategory;
        set
        {
            if (_filterService.SelectedCategory == value)
            {
                return;
            }

            //if (PageType != typeof(HomePage))
            //{
            //    PageType = typeof(HomePage);
            //}

            _filterService.SelectedCategory = value;

            WeakReferenceMessenger.Default.Send(new CategorySelectionChanged());

            OnPropertyChanged();
        }
    }

    public List<FilterCategory> Categories { get; }

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
        IDispatcher dispatcher)
    {
#if !__IOS__
        _authentication = authentication;
#endif
        _userService = userService;
        _filterService = filterService;
        _itemService = itemService;
        _dispatcher = dispatcher;

        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        GoToSettingsCommand = new RelayCommand(GoToSettings);
        LogoutCommand = new AsyncRelayCommand(DoLogout);

        Categories = filterService.Categories;

        WeakReferenceMessenger.Default.Register<CategorySelectionChanged>(this, (r, m) =>
        {
            OnPropertyChanged(nameof(SelectedCategory));
        });
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

        if (PageType != typeof(HomePage))
        {
            PageType = typeof(HomePage);
        }
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
        PageType = typeof(SettingsPage);
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