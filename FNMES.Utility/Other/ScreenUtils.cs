#if NETFRAMEWORK || WINDOWS
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FNMES.Utility.Other
{
    public class ScreenUtils
    {
#region Dll引用
        [DllImport("User32.dll", EntryPoint = "GetDC")]
        private extern static IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "ReleaseDC")]
        private extern static int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("User32.dll")]
        public static extern int GetSystemMetrics(int hWnd);


        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);

        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);
#endregion

        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;

        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;

        /// <summary>
        /// 获取DPI缩放比例
        /// </summary>
        /// <param name="dpiscalex"></param>
        /// <param name="dpiscaley"></param>
        public static void GetDPIScale(ref float dpiscalex, ref float dpiscaley)
        {
            int x = GetSystemMetrics(SM_CXSCREEN);
            int y = GetSystemMetrics(SM_CYSCREEN);
            IntPtr hdc = GetDC(IntPtr.Zero);
            int w = GetDeviceCaps(hdc, DESKTOPHORZRES);
            int h = GetDeviceCaps(hdc, DESKTOPVERTRES);
            ReleaseDC(IntPtr.Zero, hdc);
            dpiscalex = (float)w / x;
            dpiscaley = (float)h / y;
        }

        /// <summary>
        /// 获取分辨率
        /// </summary>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        private static void GetResolving(ref int width, ref int height)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            width = GetDeviceCaps(hdc, DESKTOPHORZRES);
            height = GetDeviceCaps(hdc, DESKTOPVERTRES);
            ReleaseDC(IntPtr.Zero, hdc);
        }

        public static int ScreenCount
        {
            get
            {
                return Screen.AllScreens.Length;
            }
        }

        public static ScreenSize GetScreenSize(int index)
        {
            if (index > (ScreenCount - 1))
                throw new Exception("索引异常");
            Screen screen = Screen.AllScreens[index];
            uint width = 0;
            uint height = 0;
            GetDpi(screen, DpiType.Effective, out width, out height);
            double scale = width / 96.0;
            return new ScreenSize { Left = (int)(screen.Bounds.X / scale), Top = screen.Bounds.Y, Width = (int)(screen.Bounds.Width / scale), Height = (int)(screen.Bounds.Height / scale), Scale = scale };
        }

        public static void GetDpi(Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 800, screen.Bounds.Top + 600);
            var mon = MonitorFromPoint(pnt, 2);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }

    }
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }



    public class ScreenSize
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double Scale { get; set; }
    }
}
#endif