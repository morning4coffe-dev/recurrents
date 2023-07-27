using Microsoft.UI.Windowing;

namespace ProjectSBS;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        this.InitializeComponent();
        DataContext = (Application.Current as App)?.Host?.Services.GetService<ShellViewModel>();


#if !HAS_UNO
        //TODO Make this a func somewhere
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
#endif
    }

    public ShellViewModel ViewModel => (ShellViewModel)DataContext;
}