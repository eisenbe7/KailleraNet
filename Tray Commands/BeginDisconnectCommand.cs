using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace KailleraNET
{
    class BeginDisconnectCommand : ICommand
    {
        public void Execute(object parameter)
        {
            if (MessageBox.Show("Disconnect and close Application?  Note that you can continue running Kaillera in your system tray.", "Disconnect?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                KailleraWindowController.getMgr().shutDown();
        }

        public Boolean CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
