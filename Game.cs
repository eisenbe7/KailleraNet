using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET
{
    public class Game
    {
        public string name { get; set; }
        public int id { get; set; }
        public string emuName { get; set; }
        public string users_count;
        public int numPlayers;
        public int maxPlayers;
        public byte status;    //Bytes 1 and/or 2 set
        public UserList users = new UserList();
        public User host;
        public string gameHost { get; set; }

        public string Users_count
        {
            get
            {
                return users_count;
            }
            set
            {
                if (value == null || !value.Contains('/')) return;
                users_count = value;
                string[] playerCount = users_count.Split('/');
                numPlayers = int.Parse(users_count[0].ToString());
                maxPlayers = int.Parse(playerCount[1].ToString());
            }

        }



        public Game()
        {
        }

        /// <summary>
        /// Constructor for creating a game with a specific id
        /// Called when user creates new game
        /// </summary>
        /// <param name="id"></param>
        public Game(int id)
        {
            this.id = id;
        }
    }
}
