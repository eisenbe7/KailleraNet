using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace KailleraNET
{
    /// <summary>
    /// Comand to leave the current game
    /// </summary>
    class LeaveGameCommand : ICommand
    {
        public void Execute(object parameter)
        {
            KailleraWindowController.getMgr().exitGame();
        }

        public Boolean CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
