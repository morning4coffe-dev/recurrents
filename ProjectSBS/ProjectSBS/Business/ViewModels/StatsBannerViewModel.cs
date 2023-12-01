using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ProjectSBS.Business.ViewModels;

public partial class StatsBannerViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly ISettingsService _settingsService;
    private readonly ICurrencyCache _currencyCache;

    public string Header { get; }

    private readonly ObservableCollection<double> _values = [];

    [ObservableProperty]
    private string _sum = "0";

    public ObservableCollection<ISeries> Series { get; set; }

    public StatsBannerViewModel(
        IItemService itemService,
        ISettingsService settingsService,
        IStringLocalizer localizer,
        ICurrencyCache currencyCache)
    {
        _itemService = itemService;
        _settingsService = settingsService;
        _currencyCache = currencyCache;

        Header = string.Format(localizer["LastDays"], DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToLower();

        _itemService.OnItemsChanged += ItemService_OnItemsChanged;
        _itemService.OnItemsInitialized += ItemService_OnItemsChanged;

        SetSum(_itemService.GetItems());

        ObservableCollection<ISeries> series =
        [
            new LineSeries<double>
            {
                Values = _values,
                Fill = null
            }
        ];

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
        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        var items = e
            .Where(item => !item.IsArchived);

        var tasks = items.Select(async item => await _currencyCache.ConvertToDefaultCurrency(
            item.Item?.Billing.BasePrice * item.GetPaymentsInPeriod(days) ?? 0,
            item?.Item?.Billing?.CurrencyId ?? "EUR",
            _settingsService.DefaultCurrency));

        var values = await Task.WhenAll(tasks);
        var sum = values.Sum();

        Sum = $"≈ {Math.Round(sum, 2):n} {_settingsService.DefaultCurrency}";
    }
}
