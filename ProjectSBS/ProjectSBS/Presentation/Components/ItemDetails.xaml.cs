namespace ProjectSBS.Presentation.Components;

public sealed partial class ItemDetails : Page
{
    public ItemDetails()
    {
        this.InitializeComponent();

        //TODO Log if null
        this.DataContext = App.Services?.GetRequiredService<ItemDetailsViewModel>()!;
    }

    public ItemDetailsViewModel ViewModel => (ItemDetailsViewModel)DataContext;
}
