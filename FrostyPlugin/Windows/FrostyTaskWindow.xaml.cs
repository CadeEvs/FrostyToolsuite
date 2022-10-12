using FrostySdk.Converters;
using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shell;

namespace Frosty.Core.Windows
{
    public delegate void FrostyTaskCallback(FrostyTaskWindow owner);
    public delegate void FrostyTaskCancelCallback(FrostyTaskWindow owner);

    /// <summary>
    /// Interaction logic for FrostyTaskWindow.xaml
    /// </summary>
    public partial class FrostyTaskWindow : Window, INotifyPropertyChanged
    {
        private FrostyTaskCallback _callback;

        private FrostyTaskCancelCallback _cancelCallback;

        private double progress;

        private string status;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The progress of the inner task.
        /// </summary>
        public double Progress
        {
            get
            {
                return progress;
            }
            set
            {
                if (value != progress)
                {
                    progress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The status of the inner task.
        /// </summary>
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value != status)
                {
                    status = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ILogger TaskLogger { get; private set; }

        private FrostyTaskWindow(Window owner, string task, string initialStatus, FrostyTaskCallback callback, bool showCancelButton, FrostyTaskCancelCallback cancelCallback = null)
        {
            InitializeComponent();

            taskTextBlock.Text = task;
            Progress = 0.0;
            Status = initialStatus;

            _callback = callback;
            _cancelCallback = cancelCallback;

            Owner = owner;
            TaskLogger = new FrostyTaskLogger(this);
            Loaded += FrostyTaskWindow_Loaded;

            Application.Current.MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;

            BindingOperations.SetBinding(Application.Current.MainWindow.TaskbarItemInfo, TaskbarItemInfo.ProgressValueProperty, new Binding("Progress")
            {
                Converter = new DelegateBasedValueConverter(),
                ConverterParameter = new Func<object, object>(delegate (object value)
                {
                    return (double)value / 100.0;
                }),
                Source = this,
            });

            if (showCancelButton)
            {
                cancelButton.Visibility = Visibility.Visible;
                if (_cancelCallback != null)
                {
                    cancelButton.Click += CancelButton_Click;

                    // register the "Esc" keybinding to the cancel button click event
                    CommandBindings.RegisterKeyBindings(new Dictionary<KeyGesture, ExecutedRoutedEventHandler>
                    {
                        { new KeyGesture(Key.Escape), CancelButton_Click }
                    });
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelButton.IsEnabled = false;
            _cancelCallback(this);
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
            // null is reserved for preserving the current status
            if (status != null)
            {
                Status = status;
            }

            if (progress.HasValue)
            {
                Progress = progress.Value;
            }
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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
