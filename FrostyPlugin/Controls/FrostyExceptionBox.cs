using Frosty.Controls;
using System;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using System.Windows;

namespace Frosty.Core.Controls
{
    /// <summary>
    /// Handle button click
    /// </summary>
    public class ExceptionBoxClickCommand : System.Windows.Input.ICommand
    {
        public event EventHandler CanExecuteChanged {
            add => System.Windows.Input.CommandManager.RequerySuggested += value;
            remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            System.Windows.Controls.Button btn = parameter as System.Windows.Controls.Button;
            FrostyExceptionBox parentWin = Window.GetWindow(btn) as FrostyExceptionBox;

            string buttonName = btn.Name;

            if (buttonName == "PART_CopyExceptionButton")
            {
                Clipboard.SetText(parentWin.ExceptionText);
                Clipboard.Flush();
            }
            else if (buttonName == "PART_CopyLogButton")
            {
                Clipboard.SetText(parentWin.LogText);
                Clipboard.Flush();
            }
            else if (buttonName == "PART_IgnoreButton")
            {
                parentWin.isIgnored = true;
                parentWin.Close();
            }
        }
    }

    public class FrostyExceptionBox : FrostyDockableWindow
    {
        #region -- Properties --

        public bool isIgnored { get; internal set; } = false;

        #region -- Text --
        public static readonly DependencyProperty ExceptionTextProperty = DependencyProperty.Register("ExceptionText", typeof(string), typeof(FrostyExceptionBox), new PropertyMetadata(""));
        public string ExceptionText
        {
            get => (string)GetValue(ExceptionTextProperty);
            set => SetValue(ExceptionTextProperty, value);
        }

        public static readonly DependencyProperty ExceptionMessageTextProperty = DependencyProperty.Register("ExceptionMessageText", typeof(string), typeof(FrostyExceptionBox), new PropertyMetadata(""));
        public string ExceptionMessageText {
            get => (string)GetValue(ExceptionMessageTextProperty);
            set => SetValue(ExceptionMessageTextProperty, value);
        }

        public static readonly DependencyProperty LogTextProperty = DependencyProperty.Register("LogText", typeof(string), typeof(FrostyExceptionBox), new PropertyMetadata(""));
        public string LogText {
            get => (string)GetValue(LogTextProperty);
            set => SetValue(LogTextProperty, value);
        }
        #endregion

        #endregion

        public FrostyExceptionBox()
        {
            Topmost = true;
            ShowInTaskbar = false;

            Height = 600;
            Width = 900;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Window mainWin = Application.Current.MainWindow;

            if (mainWin != null)
            {
                Icon = mainWin.Icon;

                double x = mainWin.Left + (mainWin.Width / 2.0);
                double y = mainWin.Top + (mainWin.Height / 2.0);

                Left = x - (Width / 2.0);
                Top = y - (Height / 2.0);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public static MessageBoxResult Show(Exception e, string title = "Frosty Toolsuite")
        {
            FrostyExceptionBox window = new FrostyExceptionBox
            {
                Title = title,
                ExceptionText = UnlocalizeException(e),
                LogText = (App.Logger as FrostyCore.FrostyLogger).LogText,
                ExceptionMessageText = e.Message
            };

            // Write crash log
            try
            {
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\CrashLogs");
                using (StreamWriter writer = new StreamWriter(new FileStream($"{Environment.CurrentDirectory}\\CrashLogs\\{DateTime.Now.ToString("ddMMyyyy_HHmmss")}_{e.Source}.txt", FileMode.Create)))
                {
                    writer.WriteLine(window.ExceptionText);
                    writer.WriteLine("Log:");
                    writer.Write(window.LogText);
                }
            }
            catch (IOException) // Directory maybe in use
            {
                using (StreamWriter writer = new StreamWriter(new FileStream($"crashlog_{DateTime.Now.ToString("ddMMyyyy_HHmmss")}.txt", FileMode.Create)))
                {
                    writer.WriteLine(window.ExceptionText);
                    writer.WriteLine("Log:");
                    writer.Write(window.LogText);
                }
            }
            catch
            {
                App.Logger.LogError("Failed to write crash log");
            }
            

            window.ShowDialog();

            // Return MessageBoxResult.Cancel if user click Ignore
            if (window.isIgnored) return MessageBoxResult.Cancel;
            return (window.DialogResult == true) ? MessageBoxResult.OK : MessageBoxResult.No;
        }

        /// <summary>
        /// Generate exception message in English (if possible)
        /// </summary>
        private static string UnlocalizeException(Exception ex)
        {
            try
            {
                // Call UnlocalizedExceptionGenerator to get exception message in English
                UnlocalizedExceptionGenerator ueg = new UnlocalizedExceptionGenerator(ex, System.Threading.Thread.CurrentThread.CurrentUICulture);
                System.Threading.Thread thread = new System.Threading.Thread(ueg.Run)
                {
                    CurrentCulture = CultureInfo.InvariantCulture,
                    CurrentUICulture = CultureInfo.InvariantCulture
                };
                thread.Start();
                thread.Join();

                return ueg.ExceptionDetails;
            }
            catch
            {
                App.Logger.LogError("Failed to translate exception");

                StringBuilder sb = new StringBuilder();
                sb.Append("Type=");
                sb.AppendLine(ex.GetType().ToString());
                sb.Append("HResult=");
                sb.AppendLine("0x" + ex.HResult.ToString("X"));
                sb.Append("Message=");
                sb.AppendLine(ex.Message);
                sb.Append("Source=");
                sb.AppendLine(ex.Source);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(ex.StackTrace);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Use for generate exception message in English
        /// https://stackoverflow.com/questions/209133/exception-messages-in-english
        /// </summary>
        private class UnlocalizedExceptionGenerator
        {
            private Exception _ex;
            private CultureInfo _origCultureInfo;

            public string ExceptionDetails;

            /// <summary>
            /// </summary>
            /// <param name="cultureInfo">Current(user’s) CultureInfo</param>
            public UnlocalizedExceptionGenerator(Exception ex, CultureInfo origCultureInfo)
            {
                _ex = ex;
                _origCultureInfo = origCultureInfo;
            }

            public void Run()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Type=");
                sb.AppendLine(_ex.GetType().ToString());
                sb.Append("HResult=");
                sb.AppendLine("0x" + _ex.HResult.ToString("X"));
                sb.Append("Message=");

                // Find message in .net localize resources
                string message = _ex.Message;
                try
                {
                    System.Reflection.Assembly assembly = _ex.GetType().Assembly;
                    ResourceManager rm = new ResourceManager(assembly.GetName().Name, assembly);
                    ResourceSet originalResources = rm.GetResourceSet(_origCultureInfo, true, true);
                    ResourceSet targetResources = rm.GetResourceSet(CultureInfo.InvariantCulture, true, true);
                    foreach (System.Collections.DictionaryEntry originalResource in originalResources)
                    {
                        if (originalResource.Value.ToString().Equals(_ex.Message.ToString(), StringComparison.Ordinal))
                        {
                            message = targetResources.GetString(originalResource.Key.ToString(), false);
                            break;
                        }
                    }
                }
                catch { }
                sb.AppendLine(message);

                sb.Append("Source=");
                sb.AppendLine(_ex.Source);
                sb.AppendLine("StackTrace:");
                sb.AppendLine(_ex.StackTrace);

                ExceptionDetails = sb.ToString();
            }
        }
    }
}
