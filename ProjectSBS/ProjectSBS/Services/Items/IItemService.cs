namespace ProjectSBS.Services.Items;

public interface IItemService
{
    event EventHandler<IEnumerable<ItemViewModel>> OnItemsInitialized;
    event EventHandler<IEnumerable<ItemViewModel>> OnItemsChanged;
    Task InitializeAsync();
    IEnumerable<ItemViewModel> GetItems(Func<ItemViewModel, bool>? selector = null);
    bool ClearItems();
    void NewItem(Item item, List<ItemLog>? logs = null);
    void UpdateItem(ItemViewModel item);
    void ArchiveItem(ItemViewModel item);
    void DeleteItem(ItemViewModel item);
}
