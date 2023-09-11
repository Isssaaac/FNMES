#if NETFRAMEWORK || WINDOWS
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace PopupTool
{
    [ToolboxItem(false)]
    public class Popup : ToolStripDropDown
    {
        public Control Content
        {
            get
            {
                return this.content;
            }
        }

        public bool UseFadeEffect
        {
            get
            {
                return this.fade;
            }
            set
            {
                if (this.fade != value)
                {
                    this.fade = value;
                }
            }
        }

        public bool FocusOnOpen
        {
            get
            {
                return this.focusOnOpen;
            }
            set
            {
                this.focusOnOpen = value;
            }
        }
        public bool AcceptAlt
        {
            get
            {
                return this.acceptAlt;
            }
            set
            {
                this.acceptAlt = value;
            }
        }

        public bool Resizable
        {
            get
            {
                return this.resizable && this._resizable;
            }
            set
            {
                this.resizable = value;
            }
        }

        public new Size MinimumSize
        {
            get
            {
                return this.minSize;
            }
            set
            {
                this.minSize = value;
            }
        }

        public new Size MaximumSize
        {
            get
            {
                return this.maxSize;
            }
            set
            {
                this.maxSize = value;
            }
        }
        public bool IsOpen
        {
            get
            {
                return this._isOpen;
            }
        }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 134217728;
                return createParams;
            }
        }

        public Popup(Control content)
        {
            EventHandler handler = null;
            EventHandler handler2 = null;
            PaintEventHandler handler3 = null;
            this.focusOnOpen = true;
            this.acceptAlt = true;
            if (ReferenceEquals(content, null))
            {
                throw new ArgumentNullException("content");
            }
            this.content = content;
            this.fade = SystemInformation.IsMenuAnimationEnabled && SystemInformation.IsMenuFadeEnabled;
            this._resizable = true;
            this.AutoSize = false;
            this.DoubleBuffered = true;
            base.ResizeRedraw = true;
            this.host = new ToolStripControlHost(content);
            Padding padding1 = this.host.Margin = Padding.Empty;
            Padding padding2 = this.host.Padding = padding1;
            base.Padding = base.Margin = padding2;
            this.MinimumSize = content.MinimumSize;
            content.MinimumSize = content.Size;
            this.MaximumSize = content.MaximumSize;
            content.MaximumSize = content.Size;
            base.Size = content.Size;
            content.Location = Point.Empty;
            this.Items.Add(this.host);
            if (handler == null)
            {
                handler = delegate (object sender, EventArgs e) {
                    content = null;
                    this.Dispose(true);
                };
            }
            content.Disposed += handler;
            if (handler2 == null)
            {
                handler2 = (sender, e) => this.UpdateRegion();
            }
            content.RegionChanged += handler2;
            if (handler3 == null)
            {
                handler3 = (sender, e) => this.PaintSizeGrip(e);
            }
            content.Paint += handler3;
            this.UpdateRegion();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.acceptAlt && (keyData & Keys.Alt) == Keys.Alt)
            {
                if ((keyData & Keys.F4) != Keys.F4)
                {
                    return false;
                }
                base.Close();
            }
            return base.ProcessDialogKey(keyData);
        }

        protected void UpdateRegion()
        {
            if (base.Region != null)
            {
                base.Region.Dispose();
                base.Region = null;
            }
            if (this.content.Region != null)
            {
                base.Region = this.content.Region.Clone();
            }
        }

        public void Show(Control control)
        {
            this.Show(control, control.ClientRectangle);
        }

        public void Show(Control control, bool center)
        {
            this.Show(control, control.ClientRectangle, center);
        }

        public void Show(Control control, Rectangle area)
        {
            this.Show(control, area, false);
        }

        public void Show(Control control, Rectangle area, bool center)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            this.SetOwnerItem(control);
            this.resizableTop = (this.resizableLeft = false);
            Point point = control.PointToScreen(new Point(area.Left, area.Top + area.Height));
            Rectangle workingArea = Screen.FromControl(control).WorkingArea;
            if (center)
            {
                if (point.X + (area.Width + base.Size.Width) / 2 > workingArea.Right)
                {
                    this.resizableLeft = true;
                    point.X = workingArea.Right - base.Size.Width;
                }
                else
                {
                    this.resizableLeft = true;
                    point.X -= (base.Size.Width - area.Width) / 2;
                }
            }
            else if (point.X + base.Size.Width > workingArea.Left + workingArea.Width)
            {
                this.resizableLeft = true;
                point.X = workingArea.Left + workingArea.Width - base.Size.Width;
            }
            if (point.Y + base.Size.Height > workingArea.Top + workingArea.Height)
            {
                this.resizableTop = true;
                point.Y -= base.Size.Height + area.Height;
            }
            point = control.PointToClient(point);
            base.Show(control, point, ToolStripDropDownDirection.BelowRight);
        }

        protected override void SetVisibleCore(bool visible)
        {
            double opacity = base.Opacity;
            if (visible && this.fade && this.focusOnOpen)
            {
                base.Opacity = 0.0;
            }
            base.SetVisibleCore(visible);
            if (visible && this.fade && this.focusOnOpen)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (i > 1)
                    {
                        Thread.Sleep(20);
                    }
                    base.Opacity = opacity * (double)i / 5.0;
                }
                base.Opacity = opacity;
            }
        }

        private void SetOwnerItem(Control control)
        {
            if (control != null)
            {
                if (control is Popup)
                {
                    Popup popup = control as Popup;
                    this.ownerPopup = popup;
                    this.ownerPopup.childPopup = this;
                    base.OwnerItem = popup.Items[0];
                }
                else if (control.Parent != null)
                {
                    this.SetOwnerItem(control.Parent);
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.content.MinimumSize = base.Size;
            this.content.MaximumSize = base.Size;
            this.content.Size = base.Size;
            this.content.Location = Point.Empty;
            base.OnSizeChanged(e);
        }

        protected override void OnOpening(CancelEventArgs e)
        {
            if (this.content.IsDisposed || this.content.Disposing)
            {
                e.Cancel = true;
            }
            else
            {
                this.UpdateRegion();
                base.OnOpening(e);
            }
        }

        protected override void OnOpened(EventArgs e)
        {
            if (this.ownerPopup != null)
            {
                this.ownerPopup._resizable = false;
            }
            if (this.focusOnOpen)
            {
                this.content.Focus();
            }
            this._isOpen = true;
            base.OnOpened(e);
        }

        protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
        {
            if (this.ownerPopup != null)
            {
                this.ownerPopup._resizable = true;
            }
            this._isOpen = false;
            base.OnClosed(e);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (!this.InternalProcessResizing(ref m, false))
            {
                base.WndProc(ref m);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool ProcessResizing(ref Message m)
        {
            return this.InternalProcessResizing(ref m, true);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool InternalProcessResizing(ref Message m, bool contentControl)
        {
            if (m.Msg == 134 && m.WParam != IntPtr.Zero && this.childPopup != null && this.childPopup.Visible)
            {
                this.childPopup.Hide();
            }
            bool result;
            if (!this.Resizable)
            {
                result = false;
            }
            else if (m.Msg == 132)
            {
                result = this.OnNcHitTest(ref m, contentControl);
            }
            else
            {
                result = (m.Msg == 36 && this.OnGetMinMaxInfo(ref m));
            }
            return result;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool OnGetMinMaxInfo(ref Message m)
        {
            UnsafeMethods.MINMAXINFO minmaxinfo = (UnsafeMethods.MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(UnsafeMethods.MINMAXINFO));
            minmaxinfo.maxTrackSize = this.MaximumSize;
            minmaxinfo.minTrackSize = this.MinimumSize;
            Marshal.StructureToPtr(minmaxinfo, m.LParam, false);
            return true;
        }

        private bool OnNcHitTest(ref Message m, bool contentControl)
        {
            int x = UnsafeMethods.LOWORD(m.LParam);
            int y = UnsafeMethods.HIWORD(m.LParam);
            Point pt = base.PointToClient(new Point(x, y));
            GripBounds gripBounds = new GripBounds(contentControl ? this.content.ClientRectangle : base.ClientRectangle);
            IntPtr intPtr = new IntPtr(-1);
            if (this.resizableTop)
            {
                if (this.resizableLeft && gripBounds.TopLeft.Contains(pt))
                {
                    m.Result = (contentControl ? intPtr : ((IntPtr)13));
                    return true;
                }
                if (!this.resizableLeft && gripBounds.TopRight.Contains(pt))
                {
                    m.Result = (contentControl ? intPtr : ((IntPtr)14));
                    return true;
                }
                if (gripBounds.Top.Contains(pt))
                {
                    m.Result = (contentControl ? intPtr : ((IntPtr)12));
                    return true;
                }
            }
            else
            {
                if (this.resizableLeft && gripBounds.BottomLeft.Contains(pt))
                {
                    m.Result = (contentControl ? intPtr : ((IntPtr)16));
                    return true;
                }
                if (!this.resizableLeft && gripBounds.BottomRight.Contains(pt))
                {
                    m.Result = (contentControl ? intPtr : ((IntPtr)17));
                    return true;
                }
                if (gripBounds.Bottom.Contains(pt))
                {
                    m.Result = (contentControl ? intPtr : ((IntPtr)15));
                    return true;
                }
            }
            bool result;
            if (this.resizableLeft && gripBounds.Left.Contains(pt))
            {
                m.Result = (contentControl ? intPtr : ((IntPtr)10));
                result = true;
            }
            else if (!this.resizableLeft && gripBounds.Right.Contains(pt))
            {
                m.Result = (contentControl ? intPtr : ((IntPtr)11));
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public void PaintSizeGrip(PaintEventArgs e)
        {
            if (e != null && e.Graphics != null && this.resizable)
            {
                Size clientSize = this.content.ClientSize;
                using (Bitmap bitmap = new Bitmap(16, 16))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        if (Application.RenderWithVisualStyles)
                        {
                            if (this.sizeGripRenderer == null)
                            {
                                this.sizeGripRenderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
                            }
                            this.sizeGripRenderer.DrawBackground(graphics, new Rectangle(0, 0, 16, 16));
                        }
                        else
                        {
                            ControlPaint.DrawSizeGrip(graphics, this.content.BackColor, 0, 0, 16, 16);
                        }
                    }
                    GraphicsState gstate = e.Graphics.Save();
                    e.Graphics.ResetTransform();
                    if (this.resizableTop)
                    {
                        if (this.resizableLeft)
                        {
                            e.Graphics.RotateTransform(180f);
                            e.Graphics.TranslateTransform((float)(-(float)clientSize.Width), (float)(-(float)clientSize.Height));
                        }
                        else
                        {
                            e.Graphics.ScaleTransform(1f, -1f);
                            e.Graphics.TranslateTransform(0f, (float)(-(float)clientSize.Height));
                        }
                    }
                    else if (this.resizableLeft)
                    {
                        e.Graphics.ScaleTransform(-1f, 1f);
                        e.Graphics.TranslateTransform((float)(-(float)clientSize.Width), 0f);
                    }
                    e.Graphics.DrawImage(bitmap, clientSize.Width - 16, clientSize.Height - 16 + 1, 16, 16);
                    e.Graphics.Restore(gstate);
                }
            }
        }

        private const int frames = 5;

        private const int totalduration = 100;

        private const int frameduration = 20;

        private Control content;

        private bool fade;

        private bool focusOnOpen = true;

        private bool acceptAlt = true;

        private Popup ownerPopup;

        private Popup childPopup;

        private bool _resizable;

        private bool resizable;

        private ToolStripControlHost host;

        private Size minSize;

        private Size maxSize;

        private bool _isOpen;

        private bool resizableTop;

        private bool resizableLeft;

        private VisualStyleRenderer sizeGripRenderer;
    }
}
#endif