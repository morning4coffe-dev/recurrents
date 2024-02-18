using Windows.System;
using UserModel = ProjectSBS.Business.Models;

namespace ProjectSBS.Business.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService _userService;

    private readonly INavigation _navigation;
    private readonly IItemService _itemService;

    [ObservableProperty]
    private UserModel.User? _user;

    public LoginViewModel(
        INavigation navigation,
        IUserService userService,
        IItemService itemService)
    {
        _navigation = navigation;
        _userService = userService;
        _itemService = itemService;

        Login = new AsyncRelayCommand(DoLogin);
        WithoutLogin = new RelayCommand(() =>
        {
            App.Services!.GetRequiredService<ISettingsService>().ContinueWithoutLogin = true;
            _navigation.Navigate(typeof(MainPage));
        });

        itemService.ClearItems();
    }

    [RelayCommand]
    private async Task Login()
    {
        var success = false;

        try
        {
            success = await _userService.AuthenticateAsync();

            //App.Services.GetRequiredService<ILogger<LoginViewModel>>().LogInformation("Logging in");
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
        }
    }

    [RelayCommand]
    private void ContinueWithoutLogin()
    {
        App.Services!.GetRequiredService<ISettingsService>().ContinueWithoutLogin = true;
        _navigation.Navigate(typeof(MainPage));
    }

    public ICommand Login { get; }
    public ICommand WithoutLogin { get; }
}
