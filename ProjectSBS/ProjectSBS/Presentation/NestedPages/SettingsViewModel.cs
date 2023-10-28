using ProjectSBS.Services.User;

namespace ProjectSBS.Presentation.NestedPages;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IUserService _userService;

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private string _itemName = "Settings";

    public SettingsViewModel(
    IStringLocalizer localizer,
    IAuthenticationService authentication,
    IUserService userService)
    {
        _userService = userService;
        _authentication = authentication;
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
