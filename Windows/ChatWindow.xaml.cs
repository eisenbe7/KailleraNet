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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using KailleraNET.Util;
using System.Windows.Threading;

namespace KailleraNET
{

    

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        //Delegate to update the users - stupid implementation
        public delegate void upUsers(List<User> u);
        public upUsers Userup;
        FlowDocument doc = new FlowDocument();

        //UI delegate that calls UI method
        public delegate void UpdateGUI(GUIBundle bundle);
        public UpdateGUI updater;


        /// <summary>
        /// Adds the user to the buddy list
        /// </summary>
        /// <param name="user"></param>
        public delegate void AddBuddyList(User user);
        public AddBuddyList addBuddyList;

        public delegate void SendPM(User user, string text);
        public SendPM sendPM;

        private ObservableCollection<User> displayUsers = new ObservableCollection<User>();
        private System.Net.IPAddress ip;
        private int port;
        private string username;

        //zero for main chat
        public int gameNumber;

        public ChatLogger Logger;

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

        public ChatWindow()
        {

            updater += WindowUpdate;
            InitializeComponent();
            Closing += Window_Closing;

            richTextBox1.IsDocumentEnabled = true;
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Updates the window - calls dispatcher so correct thread is invoked
        /// </summary>
        /// <param name="bundle"></param>
        public void WindowUpdate(GUIBundle bundle)
        {
            if (bundle.Users != null)
            {
                listBox1.Dispatcher.Invoke(Userup, bundle.Users.users);
            }
            if (bundle.ChatText != null)
            {

                Dispatcher.BeginInvoke((Action)(() => 
                {
                    //The message will not appear if ignored
                    if (SettingsManager.getMgr().isIgnored(bundle.TargetUserString)) return;


                    //Stupid text coloring!  Better way to do this using flow documents?

                    Random rand = new Random();

                    var dateText = new TextRange(richTextBox1.Document.ContentEnd, richTextBox1.Document.ContentEnd);
                    
                    dateText.Text = "(" + DateTime.Now.ToShortTimeString() + ") ";

                    dateText.ApplyPropertyValue(TextElement.ForegroundProperty, ColorUtil.txtColors[0]);
                    dateText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);

                    TextRange username = new TextRange(richTextBox1.Document.ContentEnd, richTextBox1.Document.ContentEnd);

                    username.Text = bundle.TargetUserString;

                    var randColor = ColorUtil.txtColors[rand.Next(0, ColorUtil.txtColors.Count)];

                    username.ApplyPropertyValue(TextElement.ForegroundProperty, randColor);
                    username.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

                    var chatText = new TextRange(richTextBox1.Document.ContentEnd, richTextBox1.Document.ContentEnd);

                    chatText.Text = ": " + bundle.ChatText + Environment.NewLine; 

                    chatText.ApplyPropertyValue(TextElement.ForegroundProperty, ColorUtil.txtColors[0]);
                    chatText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);

                    Logger.logChat(bundle.TargetUserString + " - " + bundle.ChatText);

                    //We only want the textbox to autoscroll when the scrollbar is at the bottom, otherwise no autoscroll
                    //Code adopted from MSDN: http://social.msdn.microsoft.com/Forums/en-AU/wpf/thread/25a08bda-dc5e-4689-a6b0-7d4d78aff06b

                    //Get vertical scroll position
                    double dVer = richTextBox1.VerticalOffset;
                    
                    //Get vertical size of scrollable content area
                    double dViewport = richTextBox1.ViewportHeight;

                    //Get vertical size of visible content area
                    double dExtent = richTextBox1.ExtentHeight;

                    if (dVer != 0)
                    {
                        if (dVer + dViewport == dExtent)
                        {
                           // richTextBox1.Focus();
                            richTextBox1.ScrollToEnd();
                           // textBox1.Focus();
                        }
                    }




                }
                    ));
            }
            if (bundle.MOTDText != null)
            {
                Dispatcher.BeginInvoke((Action)(() => {

                    var motd = new TextRange(richTextBox1.Document.ContentEnd, richTextBox1.Document.ContentEnd);

                    motd.Text = "(" + DateTime.Now.ToShortTimeString() + ") " + bundle.MOTDText + Environment.NewLine;

                    motd.ApplyPropertyValue(TextElement.ForegroundProperty, ColorUtil.txtColors[0]);
                    motd.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);


                    //We only want the textbox to autoscroll when the scrollbar is at the bottom, otherwise no autoscroll
                    //Code adopted from MSDN: http://social.msdn.microsoft.com/Forums/en-AU/wpf/thread/25a08bda-dc5e-4689-a6b0-7d4d78aff06b

                    //Get vertical scroll position
                    double dVer = richTextBox1.VerticalOffset;
                    
                    //Get vertical size of scrollable content area
                    double dViewport = richTextBox1.ViewportHeight;

                    //Get vertical size of visible content area
                    double dExtent = richTextBox1.ExtentHeight;

                    if (dVer != 0)
                    {
                        if (dVer + dViewport == dExtent)
                        {
                                //richTextBox1.Focus();
                                richTextBox1.ScrollToEnd();
                                //textBox1.Focus();
                            
                        }
                    }
                }
                    ));
            }

        }

        private void UpdateUsers(List<User> u)
        {
            DisplayUsers.Clear();
            foreach (User user in u)
            {
                DisplayUsers.Add(user);
            }
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
        
        }

        private void connectionClosed()
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dataGrid1_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dataGrid1_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void addToBuddyList(object sender, EventArgs e)
        {
            if (addBuddyList != null && listBox1.SelectedItems != null)
            {
                foreach (var item in listBox1.SelectedItems)
                {
                   addBuddyList((User)item);
                }
            }
        }


        /// <summary>
        /// Sends a private message to a user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendPMs(object sender, EventArgs e)
        {
            if (sendPM != null && listBox1.SelectedItems != null)
            {
                string text = Microsoft.VisualBasic.Interaction.InputBox("Please enter your message", "Message", "", 100, 100);
                if (String.IsNullOrWhiteSpace(text)) return;
                foreach (var item in listBox1.SelectedItems)
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
                Hide();
                return null;
            }, null);
            //Do not close application
            e.Cancel = true;
        }


    }
}
