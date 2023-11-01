using Microsoft.UI.Windowing;

namespace ProjectSBS.Presentation.Components;

public sealed partial class PageShell : Page
{
    public PageShell()
    {
        this.InitializeComponent();

#if WINDOWS
        this.Loaded += PageTitle_Loaded;
#endif
    }

#if WINDOWS
    private void PageTitle_Loaded(object sender, RoutedEventArgs e)
    {
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            (Application.Current as App)!.MainWindow!.ExtendsContentIntoTitleBar = true;
            (Application.Current as App)!.MainWindow!.SetTitleBar(AppTitleBar);
        }
        else
        {
            AppTitleBar.Visibility = Visibility.Collapsed;
            //TODO Log
        }
    }
#endif

    public static readonly DependencyProperty ContentViewProperty = DependencyProperty.Register(
      nameof(ContentView),
      typeof(UIElement),
      typeof(PageShell),
      new PropertyMetadata(null)
    );

    public UIElement ContentView
    {
        get { return (UIElement)GetValue(ContentViewProperty); }
        set { SetValue(ContentViewProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
      nameof(Title),
      typeof(string),
      typeof(PageShell),
      new PropertyMetadata("")
    );

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value); 
    }

    public static readonly DependencyProperty ActionBarProperty = DependencyProperty.Register(
      nameof(ActionBar),
      typeof(UIElement),
      typeof(PageShell),
      new PropertyMetadata(null)
    );

    public string TitleBarTitle
    {
        get => (string)GetValue(TitleBarTitleProperty);
        set => SetValue(TitleBarTitleProperty, value);
    }

    public static readonly DependencyProperty TitleBarTitleProperty = DependencyProperty.Register(
      nameof(TitleBarTitle),
      typeof(string),
      typeof(PageShell),
      new PropertyMetadata("")
    );

    public UIElement ActionBar
    {
        get { return (UIElement)GetValue(ActionBarProperty); }
        set { SetValue(ActionBarProperty, value); }
    }

    public static readonly DependencyProperty DesktopTitleVisibilityProperty = DependencyProperty.Register(
      nameof(DesktopTitleVisibility),
      typeof(Visibility),
      typeof(PageShell),
      new PropertyMetadata(Visibility.Collapsed)
    );

    public Visibility DesktopTitleVisibility
    {
        get => (Visibility)GetValue(DesktopTitleVisibilityProperty);
        set => SetValue(DesktopTitleVisibilityProperty, value);
    }

    public static readonly DependencyProperty MobileTitleVisibilityProperty = DependencyProperty.Register(
      nameof(MobileTitleVisibility),
      typeof(Visibility),
      typeof(PageShell),
      new PropertyMetadata(Visibility.Collapsed)
    );

    public Visibility MobileTitleVisibility
    {
        get => (Visibility)GetValue(MobileTitleVisibilityProperty);
        set => SetValue(MobileTitleVisibilityProperty, value);
    }

    public static readonly DependencyProperty BackCommandProperty = DependencyProperty.Register(
      nameof(BackCommand),
      typeof(ICommand),
      typeof(PageShell),
      new PropertyMetadata(null)
    );

    public ICommand BackCommand
    {
        get => (ICommand)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }
}
