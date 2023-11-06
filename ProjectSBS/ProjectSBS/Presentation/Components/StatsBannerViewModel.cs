using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ProjectSBS.Presentation.Components;

public partial class StatsBannerViewModel : ObservableObject
{
    ObservableCollection<double> _values = new();

    public StatsBannerViewModel()
    {
        _sum = $"{new Random().Next(0, 50000)} CZK";

        for (int i = 0; i < 12; i++)
        {
            _values.Add(new Random().Next(0, 5000));
        }

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

    [ObservableProperty]
    private string _sum;

    public ObservableCollection<ISeries> Series { get; set; }
}