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

    public List<ItemViewModel> GetItems()
    {
        return _items;
    }

    public void NewItem(Item item, List<ItemLog>? logs = null)
    {
        AddNewItem(item, logs ?? new());

        var itemsList = _items
            .Select(itemViewModel => itemViewModel.Item)
            .ToList() ?? new();
        _dataService.SaveDataAsync(itemsList);
    }

    private void AddNewItem(Item item, List<ItemLog>? logs = null)
    {
        ItemViewModel itemVM = new(item);

        logs ??= new();

        itemVM.Initialize(logs);

        ScheduleBilling(itemVM, logs);
        _items.Add(itemVM);
    }

    public ItemViewModel ScheduleBilling(ItemViewModel itemVM, List<ItemLog> logs)
    {
        var item = itemVM.Item;
        var paymentDates = _billing.GetFuturePayments(item.Billing.InitialDate, item.Billing.PeriodType, item.Billing.RecurEvery);

        Task.Run(() =>
        {
            foreach (var date in paymentDates)
            {
                _notification.ScheduleNotification(item.Id, item.Name, DateTime.Now.ToString(), date, new TimeOnly(8, 00));
            }
        });

        return itemVM;
    }
}
