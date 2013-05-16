using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using KailleraNET.Util;

namespace KailleraNET
{
    /// <summary>
    /// Command to show the proper windows from the tray commands
    /// </summary>
    class ShowUsersWindowCommand : ICommand
    {
        public void Execute(object parameter)
        {
            KailleraWindowController.getMgr().showUsersWindow();            
        }

        public Boolean CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

    }
}
