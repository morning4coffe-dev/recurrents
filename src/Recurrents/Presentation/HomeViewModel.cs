namespace Recurrents.Presentation;

public partial class HomeViewModel : ObservableObject
{
    #region Services
    private readonly IStringLocalizer _localizer;
    private readonly IItemService _itemService;
    private readonly INavigator _navigation;
    private readonly ICurrencyCache _currency;
    private readonly IDispatcher _dispatcher;
    private readonly ISettingsService _settings;
    #endregion

    [ObservableProperty]
    private string _welcomeMessage;

    [ObservableProperty]
    private string _bannerHeader;

    [ObservableProperty]
    private string _sum = "0";

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

            _selectedItem = value;
            OnPropertyChanged();

            if (value is { })
            {
                _navigation.NavigateViewModelAsync<ItemDetailsViewModel>(this, data: value);
            }
        }
    }

    public ObservableCollection<ItemViewModel> Items { get; } = [];

    public HomeViewModel(
        IItemService itemService,
        IStringLocalizer localizer,
        INavigator navigation,
        ICurrencyCache currency,
        IDispatcher dispatcher,
        ISettingsService settings)
    {
        _localizer = localizer;
        _itemService = itemService;
        _navigation = navigation;
        _currency = currency;
        _dispatcher = dispatcher;
        _settings = settings;

        BannerHeader = string.Format(localizer["LastDays"], DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

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
            RefreshItems();
        };

        _itemService.OnItemsChanged += OnItemsChanged;
        //Currently does this twice only on startup, doesn't impact performance much as the list is null here
        RefreshItems();
    }

    public void Unload()
    {
        _itemService.OnItemsChanged -= OnItemsChanged;
    }

    private void OnItemsChanged(object? sender, IEnumerable<ItemViewModel> e)
    {
        SelectedItem = null;

        RefreshItems(e);
    }

    private async void RefreshItems(IEnumerable<ItemViewModel>? items = null)
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

        RefreshSum(orderedItems);
    }

    private async void RefreshSum(IEnumerable<ItemViewModel> items)
    {
        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        var tasks = items.Select(async item => await _currency.ConvertToDefaultCurrency(
            item.Item?.Billing.BasePrice * item.GetPaymentsInPeriod(days) ?? 0,
            item?.Item?.Billing?.CurrencyId ?? _settings.DefaultCurrency,
            _settings.DefaultCurrency));

        var values = await Task.WhenAll(tasks);
        var sum = values.Sum();

        await _dispatcher.ExecuteAsync(() =>
        {
            Sum = $"≈ {Math.Round(sum, 2).ToString("C", CurrencyCache.CurrencyCultures[_settings.DefaultCurrency])}";
        });
    }
}
