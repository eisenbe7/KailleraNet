using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace KailleraNET
{
    /// <summary>
    /// Maintains references to all windows, and handles all window events
    /// </summary>
    public class KailleraWindowMananger
    {
        List<ChatWindow> ChatWindows = new List<ChatWindow>();
        KailleraMananger mgr;


        /// <summary>
        /// Updates the chat window with a GUIBundle.  At this point there shouldn't
        /// be more than one window receiving the update, but maybe in the future there will be
        /// </summary>
        /// <param name="gb">The UI update - sent to corresponding window based on game number</param>
        public void updateWindow(GUIBundle gb)
        {
            
            int gamenumber = gb.gameNum;
            IEnumerable<ChatWindow> targwind = from wind in ChatWindows where wind.gameNumber == gamenumber select wind;
            foreach (ChatWindow c in targwind)
            {
                c.updater(gb);
            }
        }


        /// <summary>
        /// Begins a connection to the Kaillera server from the connection menu, and opens main chat window
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        internal void BeginNewConnection(System.Net.IPAddress ip, int port, string username)
        {
            //Add one chat window - 0 for main server chat
            ChatWindow mainWin = new ChatWindow(0, this);
            mainWin.Closed += onMainChatClosed;
            mainWin.gameNumber = 0;
            ChatWindows.Add(mainWin);
            mainWin.Show();

            mgr = new KailleraMananger(ip, port, username);
            Thread KManager = new Thread(new ParameterizedThreadStart(mgr.Start));
            KManager.Start(this);
        }

        /// <summary>
        /// Sets KaiileraManager to stop upon main chat window close.  KailleraMananger sends logout packet before terminating.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onMainChatClosed(object sender, EventArgs e)
        {
            mgr.connectionClosed();
        }

        /// <summary>
        /// Sends message to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Chat_sendMessageIfEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            e.Handled = true;
            TextBox c = (TextBox)sender;
            string textMessage = c.Text;
            c.Text = null;
            if (String.IsNullOrEmpty(textMessage)) return;
            mgr.SendServerChatText(textMessage);
        } 



    }
}
