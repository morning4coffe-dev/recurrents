using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;

namespace ProjectSBS.Presentation.Components;

public partial class StatsBannerViewModel : ObservableObject
{
    public StatsBannerViewModel()
    {

    }

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 20, 1, 3, 5, 3, 4, 6 },
            Fill = null
        }
    };
}