using System;
using System.Drawing;
using System.Windows.Forms;

namespace TouhouPet.Class
{
    public class TransparentForm : Form
    {

        Bitmap bitmap;

        public TransparentForm(Bitmap bitmap)
        {
            InitializeComponent();

            // 不显示在任务栏中
            this.ShowInTaskbar = false;

            // 置顶
            this.TopMost = true;

            // 不显示标题栏
            this.FormBorderStyle = FormBorderStyle.None;

            this.bitmap = bitmap;
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
                //Win32.UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, Win32.ULW_ALPHA);
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TransparentForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "TransparentForm";
            this.Load += new System.EventHandler(this.TransparentForm_Load);
            this.ResumeLayout(false);

        }

        private void TransparentForm_Load(object sender, EventArgs e)
        {
            SetBits(this.bitmap);
        }
    }
}
