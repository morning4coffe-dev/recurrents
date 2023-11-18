namespace ProjectSBS.Presentation.NestedPages;

public sealed partial class ArchivePage : Page
{
    public HomeViewModel ViewModel => (HomeViewModel)DataContext;

    public ArchivePage()
    {
        this.InitializeComponent();

        this.DataContext = App.Services?.GetRequiredService<HomeViewModel>()!;
        this.Loaded += Page_Loaded;
        this.Unloaded += Page_Unloaded;
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
