using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Globalization;

namespace ProjectSBS.Presentation.Components;

public partial class StatsBannerViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly ISettingsService _settingsService;

    ObservableCollection<double> _values = new();

    public StatsBannerViewModel(IItemService itemService, ISettingsService settingsService)
    {
        _itemService = itemService;
        _settingsService = settingsService;

        //observe ItemService that items have changed
        //_sum = $"{new Random().Next(0, 50000)} CZK";

        //_itemService.OnItemsChanged += _itemService_OnItemsChanged;

        //Task.Run(Load);
    }

    private void _itemService_OnItemsChanged(object? sender, IEnumerable<ItemViewModel> e)
    {
        _values.Add(e.ToList().Count);

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

    private async Task Load() 
    {
        var items = _itemService.GetItems().ToList();
        while (items.Count == 0)
        {
            items = _itemService.GetItems().ToList();
            return;
        }

        Sum = _itemService.GetItems().Sum(item => item.Item.Billing.BasePrice).ToString("c", CultureInfo.CurrentCulture);
    }

    [ObservableProperty]
    private string _sum;

    public ObservableCollection<ISeries> Series { get; set; }
}