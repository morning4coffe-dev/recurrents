namespace ProjectSBS.Presentation.Components;

public sealed partial class StatsBanner : Page
{
    public StatsBannerViewModel ViewModel => (StatsBannerViewModel)DataContext;

    public StatsBanner()
    {
        InitializeComponent();

        DataContext = App.Services?.GetRequiredService<StatsBannerViewModel>()!;
        Loaded += Page_Loaded;
        Unloaded += Page_Unloaded;

    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Load();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Unload();
    }
}
