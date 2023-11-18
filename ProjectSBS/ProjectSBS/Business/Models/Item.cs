namespace ProjectSBS.Business.Models;

public partial record Item
{
    public Item(
        string? id, 
        string name, 
        BillingDetails? billing = default, 
        int tagId = default, 
        bool isNotify = true,
        string description = "", 
        DateTime creationDate = default)
    {
        Id = id ?? Guid.NewGuid().ToString();
        Name = name;
        Billing = billing ?? new BillingDetails(5.99M, DateOnly.FromDateTime(DateTime.Today));
        TagId = tagId;
        IsNotify = isNotify;
        Description = description;
        CreationDate = creationDate == default ? DateTime.Now : creationDate;
    }

    public string Id { get; }
    public string Name { get; set; }
    public BillingDetails Billing { get; }
    public int TagId { get; set; }
    public string? Description { get; set; }
    public bool IsNotify { get; set; }
    public DateTime CreationDate { get; set; }
}
