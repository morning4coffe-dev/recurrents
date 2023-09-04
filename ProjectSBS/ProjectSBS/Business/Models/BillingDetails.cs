namespace ProjectSBS.Business.Models;

public record BillingDetails
{
    public BillingDetails(decimal basePrice, DateOnly initialDate,
        string currencyId = "EUR",
        Period periodType = Period.Monthly, short recurEvery = 1)
    {

        BasePrice = basePrice;
        InitialDate = initialDate;
        CurrencyId = currencyId;
        PeriodType = periodType;
        RecurEvery = recurEvery;
    }

    /// <summary>
    /// Base price of the subscription in EUR. Multiply with the selected currency.
    /// </summary>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// The start Billing date of the subscription.
    /// </summary>
    public DateOnly InitialDate { get; set; }

    public string CurrencyId { get; set; }

    public Period PeriodType { get; set; }

    public short RecurEvery { get; set; }
}