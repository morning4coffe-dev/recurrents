using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using Microsoft.UI.Xaml.Input;

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
                //PreviousSelectedItem = SelectedItem;
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
            if (value != IsEditing)
            {
                SetValue(IsEditingProperty, value);
            }
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new ItemSelectionChanged(SelectedItem));
    }
}
