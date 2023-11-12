using ProjectSBS.Services.Items.Billing;
using ProjectSBS.Services.Items.Tags;
using ProjectSBS.Services.Notifications;

namespace ProjectSBS.Presentation.Components;

public partial class ItemViewModel : ObservableObject
{
    private readonly IBillingService _billingService;
    private readonly INotificationService _notification;

    public ItemViewModel(Item? item)
    {
        _item = item;

        _billingService = App.Services?.GetRequiredService<IBillingService>()!;
        _notification = App.Services?.GetRequiredService<INotificationService>()!;

        ScheduleBilling();
    }

    [ObservableProperty]
    private Item? _item;

    public Tag? DisplayTag
    {
        get
        {
            if (Item is not { } item)
            {
                return null;
            }

            return App.Services?.GetRequiredService<ITagService>().Tags.FirstOrDefault(tag => tag.Id == item.TagId);
        }
    }

    public string PaymentDate
    {
        get
        {
            var returnable = (GetFuturePayments(1).First().ToDateTime(new TimeOnly()) - DateTime.Today).Days.ToString();
            return "In " + (returnable
                ?? "N/A") + "days";
        }
    }

    public decimal TotalPrice
    {
        get
        {
            if (Item?.Billing is not { } billing)
            {
                return 0M;
            }

            var dates = _billingService.GetLastPayments(billing.InitialDate, billing.PeriodType, billing.RecurEvery);

            //TODO Doesn't account for currency or the previous value of the currency
            return Enumerable.Count(dates) * billing.BasePrice;
        }
    }

    public List<DateOnly> GetFuturePayments(int numberOfPayments = 12)
    {
        if (Item?.Billing is not { } billing)
        {
            return new();
        }

        return _billingService.GetFuturePayments(billing.InitialDate, billing.PeriodType, billing.RecurEvery, numberOfPayments);
    }

    public void Updated()
    {
        ScheduleBilling();
    }

    public async void ScheduleBilling()
    {
        if (Item is not { } item || !Item.IsNotify)
        {
            return;
        }

        _notification.RemoveScheduledNotifications();

        if (!_notification.IsEnabledOnDevice())
        {
            //TODO Notify user that notifications are disabled
            return;
        }

#if !HAS_UNO
        var paymentDates = GetFuturePayments();

        var tasks = paymentDates.Select(date =>
        Task.Run(() =>
        {
            var title = item.Name ?? "";
            //(≈{2})
            var text = string.Format("Your {0}{1} payment for {2} is due tomorrow!", item.Billing.BasePrice, item.Billing.CurrencyId, item.Name);

            //- time period before e.g. one day, time that user selected
            //DEBUG _notification.ScheduleNotification(item.Id, title, text, DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now.AddSeconds(15)));
            _notification.ScheduleNotification(item.Id, title, text, date.AddDays(-1), new TimeOnly(8, 00));
        })).ToArray();

        await Task.WhenAll(tasks);
#endif
    }

    #region FEAT: feature/IndividualPayments
    //[ObservableProperty]
    //private bool _isPaid;

    //public List<ItemLog> PaymentLogs { get; } = new();

    //private async Task OnPay()
    //{

    //await Task.CompletedTask;

    //if (IsPaid)
    //{
    //    //TODO Play sound or vibrate the device

    //    await _billingService.NewPaymentLogAsync(Item);
    //}
    //else
    //{
    //    await _billingService.RemoveLastPaymentLogAsync(Item);
    //}
    //}

    //private bool CalculateIsPaid(Item item, IEnumerable<ItemLog> logs)
    //{
    //    var itemLogs = _billingService.GetPaymentLogsForItem(item, logs);

    //    if (!Enumerable.Any(logs))
    //    {
    //        return false;
    //    }

    //    var (lastPayment, nextPayment) = _billingService.GetBillingDates(item.Billing.InitialDate, item.Billing.PeriodType, item.Billing.RecurEvery);

    //    if (itemLogs.LastOrDefault() is { } lastLog)
    //    {
    //        if (lastPayment > DateOnly.FromDateTime(DateTime.Today) ||
    //            (lastLog.PaymentDate >= lastPayment && lastLog.PaymentDate < nextPayment))
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}
    #endregion
}
