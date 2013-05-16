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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class GamesWindow : Window
    {
        public delegate void CreateChat();

        public CreateChat createChat;

        public GamesWindow()
        {
            Closing += Window_Closing;
            InitializeComponent();
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

        private void createChatButton_Click(object sender, RoutedEventArgs e)
        {
            if (createChat != null)
                createChat();
        }
    }
}
