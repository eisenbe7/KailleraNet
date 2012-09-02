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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Threading;
using System.ComponentModel;
using log4net;

namespace KailleraNET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        KailleraWindowMananger k = new KailleraWindowMananger();

        public MainWindow()
        {
            InitializeComponent();
            textBox1.Text = "UserName";
            textBox2.Text = "66.175.211.87:27888";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string username = textBox1.Text;
            if (username.Equals(""))
            {
                MessageBox.Show("Error - You must enter a username.", "Please enter a username.");
                return;
            }

            IPAddress ip;
            int port;

            try
            {
                string[] args = textBox2.Text.Split(':');
                ip = IPAddress.Parse(args[0]);
                port = int.Parse(args[1]);
            }
            catch (Exception)
            {
                MessageBox.Show("Error - cannot read ip address.  Format is <ip>:<port>.", "IP Error");
                return;
            }
/*
            ChatWindow wind = new ChatWindow();
            wind.Closed += connectionClosed;
            wind.Show();
            wind.Start(ip, port, username);
 */
            this.Visibility = Visibility.Hidden;

          k.BeginNewConnection(ip, port, username);
          this.Close();

        }

        private void connectionClosed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }
    }


}
