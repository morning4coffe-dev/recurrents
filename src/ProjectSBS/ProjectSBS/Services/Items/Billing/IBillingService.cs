namespace ProjectSBS.Services.Items.Billing;

public interface IBillingService
{
    void NewPaymentLog(Item item);
    Task<List<ItemLog>> GetPaymentLogsForItemAsync(Item item);

    List<DateOnly> GetFuturePayments(DateOnly initialDate, Period periodType, int recurEvery, int numberOfPayments = 20);

    (DateOnly lastBillingDate, DateOnly upcomingBillingDate) GetBillingDates(DateOnly date, Period periodType, int recurEvery);
    DateOnly GetNextBillingDate(DateOnly date, Period periodType, int recurEvery);
}