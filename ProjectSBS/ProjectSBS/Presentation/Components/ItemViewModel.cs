using ProjectSBS.Services.Items.Billing;

namespace ProjectSBS.Presentation.Components;

public partial class ItemViewModel : ObservableObject
{
    private readonly IBillingService _billingService;

    public ItemViewModel(Item? item)
    {
        _item = item;

        _billingService = (Application.Current as App)!.Host?.Services.GetService<IBillingService>()!;
    }

    [ObservableProperty]
    private Item? _item;

    private bool _isPaid;

    public bool IsPaid
    {
        get => _isPaid;
        set
        {
            _isPaid = value;
            if (value)
            {
                _billingService.NewPaymentLogAsync(_item).Wait();
            }
            //TODO IsPaid doesn't set when item has been unpaid
        }
    }

    public void Initialize(List<ItemLog> logs)
    {
        if (Item == null)
        {
            IsPaid = false;
            return;
        }

        IsPaid = CalculateIsPaid(Item, logs);
    }

    private bool CalculateIsPaid(Item item, List<ItemLog> logs)
    {
        _billingService.GetPaymentLogsForItem(item, logs);

        if (logs.Count == 0)
        {
            return false;
        }

        var (lastPayment, nextPayment) = _billingService.GetBillingDates(item.Billing.InitialDate, item.Billing.PeriodType, item.Billing.RecurEvery);
        var paymentDateToCheck = logs.Last().PaymentDate;

        if (paymentDateToCheck >= lastPayment && paymentDateToCheck < nextPayment)
        {
            return true;
        }

        return false;
    }
}
