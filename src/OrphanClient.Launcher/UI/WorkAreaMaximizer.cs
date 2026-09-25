using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OrphanClient.Launcher.UI;

internal static class WorkAreaMaximizer
{
    private const int GetMinMaxInfoMessage = 0x0024;
    private const int MonitorDefaultToNearest = 0x00000002;

    public static void Attach(Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;

        HwndSource.FromHwnd(handle)?.AddHook(Hook);
    }

    private static IntPtr Hook(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (message != GetMinMaxInfoMessage)
        {
            return IntPtr.Zero;
        }

        var monitor = MonitorFromWindow(hwnd, MonitorDefaultToNearest);

        if (monitor == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        var monitorInfo = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };

        if (!GetMonitorInfo(monitor, ref monitorInfo))
        {
            return IntPtr.Zero;
        }

        var info = Marshal.PtrToStructure<MinMaxInfo>(lParam);

        info.MaxPosition.X = monitorInfo.WorkArea.Left - monitorInfo.Monitor.Left;
        info.MaxPosition.Y = monitorInfo.WorkArea.Top - monitorInfo.Monitor.Top;
        info.MaxSize.X = monitorInfo.WorkArea.Right - monitorInfo.WorkArea.Left;
        info.MaxSize.Y = monitorInfo.WorkArea.Bottom - monitorInfo.WorkArea.Top;

        Marshal.StructureToPtr(info, lParam, true);
        handled = true;

        return IntPtr.Zero;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int flags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MinMaxInfo
    {
        public NativePoint Reserved;
        public NativePoint MaxSize;
        public NativePoint MaxPosition;
        public NativePoint MinTrackSize;
        public NativePoint MaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public NativeRect Monitor;
        public NativeRect WorkArea;
        public int Flags;
    }
}
