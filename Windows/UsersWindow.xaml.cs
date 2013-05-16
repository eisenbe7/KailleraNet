using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KailleraNET.Windows
{
    /// <summary>
    /// Interaction logic for UsersWindow.xaml
    /// </summary>
    public partial class UsersWindow : Window
    {
        public delegate void AddBuddyList(User user);
        public AddBuddyList addBuddyList;

        public delegate void SendPM(User user, string text);
        public SendPM sendPM;

        public delegate void DisconnectEvent();
        public DisconnectEvent disconnectEvent;

        public UsersWindow()
        {
            InitializeComponent();
            Closing += Window_Closing;
        }

        private void beginDisconnect(object sender, RoutedEventArgs e)
        {
            if (disconnectEvent != null)
            {
                disconnectEvent();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void addToBuddyList(object sender, EventArgs e)
        {
            if (addBuddyList != null && usersList.SelectedItem != null)
            {
                addBuddyList((User)usersList.SelectedItem);
            }
        }

        /// <summary>
        /// Sends a private message to a user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void sendPMs(object sender, EventArgs e)
        {
            if (sendPM != null && usersList.SelectedItems.Count != 0)
            {
                StringBuilder usersSb = new StringBuilder();
                foreach(var user in usersList.SelectedItems)
                {
                    usersSb.AppendLine(user.ToString() + " ");                
                }


                string text = Microsoft.VisualBasic.Interaction.InputBox("Please enter your message to: " + usersSb.ToString(), "Private Message", "", 100, 100);
                if (String.IsNullOrWhiteSpace(text)) return;
                foreach (var item in usersList.SelectedItems)
                {
                    sendPM((User)item, text);
                }
            }
        }

        /// <summary>
        /// Hides the window and does not close it
        /// Code from: http://balajiramesh.wordpress.com/2008/07/24/hide-a-window-instead-of-closing-it-in-wpf/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Do some stuff here 
            //Hide Window
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
            {
                Visibility = System.Windows.Visibility.Hidden;
                return null;
            }, null);
            //Do not close application
            e.Cancel = true;

        }

    }
}
