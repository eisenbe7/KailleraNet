using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KailleraNET.Windows;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;

namespace KailleraNET.ViewModels
{
    /// <summary>
    /// View model for the games list
    /// </summary>
    public class GameListViewModel
    {
        GamesWindow wind;
        public GameList gameList;

        public delegate void JoinGame(Game game);
        public JoinGame joinGame;

        ObservableCollection<Game> games = new ObservableCollection<Game>();

        public ObservableCollection<Game> Games
        {
            get
            {
                return games;
            }
            set
            {
                value = games;
            }
        }


        public GameListViewModel()
        {
            wind = new GamesWindow();
            wind.DataContext = games;
            wind.gameDataGrid.ItemsSource = Games;
            wind.gameDataGrid.MouseDoubleClick += onGameSelected;
            wind.Show();
        }

        /// <summary>
        /// Updates the games collection
        /// </summary>
        /// <param name="g"></param>
        public void onGamesChanged(GameList g)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => clearAddGames(g)));
            
        }

        private void clearAddGames(GameList g)
        {
            games.Clear();
            foreach (Game game in g.Games)
            {
                Games.Add(game);
            }
        }

        public void closeWindow()
        {
            wind.Close();
        }

        /// <summary>
        /// Fires game selected event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onGameSelected(object sender, RoutedEventArgs e)
        {
            if (joinGame != null)
            {
                joinGame((Game)wind.gameDataGrid.SelectedItem);
            }
        }





    }
}
