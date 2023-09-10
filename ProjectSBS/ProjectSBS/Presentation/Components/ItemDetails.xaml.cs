using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;

namespace ProjectSBS.Presentation.Components;

public sealed partial class ItemDetails : UserControl
{
    public ItemDetails()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
      nameof(SelectedItem),
      typeof(ItemViewModel),
      typeof(ItemDetails),
      new PropertyMetadata(null)
    );

    public ItemViewModel SelectedItem
    {
        get => (ItemViewModel)GetValue(SelectedItemProperty);
        set
        {
            if (value != SelectedItem)
            {
                IsEditing = SelectedItem.Item != null;

                SetValue(SelectedItemProperty, value);
            }
        }
    }

    public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(
      nameof(IsEditing),
      typeof(bool),
      typeof(ItemDetails),
      new PropertyMetadata(false)
    );

    public bool IsEditing
    {
        get => (bool)GetValue(IsEditingProperty);
        set
        {
            if (SelectedItem is not null && SelectedItem.Item is null)
            {
                SelectedItem.Item =
                    new Item(
                        Guid.NewGuid().ToString(),
                        string.Empty,
                        new BillingDetails(
                            4.99m,
                            DateOnly.FromDateTime(DateTime.Now)),
                        string.Empty,
                        string.Empty,
                        DateTime.Now
                    );
            }

            if (value != IsEditing)
            {
                SetValue(IsEditingProperty, value);
            }
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new ItemSelectionChanged(SelectedItem));

        //TODO Navigate back from ItemDetails after save
    }
}
