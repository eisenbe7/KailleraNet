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

namespace KailleraNET
{

    

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        //Delegate to update the users - stupid implementation
        private delegate void upUsers(List<User> u);
        upUsers Userup;
        FlowDocument doc = new FlowDocument();

        //UI delegate that calls UI method
        public delegate void UpdateGUI(GUIBundle bundle);
        public UpdateGUI updater;

        private ObservableCollection<User> displayUsers = new ObservableCollection<User>();
        private System.Net.IPAddress ip;
        private int port;
        private string username;

        //zero for main chat
        public int gameNumber;

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

        public ChatWindow(int gamenum, KailleraWindowMananger mgr)
        {
            gameNumber = gamenum;
            Userup += UpdateUsers;
            updater += WindowUpdate;
            InitializeComponent();
            listBox1.ItemsSource = DisplayUsers;
            //Set small line height to avoid large line breaks
            Paragraph p = richTextBox1.Document.Blocks.FirstBlock as Paragraph;
            p.LineHeight = 1;
            textBox1.KeyDown += mgr.Chat_sendMessageIfEnter;
        }

        public void Start(System.Net.IPAddress ip, int port, string username)
        {
/*          updater += WindowUpdate;
            Userup += UpdateUsers;
            KailleraMananger mgr = new KailleraMananger(ip, port, username);
            Thread KManager = new Thread(new ParameterizedThreadStart(mgr.Start));
            mgr.connectionClosed += connectionClosed;
            KManager.Start(updater);
 */
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
        private void WindowUpdate(GUIBundle bundle)
        {
            if (bundle.Users != null)
            {
                listBox1.Dispatcher.Invoke(Userup, bundle.Users.users);
            }
            if (bundle.ChatText != null)
            {

                Dispatcher.BeginInvoke((Action)(() => 
                {
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
                            richTextBox1.Focus();
                            richTextBox1.ScrollToEnd();
                            textBox1.Focus();
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
                            richTextBox1.Focus();
                            richTextBox1.ScrollToEnd();
                            textBox1.Focus();
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


    }
}
