
namespace Recurrents.Presentation;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel { get; private set; }

    public HomePage()
    {
        InitializeComponent();

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
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        ViewModel.SelectedItem = null;
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
