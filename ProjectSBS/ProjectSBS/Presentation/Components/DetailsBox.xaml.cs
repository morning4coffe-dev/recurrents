namespace ProjectSBS.Presentation.Components;

public sealed partial class DetailsBox : UserControl
{
	public DetailsBox()
	{
		this.InitializeComponent();
	}

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
      nameof(Title),
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

    public UIElement ContentView
    {
        get => (UIElement)GetValue(ContentViewProperty);
        set => SetValue(ContentViewProperty, value);
    }

    public static readonly DependencyProperty ContentViewProperty = DependencyProperty.Register(
      nameof(ContentView),
      typeof(UIElement),
      typeof(PageShell),
      new PropertyMetadata(null)
    );
}
