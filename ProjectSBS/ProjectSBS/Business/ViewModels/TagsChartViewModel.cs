using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ProjectSBS.Business.ViewModels;

public partial class TagsChartViewModel : ViewModelBase
{
    private readonly IItemService _itemService;
    private readonly ICurrencyCache _currencyCache;
    private readonly ISettingsService _settingsService;
    private readonly ITagService _tagService;

    public string Header { get; }

    public ObservableCollection<TagChart> Tags = [];

    public ObservableCollection<ISeries> Series { get; set; } = [];

    public TagsChartViewModel(
        IItemService itemService,
        IStringLocalizer localizer,
        ICurrencyCache currencyCache,
        ISettingsService settingsService,
        ITagService tagService)
    {
        _itemService = itemService;
        _currencyCache = currencyCache;
        _settingsService = settingsService;
        _tagService = tagService;

        Header = string.Format(localizer["LastDays"], DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToLower();

        _itemService.OnItemsChanged += ItemService_OnItemsChanged;
        _itemService.OnItemsInitialized += ItemService_OnItemsChanged;
    }

    private void ItemService_OnItemsChanged(object? sender, IEnumerable<ItemViewModel> e)
    {
        _ = UpdateVisual(e);
    }

    private async Task UpdateVisual(IEnumerable<ItemViewModel> items)
    {
        Series.Clear();
        Tags.Clear();

        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        var activeItems = items.Where(item => !item.IsArchived);
        var values = activeItems.GroupBy(item => item.Item.TagId);

        foreach (var value in values)
        {
            var tag = _tagService.Tags.FirstOrDefault(tag => tag.Id == value.Key);
            if (tag == null) continue;

            var color = new SKColor(tag.Color.Value.R, tag.Color.Value.G, tag.Color.Value.B);

            var tasks = value.Select(async item => await _currencyCache.ConvertToDefaultCurrency(
                item.Item?.Billing.BasePrice * item.GetPaymentsInPeriod(days) ?? 0,
                item?.Item?.Billing?.CurrencyId ?? "EUR",
                _settingsService.DefaultCurrency));

            var convertedPrices = await Task.WhenAll(tasks);
            var sum = convertedPrices.Sum();

            Series.Add(new PieSeries<decimal>
            {
                Values = [sum],
                Name = tag.Name,
                Fill = new SolidColorPaint(color),
            });

            Tags.Add(new(tag.Name, tag.Color, Math.Round(sum, 2)
                .ToString("C", CurrencyCache.CurrencyCultures[_settingsService.DefaultCurrency])));
        }
    }

    public async override void Load()
    {
        await UpdateVisual(_itemService.GetItems());
    }

    public override void Unload()
    {
        _itemService.OnItemsChanged -= ItemService_OnItemsChanged;
        _itemService.OnItemsInitialized -= ItemService_OnItemsChanged;
    }
}
