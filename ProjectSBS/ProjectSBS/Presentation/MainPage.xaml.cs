namespace ProjectSBS.Presentation;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainPage()
    {
        this.InitializeComponent();

        this.DataContext = App.Services?.GetRequiredService<MainViewModel>();
        this.Loaded += Page_Loaded;
        this.Unloaded += Page_Unloaded;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var nav = App.Services?.GetRequiredService<INavigation>()!;
        nav.NestedFrame = Frame;

        ViewModel.Load();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Unload();
    }

    private void OnNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        ViewModel.Navigate(args);
    }
}