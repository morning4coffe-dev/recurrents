namespace ProjectSBS.Presentation.NestedPages;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();
        this.DataContext = (Application.Current as App)!.Host?.Services.GetService<HomeViewModel>()!;
    }

    public HomeViewModel ViewModel => (HomeViewModel)DataContext;

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
