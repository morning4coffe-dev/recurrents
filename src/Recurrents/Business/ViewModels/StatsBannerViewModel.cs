using CommunityToolkit.WinUI.Helpers;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.UI.Dispatching;
using SkiaSharp;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace Recurrents.Business.ViewModels;

public partial class StatsBannerViewModel : ViewModelBase
{
    private readonly IItemService _itemService;
    private readonly ISettingsService _settingsService;
    private readonly ICurrencyCache _currencyCache;

    private readonly UISettings _themeListener = new();

    private Color _accentColor = (Color)Application.Current.Resources["SystemAccentColor"];
    private Color _textColor => (Color)Application.Current.Resources["TextFillColorPrimary"];

    public string Header { get; }

    private readonly ObservableCollection<decimal> _values = [];

    [ObservableProperty]
    private string _sum = "0";

    public ObservableCollection<ISeries> Series { get; set; }
    public ObservableCollection<ICartesianAxis> XAxes { get; set; }
    public ObservableCollection<ICartesianAxis> YAxes { get; set; }

    public StatsBannerViewModel(
        IItemService itemService,
        ISettingsService settingsService,
        IStringLocalizer localizer,
        ICurrencyCache currencyCache)
    {
        _itemService = itemService;
        _settingsService = settingsService;
        _currencyCache = currencyCache;

        Header = string.Format(localizer["LastDays"], DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

        _itemService.OnItemsChanged += ItemService_OnItemsChanged;
        _itemService.OnItemsInitialized += ItemService_OnItemsChanged;

        UpdateVisual(_itemService.GetItems());

        ObservableCollection<ISeries> series =
        [
            new LineSeries<decimal>
            {
                Values = _values,
                Fill = null,
            }
        ];

        Series = series;

        DateTime currentDate = DateTime.Now;
        var months = new string[6];

        var currentLanguage =
#if !HAS_UNO
            Windows.System.UserProfile.GlobalizationPreferences.Languages[0] ?? 
#endif            
            "en-US";
        for (int i = 0; i < 6; i++)
        {
            months[5 - i] = currentDate.AddMonths(-i).ToString("MMM", new CultureInfo(currentLanguage));
        }

        XAxes = [
            new Axis
            {
                ShowSeparatorLines = false,
                Labels = months,
                TextSize = 14
            }];

        YAxes = [
            new Axis
            {
                ShowSeparatorLines = false,
                Labeler = value => value.ToString("C", CurrencyCache.CurrencyCultures[_settingsService.DefaultCurrency]),
                TextSize = 14
            }];

        UISettings_ColorValuesChanged(null, null);
    }

    private void UISettings_ColorValuesChanged(UISettings sender, object args)
    {
        //TODO: The UISettings.ColorValueChanged is not called on Windows 10
        //https://github.com/CommunityToolkit/WindowsCommunityToolkit/issues/4412#issuecomment-1823919825

        var skColor = new SKColor(_accentColor.R, _accentColor.G, _accentColor.B);

        ((LineSeries<decimal>)Series[0]).Stroke = new SolidColorPaint(skColor) { StrokeThickness = 6 };
        ((LineSeries<decimal>)Series[0]).GeometryStroke = new SolidColorPaint(skColor) { StrokeThickness = 2 };
        ((LineSeries<decimal>)Series[0]).GeometryFill = new SolidColorPaint(skColor) { StrokeThickness = 2 };

        App.Dispatcher.TryEnqueue(DispatcherQueuePriority.High,
        () =>
        {
            var textColor = new SKColor(_textColor.R, _textColor.G, _textColor.B);
            ((Axis)XAxes[0]).LabelsPaint = new SolidColorPaint(textColor);
            ((Axis)YAxes[0]).LabelsPaint = new SolidColorPaint(textColor);
        });

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

            var tasks = items.Select(async item =>
            item.IsArchived ? 0 : await _currencyCache.ConvertToDefaultCurrency
            (
                item.Item?.Billing.BasePrice * item.GetPaymentsInPeriod(days, (DateTime.Now - date).Days) ?? 0,
                item?.Item?.Billing?.CurrencyId ?? "EUR",
                _settingsService.DefaultCurrency)
            );

            var values = await Task.WhenAll(tasks);

            _values.Insert(0, values.Sum());
        }
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

        Sum = $"≈ {Math.Round(sum, 2).ToString("C", CurrencyCache.CurrencyCultures[_settingsService.DefaultCurrency])}";
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
