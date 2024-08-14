namespace Recurrents.Business.Models;

public partial record Item
{
    public Item(
        string? id, 
        string name, 
        BillingDetails? billing = default, 
        int tagId = default, 
        bool isNotify = true,
        string description = "", 
        DateTime creationDate = default,
        List<Status>? status = default)
    {
        Id = id ?? Guid.NewGuid().ToString();
        Name = name;
        Billing = billing ?? new(5, DateOnly.FromDateTime(DateTime.Today));
        TagId = tagId;
        IsNotify = isNotify;
        Description = description;
        CreationDate = creationDate == default ? DateTime.Now : creationDate;
        Status = status ?? [];
    }

    public string Id { get; }
    public string Name { get; set; }
    public BillingDetails Billing { get; }
    public int TagId { get; set; }
    public string? Description { get; set; }
    public bool IsNotify { get; set; }
    public DateTime CreationDate { get; set; }
    public List<Status> Status { get; }
}
