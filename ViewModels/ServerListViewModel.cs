using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using KailleraNET.Util;
using KailleraNET.Windows;
using System.Windows;

namespace KailleraNET.Views
{
    /// <summary>
    /// View model for the server list 
    /// </summary>
    public class ServerListView
    {
        /// <summary>
        /// Delegate that executes when a server is chosen
        /// </summary>
        /// <param name="e"></param>
        public delegate void serverChosen(Server e);

        public serverChosen chooseServer;

        ObservableCollection<Server> servers = new ObservableCollection<Server>();

        public ObservableCollection<Server> Servers
        {
            get
            {
                return servers;
            }
        }

        ServerListWindow wind { get; set; }
        MasterServerList serverModel { get; set; }

        /// <summary>
        /// Creates and fetches master server list and shows window
        /// </summary>
        public ServerListView()
        {
            wind = new ServerListWindow();
            wind.DataContext = servers;
            wind.serverListGrid.ItemsSource = Servers;
            wind.selectButton.Click += buttonClicked;
            wind.closeButton.Click += closeClicked;
        }

        public void begin()
        {
            serverModel = new MasterServerList();
            serverModel.addServers += currServer => servers.Add(currServer);
            serverModel.fetchServers();
            wind.ShowDialog();
        }

        /// <summary>
        /// Event handler for button click - fires server chosen event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClicked(object sender, RoutedEventArgs e)
        {
            Server selected = (Server)wind.serverListGrid.SelectedItem;
            if (selected == null) return;
            chooseServer(selected);
        }

        private void closeClicked(object sender, RoutedEventArgs e)
        {
            wind.Close();
        }

    }
}
