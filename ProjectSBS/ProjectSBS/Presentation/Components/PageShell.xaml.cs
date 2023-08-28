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

    public static readonly DependencyProperty TitleVisibilityProperty = DependencyProperty.Register(
      nameof(TitleVisibility),
      typeof(Visibility),
      typeof(PageShell),
      new PropertyMetadata(Visibility.Visible)
    );

    public Visibility TitleVisibility
    {
        get { return (Visibility)GetValue(TitleVisibilityProperty); }
        set { SetValue(TitleVisibilityProperty, value); }
    }
}
