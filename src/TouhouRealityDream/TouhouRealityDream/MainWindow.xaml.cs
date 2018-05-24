using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace TouhouRealityDream
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const int deltaTime = 1;
        const double K = 5;
        DispatcherTimer timer;
        double speedy, deltay;
        long lastTime;

        public MainWindow()
        {
            InitializeComponent();

            speedy = 30;
            deltay = 0;
            lastTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // 定时器
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick; ;
            timer.IsEnabled = true;
            timer.Interval = TimeSpan.FromMilliseconds(deltaTime);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            long dt = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - lastTime;
            // Console.WriteLine(dt);
            lastTime = lastTime + dt;
            double a = K * -deltay;
            speedy += dt / 1000.0 * a;
            Thickness old = HakureiReimu.Margin;
            if (speedy > 0) deltay += dt / 1000.0 * Math.Max(0.2, speedy);
            if (speedy < 0) deltay += dt / 1000.0 * Math.Min(-0.2, speedy);
            Console.WriteLine(deltay);
            if (speedy > 0) HakureiReimu.Margin = new Thickness(old.Left, old.Top + dt / 1000.0 * Math.Max(0.2, speedy), old.Right, old.Bottom);
            if (speedy < 0) HakureiReimu.Margin = new Thickness(old.Left, old.Top + dt / 1000.0 * Math.Min(-0.2, speedy), old.Right, old.Bottom);
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
