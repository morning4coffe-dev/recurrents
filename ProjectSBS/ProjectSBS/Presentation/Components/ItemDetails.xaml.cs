namespace ProjectSBS.Presentation.Components;

public sealed partial class ItemDetails : Page
{
    public ItemDetails()
    {
        this.InitializeComponent();

        //TODO Log if null
        this.DataContext = (Application.Current as App)!.Host?.Services.GetService<ItemDetailsViewModel>()!;
    }

    public ItemDetailsViewModel ViewModel => (ItemDetailsViewModel)DataContext;
}
