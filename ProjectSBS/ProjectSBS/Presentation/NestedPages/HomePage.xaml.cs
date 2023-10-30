namespace ProjectSBS.Presentation.NestedPages;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel => (HomeViewModel)DataContext;

    public HomePage()
    {
        this.InitializeComponent();

        this.DataContext = App.Services?.GetRequiredService<HomeViewModel>()!;
        this.Loaded += Page_Loaded;
        this.Unloaded += Page_Unloaded;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Load();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Unload();
    }

    private async void DeleteItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
    {
        await ViewModel.DeleteItem(args.SwipeControl.DataContext as ItemViewModel);
    }
    private void ArchiveItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
    {
        //ArchiveItemCommand.Execute(args.SwipeControl.DataContext);
    }
    private void EditItem_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
    {
        //SelectedItem = (ItemViewModel)args.SwipeControl.DataContext;

        //EditItemCommand.Execute(args.SwipeControl.DataContext);
    }
}
