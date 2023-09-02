using ProjectSBS.Presentation.Components;
using ProjectSBS.Services.Items.Billing;
using ProjectSBS.Services.Notifications;

namespace ProjectSBS.Services.Items;

public class ItemService : IItemService
{
    private readonly IBillingService _billing;
    private readonly INotificationService _notification;

    public ItemService(IBillingService billing/*, INotificationService notification*/)
    {
        _billing = billing;
        //_notification = notification;
    }

    public ItemViewModel ScheduleBilling(ItemViewModel itemVM)
    {
        var item = itemVM.Item;
        var paymentDates = _billing.GetFuturePayments(item.Billing.InitialDate, item.Billing.PeriodType, item.Billing.RecurEvery);

        foreach (var date in paymentDates)
        {
            //_notification.ScheduleNotification(item.Id, item.Name, DateTime.Now.ToString(), date, new TimeOnly(8, 00));
        }

        return itemVM; //TODO with paymentDates to display
    }
}
