using ProjectSBS.Services.Items.Billing;
using ProjectSBS.Services.Items.Tags;

namespace ProjectSBS.Presentation.Components;

public partial class ItemViewModel : ObservableObject
{
    private readonly IBillingService _billingService;

    public ItemViewModel(Item? item)
    {
        _item = item;

        _billingService = App.Services?.GetRequiredService<IBillingService>()!;
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

            var tag = App.Services?.GetRequiredService<ITagService>().Tags.FirstOrDefault(tag => tag.Id == item.TagId);

            return tag;
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
            var price = Enumerable.Count(dates) * billing.BasePrice;
            return price;
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
