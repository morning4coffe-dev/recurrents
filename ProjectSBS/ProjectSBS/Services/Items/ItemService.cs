namespace ProjectSBS.Services.Items;

public class ItemService(IDataService dataService) : IItemService
{
    private readonly IDataService _dataService = dataService;

    private bool _isInitialized;

    private readonly List<ItemViewModel> _items = [];

    public event EventHandler<IEnumerable<ItemViewModel>>? OnItemsInitialized;
    public event EventHandler<IEnumerable<ItemViewModel>>? OnItemsChanged;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        var (items, _) = await _dataService.InitializeDatabaseAsync();

        foreach (var item in items)
        {
            AddNewItem(item);
        }

        OnItemsInitialized?.Invoke(this, _items);
        _isInitialized = true;
    }

    public IEnumerable<ItemViewModel> GetItems(Func<ItemViewModel, bool>? selector = null)
    {
        if (selector is null)
        {
            return _items;
        }

        return _items.Where(selector);
    }

    public bool ClearItems()
    {
        try
        {
            _items.Clear();
            OnItemsChanged?.Invoke(this, _items);
            _isInitialized = false;
            return true;
        }
        catch
        {
            return false;
        }
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
        if (item?.Item is not { } i)
        {
            return;   
        }

        i.Status.Add(new(item.IsArchived ? State.Active : State.Archived, DateTime.Now));

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
