namespace ProjectSBS.Presentation.Components;

public sealed partial class TagsChartBanner : Page
{
    public TagsChartViewModel ViewModel => (TagsChartViewModel)DataContext;

    public TagsChartBanner()
    {
        InitializeComponent();

        DataContext = App.Services?.GetRequiredService<TagsChartViewModel>()!;
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
