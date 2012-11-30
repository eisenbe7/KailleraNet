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

namespace KailleraNET.Windows
{
    /// <summary>
    /// Interaction logic for UsersWindow.xaml
    /// </summary>
    public partial class UsersWindow : Window
    {
        public delegate void AddBuddyList(User user);
        public AddBuddyList addBuddyList;

        public UsersWindow()
        {
            InitializeComponent();
        }

        private void beginDisconnect(object sender, RoutedEventArgs e)
        {

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
    }
}
