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

    private List<ItemViewModel> GetItems
        => _itemService
            .GetItems(item => !item.IsArchived)
            .OrderBy(i => i.PaymentDate)
            .ToList();

    public async void Load()
    {
        try
        {
            var currency = await _currency.GetCurrency(CancellationToken.None);
        }
        catch (Exception ex)
        {

        }

        await _itemService.InitializeAsync();

        if (Items.Count > 0)
        {
            Items.Clear();
        }

        if (GetItems is { } items && items.Count > 0)
        {
            foreach (var item in items)
            {
                await _dispatcher.ExecuteAsync(() =>
                {
                    Items.Add(item);
                });
            }
        }

        RefreshSum(Items);

        _itemService.OnItemChanged += OnItemChanged;
    }

    public void Unload()
    {
        _itemService.OnItemChanged -= OnItemChanged;
    }

    private void OnItemChanged(object? sender, ItemViewModel e)
    {
        var items = _itemService.GetItems().ToList();

        if (items is not { })
        {
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Item.Id == e.Item.Id)
            {
                //if (!items[i].Equals(e)) //Equals doesn't work on all properties
                Items[i] = e;
                return;
            }
        }

        Items.Add(e);

        RefreshSum(Items);
    }

    private async void RefreshSum(IEnumerable<ItemViewModel> items)
    {
        try
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
        catch
        {
            //TODO: Make show Error more user friendly
            Sum = "Connection error.";
        }
    }
}
