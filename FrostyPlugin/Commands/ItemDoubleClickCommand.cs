using System;
using System.Windows.Input;

namespace Frosty.Core.Commands
{
    public class ItemDoubleClickCommand : ICommand
    {
#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public delegate void DoubleClickCommandDelegate();
        private readonly DoubleClickCommandDelegate commandToRun;

        public ItemDoubleClickCommand(DoubleClickCommandDelegate inCommand)
        {
            commandToRun = inCommand;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            commandToRun?.Invoke();
        }
    }
}
