using Frosty.Controls;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Frosty.Core.Controls
{
    public class FrostyExceptionBox : FrostyDockableWindow
    {
        #region -- Properties --

        // Dependency property kept for backward compatibility
        public static readonly DependencyProperty ExceptionTextProperty = DependencyProperty.Register(nameof(ExceptionText), typeof(string), typeof(FrostyExceptionBox), new PropertyMetadata(""));
        public string ExceptionText
        {
            get => (string)GetValue(ExceptionTextProperty);
            set => SetValue(ExceptionTextProperty, value);
        }
        public string ExtraMessageText { get; set; } = string.Empty;
        public string LogText { get; set; } = string.Empty;
        public MessageBoxResult MessageBoxResult { get; private set; } = MessageBoxResult.Cancel;

        #endregion

        public FrostyExceptionBox()
        {
            Topmost = true;
            ShowInTaskbar = false;

            Height = 600;
            Width = 900;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Window mainWin = Application.Current?.MainWindow;

            if (mainWin != null)
            {
                Icon = mainWin.Icon;

                double x = mainWin.Left + (mainWin.Width / 2.0);
                double y = mainWin.Top + (mainWin.Height / 2.0);

                Left = x - (Width / 2.0);
                Top = y - (Height / 2.0);
            }

            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnButtonClick));
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is Button btn))
                return;

            switch (btn.Name)
            {
                case "PART_CopyExceptionButton":
                    CopyToClipboard(ExceptionText);
                    break;
                case "PART_CopyLogButton":
                    CopyToClipboard(LogText);
                    break;
                case "PART_OkButton":
                    MessageBoxResult = MessageBoxResult.OK;
                    Close();
                    break;
            }
        }

        private static void CopyToClipboard(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            try
            {
                Clipboard.SetText(text);
                Clipboard.Flush();
            }
            catch { } // Clipboard may be locked by another process
        }

        public static MessageBoxResult Show(Exception e)
            => Show(e, "Frosty Toolsuite", null);

        public static MessageBoxResult Show(Exception e, string title)
            => Show(e, title, null);

        public static MessageBoxResult Show(Exception e, string title, string extraMessage)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            string exceptionText = FormatException(e);
            string logText = (App.Logger as FrostyCore.FrostyLogger)?.LogText ?? "Logger unavailable";

            if (!WriteCrashLog(exceptionText, logText, extraMessage))
            {
                App.Logger?.LogError("Failed to write crash log file");
                logText += "\r\nFailed to write crash log file";
            }

            FrostyExceptionBox window = new FrostyExceptionBox
            {
                Title = title,
                ExceptionText = exceptionText,
                ExtraMessageText = extraMessage ?? string.Empty,
                LogText = logText
            };

            window.ShowDialog();
            return window.MessageBoxResult;
        }

        private static string FormatException(Exception e)
        {
            StringBuilder sb = new StringBuilder();

            AssemblyName entryAsm = Assembly.GetEntryAssembly()?.GetName();
            if (entryAsm != null)
                sb.AppendLine($"App: {entryAsm.Name} v{entryAsm.Version}");

            sb.AppendLine($"OS: {RuntimeInformation.OSDescription}");
            sb.AppendLine($"Culture: {CultureInfo.CurrentCulture.Name}");
            
            sb.AppendLine($"HResult: 0x{e.HResult:X8}");

            Exception baseEx = e.GetBaseException();
            if (baseEx != e && baseEx.HResult != e.HResult)
                sb.AppendLine($"Inner HResult: 0x{baseEx.HResult:X8}");

            sb.AppendLine();

            // Minimal unlocalize, the e.Message won't be translated, but HResult and Type is enough to debug.
            try
            {
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            
                sb.Append(e);
            }
            catch
            {
                sb.Append(e);
            }
            
            return sb.ToString();
        }

        private static bool WriteCrashLog(string exceptionText, string logText, string extraMessage)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = $"crashlog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";

            string[] candidatePaths =
            {
                Path.Combine(baseDir, "CrashLogs", fileName),
                Path.Combine(baseDir, fileName) // In case cannot create folder
            };

            foreach (string path in candidatePaths)
            {
                try
                {
                    string dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir))
                        Directory.CreateDirectory(dir);

                    using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        if (!string.IsNullOrEmpty(extraMessage))
                        {
                            writer.WriteLine(extraMessage);
                            writer.WriteLine();
                        }

                        writer.WriteLine("[Exception]");
                        writer.WriteLine(exceptionText);
                        writer.WriteLine();
                        writer.WriteLine("[Log]");
                        writer.Write(logText);
                    }

                    return true;
                }
                catch
                {
                    // Fall to next candidate path
                }
            }

            return false;
        }
    }
}
