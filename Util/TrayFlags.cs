using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KailleraNET.Windows;

namespace KailleraNET.Util
{
    /// <summary>
    /// Hold list of pop event names and the windows associated with them
    /// </summary>
    public static class TrayFlags
    {
        [Flags]
        public enum PopValues
        {
            None = 0,
            pmRecieved = 1,
            keywordTyped = 2,
            gameCreated = 4,
            buddyJoinedServer = 8,
            buddyJoinedGame = 16,
            buddyLeftServer = 32,
            buddyLeftGame = 64
        }

        /// <summary>
        /// Dictionary containing the events for each window
        /// </summary>
        public static Dictionary<Type, PopValues> trayDict = new Dictionary<Type, PopValues>();

        /// <summary>
        /// Messages for each event
        /// </summary>
        public static Dictionary<PopValues, string> trayLang = new Dictionary<PopValues, string>();

        public static Dictionary<Type, PopValues> TrayDict
        {
            get
            {
                return trayDict;
            }
        }

        public static Boolean containsType(Type t)
        {
            return trayDict.Keys.Contains(t);
        }

        public static Boolean handlesEvent(Object o, PopValues flags)
        {
            if (o as ChatWindow != null)
            {
                if (((ChatWindow)o).gameNumber == 0)
                {
                    return flags.HasFlag(PopValues.pmRecieved);
                }
            }
            return trayDict[o.GetType()].HasFlag(flags);
        }


        static TrayFlags()
        {
            trayDict.Add(typeof(ChatWindow), PopValues.buddyJoinedGame | PopValues.buddyLeftGame | PopValues.keywordTyped | PopValues.pmRecieved);

            trayDict.Add(typeof(GamesWindow), PopValues.gameCreated);

            trayDict.Add(typeof(UsersWindow), PopValues.buddyJoinedServer | PopValues.buddyLeftServer);                      

            // # replaced by username
            // $ replaced by keyword
            trayLang.Add(PopValues.pmRecieved, "You've received a private message from #!  \"$\"");
            trayLang.Add(PopValues.buddyJoinedServer, "# has joined the server");
            trayLang.Add(PopValues.buddyLeftServer, "# has left the server");
            trayLang.Add(PopValues.keywordTyped, "$ has been typed by #");
            trayLang.Add(PopValues.buddyJoinedGame, "# has joined the game");
            trayLang.Add(PopValues.buddyLeftGame, "# has left the game");
            trayLang.Add(PopValues.gameCreated, "# has created a game: $");

        }
    }
}
