namespace ProjectSBS.Presentation.Components;

public sealed partial class DetailsBox : UserControl
{
	public DetailsBox()
	{
		this.InitializeComponent();
	}

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
      nameof(TitleText),
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
