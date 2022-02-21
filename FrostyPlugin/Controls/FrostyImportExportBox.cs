using Frosty.Controls;
using System.Collections.Generic;
using System.Windows;

namespace Frosty.Core.Controls
{
    public enum FrostyImportExportType
    {
        Import,
        Export
    }

    public class FrostyImportExportBox : FrostyDockableWindow
    {
        public MessageBoxResult Result { get; set; }
        public IEnumerable<object> OptionsData { get; private set; }
        public bool IsImport { get; private set; }

        public FrostyImportExportBox()
        {
            Topmost = true;
            ShowInTaskbar = false;
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Width = 600;
            Height = 300;

            Window win = Application.Current.MainWindow;
            Icon = win.Icon;

            Result = MessageBoxResult.Cancel;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public void RequestClose(MessageBoxResult result)
        {
            Result = result;
            Close();
        }

        public static MessageBoxResult Show<OptionsType>(string title, FrostyImportExportType type, OptionsType data)
        {
            FrostyImportExportBox window = new FrostyImportExportBox
            {
                Title = title,
                OptionsData = new object[] {data},
                IsImport = (type == FrostyImportExportType.Import)
            };

            window.ShowDialog();
            return window.Result;
        }
    }
}
