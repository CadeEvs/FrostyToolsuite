using Frosty.Controls;
using System;
using System.Diagnostics;
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
        public string CrashLogPath { get; private set; }
        public bool HasCrashLog => !string.IsNullOrEmpty(CrashLogPath);
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
                case "PART_LocateCrashLogButton":
                    LocateCrashLog();
                    break;
                case "PART_OkButton":
                    MessageBoxResult = MessageBoxResult.OK;
                    Close();
                    break;
            }
        }

        private void LocateCrashLog()
        {
            if (string.IsNullOrEmpty(CrashLogPath))
                return;

            try
            {
                string fullPath = Path.GetFullPath(CrashLogPath);
                if (!File.Exists(fullPath))
                    return;

                string explorerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
                using (Process.Start(new ProcessStartInfo(explorerPath, $"/select,\"{fullPath}\"")
                {
                    UseShellExecute = false
                })) { }
            }
            catch
            {
                // Ignore
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
            catch
            {
                // Clipboard may be locked by another process
            }
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

            string crashLogPath = WriteCrashLog(exceptionText, logText, extraMessage);
            if (crashLogPath == null)
            {
                App.Logger?.LogError("Failed to write crash log file");
                logText += "\r\nFailed to write crash log file";
            }

            FrostyExceptionBox window = new FrostyExceptionBox
            {
                Title = title,
                ExceptionText = exceptionText,
                ExtraMessageText = extraMessage ?? string.Empty,
                LogText = logText,
                CrashLogPath = crashLogPath
            };

            window.ShowDialog();
            return window.MessageBoxResult;
        }

        private static string FormatException(Exception e)
        {
            StringBuilder sb = new StringBuilder();

            AssemblyName entryAsm = Assembly.GetEntryAssembly()?.GetName();
            if (entryAsm != null)
            {
                string appVersion = Application.Current?.GetType().GetField("Version", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string;
                if (string.IsNullOrEmpty(appVersion))
                    appVersion = entryAsm.Version.ToString();

                sb.AppendLine($"App: {entryAsm.Name} v{appVersion}");
            }

            sb.AppendLine($"OS: {RuntimeInformation.OSDescription.Trim()}");
            sb.AppendLine($"Culture: {CultureInfo.CurrentCulture.Name}");
            
            sb.AppendLine($"HResult: 0x{e.HResult:X8}");

            Exception baseEx = e.GetBaseException();
            if (baseEx != e && baseEx.HResult != e.HResult)
                sb.AppendLine($"Inner HResult: 0x{baseEx.HResult:X8}");

            sb.AppendLine();

            // Minimal unlocalize, the e.Message won't be translated, but HResult and Type is enough to debug.
            CultureInfo origCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo origUiCulture = Thread.CurrentThread.CurrentUICulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                sb.Append(e);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = origCulture;
                Thread.CurrentThread.CurrentUICulture = origUiCulture;
            }
            
            return sb.ToString();
        }

        private static string WriteCrashLog(string exceptionText, string logText, string extraMessage)
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

                    return Path.GetFullPath(path);
                }
                catch
                {
                    // Fall to next candidate path
                }
            }

            return null;
        }
    }
}
