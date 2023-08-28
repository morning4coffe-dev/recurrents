using ProjectSBS.Services.User;

namespace ProjectSBS.Presentation;

public partial class HomeViewModel : ObservableObject
{
    private IAuthenticationService _authentication;
    private IUserService _userService;

    private IDispatcher _dispatch;
    private INavigator _navigator;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private User? _user;

    public HomeViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        IAuthenticationService authentication,
        IUserService userService,
        IDispatcher dispatch,
        INavigator navigator)
    {
        _authentication = authentication;
        _userService = userService;
        _dispatch = dispatch;
        _navigator = navigator;

        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
        GoToSecond = new AsyncRelayCommand(GoToSecondView);
        Logout = new AsyncRelayCommand(DoLogout);

        Initialize();
    }
    public string? Title { get; }

    public ICommand GoToSecond { get; }

    public ICommand Logout { get; }

    public async void Initialize()
    {
        var user = await _userService.GetUser();

        await _dispatch.ExecuteAsync(() =>
        {
            User = user;
            Name = User.Name;
        });
    }

    private async Task GoToSecondView()
    {
        await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: new Entity(Name!));
    }

    public async Task DoLogout(CancellationToken token)
    {
        await _authentication.LogoutAsync(_dispatch, token);
        //TODO probabbly will have to clean the token too
    }
}