#if NETFRAMEWORK || WINDOWS
using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal class UnsafeMethods
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(IntPtr hwnd, IntPtr hDC);

    [DllImport("user32")]
    public static extern bool TrackMouseEvent(ref UnsafeMethods.TRACKMOUSEEVENT lpEventTrack);

    [DllImport("user32.dll")]
    public static extern bool SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    internal static int HIWORD(int n)
    {
        return n >> 16 & 65535;
    }

    internal static int HIWORD(IntPtr n)
    {
        return UnsafeMethods.HIWORD((int)((long)n));
    }

    internal static int LOWORD(int n)
    {
        return n & 65535;
    }

    internal static int LOWORD(IntPtr n)
    {
        return UnsafeMethods.LOWORD((int)((long)n));
    }

    public const int WM_LBUTTONDOWN = 513;

    public const int WM_RBUTTONDOWN = 516;

    public const int WM_MBUTTONDOWN = 519;

    public const int WM_NCLBUTTONDOWN = 161;

    public const int WM_NCRBUTTONDOWN = 164;

    public const int WM_NCMBUTTONDOWN = 167;

    public const int WM_NCCALCSIZE = 131;

    public const int WM_NCHITTEST = 132;

    public const int WM_NCPAINT = 133;

    public const int WM_NCACTIVATE = 134;

    public const int WM_MOUSELEAVE = 675;

    public const int WS_EX_NOACTIVATE = 134217728;

    public const int HTTRANSPARENT = -1;

    public const int HTLEFT = 10;

    public const int HTRIGHT = 11;

    public const int HTTOP = 12;

    public const int HTTOPLEFT = 13;

    public const int HTTOPRIGHT = 14;

    public const int HTBOTTOM = 15;

    public const int HTBOTTOMLEFT = 16;

    public const int HTBOTTOMRIGHT = 17;

    public const int WM_USER = 1024;

    public const int WM_REFLECT = 8192;

    public const int WM_COMMAND = 273;

    public const int CBN_DROPDOWN = 7;

    public const int WM_GETMINMAXINFO = 36;

    public static readonly IntPtr TRUE = new IntPtr(1);

    public static readonly IntPtr FALSE = new IntPtr(0);

    public enum TrackerEventFlags : uint
    {
        TME_HOVER = 1U,
        TME_LEAVE,
        TME_QUERY = 1073741824U,
        TME_CANCEL = 2147483648U
    }
    internal struct TRACKMOUSEEVENT
    {
        public uint cbSize;

        public uint dwFlags;

        public IntPtr hwndTrack;

        public uint dwHoverTime;
    }

    internal struct MINMAXINFO
    {
        public Point reserved;

        public Size maxSize;

        public Point maxPosition;

        public Size minTrackSize;

        public Size maxTrackSize;
    }

    internal struct RECT
    {
        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public Rectangle Rect
        {
            get
            {
                return new Rectangle(this.left, this.top, this.right - this.left, this.bottom - this.top);
            }
        }

        public static UnsafeMethods.RECT FromXYWH(int x, int y, int width, int height)
        {
            return new UnsafeMethods.RECT(x, y, x + width, y + height);
        }

        public static UnsafeMethods.RECT FromRectangle(Rectangle rect)
        {
            return new UnsafeMethods.RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    internal struct WINDOWPOS
    {
        internal IntPtr hwnd;
        internal IntPtr hWndInsertAfter;
        internal int x;
        internal int y;
        internal int cx;
        internal int cy;
        internal uint flags;
    }

    public struct NCCALCSIZE_PARAMS
    {
        public UnsafeMethods.RECT rectProposed;
        public UnsafeMethods.RECT rectBeforeMove;
        public UnsafeMethods.RECT rectClientBeforeMove;
        public UnsafeMethods.WINDOWPOS lpPos;
    }
}
#endif