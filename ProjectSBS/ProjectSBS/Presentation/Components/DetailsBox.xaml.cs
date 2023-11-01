namespace ProjectSBS.Presentation.Components;

public sealed partial class DetailsBox : UserControl
{
	public DetailsBox()
	{
		this.InitializeComponent();
	}

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
      nameof(Header),
      typeof(string),
      typeof(PageShell),
      new PropertyMetadata("")
    );

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
      nameof(Text),
      typeof(string),
      typeof(PageShell),
      new PropertyMetadata(null)
    );

    public UIElement Content
    {
        get => (UIElement)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
      nameof(Content),
      typeof(UIElement),
      typeof(PageShell),
      new PropertyMetadata(null)
    );
}
