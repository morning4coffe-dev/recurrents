using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;

namespace ProjectSBS.Presentation.Components;

public sealed partial class StatsBanner : Page
{
    public StatsBanner()
    {
        this.InitializeComponent();

        this.DataContext = (Application.Current as App)!.Host?.Services.GetService<StatsBannerViewModel>()!;
    }

    public StatsBannerViewModel ViewModel => (StatsBannerViewModel)DataContext;

    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 20, 1, 3, 5, 3, 24, 6 },
            Fill = null
        }
    };
}
