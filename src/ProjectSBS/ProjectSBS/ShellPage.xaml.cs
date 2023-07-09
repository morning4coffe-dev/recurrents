namespace ProjectSBS;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        this.InitializeComponent();
        DataContext = (Application.Current as App)?.Host?.Services.GetService<ShellViewModel>();
    }

    public ShellViewModel ViewModel => (ShellViewModel)DataContext;
}