namespace ProjectSBS.Presentation.NestedPages;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IUserService _userService;

    [ObservableProperty]
    private User? _user;

    public readonly string TitleText;
    public SettingsViewModel(
    IStringLocalizer localizer,
    IAuthenticationService authentication,
    IUserService userService)
    {
        _userService = userService;
        _authentication = authentication;

        TitleText = localizer["Settings"];
    }

    public override async void Load()
    {
        User = await _userService.GetUser();
    }

    public override void Unload()
    {
        //throw new NotImplementedException();
    }
}
