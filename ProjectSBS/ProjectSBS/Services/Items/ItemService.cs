using ProjectSBS.Services.Storage.Data;

namespace ProjectSBS.Services.Items;

public class ItemService : IItemService
{
    private readonly IDataService _dataService;

    private readonly List<ItemViewModel> _items = new();

    public event EventHandler<IEnumerable<ItemViewModel>>? OnItemsInitialized;
    public event EventHandler<IEnumerable<ItemViewModel>>? OnItemsChanged;

    public ItemService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task InitializeAsync()
    {
        var (items, logs) = await _dataService.InitializeDatabaseAsync();

        foreach (var item in items)
        {
            AddNewItem(item, logs);
        }

        OnItemsInitialized?.Invoke(this, _items);
    }

    public IEnumerable<ItemViewModel> GetItems(Func<ItemViewModel, bool>? selector = null)
    {
        if (selector is null)
        {
            return _items;
        }

        return _items.Where(selector);
    }

    public void NewItem(Item item, List<ItemLog>? logs = null)
    {
        AddNewItem(item, logs ?? new());

        SaveDataAsync();
    }

    private void AddNewItem(Item item, List<ItemLog>? logs = null)
    {
        ItemViewModel itemVM = new(item);

        //logs ??= new();

        _items.Add(itemVM);
    }

    public void UpdateItem(ItemViewModel item)
    {
        var index = _items.IndexOf(item);
        _items[index] = item;

        SaveDataAsync();
        item.Updated();

        OnItemsChanged?.Invoke(this, _items);
    }

    public void DeleteItem(ItemViewModel item)
    {
        _items.Remove(item);

        SaveDataAsync();
    }

    private void SaveDataAsync()
    {
        var itemsList = _items
            .Select(itemViewModel => itemViewModel.Item)
            .ToList() ?? new();

        if (itemsList is { })
        {
            _dataService.SaveDataAsync(itemsList);
        }
    }
}
