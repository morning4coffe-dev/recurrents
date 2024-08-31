namespace Recurrents.Presentation;

public partial class HomeViewModel : ObservableObject
{
    #region Services
    private readonly IStringLocalizer _localizer;
    //private readonly IUserService _userService;
    private readonly IItemService _itemService;
    //private readonly IItemFilterService _filterService;
    private readonly INavigator _navigation;
    private readonly ICurrencyCache _currency;
    private readonly IDispatcher _dispatcher;
    //private readonly IDialogService _dialog;
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

    [ObservableProperty]
    public ItemViewModel? _selectedItem;

    public ObservableCollection<ItemViewModel> Items { get; } = [];


    //public List<Tag> FilterCategories => _filterService.Categories;

    //public Tag SelectedFilter
    //{
    //    get => _filterService.SelectedCategory ?? _filterService.Categories[0];
    //    set
    //    {
    //        if (_filterService.SelectedCategory == value)
    //        {
    //            return;
    //        }

    //        _filterService.SelectedCategory = value;
    //        WeakReferenceMessenger.Default.Send(new CategorySelectionChanged());

    //        OnPropertyChanged();
    //        _ = RefreshItems();
    //    }
    //}

    public HomeViewModel(
        //IUserService userService,
        IItemService itemService,
        //IItemFilterService filterService,
        IStringLocalizer localizer,
        INavigator navigation,
        ICurrencyCache currency,
        IDispatcher dispatcher
        )
    {
        _localizer = localizer;
        //_userService = userService;
        _itemService = itemService;
        //_filterService = filterService;
        _navigation = navigation;
        _currency = currency;
        _dispatcher = dispatcher;

        //_userService.OnLoggedInChanged += (s, e) =>
        //{
        //    User = e;
        //    IsLoggedIn = e is not null;
        //    DisplayName = User?.Name;
        //};

        WelcomeMessage = DateTime.Now.Hour switch
        {
            >= 5 and < 12 => localizer["GoodMorning"],
            >= 12 and < 17 => localizer["GoodAfternoon"],
            >= 17 and < 20 => localizer["GoodEvening"],
            _ => localizer["GoodNight"]
        };

        Load();
    }

    public async void Load()
    {
        try
        {
            var currency = await _currency.GetCurrency(CancellationToken.None);
        }
        catch (Exception ex)
        {

        }

        _ = _itemService.InitializeAsync();

        //User = await _userService.RetrieveUser();
        //DisplayName = User?.Name;

        SelectedItem = null;

        _itemService.OnItemsInitialized += (s, e) =>
        {
            _ = RefreshItems();

            _itemService.OnItemsChanged += OnItemsChanged;
        };
        //Currently does this twice only on startup, doesn't impact performance much as the list is null here
        _ = RefreshItems();

        //IsStatsVisible = _navigation.SelectedCategory.Id == 0;

#if HAS_UNO
        //There is a difference in how Navigation and NavigationCacheMode is handled across platforms, 
        //FeatureConfiguration.Frame.UseWinUIBehavior could fix it, but doesn't seem to work with extensions navigation

        //_itemService.OnItemsChanged -= OnItemsChanged
#endif
    }

    public void Unload()
    {
        _itemService.OnItemsChanged -= OnItemsChanged;
    }

    private void OnItemsChanged(object? sender, IEnumerable<ItemViewModel> e)
    {
        SelectedItem = null;

        //var items =
        RefreshItems(e);

        //Sum = items.Sum(i => i.Item.Billing.BasePrice);
    }

    private IEnumerable<ItemViewModel> RefreshItems(IEnumerable<ItemViewModel>? items = default)
    {
        var effectiveItems = items ?? _itemService.GetItems(item => !item.IsArchived)
                                                   .OrderBy(i => i.PaymentDate);

        var filteredAndSortedItems = effectiveItems.Where(item => !item.IsArchived)
                                                   .OrderBy(i => i.PaymentDate)
                                                   .ToList();

        _dispatcher.TryEnqueue(() =>
        {
            Items.Clear();
            Items.AddRange(filteredAndSortedItems);
        });

        return filteredAndSortedItems;
    }

    //    [RelayCommand]
    //    private void AddNew()
    //    {
    //        SelectedItem = null;

    //        WeakReferenceMessenger.Default.Send(
    //            new ItemSelectionChanged(
    //                new ItemViewModel(new Item(null, string.Empty)),
    //                true,
    //                true)
    //            );
    //        IsPaneOpen = true;

    //        AnalyticsService.TrackEvent(AnalyticsService.ItemEvent, "Added", "True");
    //    }

    //    [RelayCommand]
    //    public async Task Archive(ItemViewModel? item = null)
    //    {
    //        if (item is not { } && SelectedItem is not { })
    //        {
    //            return;
    //        }

    //        ContentDialogResult result;

    //        if (!(item ?? SelectedItem).IsArchived)
    //        {
    //            result = await _dialog.ShowAsync(
    //                _localizer["ArchiveDialogTitle"],
    //                _localizer["ArchiveDialogDescription"],
    //                _localizer["ArchiveVerb"]);
    //        }
    //        else
    //        {
    //            result = ContentDialogResult.Primary;
    //        }

    //        if (result == ContentDialogResult.Primary)
    //        {
    //            _itemService.ArchiveItem(item ?? SelectedItem);

    //            AnalyticsService.TrackEvent(AnalyticsService.ItemEvent, "Archived",
    //                (item ?? SelectedItem).IsArchived.ToString());

    //            SelectedItem = null;
    //            RefreshItems();
    //        }
    //    }

    //    [RelayCommand]
    //    public async Task Delete(ItemViewModel? item = null)
    //    {
    //        if (item is not { } && SelectedItem is not { })
    //        {
    //            return;
    //        }

    //        var result = await _dialog.ShowAsync(
    //            _localizer["DeleteDialogTitle"],
    //            _localizer["DeleteDialogDescription"],
    //            _localizer["Delete"]);

    //        if (result == ContentDialogResult.Primary)
    //        {
    //            _itemService.DeleteItem(item ?? SelectedItem);

    //            AnalyticsService.TrackEvent(AnalyticsService.ItemEvent, "Deleted", "True");

    //            SelectedItem = null;
    //            RefreshItems();
    //        }
    //    }

    [RelayCommand]
    private void OpenSettings()
        => _navigation.NavigateViewModelAsync<SettingsViewModel>(this);
}
