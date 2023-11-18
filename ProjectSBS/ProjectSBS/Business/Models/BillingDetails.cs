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

    public decimal BasePrice { get; set; }

    public DateOnly InitialDate { get; set; }

    public string CurrencyId { get; set; }

    public Period PeriodType { get; set; }

    public short RecurEvery { get; set; }
}
