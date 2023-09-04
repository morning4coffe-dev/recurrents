using Microsoft.UI.Xaml.Input;

namespace ProjectSBS.Presentation.Mobile;

public sealed partial class ItemDetailsPage : Page
{
    public ItemDetailsPage()
    {
        this.InitializeComponent();
    }

    private void spinme_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        spinrect.Begin();
    }
}