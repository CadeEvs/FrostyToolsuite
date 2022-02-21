using Frosty.Controls;
using Frosty.Core.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Frosty.Core.Controls
{
    public class FrostyExceptionBox : FrostyDockableWindow
    {
        #region -- Properties --

        #region -- ExceptionText --
        public static readonly DependencyProperty ExceptionTextProperty = DependencyProperty.Register("ExceptionText", typeof(string), typeof(FrostyExceptionBox), new PropertyMetadata(""));
        public string ExceptionText
        {
            get => (string)GetValue(ExceptionTextProperty);
            set => SetValue(ExceptionTextProperty, value);
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

        public static MessageBoxResult Show(Exception e, string title)
        {
            FrostyExceptionBox window = new FrostyExceptionBox
            {
                Title = title,
                ExceptionText = e.Message + "\n\n" + e.StackTrace
            };

            return (window.ShowDialog() == true) ? MessageBoxResult.OK : MessageBoxResult.Cancel;
        }
    }
}
