using FrostySdk.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Frosty.Core.Windows
{
    public delegate void FrostyTaskCallback(FrostyTaskWindow owner);
    public delegate void FrostyTaskCancelCallback(FrostyTaskWindow owner);

    /// <summary>
    /// Interaction logic for FrostyTaskWindow.xaml
    /// </summary>
    public partial class FrostyTaskWindow : Window
    {
        private class FrostyTaskLogger : ILogger
        {
            private FrostyTaskWindow task;
            public FrostyTaskLogger(FrostyTaskWindow inTask)
            {
                task = inTask;
            }

            public void Log(string text, params object[] vars)
            {
                if (text.StartsWith("progress:"))
                {
                    text = text.Replace("progress:", "");
                    task.Update(progress: double.Parse(text));
                }
                else
                {
                    task.Update(string.Format(text, vars));
                }
            }

            public void LogWarning(string text, params object[] vars)
            {
                throw new NotImplementedException();
            }

            public void LogError(string text, params object[] vars)
            {
                throw new NotImplementedException();
            }
        }

        public ILogger TaskLogger { get; private set; }
        private FrostyTaskCallback _callback;
        private FrostyTaskCancelCallback _cancelCallback;

        private FrostyTaskWindow(Window owner, string task, string initialStatus, FrostyTaskCallback callback, bool showCancelButton, FrostyTaskCancelCallback cancelCallback = null)
        {
            InitializeComponent();

            taskTextBlock.Text = task;
            taskProgressBar.Value = 0.0;
            statusTextBox.Text = initialStatus;
            _callback = callback;
            _cancelCallback = cancelCallback;

            Owner = owner;
            TaskLogger = new FrostyTaskLogger(this);
            Loaded += FrostyTaskWindow_Loaded;

            if (showCancelButton)
            {
                cancelButton.Visibility = Visibility.Visible;
                if (_cancelCallback != null)
                {
                    cancelButton.Click += (o, e) =>
                    {
                        _cancelCallback.Invoke(this);
                        cancelButton.IsEnabled = false;
                    };
                }
                    
            }
        }

        private async void FrostyTaskWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                _callback(this);
            });

            Application.Current.MainWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;

            Close();
        }

        public void Update(string status = null, double? progress = null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (status != null)
                    statusTextBox.Text = status;
                if (progress.HasValue)
                {
                    taskProgressBar.Value = progress.Value;
                    Application.Current.MainWindow.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                    Application.Current.MainWindow.TaskbarItemInfo.ProgressValue = progress.Value / 100.0d;
                }
            });
        }

        public void SetIndeterminate(bool newIndeterminate)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                taskProgressBar.IsIndeterminate = newIndeterminate;
                Application.Current.MainWindow.TaskbarItemInfo.ProgressState = (newIndeterminate) ? System.Windows.Shell.TaskbarItemProgressState.Indeterminate : System.Windows.Shell.TaskbarItemProgressState.Normal;
            });
        }

        public static void Show(Window owner, string task, string initialStatus, FrostyTaskCallback callback, bool showCancelButton = false, FrostyTaskCancelCallback cancelCallback = null)
        {
            FrostyTaskWindow win = new FrostyTaskWindow(owner, task, initialStatus, callback, showCancelButton, cancelCallback);
            win.ShowDialog();
        }

        public static void Show(string task, string initialStatus, FrostyTaskCallback callback, bool showCancelButton = false, FrostyTaskCancelCallback cancelCallback = null)
        {
            Show(Application.Current.MainWindow, task, initialStatus, callback, showCancelButton, cancelCallback);
        }
    }
}
