using System;
using System.Drawing;
using System.Windows.Forms;

namespace TouhouPet.Class
{
    public class PNGForm : Form
    {
        // 展示的背景图片
        Bitmap DisplayBitmap = null;
        // 是否可以拖动整个窗体
        protected bool Draggable = false;

        protected int recordX, recordY;
        public void RecordPosition()
        {
            recordX = this.Location.X;
            recordY = this.Location.Y;
        }

        public PNGForm(Bitmap displayBitmap)
        {
            InitializeComponent();

            // 不显示在任务栏中
            this.ShowInTaskbar = false;

            // 置顶
            this.TopMost = true;

            // 不显示标题栏
            this.FormBorderStyle = FormBorderStyle.None;

            // 背景bitmap
            this.DisplayBitmap = displayBitmap;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Load += new System.EventHandler(this.TransparentForm_Load);
            this.MouseDown += PNGForm_MouseDown;
            this.MouseMove += PNGForm_MouseMove;
            this.MouseUp += PNGForm_MouseUp;
            this.ResumeLayout(false);
        }

        // 记录当前窗体是否正在拖拽
        bool isDragging = false;
        // 记录上次事件发生的位置
        Point lastLocation;

        private void PNGForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.Draggable)
            {
                isDragging = true;
                lastLocation = e.Location;
            }
        }

        private void PNGForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                this.Location = new Point(
                    this.Location.X - lastLocation.X + e.X,
                    this.Location.Y - lastLocation.Y + e.Y
                );
                this.Update();
            }
        }

        private void PNGForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                RecordPosition();
            }
        }

        bool haveHandle = false;

        protected override void OnHandleCreated(EventArgs e)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            UpdateStyles();
            base.OnHandleCreated(e);
            haveHandle = true;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cParms = base.CreateParams;
                cParms.ExStyle |= 0x00080000; // WS_EX_LAYERED
                cParms.ExStyle |= 0x00080000;
                return cParms;
            }
        }

        public void SetBits(Bitmap bitmap)
        {
            if (!haveHandle) return;

            if (!Bitmap.IsCanonicalPixelFormat(bitmap.PixelFormat) || !Bitmap.IsAlphaPixelFormat(bitmap.PixelFormat))
                throw new ApplicationException("图片必须是32位带Alhpa通道的图片。");

            IntPtr oldBits = IntPtr.Zero;
            IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr memDc = Win32.CreateCompatibleDC(screenDC);

            try
            {
                Win32.Point topLoc = new Win32.Point(this.Location.X, this.Location.Y);
                Win32.Size bitMapSize = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.BLENDFUNCTION blendFunc = new Win32.BLENDFUNCTION();
                Win32.Point srcLoc = new Win32.Point(0, 0);

                // 将bitmap创建为一个GDI位图对象
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                // 将hBitmap绘制到memDc当中，并记录被替换下的Bits
                oldBits = Win32.SelectObject(memDc, hBitmap);

                blendFunc.BlendOp = Win32.AC_SRC_OVER;
                blendFunc.SourceConstantAlpha = 255;
                blendFunc.AlphaFormat = Win32.AC_SRC_ALPHA;
                blendFunc.BlendFlags = 0;

                Win32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32.ULW_ALPHA);
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBits);
                    Win32.DeleteObject(hBitmap);
                }
                Win32.ReleaseDC(IntPtr.Zero, screenDC);
                Win32.DeleteDC(memDc);
            }
        }

        private void TransparentForm_Load(object sender, EventArgs e)
        {
            RecordPosition();
            SetBits(this.DisplayBitmap);
        }
    }
}
