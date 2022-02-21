using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostyEditor.Windows;
using System;
using System.Windows.Input;

namespace FrostyEditor.Commands
{
    class ExportModMenuItemCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return (App.AssetManager != null && App.AssetManager.GetModifiedCount() != 0);
        }

        public void Execute(object parameter)
        {
            MainWindow mainWin = parameter as MainWindow;
            ModSettingsWindow win = new ModSettingsWindow(mainWin.Project);
            win.ShowDialog();

            if (win.DialogResult == true)
            {
                FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save Mod", "*.fbmod (Frosty Mod)|*.fbmod", "Mod");
                if (sfd.ShowDialog())
                {
                    string filename = sfd.FileName;
                    FrostyTaskWindow.Show("Saving Mod", "", (task) => { mainWin.ExportMod(mainWin.Project.GetModSettings(), filename, false); });
                }
            }
        }
    }
}
