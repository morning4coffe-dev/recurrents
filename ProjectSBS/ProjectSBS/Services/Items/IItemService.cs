namespace ProjectSBS.Services.Items;

public interface IItemService
{
    event EventHandler<bool>? OnItemsInitialized;
    //event EventHandler<IEnumerable<ItemViewModel>> OnItemsChanged;
    Task InitializeAsync();
    IEnumerable<ItemViewModel> GetItems(Func<ItemViewModel, bool>? selector = null);
    void NewItem(Item item, List<ItemLog>? logs = null);
    void UpdateItem(ItemViewModel item);
    void DeleteItem(ItemViewModel item);
    ItemViewModel ScheduleBilling(ItemViewModel item, List<ItemLog> logs);
}