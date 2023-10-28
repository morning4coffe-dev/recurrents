using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using ProjectSBS.Presentation.NestedPages;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.Items.Filtering;
using ProjectSBS.Services.User;
using Uno.Extensions;

namespace ProjectSBS.Presentation;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IUserService _userService;
    private readonly IDispatcher _dispatcher;
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

            if (PageType != typeof(HomePage))
            {
                PageType = typeof(HomePage);
            }

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
        _dispatcher = dispatcher;

        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        GoToSettingsCommand = new AsyncRelayCommand(GoToSettings);
        LogoutCommand = new AsyncRelayCommand(DoLogout);

        Categories = filterService.Categories;

        WeakReferenceMessenger.Default.Register<CategorySelectionChanged>(this, (r, m) =>
        {
            OnPropertyChanged(nameof(SelectedCategory));
        });
    }

    public async override void Load()
    {
        User = await _userService.GetUser();

        IsSignedIn = await _authentication.IsAuthenticated();

        PageType = typeof(HomePage);
    }

    private async Task GoToSettings()
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
        throw new NotImplementedException();
    }
}