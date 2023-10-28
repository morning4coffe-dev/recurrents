using ProjectSBS.Services.User;
using UserModel = ProjectSBS.Business.Models;
using Windows.System;
using Uno.Extensions.Authentication;
using Uno.Extensions.Navigation;

namespace ProjectSBS.Presentation;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthenticationService _authentication;
    private readonly IUserService _userService;

    private readonly IStringLocalizer _localization;
    private readonly IDispatcher _dispatcher;

    private readonly INavigation _navigation;
    private readonly INavigator _navigator;


    [ObservableProperty]
    private UserModel.User? _user;

    public LoginViewModel(
        INavigation navigation,
        IAuthenticationService authentication,
        IUserService userService,
        IStringLocalizer localization,
        INavigator navigator,
        IDispatcher dispatcher)
    {
        _navigation = navigation;
        _authentication = authentication;
        _userService = userService;
        _localization = localization;
        _dispatcher = dispatcher;
        _navigator = navigator;

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

            //App.Services.GetRequiredService<ILogger<LoginViewModel>>().LogInformation("Logging in");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        if (success)
        {
            var user = await _userService.GetUser();

            App.Dispatcher.TryEnqueue(() =>
            {
                User = user;
            });

            //TODO [Optimization] instead of waiting, maybe load items, or stuff like that here
            await Task.Delay(100);

            App.Dispatcher.TryEnqueue(() =>
            {
                _navigator.NavigateViewModelAsync<MainViewModel>(this, qualifier: Qualifiers.ClearBackStack);
            });
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