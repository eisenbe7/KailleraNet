using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace KailleraNET
{
    class ShowGameListCommand : ICommand
    {
        public void Execute(object parameter)
        {
            var k = KailleraWindowController.getMgr();
            k.showGamesWindow();
        }

        public Boolean CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;    
    }
}
