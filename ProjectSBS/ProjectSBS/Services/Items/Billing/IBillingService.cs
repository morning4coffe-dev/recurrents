namespace ProjectSBS.Services.Items.Billing;

public interface IBillingService
{
    Task NewPaymentLogAsync(Item item);
    List<ItemLog> GetPaymentLogsForItem(Item item, List<ItemLog> logs);

    List<DateOnly> GetFuturePayments(DateOnly initialDate, Period periodType, int recurEvery, int numberOfPayments = 20);

    (DateOnly lastBillingDate, DateOnly upcomingBillingDate) GetBillingDates(DateOnly date, Period periodType, int recurEvery);
    DateOnly GetNextBillingDate(DateOnly date, Period periodType, int recurEvery);
}