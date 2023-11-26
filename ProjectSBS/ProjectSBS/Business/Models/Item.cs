namespace ProjectSBS.Business.Models;

public partial record Item
{
    public Item(
        string? id, 
        string name, 
        BillingDetails? billing = default, 
        int tagId = default, 
        bool isNotify = true,
        List<Status> status = default,
        string description = "", 
        DateTime creationDate = default)
    {
        Id = id ?? Guid.NewGuid().ToString();
        Name = name;
        Billing = billing ?? new(5, DateOnly.FromDateTime(DateTime.Today));
        TagId = tagId;
        IsNotify = isNotify;
        Status = status ?? [];
        Description = description;
        CreationDate = creationDate == default ? DateTime.Now : creationDate;
    }

    public string Id { get; }
    public string Name { get; set; }
    public BillingDetails Billing { get; }
    public int TagId { get; set; }
    public List<Status> Status { get; }
    public string? Description { get; set; }
    public bool IsNotify { get; set; }
    public DateTime CreationDate { get; set; }
}
