using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET
{
    /// <summary>
    /// List of games
    /// </summary>
    public class GameList
    {
        List<Game> games = new List<Game>();
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
