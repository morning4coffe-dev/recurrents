namespace ProjectSBS.Presentation.NestedPages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();

        this.DataContext = (Application.Current as App)!.Host?.Services.GetService<SettingsViewModel>()!;
    }

    public SettingsViewModel ViewModel => (SettingsViewModel)DataContext;
}
