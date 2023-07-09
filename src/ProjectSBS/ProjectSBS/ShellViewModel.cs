using ProjectSBS.Services.Notifications;

namespace ProjectSBS;

public class ShellViewModel : ObservableObject
{
    private readonly ICurrencyCache _api;
    private readonly INotificationService _notifications;

    public ShellViewModel(ICurrencyCache api, INotificationService notifications)
    {
        _api = api;
        _notifications = notifications;

        Init();
    }

    public async void Init()
    {
        var c = await _api.GetCurrency(new CancellationToken());

        _notifications.ShowBasicToastNotification("CZK is currently", c.Rates["CZK"].ToString() + " Kč");
    }
}
