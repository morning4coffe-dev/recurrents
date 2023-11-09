using ProjectSBS.Services.Items.Billing;
using ProjectSBS.Services.Notifications;
using ProjectSBS.Services.Storage.Data;

namespace ProjectSBS.Services.Items;

public class ItemService : IItemService
{
    private readonly IBillingService _billing;
    private readonly IDataService _dataService;
#if !__WASM__
    private readonly INotificationService _notification;
#endif

    private readonly List<ItemViewModel> _items = new();

    public event EventHandler<bool>? OnItemsInitialized;
    //public event EventHandler<IEnumerable<ItemViewModel>> OnItemsChanged;

    public ItemService(
        IBillingService billing,
        IDataService dataService,
#if !HAS_UNO_WASM
        INotificationService notification
#endif
        )
    {
        _billing = billing;
        _dataService = dataService;
#if !HAS_UNO_WASM
        _notification = notification;
#endif
    }

    public async Task InitializeAsync()
    {
        var (items, logs) = await _dataService.InitializeDatabaseAsync();

        foreach (var item in items)
        {
            AddNewItem(item, logs);
        }

        OnItemsInitialized?.Invoke(this, true);
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

        logs ??= new();

        ScheduleBilling(itemVM, logs);
        _items.Add(itemVM);
    }

    public void UpdateItem(ItemViewModel item)
    {
        var index = _items.IndexOf(item);
        _items[index] = item;

        SaveDataAsync();

        //OnItemsChanged.Invoke(this, _items);
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
        _dataService.SaveDataAsync(itemsList);
    }

    public ItemViewModel ScheduleBilling(ItemViewModel itemVM, List<ItemLog> logs)
    {
        var item = itemVM.Item;
        var paymentDates = _billing.GetFuturePayments(item.Billing.InitialDate, item.Billing.PeriodType, item.Billing.RecurEvery);

        Task.Run(() =>
        {
            foreach (var date in paymentDates)
            {
                //TODO Remove before scheduling new 
#if !HAS_UNO_WASM
                //_notification.ScheduleNotification(item.Id, item.Name, DateTime.Now.ToString(), date, new TimeOnly(8, 00));
#endif
            }
        });

        return itemVM;
    }
}
