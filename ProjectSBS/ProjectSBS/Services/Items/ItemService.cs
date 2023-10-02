using ProjectSBS.Services.Items.Billing;
using ProjectSBS.Services.Notifications;
using ProjectSBS.Services.Storage.Data;

namespace ProjectSBS.Services.Items;

public class ItemService : IItemService
{
    private readonly IBillingService _billing;
    private readonly IDataService _dataService;
    private readonly INotificationService _notification;

    private readonly List<ItemViewModel> _items = new();

    public ItemService(
        IBillingService billing,
        IDataService dataService,
        INotificationService notification)
    {
        _billing = billing;
        _dataService = dataService;
        _notification = notification;
    }

    public async Task InitializeAsync()
    {
        var (items, logs) = await _dataService.InitializeDatabaseAsync();

        foreach (var item in items)
        {
            AddNewItem(item, logs);
        }
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

        itemVM.Initialize(logs);

        ScheduleBilling(itemVM, logs);
        _items.Add(itemVM);
    }

    public void UpdateItem(ItemViewModel item)
    {
        var index = _items.IndexOf(item);
        _items[index] = item;

        SaveDataAsync();
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
                //TODO _notification.ScheduleNotification(item.Id, item.Name, DateTime.Now.ToString(), date, new TimeOnly(8, 00));
            }
        });

        return itemVM;
    }
}
