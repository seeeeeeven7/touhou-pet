using System;
using System.Drawing;
using System.Windows.Forms;
using TouhouPet.Class;

namespace TouhouPet
{
    public partial class Avatar : PNGForm
    {
        Timer timer;
        Timer bubbleTimer;

        public Avatar() : base(Properties.Resources.HakureiReimu)
        {
            InitializeComponent();
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


        Bitmap bitmap = new Bitmap(Properties.Resources.HakureiReimu);
        Bitmap bllm = new Bitmap(Properties.Resources.HakureiReimu);

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
            TalkBubble talkBubble = new TalkBubble();
            talkBubble.Show();
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

        }*/
    }
}