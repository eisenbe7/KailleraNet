using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Documents;
using KailleraNET.Util;
using System.Windows;
using System.Windows.Controls;


namespace KailleraNET.Views
{
    class ChatViewModel
    {
        public ChatWindow wind;

        private ObservableCollection<User> displayUsers = new ObservableCollection<User>();
        private System.Net.IPAddress ip;


        public delegate void onClose(object sender, EventArgs e);

        public onClose winClosed = delegate {};

        public delegate void LeaveGame(int gameNum);
        public LeaveGame leaveGame;

        public delegate void GameChat(int gamenum, string text);
        public GameChat sendGameChat;

        public delegate void AddBuddyList(User user);
        public AddBuddyList addBuddyList;




        //Delegate to update the users - stupid implementation
        private delegate void upUsers(List<User> u);
        FlowDocument doc = new FlowDocument();

        //zero for main chat
        public int gameNumber;

        //UI delegate that calls UI method
        public delegate void UpdateGUI(GUIBundle bundle);
        public UpdateGUI updater;


        private ObservableCollection<User> DisplayUsers
        {
            get
            {
                return displayUsers;
            }
            set
            {
                value = displayUsers;
            }

        }

        public ChatViewModel(int gamenum, KailleraWindowController mgr, Game game = null)
        {
            wind = new ChatWindow();
            gameNumber = gamenum;
            wind.Userup += UpdateUsers;
            updater += wind.WindowUpdate;            
            wind.listBox1.ItemsSource = DisplayUsers;
            wind.addBuddyList += addBuddy;

            wind.Logger = ChatLogger.getLog(game);

            //Set small line height to avoid large line breaks
            Paragraph p = wind.richTextBox1.Document.Blocks.FirstBlock as Paragraph;
            p.LineHeight = 1;
            wind.textBox1.KeyDown += Chat_sendMessageIfEnter;

            wind.Closed += (object sender, EventArgs e) => winClosed(sender, e);

            wind.gameNumber = gamenum;

            //If this is a game chat, we want to leave the game upon window close
            if (gamenum != 0)
            {
                wind.Closed += beginLeaveGame;
                wind.Title = game.name;
            }
            wind.Show();

            //Make this the active window in the tray manager
            KailleraTrayManager.Instance.addActiveWindow(wind);
        }

        private void beginLeaveGame(object sender, EventArgs e)
        {
            leaveGame(gameNumber);
        }

        private void UpdateUsers(List<User> u)
        {
            DisplayUsers.Clear();
            foreach (User user in u)
            {
                DisplayUsers.Add(user);
            }
        }

        /// <summary>
        /// Closes the window and fires leave game event
        /// </summary>
        public void closeWindow()
        {
            wind.Close();
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
            sendGameChat(gameNumber, textMessage);
        }

        public void addBuddy(User user)
        {
            if (addBuddyList != null)
            {
                addBuddyList(user);
            }
        }
    }
}
