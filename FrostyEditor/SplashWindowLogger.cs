using FrostyEditor.Windows;
using FrostySdk.Converters;
using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shell;

namespace FrostyEditor
{
    internal class SplashWindowLogger : ILogger, INotifyPropertyChanged
    {
        private SplashWindow parent;

        private double progress;

        private string status;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The progress of the current task.
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
        /// The splash window's status.
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

        public SplashWindowLogger(SplashWindow inParent)
        {
            parent = inParent;

            // Utilize DataBindings to eliminate need for Dispatcher
            BindingOperations.SetBinding(parent.logTextBox, TextBlock.TextProperty, new Binding("Status")
            {
                Source = this
            });
            BindingOperations.SetBinding(parent.progressBar, ProgressBar.ValueProperty, new Binding("Progress")
            {
                Source = this
            });

            parent.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;

            BindingOperations.SetBinding(parent.TaskbarItemInfo, TaskbarItemInfo.ProgressValueProperty, new Binding("Progress")
            {
                Converter = new FunctionBasedValueConverter(),
                ConverterParameter = new Func<object, object>(delegate (object value)
                {
                    return (double)value / 100.0;
                }),
                Source = this,
            });
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Log(string text, params object[] vars)
        {
            string fullText = string.Format(text, vars);

            if (fullText.StartsWith("progress:"))
            {
                fullText = fullText.Replace("progress:", "");
                Progress = double.Parse(fullText);
            }
            else
            {
                Status = fullText;
            }
        }

        public void LogError(string text, params object[] vars)
        {
        }

        public void LogWarning(string text, params object[] vars)
        {
        }
    }
}
