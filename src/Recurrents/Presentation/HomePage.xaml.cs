
namespace Recurrents.Presentation;

public sealed partial class HomePage : Page
{
//#if WINDOWS
    public HomeViewModel ViewModel { get; private set; }
//#else
//    public HomeViewModel ViewModel => (HomeViewModel)DataContext;
//#endif

    public HomePage()
    {
        InitializeComponent();

//#if WINDOWS
        //Current Uno differences in DataContext handling
        ViewModel = (HomeViewModel)DataContext;

        DataContextChanged += (s, e) =>
        {
            if (e.NewValue is HomeViewModel viewModel)
            {
                ViewModel = viewModel;

                Loaded += Page_Loaded;
                Unloaded += Page_Unloaded;
            }
        };
//#endif
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (ViewModel is { })
        {
            ViewModel.SelectedItem = null;

            //ViewModel.RefreshItems();
        }
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        //ViewModel.Load();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Unload();
    }
}
