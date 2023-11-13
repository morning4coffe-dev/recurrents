using Windows.System;
using UserModel = ProjectSBS.Business.Models;

namespace ProjectSBS.Presentation;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService _userService;

    private readonly IStringLocalizer _localization;
    private readonly INavigation _navigation;


    [ObservableProperty]
    private UserModel.User? _user;

    public LoginViewModel(
        INavigation navigation,
        IUserService userService,
        IStringLocalizer localization)
    {
        _navigation = navigation;
        _userService = userService;
        _localization = localization;

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
            success = await _userService.LoginUser();

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
            //await Task.Delay(100);

            App.Dispatcher.TryEnqueue(() =>
            {
                _navigation.Navigate(typeof(MainPage));
                //_navigator.NavigateViewModelAsync<MainViewModel>(this, qualifier: Qualifiers.ClearBackStack);
            });
        }
    }

    private async Task DoWithoutLogin()
    {
        _navigation.Navigate(typeof(MainPage));
        //await _navigator.NavigateViewModelAsync<MainViewModel>(this, qualifier: Qualifiers.ClearBackStack);
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