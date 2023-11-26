namespace ProjectSBS.Services.Items;

public class ItemService : IItemService
{
    private readonly IDataService _dataService;

    private readonly List<ItemViewModel> _items = [];

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
        AddNewItem(item, logs ?? []);

        SaveDataAsync();
    }

    private void AddNewItem(Item item, List<ItemLog>? logs = null)
    {
        ItemViewModel itemVM = new(item);

        _items.Add(itemVM);
    }

    public void UpdateItem(ItemViewModel item)
    {
        var index = _items.IndexOf(item);
        _items[index] = item;

        SaveDataAsync();
        item.Updated();
    }

    public void ArchiveItem(ItemViewModel item)
    {
        item.Item?.Status.Push(new(item.IsArchived ? State.Active : State.Archived, DateOnly.FromDateTime(DateTime.Now)));

        SaveDataAsync();
        item.Updated();
    }

    public void DeleteItem(ItemViewModel item)
    {
        _items.Remove(item);

        SaveDataAsync();
        item.Updated();
    }

    private void SaveDataAsync()
    {
        var itemsList = _items
            .Select(itemViewModel => itemViewModel.Item)
            .ToList() ?? [];

        if (itemsList is { })
        {
            _dataService.SaveDataAsync(itemsList);
        }

        OnItemsChanged?.Invoke(this, _items);
    }
}
