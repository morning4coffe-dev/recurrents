using Windows.UI.Core;

namespace ProjectSBS.Business.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly IItemService _itemService;
    private readonly IItemFilterService _filterService;
    private readonly INavigation _navigation;

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private string? _displayName;

    [ObservableProperty]
    private decimal _sum;

    [ObservableProperty]
    private Type? _itemDetails;

    [ObservableProperty]
    private string _welcomeMessage;

    [ObservableProperty]
    private bool _isPaneOpen;

    [ObservableProperty]
    public bool _isStatsVisible;

    [ObservableProperty]
    public bool _isLoggedIn;

    private ItemViewModel? _selectedItem;
    public ItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            IsPaneOpen = value is { };

            if (_selectedItem == value)
            {
                return;
            }

            //Created only after user first requests opening item
                //TODO ItemDetails ??= typeof(ItemDetails);

            WeakReferenceMessenger.Default.Send(new ItemSelectionChanged(value, (value?.Item is null)));

            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ItemViewModel> Items { get; } = new();

    public List<Tag> FilterCategories { get; }

    public Tag SelectedFilter
    {
        get => _filterService.SelectedCategory == null ? _filterService.Categories[0] : _filterService.SelectedCategory;
        set
        {
            if (_filterService.SelectedCategory == value)
            {
                return;
            }

            _filterService.SelectedCategory = value;

            WeakReferenceMessenger.Default.Send(new CategorySelectionChanged());

            OnPropertyChanged();

            _ = RefreshItems();
        }
    }

    public NavigationCategory SelectedCategory => _navigation.SelectedCategory;

    //public ICommand Logout { get; }
    public ICommand AddNewCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand OpenSettingsCommand { get; }

    public HomeViewModel(
        IUserService userService,
        IItemService itemService,
        IItemFilterService filterService,
        IStringLocalizer localizer,
        INavigation navigation)
    {
        _userService = userService;
        _itemService = itemService;
        _filterService = filterService;
        _navigation = navigation;

        AddNewCommand = new RelayCommand(AddNew);
        DeleteCommand = new AsyncRelayCommand(() => DeleteItem());
        OpenSettingsCommand = new RelayCommand(() => _navigation.NavigateNested(typeof(SettingsPage)));

        FilterCategories = filterService.Categories;

        _userService.OnLoggedInChanged += (s, e) =>
        {
            User = e;
            IsLoggedIn = e is null;
            DisplayName = User?.Name;
        };

        WelcomeMessage = DateTime.Now.Hour switch
        {
            >= 5 and < 12 => localizer["GoodMorning"],
            >= 12 and < 17 => localizer["GoodAfternoon"],
            >= 17 and < 20 => localizer["GoodEvening"],
            _ => localizer["GoodNight"]
        };
    }

    public override async void Load()
    {
        User = await _userService.GetUser();
        DisplayName = User?.Name;

        _itemService.OnItemsInitialized += (s, e) =>
        {
            _ = RefreshItems();
        };
        //Currently does this twice only on startup, doesn't impact performance much as the list is null here
        _ = RefreshItems();

        IsStatsVisible = _navigation.SelectedCategory.Id == 0;

#if HAS_UNO
        SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
#endif

        WeakReferenceMessenger.Default.Register<ItemUpdated>(this, (r, m) =>
        {
            if (m.Canceled)
            {
                SelectedItem = null;
                return;
            }

            ItemViewModel? item = null;

            if (_selectedItem is not null)
            {
                item = Items.FirstOrDefault(i => i.Item.Id == _selectedItem.Item?.Id);
            }

            if (m.ToSave)
            {
                if (item is null)
                {
                    _itemService.NewItem(m.Item.Item);
                }
                else
                {
                    _itemService.UpdateItem(item);
                }
            }

            SelectedItem = null;

            var items = RefreshItems();

            Sum = items.Sum(i => i.Item.Billing.BasePrice);
        });

        WeakReferenceMessenger.Default.Register<ItemDeleted>(this, (r, m) =>
        {
            DeleteItem(m.Item);
        });
    }

    public override void Unload()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);

#if HAS_UNO
        SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
#endif
    }

    private IEnumerable<ItemViewModel> RefreshItems()
    {
        //TODO Fails here when Filter item gets deselected

        IEnumerable<ItemViewModel> items;

        // Check for sentry value of -1 = None tag
        if (SelectedFilter.Id == -1)
        {
            items = _itemService.GetItems()
            .OrderBy(i => i.PaymentDate);
        }
        else
        {
            items = _itemService.GetItems(item => item.Item.TagId == SelectedFilter.Id)
            .OrderBy(i => i.PaymentDate);
        }

        Items.Clear();
        //TODO Dont clear the whole thing, just removem update and add changed
        Items.AddRange(items);

        return items;
    }

    private void System_BackRequested(object? sender, BackRequestedEventArgs e)
    {
        e.Handled = true;
        SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
    }

    private void AddNew()
    {
        SelectedItem = null;

        WeakReferenceMessenger.Default.Send(
            new ItemSelectionChanged(
                new ItemViewModel(new Item(null, string.Empty)),
                true,
                true)
            );
        IsPaneOpen = true;
    }

    public async Task DeleteItem(ItemViewModel? item = null)
    {
        _itemService.DeleteItem(item ?? SelectedItem);

        SelectedItem = null;
        RefreshItems();

        return;
    }
}