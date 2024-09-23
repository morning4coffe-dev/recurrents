namespace Recurrents.Presentation;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainPage()
    {
        this.InitializeComponent();
    }
}
