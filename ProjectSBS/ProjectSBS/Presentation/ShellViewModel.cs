namespace ProjectSBS.Presentation;

public class ShellViewModel
{
    private readonly IAuthenticationService _authentication;


    private readonly INavigator _navigator;

    public ShellViewModel(
#if !__IOS__
        IAuthenticationService authentication,
#endif
        INavigator navigator)
    {
        _navigator = navigator;
#if !__IOS__
        _authentication = authentication;
        _authentication.LoggedOut += LoggedOut;
#endif
    }

    private async void LoggedOut(object? sender, EventArgs e)
    {
        await _navigator.NavigateViewModelAsync<LoginViewModel>(this, qualifier: Qualifiers.ClearBackStack);
    }
}