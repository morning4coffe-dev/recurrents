namespace Recurrents.Presentation;

public partial class HomeViewModel : ObservableObject
{
    #region Services
    private readonly IStringLocalizer _localizer;
    private readonly IItemService _itemService;
    private readonly INavigator _navigation;
    private readonly ICurrencyCache _currency;
    private readonly IDispatcher _dispatcher;
    #endregion

    [ObservableProperty]
    private string _welcomeMessage;

    [ObservableProperty]
    public ItemViewModel? _selectedItem;

    public ObservableCollection<ItemViewModel> Items { get; } = [];

    public HomeViewModel(
        IItemService itemService,
        IStringLocalizer localizer,
        INavigator navigation,
        ICurrencyCache currency,
        IDispatcher dispatcher)
    {
        _localizer = localizer;
        _itemService = itemService;
        _navigation = navigation;
        _currency = currency;
        _dispatcher = dispatcher;

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

        _itemService.OnItemsInitialized += (s, e) =>
        {
            _ = RefreshItems();
        };

        _itemService.OnItemsChanged += OnItemsChanged;
        //Currently does this twice only on startup, doesn't impact performance much as the list is null here
        await RefreshItems();
    }

    public void Unload()
    {
        _itemService.OnItemsChanged -= OnItemsChanged;
    }

    private async void OnItemsChanged(object? sender, IEnumerable<ItemViewModel> e)
    {
        SelectedItem = null;

        await RefreshItems(e);
    }

    private async Task<List<ItemViewModel>> RefreshItems(IEnumerable<ItemViewModel>? items = null)
    {
        IEnumerable<ItemViewModel> effectiveItems;

        if (items is { })
        {
            effectiveItems = items.Where(item => !item.IsArchived);
        }
        else
        {
            effectiveItems = _itemService.GetItems(item => !item.IsArchived);
        }

        var orderedItems = effectiveItems.OrderBy(i => i.PaymentDate)
                                          .ToList();

        await _dispatcher.ExecuteAsync(() =>
        {
            Items.Clear();
            Items.AddRange(orderedItems);
        });

        return orderedItems;
    }
}
