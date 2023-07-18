#if !HAS_UNO
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using WinRT.Interop;

namespace ProjectSBS.Services.Interop;

public class WindowsInteropService : IInteropService
{
    private ApplicationTheme _appTheme;

    public WindowsInteropService()
    {
        UISettings uiSettings = new UISettings();
        uiSettings.ColorValuesChanged += HandleSystemThemeChange;
    }

    private void HandleSystemThemeChange(UISettings sender, object args)
    {
        if (Window.Current.Content is FrameworkElement frameworkElement)
        {
            //UpdateTitleBar(frameworkElement.RequestedTheme);
        }
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        await SetRequestedThemeAsync(theme);
    }

    public Version GetAppVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return AssemblyName.GetAssemblyName(assembly.Location).Version ?? new Version(1, 0);
    }

    private async Task SetRequestedThemeAsync(ElementTheme theme)
    {
        if ((Application.Current as App)!.MainWindow!.Content is FrameworkElement frameworkElement)
        {
            frameworkElement.RequestedTheme = theme;

            //UpdateTitleBar(theme);

            SetCaptionButtonColors((Application.Current as App)!.MainWindow!, Colors.DarkRed);

            //TODO UpdateTitleBar(theme); on Windows
        }
    }

    public static void SetCaptionButtonColors(Window window, Windows.UI.Color color)
    {
        var res = Application.Current.Resources;
        res["WindowCaptionForeground"] = color;
        TriggerTitleBarRepaint(window);
    }

    //TODO More this into separate file
    // https://github.com/microsoft/WinUI-Gallery/blob/9b0d8f6fa5062450163cd90277cc212e96064a86/WinUIGallery/Common/Win32.cs

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    public const int WM_ACTIVATE = 0x0006;
    public const int WA_ACTIVE = 0x01;
    //static int WA_CLICKACTIVE = 0x02;
    public const int WA_INACTIVE = 0x00;

    public const int WM_SETICON = 0x0080;
    public const int ICON_SMALL = 0;
    public const int ICON_BIG = 1;

    private static void TriggerTitleBarRepaint(Window window)
    {
        // to trigger repaint tracking task id 38044406
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var activeWindow = GetActiveWindow();
        if (hwnd == activeWindow)
        {
            SendMessage(hwnd, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
            SendMessage(hwnd, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }
        else
        {
            SendMessage(hwnd, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
            SendMessage(hwnd, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

    }

    //


    public async Task OpenStoreReviewUrlAsync()
    {
        // TODO: Packaged version
        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9PN77P9WJ3CX"));
    }

    public void UpdateAppTitle(string title)
    {
        ApplicationView.GetForCurrentView().Title = title;
    }
}
#endif
