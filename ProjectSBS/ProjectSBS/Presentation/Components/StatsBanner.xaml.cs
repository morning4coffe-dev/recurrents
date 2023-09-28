namespace ProjectSBS.Presentation.Components;

public sealed partial class StatsBanner : Page
{
    public StatsBanner()
    {
        this.InitializeComponent();

        this.DataContext = (Application.Current as App)!.Host?.Services.GetService<StatsBannerViewModel>()!;
    }

    public StatsBannerViewModel ViewModel => (StatsBannerViewModel)DataContext;
}
