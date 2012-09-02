using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET.Views
{
    class ChatView
    {
        ChatWindow wind;


        public static void sendMessageIfEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            e.Handled = true;

        } 


    }
}
