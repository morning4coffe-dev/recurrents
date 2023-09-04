namespace ProjectSBS.Business.Models;

public class ItemLog
{
    public ItemLog( 
        string itemId, 
        DateOnly paymentDate, 
        decimal price, 
        string currencyId)
    {
        ItemId = itemId;
        PaymentDate = paymentDate;
        Price = price;
        CurrencyId = currencyId;
    }
    public string ItemId { get; set; }
    public DateOnly PaymentDate { get; set; }
    public decimal Price { get; set; }
    public string CurrencyId { get; set; }
}
