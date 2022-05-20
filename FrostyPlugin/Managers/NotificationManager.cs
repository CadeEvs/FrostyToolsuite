using Frosty.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Frosty.Core.Managers
{
    public class NotificationManager
    {
        private List<FrostyNotification> notifications = new List<FrostyNotification>();

        private void AddNotification(FrostyNotification notification)
        {
            var window = App.EditorWindow as Window;
            if (window != null)
            {
                notification.Owner = window;
                notification.Top = window.Top + window.ActualHeight - notification.Height - 10;
                notification.Left = window.Left + window.ActualWidth - notification.Width - 10;

                foreach (var existingNotification in notifications)
                {
                    existingNotification.Top -= 51;
                }

                notifications.Add(notification);
                notification.Show();
            }
        }

        public void RemoveNotification(FrostyNotification notification)
        {
            notifications.Remove(notification);
            notification.Close();
        }

        public async void Show(string text, int duration = 3)
        {
            if (Thread.CurrentThread == Application.Current.Dispatcher.Thread)
            {
                var notification = new FrostyNotification(text, duration);
                AddNotification(notification);
            }
            else
            {
                await Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    var notification = new FrostyNotification(text, duration);
                    AddNotification(notification);
                }));
            }
        }
    }
}
