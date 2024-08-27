namespace Recurrents.Presentation;

public sealed partial class ItemEdit : Page
{
    public ItemEditViewModel ViewModel => (ItemEditViewModel)DataContext;

    public ItemEdit()
    {
        this.InitializeComponent();
    }
}
