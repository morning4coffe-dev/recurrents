namespace ProjectSBS.Business.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region Services
    private readonly IUserService _userService;
    private readonly IStringLocalizer _localizer;
    private readonly IItemService _itemService;
    public readonly INavigation _navigation;
    private readonly ICurrencyCache _currency;
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
    private bool _indicateLoading;

    public IEnumerable<NavigationCategory> Categories
        => _navigation.Categories;

    private NavigationCategory _selectedCategory;
    public NavigationCategory SelectedCategory
    {
        set
        {
            if (_selectedCategory != value)
            {
                _selectedCategory = value;

                _navigation.NavigateCategory(value);
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }
        get => _selectedCategory;
    }

    public string? Title { get; }

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

        _userService.OnLoggedInChanged += (s, e) =>
        {
            User = e;
            IsLoggedIn = e is { };
        };
    }

    public async override void Load()
    {
        _navigation.CategoryChanged += Navigation_CategoryChanged;
        _navigation.NavigateCategory(_navigation.Categories[0]);

        IndicateLoading = true;

        User = await _userService.RetrieveUser();
        IsLoggedIn = User is { };

        _ = await _currency.GetCurrency(CancellationToken.None);
        _ = Task.Run(() => _currency.GetCurrency(CancellationToken.None));

        await _itemService.InitializeAsync();
        IndicateLoading = false;

        await _itemService.InitializeAsync();

        IndicateLoading = false;
    }

    public override void Unload()
    {
        _navigation.CategoryChanged -= Navigation_CategoryChanged;
    }

    private void Navigation_CategoryChanged(object? sender, NavigationCategory e) 
        => SelectedCategory = e;

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

        _navigation.NavigateCategory((args.SelectedItem as NavigationCategory) ?? SelectedCategory);
    }

    [RelayCommand]
    private void GoToSettings()
        => _navigation.NavigateCategory(_navigation.Categories.FirstOrDefault(category => category.Id == 5)
                    ?? throw new($"Settings category wasn't found in the Categories list on {this}."));

    [RelayCommand]
    private void Login()
    {
        _navigation.Navigate(typeof(LoginPage));
        _itemService.ClearItems();
    }
}
