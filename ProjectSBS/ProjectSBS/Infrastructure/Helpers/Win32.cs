#if WINDOWS
using System.Runtime.InteropServices;

namespace ProjectSBS.Infrastructure.Helpers;

internal class Win32
{
    public const int WM_ACTIVATE = 0x0006;
    public const int WA_ACTIVE = 0x01;
    public const int WA_INACTIVE = 0x00;

    public const int WM_SETICON = 0x0080;
    public const int ICON_SMALL = 0;
    public const int ICON_BIG = 1;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetModuleHandle(IntPtr moduleName);


    public static void SetCaptionButtonColors(Window window, Windows.UI.Color color)
    {
        Application.Current.Resources["WindowCaptionForeground"] = color;
        TriggerTitleBarRepaint(window);
    }

    public static void SetTitleBackgroundTransparent(Window window)
    {
        Application.Current.Resources["WindowCaptionBackground"] = Microsoft.UI.Colors.Transparent;
        TriggerTitleBarRepaint(window);
    }

    private static void TriggerTitleBarRepaint(Window window)
    {
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
}
#endif