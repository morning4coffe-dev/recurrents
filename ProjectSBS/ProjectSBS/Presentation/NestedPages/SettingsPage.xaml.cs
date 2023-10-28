namespace ProjectSBS.Presentation.NestedPages;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel => (SettingsViewModel)DataContext;

    public SettingsPage()
    {
        this.InitializeComponent();

        this.DataContext = App.Services?.GetRequiredService<SettingsViewModel>()!;
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
