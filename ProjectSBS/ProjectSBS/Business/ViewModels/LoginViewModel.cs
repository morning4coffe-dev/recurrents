using Windows.System;
using UserModel = ProjectSBS.Business.Models;

namespace ProjectSBS.Business.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService _userService;

    private readonly IStringLocalizer _localization;
    private readonly INavigation _navigation;
    private readonly IItemService _itemService;

    [ObservableProperty]
    private UserModel.User? _user;

    [ObservableProperty]
    private bool _indicateLoading;

    public LoginViewModel(
        INavigation navigation,
        IUserService userService,
        IItemService itemService,
        IStringLocalizer localization)
    {
        _navigation = navigation;
        _userService = userService;
        _localization = localization;
        _itemService = itemService;

        _titleText = _localization["Welcome"];
        _authorText = _localization["Author"];
        _privacyPolicyText = _localization["PrivacyPolicy"];
        _loginWithMicrosoftText = _localization["LoginWithMicrosoft"];
        _continueWithoutLoginText = _localization["ContinueWithoutLogin"];

        itemService.ClearItems();
    }

    [RelayCommand]
    private async Task Login()
    {
        var success = false;
        IndicateLoading = true;

        try
        {
            success = await _userService.AuthenticateAsync();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        if (success)
        {
            var user = await _userService.RetrieveUser();

            App.Dispatcher.TryEnqueue(() =>
            {
                User = user;
            });

            await _itemService.InitializeAsync();

            App.Dispatcher.TryEnqueue(() =>
            {
                _navigation.Navigate(typeof(MainPage));
            });

            SendAnalytics(true);
        }

        IndicateLoading = false;
    }

    private void SendAnalytics(bool loggedIn)
    {
        Dictionary<string, string> logInStats = new()
        {
            { "Logged In", loggedIn.ToString() },
            { "Provider", loggedIn ? "Microsoft" : "N/A" },
        };

        AnalyticsService.TrackEvent(AnalyticsService.LogIn, logInStats);
    }

    private void SendAnalytics(bool loggedIn)
    {
        Dictionary<string, string> logInStats = new()
        {
            { "Logged In", loggedIn.ToString() },
            { "Provider", loggedIn ? "Microsoft" : "N/A" },
        };

        AnalyticsService.TrackEvent(AnalyticsService.LogIn, logInStats);
    }

    [RelayCommand]
    private void ContinueWithoutLogin()
    {
        IndicateLoading = true;
        App.Services!.GetRequiredService<ISettingsService>().ContinueWithoutLogin = true;
        _navigation.Navigate(typeof(MainPage));
        IndicateLoading = false;
    }

    [RelayCommand]
    private async Task OpenPrivacyPolicy() =>
        await  Launcher.LaunchUriAsync(new Uri("https://github.com/morning4coffe-dev/recurrents/blob/ebf622cb65d60c7d353af69824f63d88fa796bde/privacy-policy.md"));

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
}
