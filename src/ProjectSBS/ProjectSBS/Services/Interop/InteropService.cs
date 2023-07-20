using Microsoft.UI;
using ProjectSBS.Infrastructure.Helpers;
using System.Reflection;
using Windows.UI.ViewManagement;

namespace ProjectSBS.Services.Interop;

public class InteropService : IInteropService
{
    public InteropService()
    {
        UISettings uiSettings = new();
        uiSettings.ColorValuesChanged += HandleSystemThemeChange;
    }

    private void HandleSystemThemeChange(UISettings sender, object args)
    {
        if (Window.Current.Content is FrameworkElement frameworkElement)
        {
            UpdateTitleBarTheme(frameworkElement.RequestedTheme);
        }
    }

    public Version GetAppVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return AssemblyName.GetAssemblyName(assembly.Location).Version ?? new Version(1, 0);
    }

    public void SetTheme(ElementTheme theme)
    {
        if ((Application.Current as App)!.MainWindow!.Content is FrameworkElement frameworkElement)
        {
            frameworkElement.RequestedTheme = theme;

            UpdateTitleBarTheme(theme);
        }
    }

    private void UpdateTitleBarTheme(ElementTheme theme)
    {
#if !HAS_UNO
            if(theme == ElementTheme.Light)
            {
                Win32.SetCaptionButtonColors((Application.Current as App)!.MainWindow!, Colors.Black);
            }
            else
            {
                Win32.SetCaptionButtonColors((Application.Current as App)!.MainWindow!, Colors.White);
            }
#endif
    }

    public async Task OpenStoreReviewUrlAsync()
    {
#if !HAS_UNO
        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9PN77P9WJ3CX"));
#else
        throw new NotImplementedException("OpenStoreReviewUrlAsync is not yet implemented on this platform!");
#endif

    }

    public void UpdateAppTitle(string title)
    {
        ApplicationView.GetForCurrentView().Title = title;
    }
}
