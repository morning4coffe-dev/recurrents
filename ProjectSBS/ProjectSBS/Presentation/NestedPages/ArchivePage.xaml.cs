namespace ProjectSBS.Presentation.NestedPages;

public sealed partial class ArchivePage : Page
{
    public HomeViewModel ViewModel => (HomeViewModel)DataContext;

    public ArchivePage()
    {
        InitializeComponent();

        DataContext = App.Services?.GetRequiredService<HomeViewModel>()!;
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

    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        _ = ViewModel.Archive(e.ClickedItem as ItemViewModel);
    }
}
