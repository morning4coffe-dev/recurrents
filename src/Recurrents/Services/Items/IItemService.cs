namespace Recurrents.Services.Items;

public interface IItemService
{
    event EventHandler<IEnumerable<ItemViewModel>> OnItemsInitialized;
    event EventHandler<IEnumerable<ItemViewModel>> OnItemsChanged;
    Task InitializeAsync();
    IEnumerable<ItemViewModel> GetItems(Func<ItemViewModel, bool>? selector = null);
    void ClearItems();
    void AddNewItem(Item item);
    void AddOrUpdateItem(ItemViewModel item);
    void ArchiveItem(ItemViewModel item);
    void DeleteItem(ItemViewModel item);
}
