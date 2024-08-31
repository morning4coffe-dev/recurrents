namespace Recurrents.Services.Items;

public class ItemService(IDataService dataService) : IItemService
{
    private readonly IDataService _dataService = dataService;
    private bool _isInitialized = false;

    private readonly List<ItemViewModel> _items = [];

    public event EventHandler<IEnumerable<ItemViewModel>>? OnItemsInitialized;
    public event EventHandler<IEnumerable<ItemViewModel>>? OnItemsChanged;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        var (items, _) = await _dataService.InitializeDatabaseAsync();

        foreach (var item in items)
        {
            AddNewItem(item);
        }

        RaiseItemsInitialized();
        _isInitialized = true;
    }

    public IEnumerable<ItemViewModel> GetItems(Func<ItemViewModel, bool>? selector = null) =>
        selector is null ? _items : _items.Where(selector);

    public void ClearItems()
    {
        _items.Clear();
        RaiseItemsChanged();
        _isInitialized = false;
    }

    public void AddNewItem(Item item, List<ItemLog>? logs = null)
    {
        var itemVM = new ItemViewModel(item);
        _items.Add(itemVM);

        SaveDataAsync().ConfigureAwait(false);
    }

    public void AddOrUpdateItem(ItemViewModel item)
    {
        var index = _items.IndexOf(item);
        if (index >= 0)
        {
            _items[index] = item;
        }
        else
        {
            _items.Add(item);
        }

        SaveDataAsync().ConfigureAwait(false);
        item.Updated();
    }

    public void ArchiveItem(ItemViewModel item)
    {
        if (item?.Item is not { } i)
            return;

        i.Status.Add(new(item.IsArchived ? State.Active : State.Archived, DateTime.Now));
        SaveDataAsync().ConfigureAwait(false);
        item.Updated();
    }

    public void DeleteItem(ItemViewModel item)
    {
        _items.Remove(item);
        SaveDataAsync().ConfigureAwait(false);
        item.Updated();
    }

    private async Task SaveDataAsync()
    {
        var itemsList = _items.Select(itemViewModel => itemViewModel.Item).ToList();

        if (itemsList is { })
        {
            await _dataService.SaveDataAsync(itemsList);
        }

        RaiseItemsChanged();
    }

    private void RaiseItemsInitialized() => OnItemsInitialized?.Invoke(this, _items);

    private void RaiseItemsChanged() => OnItemsChanged?.Invoke(this, _items);
}
