using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET.Instructions
{
    class LoginSuccess
    {
        
        /// <summary>
        /// Parses LoginSuccess instructions for users
        /// and adds them to the UserList
        /// </summary>
        /// <param name="content">Byte array containing
        /// all recieved buffer</param>
        /// <param name="users">Userlist to add to</param>
        /// returns index of beginning of gamelist
        /// <param name="games">Game list to add 
        /// games to</param>
        /// <param name="currindex">index of buffer to
        /// begin parsing</param>
        public static void parseUsersGames(byte[] content, ref UserList users, ref GameList games, int currindex)
        {

            currindex += 2;      
            int numUsers = BitConverter.ToInt32(content, currindex);
            currindex += 4;
            int numGames = BitConverter.ToInt32(content, currindex);
            currindex += 4;

            //Parse the users
            for (int i = 0; i < numUsers; i++)
            {
                User user = new User();
                StringBuilder usrName = new StringBuilder();
                while (content[currindex] != 0)
                {
                    usrName.Append((char)content[currindex++]);
                }
                user.Name = usrName.ToString();
                currindex++;
                user.ping = BitConverter.ToInt32(content, currindex);
                currindex += 4;
                user.status = content[currindex];
                currindex++;
                user.id = BitConverter.ToInt16(content, currindex);
                currindex += 2;
                user.connection = content[currindex++];
                users.AddUser(user);
            }

            //Now for the games
            for (int i = 0; i < numGames; i++)
            {
                Game game = new Game();
                StringBuilder gamename = new StringBuilder();
                while (content[currindex] != 0)
                {
                    gamename.Append((char)content[currindex++]);
                }
                game.name = gamename.ToString();
                currindex++;
                game.id = BitConverter.ToInt32(content, currindex);
                currindex += 4;
                StringBuilder emuName = new StringBuilder();
                while (content[currindex] != 0)
                {
                    emuName.Append((char)content[currindex++]);
                }
                game.emuName = emuName.ToString();
                currindex++;
                StringBuilder hostUser = new StringBuilder();
                
                while (content[currindex] != 0)
                {
                    hostUser.Append((char)content[currindex++]);
                }


                game.host_name = hostUser.ToString();
                currindex++;

                StringBuilder usersStr = new StringBuilder();

                while (content[currindex] != 0)
                {
                    usersStr.Append((char)content[currindex++]);
                }

                //Note property setter also sets game users and max users
                game.Users_count = usersStr.ToString();

                game.status = content[++currindex];
                games.AddGame(game);
                currindex++;

            }
        }
    }
}
