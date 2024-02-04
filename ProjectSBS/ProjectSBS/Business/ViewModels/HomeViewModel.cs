using Windows.UI.Core;

namespace ProjectSBS.Business.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    #region Services
    private readonly IStringLocalizer _localizer;
    private readonly IUserService _userService;
    private readonly IItemService _itemService;
    private readonly IItemFilterService _filterService;
    private readonly INavigation _navigation;
    private readonly IDialogService _dialog;
    #endregion

    #region Localization Strings
    public string NewItemText => _localizer["NewItem"];
    public string DeleteText => _localizer["Delete"];
    public string UnarchiveText => _localizer["Unarchive"];
    public string ItemsEmptyTitleText => _localizer["ItemsEmptyTitle"];
    public string ItemsEmptyDescriptionText => _localizer["ItemsEmptyDescription"];
    public string ArchiveEmptyTitleText => _localizer["ItemsEmptyTitle"];
    public string ArchiveEmptyDescriptionText => _localizer["ArchiveEmptyDescription"];
    #endregion

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private string? _displayName;

    [ObservableProperty]
    private decimal _sum;

    [ObservableProperty]
    private string _welcomeMessage;

    [ObservableProperty]
    private bool _isPaneOpen;

    [ObservableProperty]
    private bool _isStatsVisible;

    [ObservableProperty]
    private bool _isLoggedIn;

    public bool IsEdit;

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

            WeakReferenceMessenger.Default.Send(new ItemSelectionChanged(value, IsEdit, (value?.Item is null)));
            IsEdit = false;

            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ItemViewModel> Items { get; } = [];

    public List<Tag> FilterCategories => _filterService.Categories;

    public Tag SelectedFilter
    {
        get => _filterService.SelectedCategory ?? _filterService.Categories[0];
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

    public HomeViewModel(
        IUserService userService,
        IItemService itemService,
        IItemFilterService filterService,
        IStringLocalizer localizer,
        INavigation navigation,
        IDialogService dialog)
    {
        _localizer = localizer;
        _userService = userService;
        _itemService = itemService;
        _filterService = filterService;
        _navigation = navigation;
        _dialog = dialog;

        FilterCategories = filterService.Categories;

        _userService.OnLoggedInChanged += (s, e) =>
        {
            User = e;
            IsLoggedIn = e is not null;
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
        User = await _userService.RetrieveUser();
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

        WeakReferenceMessenger.Default.Register<ItemArchived>(this, (r, m) =>
        {
            _ = Archive(m.Item);
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
        IEnumerable<ItemViewModel> items;

        //TODO Must add a listener for when the category changes

        if (SelectedCategory is not { } category)
        {
            return new List<ItemViewModel>();
        }

        //If ArchivePage is selected, show archived items
        if (category.Page == typeof(ArchivePage))
        {
            items = _itemService.GetItems(item => item.IsArchived)
            .OrderBy(i => i.PaymentDate);
        }
        else
        {
            // Check for sentry value of -1 = None tag
            if (SelectedFilter.Id == -1)
            {
                items = _itemService.GetItems(item => !item.IsArchived)
                .OrderBy(i => i.PaymentDate);
            }
            else
            {
                items = _itemService.GetItems(item => !item.IsArchived && item.Item?.TagId == SelectedFilter.Id)
                .OrderBy(i => i.PaymentDate);
            }
        }

        Items.Clear();
        Items.AddRange(items);

        return items;
    }

    private void System_BackRequested(object? sender, BackRequestedEventArgs e)
    {
        e.Handled = true;
        SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
    }

    [RelayCommand]
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

        AnalyticsService.TrackEvent(AnalyticsService.ItemEvent, "Added", "True");
    }

    [RelayCommand]
    public async Task Archive(ItemViewModel? item = null)
    {
        ContentDialogResult result;

        if (!(item ?? SelectedItem).IsArchived)
        {
            result = await _dialog.ShowAsync(
                _localizer["ArchiveDialogTitle"],
                _localizer["ArchiveDialogDescription"],
                _localizer["ArchiveVerb"]);
        }
        else
        {
            result = ContentDialogResult.Primary;
        }

        if (result == ContentDialogResult.Primary)
        {
            _itemService.ArchiveItem(item ?? SelectedItem);

            SelectedItem = null;
            RefreshItems();

            AnalyticsService.TrackEvent(AnalyticsService.ItemEvent, "Archived",
                (item ?? SelectedItem).IsArchived.ToString());
        }
    }

    [RelayCommand]
    public async Task Delete(ItemViewModel? item = null)
    {
        var result = await _dialog.ShowAsync(
            _localizer["DeleteDialogTitle"],
            _localizer["DeleteDialogDescription"],
            _localizer["Delete"]);

        if (result == ContentDialogResult.Primary)
        {
            _itemService.DeleteItem(item ?? SelectedItem);

            SelectedItem = null;
            RefreshItems();

            AnalyticsService.TrackEvent(AnalyticsService.ItemEvent, "Deleted", "True");
        }
    }

    [RelayCommand]
    private void OpenSettings()
    {
        _navigation.NavigateNested(typeof(SettingsPage));
    }
}
