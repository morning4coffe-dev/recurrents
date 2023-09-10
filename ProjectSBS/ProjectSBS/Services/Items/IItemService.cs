namespace ProjectSBS.Services.Items;

public interface IItemService
{
    Task InitializeAsync();
    List<ItemViewModel> GetItems();
    void NewItem(Item item, List<ItemLog>? logs = null);
    ItemViewModel ScheduleBilling(ItemViewModel item, List<ItemLog> logs);
}