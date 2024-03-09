using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace ProjectSBS.Business.ViewModels;

public partial class TagsChartViewModel : ViewModelBase
{
    private readonly IItemService _itemService;
    private readonly ISettingsService _settingsService;
    private readonly ICurrencyCache _currencyCache;

    private readonly UISettings _themeListener = new();

    public string Header { get; }

    private readonly ObservableCollection<decimal> _values = [];

    [ObservableProperty]
    private string _sum = "0";

    public ObservableCollection<ISeries> Series { get; set; }

    public TagsChartViewModel(
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

        UpdateVisual(_itemService.GetItems());

        var accentColor = (Color)Application.Current.Resources["SystemAccentColor"];
        var skColor = new SKColor(accentColor.R, accentColor.G, accentColor.B);

        ObservableCollection<ISeries> series =
        [
            new PieSeries<decimal>
            {
                Values = _values,
                Fill = null,
                Stroke = new SolidColorPaint(skColor) { StrokeThickness = 6 },
            }
        ];

        Series = series;

        DateTime currentDate = DateTime.Now;
        var months = new string[6];

        for (int i = 0; i < 6; i++)
        {
            months[5 - i] = currentDate.AddMonths(-i).ToString("MMM", CultureInfo.CurrentCulture);
        }
    }

    private void UISettings_ColorValuesChanged(UISettings sender, object args)
    {
        //TODO: The UISettings.ColorValueChanged is not called on Windows 10
        //https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4412#issuecomment-1823919825

        var accentColor = (Color)Application.Current.Resources["SystemAccentColor"];
        var skColor = new SKColor(accentColor.R, accentColor.G, accentColor.B);

        ((LineSeries<decimal>)Series[0]).Stroke = new SolidColorPaint(skColor) { StrokeThickness = 6 };
        ((LineSeries<decimal>)Series[0]).GeometryStroke = new SolidColorPaint(skColor) { StrokeThickness = 0 };
        ((LineSeries<decimal>)Series[0]).GeometryFill = new SolidColorPaint(skColor) { StrokeThickness = 0 };
    }

    private void ItemService_OnItemsChanged(object? sender, IEnumerable<ItemViewModel> e)
    {
        UpdateVisual(e);
    }

    private async void UpdateVisual(IEnumerable<ItemViewModel> items)
    {
        SetSum(items);

        _values.Clear();
        for (int i = 0; i < 6; i++) 
        {
            var date = DateTime.Now.AddMonths(-i);
            var days = DateTime.DaysInMonth(date.Year, date.Month);

            var values = items.Select(item => 
                item.IsArchived ? 0 : item.Item.TagId
            );

            _values.Insert(0, values.Sum());
        }
    }

    private async void SetSum(IEnumerable<ItemViewModel> e)
    {
    }

    public override void Load()
    {
        _themeListener.ColorValuesChanged += UISettings_ColorValuesChanged;
    }

    public override void Unload()
    {
        _itemService.OnItemsChanged -= ItemService_OnItemsChanged;
        _itemService.OnItemsInitialized -= ItemService_OnItemsChanged;

        _themeListener.ColorValuesChanged -= UISettings_ColorValuesChanged;
    }
}
