using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostyEditor.Windows;
using System;
using System.IO;
using System.Threading;
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

                    // setup ability to cancel the process
                    CancellationTokenSource cancelToken = new CancellationTokenSource();

                    FrostyTaskWindow.Show("Saving Mod", "", (task) =>
                    {
                        try
                        {
                            mainWin.ExportMod(mainWin.Project.GetModSettings(), filename, false, cancelToken.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            // process was cancelled
                            App.Logger.Log("Export Cancelled");

                            if (File.Exists(filename))
                            {
                                File.Delete(filename);
                            }
                        }
                    }, showCancelButton: true, cancelCallback: (task) => cancelToken.Cancel());
                }
            }
        }
    }
}
