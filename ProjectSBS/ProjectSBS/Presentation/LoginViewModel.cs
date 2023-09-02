using Windows.System;

namespace ProjectSBS.Presentation;

public partial class LoginViewModel : ObservableObject
{
    private IAuthenticationService _authentication;
    private IStringLocalizer _localization;
    private readonly IDispatcher _dispatcher;

    private INavigator _navigator;

    public LoginViewModel(
        INavigator navigator,
        IAuthenticationService authentication,
        IStringLocalizer localization,
        IDispatcher dispatcher)
    {
        _navigator = navigator;
        _authentication = authentication;
        _localization = localization;
        _dispatcher = dispatcher;

        Login = new AsyncRelayCommand(DoLogin);
        WithoutLogin = new AsyncRelayCommand(DoWithoutLogin);

        _titleText = _localization["Welcome"];
        _authorText = _localization["Author"];
        _privacyPolicyText = _localization["PrivacyPolicy"];
        _loginWithMicrosoftText = _localization["LoginWithMicrosoft"];
        _continueWithoutLoginText = _localization["ContinueWithoutLogin"];
    }

    private async Task DoLogin()
    {
        var success = false;

        try
        {
            success = await _authentication.LoginAsync(_dispatcher);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        if (success)
        {
            await _navigator.NavigateViewModelAsync<MainViewModel>(this, qualifier: Qualifiers.ClearBackStack);
        }
    }

    private async Task DoWithoutLogin()
    {
        await _navigator.NavigateViewModelAsync<MainViewModel>(this, qualifier: Qualifiers.ClearBackStack);
    }

    [RelayCommand]
    private void PrivacyPolicy()
    {
        //TODO add proper privacy policy link
        _ = Launcher.LaunchUriAsync(new Uri("https://www.privacypolicies.com/"));
    }

    [ObservableProperty]
    private string _titleText;

    [ObservableProperty]
    private string _authorText;

    [ObservableProperty]
    private string _privacyPolicyText;

    [ObservableProperty]
    private string _loginWithMicrosoftText;

    [ObservableProperty]
    private string _continueWithoutLoginText;

    public ICommand Login { get; }
    public ICommand WithoutLogin { get; }
}