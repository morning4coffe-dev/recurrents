namespace ProjectSBS.Presentation;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainPage()
    {
        this.InitializeComponent();

        this.DataContext = App.Services?.GetRequiredService<MainViewModel>();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        while (App.Services?.GetRequiredService<MainViewModel>() == null)
        {
            //Wait till Host is created
            await Task.Delay(200);

            //TODO Add loading indicator
        }

        this.DataContext = App.Services?.GetRequiredService<MainViewModel>();

        ViewModel.Load();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Unload();
    }

    private void OnNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        this.DataContext = App.Services?.GetRequiredService<MainViewModel>();

        if (args.IsSettingsSelected)
        {
            //contentFrame.Navigate(typeof(SampleSettingsPage));
        }
    }

}