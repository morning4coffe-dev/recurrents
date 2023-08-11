namespace ProjectSBS.Business.Models;

public class ItemLog
{
    public required string ItemId { get; set; }
    public DateOnly PaymentDate { get; set; }
    public decimal Price { get; set; }
    public string CurrencyId { get; set; }
}
