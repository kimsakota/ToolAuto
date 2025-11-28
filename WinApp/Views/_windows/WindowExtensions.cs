using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

public static class WindowExtensions
{
    // Win32 constants
    public const int GWL_EXSTYLE = -20;
    public const int WS_EX_NOACTIVATE = 0x08000000;

    // P/Invoke declarations
    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    // Optional: For setting position without activating
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    const uint SWP_NOACTIVATE = 0x0010;
    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOMOVE = 0x0002;

    public static void SetCanFocus(this Window wnd, bool b)
    {
        wnd.Topmost = !b;
        if (b)
        {
            ClearWindowExSlyte(wnd, WS_EX_NOACTIVATE);
        }
        else
        {
            SetWindowExStyle(wnd, WS_EX_NOACTIVATE);
        }
    }

    public static void ClearWindowExSlyte(this Window wnd, int flags)
    {
        WindowInteropHelper helper = new WindowInteropHelper(wnd);
        IntPtr handle = helper.Handle;

        int exStyle = GetWindowLong(handle, GWL_EXSTYLE);
        exStyle = exStyle & (~flags);

        SetWindowLong(handle, GWL_EXSTYLE, exStyle);
    }
    public static void SetWindowExStyle(this Window wnd, int flags)
    {
        WindowInteropHelper helper = new WindowInteropHelper(wnd);
        IntPtr handle = helper.Handle;

        int exStyle = GetWindowLong(handle, GWL_EXSTYLE);
        exStyle = exStyle | flags;

        SetWindowLong(handle, GWL_EXSTYLE, exStyle);
    }
}
