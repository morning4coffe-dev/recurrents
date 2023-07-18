using ProjectSBS.Services.Interop;
using ProjectSBS.Services.Notifications;

namespace ProjectSBS;

public class ShellViewModel : ObservableObject
{
    private readonly ICurrencyCache _api;
    private readonly INotificationService _notifications;
    private readonly IInteropService _interopService;

    public ShellViewModel(ICurrencyCache api, INotificationService notifications, IInteropService interop)
    {
        _api = api;
        _notifications = notifications;

        _interopService = interop;

        Init();
    }

    public async void Init()
    {
        var c = await _api.GetCurrency(new CancellationToken());

        //_notifications.ShowBasicToastNotification("CZK is currently", c.Rates["CZK"].ToString() + " Kč");

        await _interopService.SetThemeAsync(ElementTheme.Light);

        //await Task.Delay(5000);
        //await _interopService.SetThemeAsync(ElementTheme.Default);

        //_ = _interopService.OpenStoreReviewUrlAsync();
    }
}
