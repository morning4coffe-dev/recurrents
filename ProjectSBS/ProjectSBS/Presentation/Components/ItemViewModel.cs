using ProjectSBS.Services.Items.Billing;

namespace ProjectSBS.Presentation.Components;

public partial class ItemViewModel : ObservableObject
{
    private readonly IBillingService _billingService;

    public ItemViewModel(Item item)
    {
        _item = item;

        //(Application.Current as App)!.Host?.Services.GetService<IItemService>();
        _billingService = (Application.Current as App)!.Host?.Services.GetService<IBillingService>()!;
    }

    [ObservableProperty]
    private Item _item;

    private bool _isPaid;

    public bool IsPaid
    {
        get => _isPaid;
        set
        {
            _isPaid = value;
            if (value)
            {
                _billingService.NewPaymentLog(Item);
            }
        }
    }

    public async Task InitializeAsync()
    {
        _isPaid = await CalculateIsPaidAsync(Item);
    }

    private async Task<bool> CalculateIsPaidAsync(Item item)
    {
        var logs = await _billingService.GetPaymentLogsForItemAsync(item);

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
        else
        {
            return false;
        }
    }
}
