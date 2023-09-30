using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.User;
using System.Collections.ObjectModel;
using Windows.System.Profile;
using ProjectSBS.Services.Items.Filtering;
using ProjectSBS.Presentation.NestedPages;

namespace ProjectSBS.Presentation;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthenticationService _authentication;
    private readonly IUserService _userService;

    private readonly IItemFilterService _filterService;

    public static IDispatcher? Dispatch { get; private set; }
    public static INavigator? Navigator { get; private set; }

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private Type? _pageType;

    [ObservableProperty]
    private Type? _settingsPage;

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

    public bool IsSignedIn { get; private set; }

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
        IDispatcher dispatch,
        INavigator navigator,
        IItemFilterService filterService)
    {
#if !__IOS__
        _authentication = authentication;
#endif
        _userService = userService;
        _filterService = filterService;

        Dispatch = dispatch;
        Navigator = navigator;

        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        GoToSettingsCommand = new AsyncRelayCommand(GoToSettings);
        LogoutCommand = new AsyncRelayCommand(DoLogout);

        Categories = filterService.Categories;

        Task.Run(InitializeAsync);

        WeakReferenceMessenger.Default.Register<CategorySelectionChanged>(this, (r, m) =>
        {
            OnPropertyChanged(nameof(SelectedCategory));
        });
    }

    private async Task InitializeAsync()
    {
        var user = await _userService.GetUser();

        //TODO Log Dispatch is null on Initialization

        await Dispatch.ExecuteAsync(async () =>
        {
            if (user is not null)
            {
                User = user;
                Name = User.Name;
            }

            IsSignedIn = await _authentication.IsAuthenticated();
        });

        while ((Application.Current as App)!.Host == null)
        {
            //Wait till Host is created
            await Task.Delay(200);

            //TODO Add loading indicator
        }

        await Dispatch.ExecuteAsync(() =>
        {
            PageType = typeof(HomePage);
        });
    }

    private async Task GoToSettings()
    {
        PageType = typeof(SettingsPage);
    }

    public async Task DoLogout(CancellationToken token)
    {
        await _authentication.LogoutAsync(Dispatch, token);
        //TODO probably will have to clean the token too
    }
}