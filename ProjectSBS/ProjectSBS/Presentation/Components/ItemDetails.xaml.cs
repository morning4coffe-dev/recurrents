namespace ProjectSBS.Presentation.Components;

public sealed partial class ItemDetails : Page
{
	public ItemDetails()
	{
		this.InitializeComponent();
	}

    private static readonly DependencyProperty PreviousSelectedItemProperty = DependencyProperty.Register(
      nameof(PreviousSelectedItem),
      typeof(ItemViewModel),
      typeof(ItemDetails),
      new PropertyMetadata(null)
    );

    public ItemViewModel? PreviousSelectedItem
    {
        get => (ItemViewModel?)GetValue(PreviousSelectedItemProperty);
        set => SetValue(PreviousSelectedItemProperty, value);
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
                PreviousSelectedItem = SelectedItem;
                IsEditing = SelectedItem != null;

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
}
