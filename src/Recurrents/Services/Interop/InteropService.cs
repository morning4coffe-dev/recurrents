using Microsoft.UI;
using System.Reflection;
using Windows.UI.ViewManagement;

#if !HAS_UNO
using Recurrents.Infrastructure.Helpers;
#endif

namespace Recurrents.Services.Interop;

public class InteropService : IInteropService
{
    public InteropService()
    {
        UISettings uiSettings = new();
        uiSettings.ColorValuesChanged += HandleSystemThemeChange;
    }

    private void HandleSystemThemeChange(UISettings sender, object args)
    {
#if WINDOWS
        if (Window.Current.Content is FrameworkElement frameworkElement)
        {
            UpdateTitleBarTheme(frameworkElement.RequestedTheme);
        }
#endif
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

#if WINDOWS
            UpdateTitleBarTheme(theme);
#endif
        }
    }

#if WINDOWS
    private void UpdateTitleBarTheme(ElementTheme theme)
    {
        if(theme == ElementTheme.Light)
        {
            Win32.SetCaptionButtonColors((Application.Current as App)!.MainWindow!, Colors.Black);
        }
        else
        {
            Win32.SetCaptionButtonColors((Application.Current as App)!.MainWindow!, Colors.White);
        }
    }
#endif

    public async Task OpenStoreReviewUrlAsync()
    {
        //TODO: Implement on all platforms
#if !HAS_UNO
        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9N5MJT8G06KC"));
#else
        throw new NotImplementedException("OpenStoreReviewUrlAsync is not yet implemented on this platform!");
#endif

    }

    public void UpdateAppTitle(string title)
    {
#if !HAS_UNO
        ApplicationView.GetForCurrentView().Title = title;
#endif
    }
}
