namespace ProjectSBS.Business.Models;

public partial record Item
{
    public Item(
        string id, 
        string name, 
        BillingDetails billing, 
        int tagId, 
        string? description, 
        DateTime creationDate)
    {
        Id = id;
        Name = name;
        Billing = billing;
        TagId = tagId;
        Description = description;
        CreationDate = creationDate;
    }

    public string Id { get; }
    public string Name { get; set; }
    public BillingDetails Billing { get; }
    public int TagId { get; set; }
    public string? Description { get; set; }
    public DateTime CreationDate { get; set; }
}
