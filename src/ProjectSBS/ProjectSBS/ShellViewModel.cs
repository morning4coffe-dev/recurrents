using ProjectSBS.Models;
using ProjectSBS.Services.Analytics;
using ProjectSBS.Services.Authentication;
using ProjectSBS.Services.FileManagement.Data;
using ProjectSBS.Services.Interop;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.Notifications;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.UI.Core;

namespace ProjectSBS;

public partial class ShellViewModel : ObservableObject
{
    private readonly ICurrencyCache _api;
    private readonly INotificationService _notifications;
    private readonly IInteropService _interopService;
    private readonly IDataService _dataService;
    private readonly IItemService _itemService;
    private readonly IStringLocalizer _stringLocalizer;
    private readonly IAnalyticsService _analyticsService;

    public ShellViewModel(ICurrencyCache api, INotificationService notifications, IInteropService interop, IDataService data, IItemService item, IStringLocalizer stringLocalizer, IAnalyticsService analytics)
    {
        _api = api;
        _notifications = notifications;
        _interopService = interop;
        _dataService = data;
        _itemService = item;
        _stringLocalizer = stringLocalizer;
        _analyticsService = analytics;

        Items = new ObservableCollection<ItemViewModel>();

        Title = Package.Current.DisplayName;
        Init();

        _page = typeof(MainPage);
    }

    public ObservableCollection<ItemViewModel> Items { get; set; }

    [ObservableProperty]
    public Type _page;

    [ObservableProperty]
    public string _title;

    [RelayCommand]
    private void Theme()
    {
        var random = new Random();

        if (random.Next(0, 2) == 1)
        {
            _interopService.SetTheme(ElementTheme.Light);
            return;
        }
        _interopService.SetTheme(ElementTheme.Dark);
    }

    public async void Init()
    {
        //var c = await _api.GetCurrency(new CancellationToken());

        //_notifications.RemoveScheduledNotifications();

        //var culture = CultureInfo.GetCultureInfo("hu-HU");

        //string notificationTitle = String.Format(_stringLocalizer["ItemNotificationTitle"], $"PremiumSUB {DateTime.Now}", "today");
        //string notificationText = String.Format(_stringLocalizer["ItemNotificationText"], c.Rates["HUF"].ToString("C", culture), "today");


        //TimeOnly time = TimeOnly.FromDateTime(DateTime.Now.Add(TimeSpan.FromSeconds(2)));
        //for (int i = 0; i < 1; i++)
        //{
        //    //time = time.Add(TimeSpan.FromMinutes(2));
        //    _notifications.ScheduleNotification((new Random().Next(0, 1000)).ToString(), notificationTitle, notificationText, DateOnly.FromDateTime(DateTime.Now), time);
        //}

        var currentDate = DateTime.Now;

        // Sample BillingDetails for Item 1
        var billingDetails1 = new BillingDetails(
            basePrice: 50.00m,
            initialDate: new DateOnly(2023, 8, 1),
            currencyId: "USD",
            periodType: Period.Daily,
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
            initialDate: new DateOnly(2019, 4, 16),
            currencyId: "EUR",
            periodType: Period.Daily,
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
            initialDate: new DateOnly(2022, 12, 1),
            currencyId: "GBP",
            periodType: Period.Weekly,
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



        //await _dataService.SaveDataAsync(sampleItems);

        var (items, _) = await _dataService.InitializeDatabaseAsync();

        foreach (var item in items)
        {
            ItemViewModel itemVM = new(item)
            {
                //IsPaid = true
            };

            await itemVM.InitializeAsync();

            _itemService.ScheduleBilling(itemVM);

            Items.Add(itemVM);

            var isPaid = itemVM.IsPaid;
        }

        //Authentication.Authenticate();

        //_interopService.SetTheme(ElementTheme.Light);

        //await Task.Delay(5000);
        //_interopService.SetTheme(ElementTheme.Dark);

        //await Task.Delay(10000);
        //_interopService.SetTheme(ElementTheme.Default);

        //_ = _interopService.OpenStoreReviewUrlAsync();
    }
}
