namespace ProjectSBS.Presentation.Components;

public sealed partial class ItemDetails : Page
{
    public ItemDetails()
    {
        InitializeComponent();
        DataContext = App.Services?.GetRequiredService<ItemDetailsViewModel>()!;
    }

    public ItemDetailsViewModel ViewModel => (ItemDetailsViewModel)DataContext;
}
