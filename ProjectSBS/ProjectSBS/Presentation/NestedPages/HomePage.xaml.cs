namespace ProjectSBS.Presentation.NestedPages;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel => (HomeViewModel)DataContext;

    public HomePage()
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

    private async void ArchiveItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
    {
        await ViewModel.Archive(args.SwipeControl.DataContext as ItemViewModel);
    }
    private void EditItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
    {
        //SelectedItem = (ItemViewModel)args.SwipeControl.DataContext;

        //EditItemCommand.Execute(args.SwipeControl.DataContext);
    }

    private void RefreshContainer_RefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
    {

    }
}
