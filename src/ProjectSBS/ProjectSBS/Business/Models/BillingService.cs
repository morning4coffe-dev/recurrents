namespace ProjectSBS.Models;

public record BillingService
{

    /// <summary>
    /// Base price of the subscription in EUR. Multiply with the selected currency.
    /// </summary>
    public decimal BasePrice { get; }

    /// <summary>
    /// The start Billing date of the subscription.
    /// </summary>
    public DateOnly InitialDate { get; }

    public string CurrencyId { get; }

    public Period PeriodType { get; }

    public short RecurEvery { get; }

    public BillingService(decimal basePrice, DateOnly initialDate,
        string currencyId = "EUR",
        Period periodType = Period.Monthly, short recurEvery = 1)
    {

        BasePrice = basePrice;
        InitialDate = initialDate;
        CurrencyId = currencyId;
        PeriodType = periodType;
        RecurEvery = recurEvery;
    }
}