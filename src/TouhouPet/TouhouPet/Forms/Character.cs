using System;
using System.Drawing;
using System.Windows.Forms;
using TouhouPet.Class;

namespace TouhouPet.Forms
{
    class Character : PNGForm
    {
        Timer timer;
        Timer bubbleTimer;

        public Character() : base(Properties.Resources.HakureiReimu)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Draggable = true;
            this.Load += Character_Load;
            this.MouseClick += Character_MouseClick;
            this.MouseDown += Character_MouseDown;
            this.MouseUp += Character_MouseUp;
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Properties.Resources.HakureiReimu.Size.Width - 50, 
                workingArea.Bottom - Properties.Resources.HakureiReimu.Size.Height - 50);

            this.ResumeLayout(false);

            // 定时器
            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Enabled = true;
            timer.Interval = deltaTime;
            Console.WriteLine(this.Location);

            bubbleTimer = new Timer();
            bubbleTimer.Tick += BubbleTimer_Tick;
            bubbleTimer.Enabled = true;
            bubbleTimer.Interval = 2000;
        }

        private void Character_MouseDown(object sender, MouseEventArgs e)
        {
            timer.Enabled = false;
        }

        private void Character_MouseUp(object sender, MouseEventArgs e)
        {
            timer.Enabled = true;
            ResetAnimation();
        }

        const int deltaTime = 1;
        const double K = 5;
        double speedy, deltay;
        long lastTime;

        private void ResetAnimation()
        {
            speedy = 30;
            deltay = 0;
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
            this.Location = new Point(recordX, recordY + (int)deltay);
        }

        private void BubbleTimer_Tick(object sender, EventArgs e)
        {
            TalkBubble talkBubble = new TalkBubble();
            talkBubble.Show();
            bubbleTimer.Enabled = false;
        }

        private void Character_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        private void Character_Load(object sender, EventArgs e)
        {
            ResetAnimation();
        }
    }
}
