using ProjectSBS.Models;
using ProjectSBS.Services.FileManagement.Data;
using ProjectSBS.Services.Interop;
using ProjectSBS.Services.Notifications;
using System.Collections.ObjectModel;

namespace ProjectSBS;

public partial class ShellViewModel : ObservableObject
{
    private readonly ICurrencyCache _api;
    private readonly INotificationService _notifications;
    private readonly IInteropService _interopService;
    private readonly IDataService _dataService;

    public ShellViewModel(ICurrencyCache api, INotificationService notifications, IInteropService interop, IDataService data)
    {
        _api = api;
        _notifications = notifications;
        _interopService = interop;
        _dataService = data;

        Title = Package.Current.DisplayName;
        Init();
    }

    public ObservableCollection<Item> Items { get; set; }

    [ObservableProperty]
    public string _title;

    public async void Init()
    {
        var c = await _api.GetCurrency(new CancellationToken());

        //TODO clean planned notifications

        //_notifications.ShowBasicToastNotification("CZK is currently", c.Rates["CZK"].ToString() + " Kč");

        var currentDate = DateTime.Now;

        // Sample BillingDetails for Item 1
        var billingDetails1 = new BillingDetails(
            basePrice: 50.00m,
            initialDate: DateOnly.FromDateTime(currentDate),
            currencyId: "USD",
            periodType: Period.Monthly,
            recurEvery: 1
        );

        // Sample Item 1
        var sampleItem1 = new Item(
            id: "001",
            name: "Sample Item 1",
            billing: billingDetails1,
            tagId: "tag-001",
            description: "This is a sample item for testing purposes.",
            creationDate: currentDate
        );

        // Sample BillingDetails for Item 2
        var billingDetails2 = new BillingDetails(
            basePrice: 30.00m,
            initialDate: DateOnly.FromDateTime(currentDate),
            currencyId: "EUR",
            periodType: Period.Quarterly,
            recurEvery: 2
        );

        // Sample Item 2
        var sampleItem2 = new Item(
            id: "002",
            name: "Sample Item 2",
            billing: billingDetails2,
            tagId: "tag-002",
            description: "This is another sample item for testing purposes.",
            creationDate: currentDate
        );

        // Sample BillingDetails for Item 3
        var billingDetails3 = new BillingDetails(
            basePrice: 100.00m,
            initialDate: DateOnly.FromDateTime(currentDate),
            currencyId: "GBP",
            periodType: Period.Annually,
            recurEvery: 1
        );

        // Sample Item 3
        var sampleItem3 = new Item(
            id: "003",
            name: "Sample Item 3",
            billing: billingDetails3,
            tagId: "tag-003",
            description: "This is a third sample item for testing purposes.",
            creationDate: currentDate
        );

        // Create a list of items
        var sampleItems = new List<Item>
        {
            sampleItem1,
            sampleItem2,
            sampleItem3
        };

        await _dataService.SaveLocalAsync(sampleItems);

        var results = await _dataService.InitializeDatabaseAsync();

        Items = new ObservableCollection<Item>();

        foreach (var item in results)
        {
            Items.Add(item);
        }

        _interopService.SetTheme(ElementTheme.Light);

        await Task.Delay(5000);
        _interopService.SetTheme(ElementTheme.Dark);

        await Task.Delay(10000);
        _interopService.SetTheme(ElementTheme.Default);


        //_ = _interopService.OpenStoreReviewUrlAsync();
    }
}
