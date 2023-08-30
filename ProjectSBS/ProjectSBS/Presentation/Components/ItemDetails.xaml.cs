namespace ProjectSBS.Presentation.Components;

public sealed partial class ItemDetails : Page
{
    public ItemDetails()
    {
        this.InitializeComponent();
    }

    //After the SelectedItem is updated and user presses edit there should be a
    //flow to clone the value and display it in the edit fields and after user
    //presses save the SelectedItem value should be updated and saved in the database

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
      nameof(SelectedItem),
      typeof(ItemViewModel),
      typeof(ItemDetails),
      new PropertyMetadata(null)
    );

    public ItemViewModel SelectedItem
    {
        get => (ItemViewModel)GetValue(SelectedItemProperty); 
        set => SetValue(SelectedItemProperty, value);
    }
}