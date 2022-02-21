using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

namespace Frosty.Controls
{
    #region -- Win32 Native --
    internal static class Native
    {
        public const Int32 MONITOR_DEFAULTTOPRIMARY = 0x00000001;
        public const Int32 MONITOR_DEFAULTTONEAREST = 0x00000002;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONITORINFO
        {
            public int Size;
            public RECT Monitor;
            public RECT WorkArea;
            public uint Flags;

            public static MONITORINFO Default
            {
                get
                {
                    MONITORINFO mi = new MONITORINFO {Size = 40};
                    return mi;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NCCALCSIZE_PARAMS
        {
            public RECT rgrc1;
            public RECT rgrc2;
            public RECT rgrc3;
            public IntPtr lppos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public bool lParam;
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040,
        }

        public enum ABMsg
        {
            ABM_NEW = 0,
            ABM_REMOVE = 1,
            ABM_QUERYPOS = 2,
            ABM_SETPOS = 3,
            ABM_GETSTATE = 4,
            ABM_GETTASKBARPOS = 5,
            ABM_ACTIVATE = 6,
            ABM_GETAUTOHIDEBAR = 7,
            ABM_SETAUTOHIDEBAR = 8,
            ABM_WINDOWPOSCHANGED = 9,
            ABM_SETSTATE = 10
        }

        [DllImport("user32", EntryPoint = "GetWindowRect")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("Shcore.dll", EntryPoint = "SetProcessDpiAwareness")]
        public static extern int SetProcessDpiAwareness(int value);

        [DllImport("user32.dll", EntryPoint = "MonitorFromWindow")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, Int32 flags);

        [DllImport("user32.dll", EntryPoint = "GetMonitorInfo")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("shell32", CallingConvention = CallingConvention.StdCall, EntryPoint = "SHAppBarMessage")]
        public static extern int SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [DllImport("user32", SetLastError = true, EntryPoint = "FindWindow")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        private static extern int ShowWindow(IntPtr hwnd, int command);

        [DllImport("user32.dll", EntryPoint = "EnumWindows")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetClassName", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public static void GetAppBarData(out RECT rc, out int edge, out bool autoHide, IntPtr monitor)
        {
            MONITORINFO mi = MONITORINFO.Default;
            APPBARDATA abd = new APPBARDATA();

            GetMonitorInfo(monitor, ref mi);
            Rect monitorRect = new Rect(mi.Monitor.left, mi.Monitor.top, mi.Monitor.right - mi.Monitor.left, mi.Monitor.bottom - mi.Monitor.top);

            IntPtr hwnd = IntPtr.Zero;
            EnumWindows((IntPtr hWnd, IntPtr lParam) =>
            {
                if (hWnd == IntPtr.Zero)
                    return false;
                StringBuilder sb = new StringBuilder(1024);
                GetClassName(hWnd, sb, 1024);

                // find all taskbar windows
                if (sb.ToString().Equals("Shell_TrayWnd", StringComparison.OrdinalIgnoreCase))
                {
                    abd.cbSize = Marshal.SizeOf(abd);
                    abd.hWnd = hwnd;

                    // see if its overlapping the same monitor as the source window
                    SHAppBarMessage((int)ABMsg.ABM_GETTASKBARPOS, ref abd);
                    Rect tbRect = new Rect(abd.rc.left, abd.rc.top, abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top);

                    if (tbRect.IntersectsWith(monitorRect))
                    {
                        hwnd = hWnd;
                        return false;
                    }
                }
                return true;

            }, IntPtr.Zero);

            rc.left = rc.right = rc.top = rc.bottom = 0;
            edge = -1;
            autoHide = false;

            if (hwnd != IntPtr.Zero)
            {
                rc = abd.rc;
                autoHide = Convert.ToBoolean(SHAppBarMessage((int)ABMsg.ABM_GETSTATE, ref abd));

                if (rc.top == rc.left && rc.bottom > rc.right)
                    edge = 0;
                else if (rc.top == rc.left && rc.bottom < rc.right)
                    edge = 1;
                else if (rc.top > rc.left)
                    edge = 3;
                else
                    edge = 2;
            }
        }
    }
    #endregion

    #region -- Window Button Commands --
    public class WindowCloseCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public void Execute(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }

    public class WindowMaximizeCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public void Execute(object parameter)
        {
            if (parameter is Window window)
            {
                window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }
    }

    public class WindowMinimizeCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public void Execute(object parameter)
        {
            if (parameter is Window window)
            {
                window.WindowState = WindowState.Minimized;
            }
        }
    }
    #endregion

    public class FrostyWindow : Window
    {
        private class WindowStateToThicknessConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                FrostyWindow win = parameter as FrostyWindow;
                WindowState state = (WindowState)value;

                if (state == WindowState.Maximized)
                {
                    IntPtr handle = (new WindowInteropHelper(win)).Handle;
                    Native.RECT rect = new Native.RECT();
                    Native.MONITORINFO monitorInfo = Native.MONITORINFO.Default;

                    Native.GetWindowRect(handle, ref rect);

                    double dpiX = 1.0 / win.winDpiScale.DpiScaleX;
                    double dpiY = 1.0 / win.winDpiScale.DpiScaleY;

                    // obtain monitor info that the application is currently on
                    IntPtr monitor = Native.MonitorFromWindow(handle, Native.MONITOR_DEFAULTTONEAREST);
                    Native.GetMonitorInfo(monitor, ref monitorInfo);

                    // try to find a taskbar on the same monitor as the window
                    Native.GetAppBarData(out Native.RECT rc, out int edge, out bool autoHide, monitor);

                    // adjust for monitor
                    double insetLX = (rect.left - monitorInfo.Monitor.left);
                    double insetTY = (rect.top - monitorInfo.Monitor.top);
                    double insetRX = (monitorInfo.Monitor.right - rect.right);
                    double insetBY = (monitorInfo.Monitor.bottom - rect.bottom);

                    // adjust for taskbar
                    double width = rc.right - rc.left;
                    double height = rc.bottom - rc.top;

                    if (autoHide)
                    {
                        width = 0;
                        height = 0;
                    }

                    switch (edge)
                    {
                        case 0: insetLX -= width; break;
                        case 1: insetTY -= height; break;
                        case 2: insetRX -= width; break;
                        case 3: insetBY -= height; break;
                    }

                    return new Thickness(Math.Abs(insetLX) * dpiX, Math.Abs(insetTY) * dpiY, Math.Abs(insetRX) * dpiX, Math.Abs(insetBY) * dpiY);
                }

                return new Thickness(0);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

//#if FROSTY_ALPHA
//        public string UserName
//        {
//            get
//            {
//                byte[][] blackList = new byte[][] 
//                {
//                    //new byte[] { 0x7c, 0x7d, 0x6a, 0x6a, 0x71, 0x7b, 0x73, 0x15, 0x68, 0x7b },
//                    //new byte[] { 0x74, 0x79, 0x68, 0x6c, 0x77, 0x68, 0x15, 0x76, 0x77, 0x0a, 0x0f, 0x6a, 0x68, 0x09, 0x00 }
//                };

//                string value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
//                byte[] xValue = xForm(value.ToUpper());

//                foreach (byte[] s in blackList)
//                {
//                    if (s.Length < xValue.Length)
//                    {
//                        bool bMatch = true;
//                        for (int i = 0; i < s.Length; i++)
//                        {
//                            if (s[i] != xValue[i])
//                            {
//                                bMatch = false;
//                                break;
//                            }
//                        }

//                        if (bMatch)
//                        {
//                            Application.Current.Shutdown();
//                        }
//                    }
//                }

//                return value;
//            }
//        }

//        private byte[] xForm(string inStr)
//        {
//            byte[] outStr = new byte[inStr.Length];
//            for (int i = 0; i < inStr.Length; i++)
//                outStr[i] = (byte)(inStr[i] ^ 0x38);
//            return outStr;
//        }
//#endif

        public event EventHandler FrostyLoaded;
        private DpiScale winDpiScale = new DpiScale(1, 1);
        private Grid windowBorder;

        static FrostyWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyWindow), new FrameworkPropertyMetadata(typeof(FrostyWindow)));
        }

        public FrostyWindow()
        {
            WindowChrome chrome = new WindowChrome
            {
                NonClientFrameEdges = NonClientFrameEdges.None,
                GlassFrameThickness = new Thickness(1),
                UseAeroCaptionButtons = false,
                CornerRadius = new CornerRadius(0),
                CaptionHeight = 32
            };
            WindowChrome.SetWindowChrome(this, chrome);
            
            Loaded += FrostyWindow_Initialized;
            ContentRendered += (sender, e) => FrostyLoaded?.Invoke(sender, e);
        }

        public override void OnApplyTemplate()
        {
            // grab Dpi from the presentation source (just in case the initial OnDpiChanged is not called)
            PresentationSource presentationsource = PresentationSource.FromVisual(this);
            winDpiScale = new DpiScale(presentationsource.CompositionTarget.TransformToDevice.M11, presentationsource.CompositionTarget.TransformToDevice.M22);

            base.OnApplyTemplate();
            windowBorder = GetTemplateChild("WindowBorder") as Grid;

            if (windowBorder != null)
            {
                Binding b = new Binding("WindowState")
                {
                    Source = this,
                    Mode = BindingMode.OneWay,
                    Converter = new WindowStateToThicknessConverter(),
                    ConverterParameter = this
                };
                BindingOperations.SetBinding(windowBorder, Grid.MarginProperty, b);
            }
        }

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            winDpiScale = newDpi;

            // make sure to have a fallback in case Dpi is set incorrectly
            if (winDpiScale.DpiScaleX < 1.0 || winDpiScale.DpiScaleY < 1.0)
                winDpiScale = new DpiScale(1.0, 1.0);

            if (windowBorder != null && WindowState == WindowState.Maximized)
            {
                Native.RECT windowRect = new Native.RECT();
                Native.GetWindowRect(new WindowInteropHelper(this).Handle, ref windowRect);

                Width = (windowRect.right - windowRect.left) * winDpiScale.DpiScaleX;
                Height = (windowRect.bottom - windowRect.top) * winDpiScale.DpiScaleX;

                WindowState = WindowState.Normal;
            }

            base.OnDpiChanged(oldDpi, newDpi);
        }

        private void FrostyWindow_Initialized(object sender, EventArgs e)
        {
            if (ResizeMode == ResizeMode.NoResize)
            {
                Button minimizeButton = FindChild<Button>(this, "minimizeButton");
                Button maximizeButton = FindChild<Button>(this, "maximizeButton");

                minimizeButton.Visibility = Visibility.Collapsed;
                maximizeButton.Visibility = Visibility.Collapsed;
            }
            else if (ResizeMode == ResizeMode.CanMinimize)
            {
                Button maximizeButton = FindChild<Button>(this, "maximizeButton");
                maximizeButton.Visibility = Visibility.Collapsed;
            }

            if (WindowState == WindowState.Maximized)
                OnStateChanged(new EventArgs());
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            UpdateState();
        }

        private void UpdateState()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowChrome chrome = WindowChrome.GetWindowChrome(this);
                chrome.ResizeBorderThickness = new Thickness(0);

                Button restoreButton = FindChild<Button>(this, "restoreButton");
                Button maximizeButton = FindChild<Button>(this, "maximizeButton");

                if (maximizeButton != null) maximizeButton.Visibility = Visibility.Collapsed;
                if (restoreButton != null) restoreButton.Visibility = Visibility.Visible;
            }
            else if (WindowState == WindowState.Normal)
            {
                WindowChrome chrome = WindowChrome.GetWindowChrome(this);
                chrome.ResizeBorderThickness = new Thickness(2);

                Button restoreButton = FindChild<Button>(this, "restoreButton");
                Button maximizeButton = FindChild<Button>(this, "maximizeButton");

                if (maximizeButton != null) maximizeButton.Visibility = Visibility.Visible;
                if (restoreButton != null) restoreButton.Visibility = Visibility.Collapsed;
            }
        }

        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null)
                return null;

            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (!(child is T))
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null)
                        break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}
