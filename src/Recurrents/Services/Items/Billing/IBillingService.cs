namespace Recurrents.Services.Items.Billing;

public interface IBillingService
{
    #region FEAT: feature/IndividualPayments
    //Task NewPaymentLogAsync(Item item);
    //Task RemoveLastPaymentLogAsync(Item item);
    //IEnumerable<ItemLog> GetPaymentLogsForItem(Item item, IEnumerable<ItemLog> logs);
    #endregion

    List<DateOnly> GetFuturePayments(DateOnly initialDate, Period periodType, int recurEvery, int numberOfPayments = 20);
    IEnumerable<DateOnly> GetLastPayments(DateOnly initialDate, Period periodType, int recurEvery);

    (DateOnly lastBillingDate, DateOnly upcomingBillingDate) GetBillingDates(DateOnly date, Period periodType, int recurEvery);
    DateOnly GetNextBillingDate(DateOnly date, Period periodType, int recurEvery);
}