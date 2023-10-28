namespace ProjectSBS.Presentation.Components;

public sealed partial class StatsBanner : Page
{
    public StatsBanner()
    {
        this.InitializeComponent();

        this.DataContext = App.Services?.GetRequiredService<StatsBannerViewModel>()!;
    }

    public StatsBannerViewModel ViewModel => (StatsBannerViewModel)DataContext;
}
