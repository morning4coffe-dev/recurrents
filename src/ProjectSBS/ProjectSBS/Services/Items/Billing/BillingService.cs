namespace ProjectSBS.Services.Items.Billing;

class BillingService : IBillingService
{
    public List<DateOnly> GetFuturePayments(DateOnly initialDate, Period periodType, int recurEvery, int numberOfPayments = 20)
    {
        List<DateOnly> paymentList = new();
        DateOnly nextBillingDate = GetNextBillingDate(initialDate, periodType, recurEvery);

        while (nextBillingDate <= DateOnly.FromDateTime(DateTime.Now.Date))
        {
            nextBillingDate = GetNextBillingDate(nextBillingDate, periodType, recurEvery);
        }

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