using System;
using System.Collections.Generic;
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
        // 涉及弹簧模型的相关常数
        const int DELTA_TIME = 1;
        const double FORCE_COEF = 5;
        const double MIN_SPEED = 0.2;

        // 涉及弹簧模型的相关变量
        DispatcherTimer timer;
        double speedy, deltay;
        long lastTime;

        // 涉及气泡的相关常数
        const int MIN_WAIT_TIME = 10;
        const int MAX_WAIT_TIME = 500;

        // 涉及气泡的相关变量
        DispatcherTimer bubbleTimer;
        string bubbleString = "<占位符>";

        public MainWindow()
        {
            InitializeComponent();

            // 初始化弹簧模型
            speedy = 30;
            deltay = 0;
            lastTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // 弹簧定时器
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick; ;
            timer.IsEnabled = true;
            timer.Interval = TimeSpan.FromMilliseconds(DELTA_TIME);

            // 气泡定时器
            bubbleTimer = new DispatcherTimer();
            bubbleTimer.Tick += BubbleTimer_Tick;
            setRandomBubble();
        }

        // 处理一个弹簧模型的关键帧
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 计算距上一个关键帧过去的时间
            long dt = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - lastTime;
            // 记录当前关键帧的时间
            lastTime = lastTime + dt;
            // 根据弹簧模型计算当前加速度
            double a = FORCE_COEF * -deltay;
            // 根据加速度和时间间隔修改速度
            speedy += dt / 1000.0 * a;
            // 根据速度修正控件的坐标
            if (speedy > 0)
            {
                deltay += dt / 1000.0 * Math.Max(MIN_SPEED, speedy);
                HakureiReimu.Margin = new Thickness(
                    HakureiReimu.Margin.Left,
                    HakureiReimu.Margin.Top + dt / 1000.0 * Math.Max(MIN_SPEED, speedy),
                    HakureiReimu.Margin.Right,
                    HakureiReimu.Margin.Bottom
                );
            }
            else
            {
                deltay += dt / 1000.0 * Math.Min(-MIN_SPEED, speedy);
                HakureiReimu.Margin = new Thickness(
                    HakureiReimu.Margin.Left,
                    HakureiReimu.Margin.Top + dt / 1000.0 * Math.Min(-MIN_SPEED, speedy),
                    HakureiReimu.Margin.Right,
                    HakureiReimu.Margin.Bottom
                );
            }
        }

        // 设置一个随机间隔的气泡
        private void setRandomBubble()
        {
            Random rnd = new Random();
            int second = rnd.Next(MIN_WAIT_TIME, MAX_WAIT_TIME);
            bubbleTimer.Interval = TimeSpan.FromSeconds(second);
            bubbleTimer.IsEnabled = true;
        }

        // 弹出一个随机内容的气泡
        private void BubbleTimer_Tick(object sender, EventArgs e)
        {
            List<string> mylist = new List<string>(new string[] { "你好！我是灵梦，是一名巫女！", "梦想封印！"});
            Random rnd = new Random();
            TalkBubbleContent.Text = mylist[rnd.Next(0, 2)];
            TalkBubble.Visibility = Visibility.Visible;
            TalkBubbleContent.Visibility = Visibility.Visible;
            bubbleTimer.IsEnabled = false;
        }

        private void TalkBubble_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TalkBubble.Visibility = Visibility.Hidden;
            TalkBubbleContent.Visibility = Visibility.Hidden;
            setRandomBubble();
        }

        // 置于右下角
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        // 处理拖拽和动画暂停
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.timer.IsEnabled = true;
                lastTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }
        }

        // 处理拖拽和动画暂停
        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.timer.IsEnabled = false;
                this.DragMove();
            }
        }
    }
}
