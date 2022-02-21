using Frosty.Core.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Frosty.Core.Commands
{
    public class ImportExportBoxClickCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Button btn = parameter as Button;
            FrostyImportExportBox parentWin = Window.GetWindow(btn) as FrostyImportExportBox;

            string buttonName = (string)btn.Content;

            MessageBoxResult result = MessageBoxResult.None;
            if (buttonName == "Import") result = MessageBoxResult.OK;
            else if (buttonName == "Export") result = MessageBoxResult.OK;
            else result = MessageBoxResult.Cancel;

            parentWin.RequestClose(result);
        }
    }
}
