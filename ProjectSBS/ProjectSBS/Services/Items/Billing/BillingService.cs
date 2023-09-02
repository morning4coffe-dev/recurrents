using ProjectSBS.Services.Storage.Data;

namespace ProjectSBS.Services.Items.Billing;

class BillingService : IBillingService
{
    private readonly IDataService _dataService;

    public BillingService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public void NewPaymentLog(Item item)
    {
        var log = new ItemLog(
            item.Id,
            DateOnly.FromDateTime(DateTime.Now.Date),
            item.Billing.BasePrice,
            item.Billing.CurrencyId
        );

        Task.Run(async () => await _dataService.AddLogAsync(log)).Wait();
    }

    public async Task<List<ItemLog>> GetPaymentLogsForItemAsync(Item item)
    {
        var logs = await _dataService.LoadLogsAsync();

        return logs.Where(log => log.ItemId == item.Id).ToList();
    }

    public List<DateOnly> GetFuturePayments(DateOnly initialDate, Period periodType, int recurEvery, int numberOfPayments = 20)
    {
        var nextBillingDate = GetBillingDates(initialDate, periodType, recurEvery).upcomingBillingDate;

        List<DateOnly> paymentList = new();

        for (int i = 0; i < numberOfPayments; i++)
        {
            DateOnly billingDate = nextBillingDate;
            if (billingDate < initialDate)
            {
                billingDate = billingDate.AddMonths(1);
            }

            paymentList.Add(billingDate);
            nextBillingDate = GetNextBillingDate(billingDate, periodType, recurEvery);
        }

        return paymentList;
    }

    public (DateOnly lastBillingDate, DateOnly upcomingBillingDate) GetBillingDates(DateOnly initialDate, Period periodType, int recurEvery)
    {
        DateOnly lastBillingDate = initialDate;
        DateOnly nextBillingDate = initialDate;

        while (nextBillingDate <= DateOnly.FromDateTime(DateTime.Now.Date))
        {
            lastBillingDate = nextBillingDate;
            nextBillingDate = GetNextBillingDate(nextBillingDate, periodType, recurEvery);
        }

        return (lastBillingDate, nextBillingDate);
    }

    public DateOnly GetNextBillingDate(DateOnly initialDate, Period periodType, int recurEvery)
    {
        return PeriodToTimeSpanMap[periodType](initialDate, recurEvery);
    }

    private readonly Dictionary<Period, Func<DateOnly, int, DateOnly>> PeriodToTimeSpanMap = new()
    {
        { Period.Daily, (date, recurEvery) => date.AddDays(recurEvery) },
        { Period.Weekly, (date, recurEvery) => date.AddDays(recurEvery * 7) },
        { Period.Monthly, (date, recurEvery) => date.AddMonths(recurEvery) },
        { Period.Quarterly, (date, recurEvery) => date.AddMonths(recurEvery * 3) },
        { Period.Annually, (date, recurEvery) => date.AddYears(recurEvery) }
    };
}