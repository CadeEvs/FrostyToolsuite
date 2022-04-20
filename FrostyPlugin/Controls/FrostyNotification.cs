using Frosty.Core.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace Frosty.Core.Controls
{
    public class FrostyNotification : Window
    {
        public string NotificationTitle => title;

        private string title;
        private int duration;

        private FrostyTimer timer;

        static FrostyNotification()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyNotification), new FrameworkPropertyMetadata(typeof(FrostyNotification)));
        }

        public FrostyNotification(string textToDisplay, int displayTime)
        {
            // update bindings
            title = textToDisplay;
            duration = displayTime;

            // styling
            WindowStyle = WindowStyle.None;
            Width = Math.Max(300, Math.Min(500, textToDisplay.Length * 8));
            Height = 50;
            ShowInTaskbar = false;
            Focusable = false;
            IsHitTestVisible = false;
            ShowActivated = false;
            ResizeMode = ResizeMode.NoResize;

            // events
            Loaded += Notification_Loaded;
        }

        private void Notification_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new FrostyTimer(Notification_TimerExpired, duration * 1000, false);

            Console.WriteLine(timer.TimeLeft);
        }

        private void Notification_TimerExpired(object sender, ElapsedEventArgs e)
        {
            timer.Dispose();

            Dispatcher.Invoke(() =>
            {
                App.NotificationManager.RemoveNotification(this);
            });
        }
    }
}
