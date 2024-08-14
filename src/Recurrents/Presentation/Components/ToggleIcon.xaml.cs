namespace Recurrents.Presentation.Components;

public sealed partial class ToggleIcon : UserControl
{
    public ToggleIcon()
    {
        this.InitializeComponent();
    }
    public bool IsChecked
    {
        get => (bool)this.GetValue(IsCheckedProperty);
        set => this.SetValue(IsCheckedProperty, value);
    }

    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
      nameof(IsChecked),
      typeof(bool),
      typeof(ToggleIcon),
      new PropertyMetadata(null)
    );

    public IconElement TrueIcon
    {
        get => (IconElement)this.GetValue(TrueIconProperty);
        set => this.SetValue(TrueIconProperty, value);
    }

    public static readonly DependencyProperty TrueIconProperty = DependencyProperty.Register(
      nameof(TrueIcon),
      typeof(IconElement),
      typeof(ToggleIcon),
      new PropertyMetadata(null)
    );

    public IconElement FalseIcon
    {
        get => (IconElement)this.GetValue(FalseIconProperty);
        set => this.SetValue(FalseIconProperty, value);
    }

    public static readonly DependencyProperty FalseIconProperty = DependencyProperty.Register(
      nameof(FalseIcon),
      typeof(IconElement),
      typeof(ToggleIcon),
      new PropertyMetadata(null)
    );
}
