namespace Recurrents.Presentation;

public sealed partial class ItemDetails : Page
{
    public ItemDetailsViewModel ViewModel => (ItemDetailsViewModel)DataContext;

    public ItemDetails()
    {
        this.InitializeComponent();
        this.Unloaded += ItemDetails_Unloaded;
    }

    private void ItemDetails_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Unload();
    }
}
