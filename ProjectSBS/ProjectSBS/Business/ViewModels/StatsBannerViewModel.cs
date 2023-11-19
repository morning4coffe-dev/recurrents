using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ProjectSBS.Business.ViewModels;

public partial class StatsBannerViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly ISettingsService _settingsService;
    private readonly ICurrencyCache _currencyCache;

    private readonly ObservableCollection<double> _values = [];

    [ObservableProperty]
    private string _sum = "0";

    public ObservableCollection<ISeries> Series { get; set; }

    public StatsBannerViewModel(
        IItemService itemService,
        ISettingsService settingsService,
        ICurrencyCache currencyCache)
    {
        _itemService = itemService;
        _settingsService = settingsService;
        _currencyCache = currencyCache;

        //observe ItemService that items have changed
        //_sum = $"{new Random().Next(0, 50000)} CZK";

        _itemService.OnItemsChanged += ItemService_OnItemsChanged;
        _itemService.OnItemsInitialized += ItemService_OnItemsChanged;

        SetSum(_itemService.GetItems());

        ObservableCollection<ISeries> series = new()
            {
                new LineSeries<double>
                {
                    Values = _values,
                    Fill = null
                }
            };

        Series = series;
    }

    private void ItemService_OnItemsChanged(object? sender, IEnumerable<ItemViewModel> e)
    {
        //TODO Sum for current period
        SetSum(e);
        //    _values.Add(e.ToList().Count);

        //    ObservableCollection<ISeries> series = new()
        //    {
        //        new LineSeries<double>
        //        {
        //            Values = _values,
        //            Fill = null
        //        }
        //    };

        //    Series = series;
    }

    private async void SetSum(IEnumerable<ItemViewModel> e)
    {
        var items = e
            .Where(item => item?.PaymentDate <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        var tasks = items.Select(async item => await _currencyCache.ConvertToDefaultCurrency(
            item.Item?.Billing.BasePrice ?? 0,
            item?.Item?.Billing?.CurrencyId ?? "EUR",
            _settingsService.DefaultCurrency));

        var values = await Task.WhenAll(tasks);
        var sum = values.Sum();

        Sum = $"≈ {Math.Round(sum, 2)} {_settingsService.DefaultCurrency}";
    }

    private async Task Load()
    {
        var items = _itemService.GetItems().ToList();
        while (items.Count == 0)
        {
            items = _itemService.GetItems().ToList();
            return;
        }

    }
}
