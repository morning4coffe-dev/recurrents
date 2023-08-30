using System.Collections.ObjectModel;

namespace ProjectSBS.Presentation.Components;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
      nameof(Items),
      typeof(ObservableCollection<ItemViewModel>),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public ObservableCollection<ItemViewModel> Items
    {
        get { return (ObservableCollection<ItemViewModel>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
      nameof(SelectedItem),
      typeof(ItemViewModel),
      typeof(HomePage),
      new PropertyMetadata(null)
    );

    public ItemViewModel SelectedItem
    {
        get { return (ItemViewModel)GetValue(SelectedItemProperty); }
        set { SetValue(SelectedItemProperty, value); }
    }
}