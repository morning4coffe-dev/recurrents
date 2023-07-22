using ProjectSBS.Services.Items.Billing;
using ProjectSBS.Services.Notifications;

namespace ProjectSBS.Services.Items;

internal class ItemService
{
    private readonly IBillingService _billing;
    private readonly INotificationService _notification;

    public ItemService(IBillingService billing, INotificationService notification)
    {
        _billing = billing;
        _notification = notification;    


    }

    public void ScheduleBilling(Item item)
    {
        var payments = _billing.GetFuturePayments(item.Billing.InitialDate, item.Billing.PeriodType, item.Billing.RecurEvery);

        foreach (var payment in payments)
        {
            //TODO _notification.Schedule...
        }
    }
}
