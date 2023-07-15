namespace ProjectSBS.Services.Items.Billing;

internal static class BillingService
{
    public static DateOnly GetNextBillingDate(DateOnly initialDate, Period periodType, int recurEvery)
    {
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now.Date);
        DateOnly date = initialDate;

        while (date <= currentDate)
        {
            date = CalculateNextBillingDate(date, periodType, recurEvery);
        }

        return date;
    }

    public static List<DateOnly> GetFuturePayments(DateOnly initialDate, Period periodType, int recurEvery, int numberOfPayments = 20)
    {
        List<DateOnly> paymentList = new();

        for (int i = 0; i < numberOfPayments; i++)
        {
            DateOnly date = CalculateNextBillingDate(initialDate, periodType, recurEvery);
            paymentList.Add(date);
        }

        return paymentList;
    }

    private static readonly Dictionary<Period, Func<DateOnly, int, DateOnly>> PeriodToTimeSpanMap = new()
    {
        { Period.Daily, (date, recurEvery) => date.AddDays(recurEvery) },
        { Period.Weekly, (date, recurEvery) => date.AddDays(recurEvery * 7) },
        { Period.Monthly, (date, recurEvery) => date.AddMonths(recurEvery) },
        { Period.Quarterly, (date, recurEvery) => date.AddMonths(recurEvery * 3) },
        { Period.Yearly, (date, recurEvery) => date.AddYears(recurEvery) }
    };

    private static DateOnly CalculateNextBillingDate(DateOnly date, Period periodType, int recurEvery)
    {
        return PeriodToTimeSpanMap[periodType](date, recurEvery);
    }
}
