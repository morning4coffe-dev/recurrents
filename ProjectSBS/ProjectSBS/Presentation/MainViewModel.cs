using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.Notifications;
using ProjectSBS.Services.Storage.Data;
using ProjectSBS.Services.User;
using System.Collections.ObjectModel;
using Windows.System.Profile;
using ProjectSBS.Business;

namespace ProjectSBS.Presentation;

public partial class MainViewModel : ObservableObject
{
    private IAuthenticationService _authentication;
    private IUserService _userService;

    private readonly IItemService _itemService;
    private readonly INotificationService _notificationService;
    private readonly ICurrencyCache _api;
    private readonly IDataService _dataService;

    private IDispatcher _dispatch;
    private INavigator _navigator;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private bool _isEditing;

    public bool IsMobile
    {
        get
        {
            var deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            return !deviceFamily.Contains("Mobile");
        }
    }

    public string? Title { get; }

    public ObservableCollection<ItemViewModel> Items { get; } = new();

    private ItemViewModel? _selectedItem;

    public ItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (_selectedItem == value)
            {
                return;
            }

            if (IsMobile && value != null)
            {
                SentItem = value;
                _ = _navigator.NavigateViewModelAsync<ItemDetailsViewModel>(this);
            }

            IsEditing = false;

            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    private static ItemViewModel? _sentItem;
    public static ItemViewModel? SentItem
    {
        get => _sentItem;
        set
        {
            _sentItem = value;
        }
    }

    public ICommand GoToSecond { get; }
    public ICommand Logout { get; }
    public ICommand AddNewCommand { get; }
    public ICommand SubmitChangesCommand { get; }
    public ICommand CloseDetailsCommand { get; }
    public ICommand EnableEditingCommand { get; }
    public ICommand DeleteItemCommand { get; }

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        IAuthenticationService authentication,
        IUserService userService,
        IItemService itemService,
        IDispatcher dispatch,
        ICurrencyCache api,
        //INotificationService notifications,
        INavigator navigator,
        IDataService dataService)
    {
        _authentication = authentication;
        _userService = userService;
        _itemService = itemService;
        //_notificationService = notifications;
        _api = api;
        _dispatch = dispatch;
        _navigator = navigator;

        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        GoToSecond = new AsyncRelayCommand(GoToSecondView);
        Logout = new AsyncRelayCommand(DoLogout);
        AddNewCommand = new RelayCommand(AddNew);
        SubmitChangesCommand = new AsyncRelayCommand(SubmitChanges);
        CloseDetailsCommand = new AsyncRelayCommand(CloseDetails);
        EnableEditingCommand = new AsyncRelayCommand(EnableEditing);
        DeleteItemCommand = new AsyncRelayCommand(DeleteItem);

        Initialize();
        _dataService = dataService;

        //if (IsMobile)
        {
            WeakReferenceMessenger.Default.Register<ItemSelectionChanged>(this, (r, m) =>
            {
                if (SelectedItem is null)
                {
                    SelectedItem = null;
                    return;
                }
                var item = Items.Where(i => i.Item.Id == SelectedItem.Item.Id).First();

                item = m.Item;

                var items = Items;

                SelectedItem = null;
                //m.Item;
            });
        }
    }

    public async void Initialize()
    {
        var user = await _userService.GetUser();

        await _dispatch.ExecuteAsync(() =>
        {
            if (user is not null)
            {
                User = user;
                Name = User.Name;
            }
            else
            {
                //TODO enable login button
            }
        });

        var c = await _api.GetCurrency(new CancellationToken());

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

            //_itemService.ScheduleBilling(itemVM);

            await _dispatch.ExecuteAsync(() =>
            {
                Items.Add(itemVM);
            });

            var isPaid = itemVM.IsPaid;
        }
    }

    private async Task GoToSecondView()
    {
        //await _navigator.NavigateViewModelAsync<ItemDetailsViewModel>(this, data: new Entity(Name!));
    }

    public async Task DoLogout(CancellationToken token)
    {
        await _authentication.LogoutAsync(_dispatch, token);
        //TODO probably will have to clean the token too
    }

    private void AddNew()
    {
        SelectedItem = new ItemViewModel(null);

        IsEditing = true;
    }

    private async Task SubmitChanges()
    {
        //var item = Items.Where(i => i.Item.Id == SelectedItem.Item.Id).First();

        //item = 
        //TODO Add new item to database
        SelectedItem = null;

        IsEditing = false;
    }

    private async Task EnableEditing()
    {
        IsEditing = true;
    }

    private async Task DeleteItem()
    {
        //TODO Find item by Id in database and delete it
        return;
    }

    private async Task CloseDetails()
    {
        //TODO Instead of IsEditing, check if the item is dirty
        if (IsEditing)
        {
            await _navigator.ShowMessageDialogAsync(this, title: "...", content: "Really?");
        }
        else
        {
            SelectedItem = null;
        }

        IsEditing = false;
    }
}