using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TouhouPet
{
    public partial class Avatar : Form
    {
        bool haveHandle = false;
        Timer timer;
        Timer bubbleTimer;

        public Avatar()
        {
            InitializeComponent();
            
            // 不显示标题栏
            this.FormBorderStyle = FormBorderStyle.None;

            // 不显示在任务栏中
            this.ShowInTaskbar = false;

            // 置顶
            this.TopMost = true;
        }

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
                Win32.Point topLoc = new Win32.Point(recordX, (int)(recordY + deltay));
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

        Bitmap bitmap = new Bitmap(Properties.Resources.bllm);
        Bitmap bllm = new Bitmap(Properties.Resources.bllm);

        // 加载响应
        private void Avatar_Load(object sender, EventArgs e)
        {
            ResetAnimation();

            SetBits(bitmap);

            // 定时器
            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Enabled = true;
            timer.Interval = deltaTime;

            bubbleTimer = new Timer();
            bubbleTimer.Tick += BubbleTimer_Tick;
            bubbleTimer.Enabled = true;
            bubbleTimer.Interval = 2000;
        }

        private void BubbleTimer_Tick(object sender, EventArgs e)
        {
            Bubble bubble = new Bubble(Properties.Resources.bubble);
            bubble.Show();
            bubbleTimer.Enabled = false;
        }

        const int deltaTime = 1;
        const double K = 5;
        double speedy, deltay;
        int recordX, recordY;
        long lastTime;

        private void ResetAnimation()
        {
            speedy = 30;
            deltay = 0;
            recordX = this.Location.X;
            recordY = this.Location.Y;
            lastTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond; 
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            long dt = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - lastTime;
            lastTime = lastTime + dt;
            double a = K * -deltay;
            speedy += dt / 1000.0 * a;
            if (speedy > 0) deltay += dt / 1000.0 * Math.Max(0.2, speedy);
            if (speedy < 0) deltay += dt / 1000.0 * Math.Min(-0.2, speedy);
            SetBits(bitmap);
        }

        // 拖拽响应
        bool mouseDown = false;
        private Point lastLocation;

        private void Avatar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                timer.Enabled = false;
                lastLocation = e.Location;
            }
        }

        private void Avatar_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    this.Location.X - lastLocation.X + e.X,
                    this.Location.Y - lastLocation.Y + e.Y
                );
                this.Update();
            }
        }

        private void Avatar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = false;
                timer.Enabled = true;
                ResetAnimation();
            }
        }

        // 点击响应
        private void Avatar_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}