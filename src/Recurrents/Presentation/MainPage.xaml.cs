namespace Recurrents.Presentation;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainPage()
    {
        InitializeComponent();

        DataContext = App.Services?.GetRequiredService<MainViewModel>();
        Loaded += Page_Loaded;
        Unloaded += Page_Unloaded;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var nav = App.Services?.GetRequiredService<INavigation>()!;
        nav.NestedFrame = Frame;

        ViewModel.Load();

        ViewModel._navigation.CategoryChanged += Navigation_CategoryChanged;
        navigationBar.SelectedIndex = ViewModel._navigation.Categories.IndexOf
            (ViewModel._navigation.SelectedCategory);
    }

    //This is a workaround for TabBar bug, setting the item doesn't work, index does
    private void Navigation_CategoryChanged(object? sender, NavigationCategory e)
    {
        navigationBar.SelectedIndex = ViewModel._navigation.Categories.IndexOf(e);
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel._navigation.CategoryChanged += Navigation_CategoryChanged;

        ViewModel.Unload();
    }
}
