using ProjectSBS.Models;

namespace ProjectSBS.Business.Models;

public partial record Item
{
    public Item(
        string id, 
        string name, 
        BillingDetails billing, 
        string tagId, 
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
    public string Name { get; }
    public BillingDetails Billing { get; }
    public string TagId { get; }
    public string? Description { get; }
    public DateTime CreationDate { get; }
}
