using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace KailleraNET
{
    /// <summary>
    /// List of games
    /// </summary>
    public class GameList
    {
        ObservableCollection<Game> games = new ObservableCollection<Game>();

        public ObservableCollection<Game> Games
        {
            get
            {
                return games;
            }
        }

        public int numGames;


        public void AddGame(Game game)
        {
            numGames++;
            games.Add(game);
        }

        public void RemoveGame(Game game)
        {
            numGames--;
            games.Remove(game);
        }

        public Game GetGameFromID(int id)
        {
            for (int i = 0; i < numGames; i++)
            {
                if (games[i].id == id)
                    return games[i];
            }
            return null;
        }
    }
}
