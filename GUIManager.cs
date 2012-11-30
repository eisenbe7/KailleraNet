using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace KailleraNET
{
    /// <summary>
    /// Class that manages application GUI for specific window or instance
    /// DEPRECATED
    /// </summary>
    class GUIManager
    {
        public static ObservableCollection<User> displayUsers = new ObservableCollection<User>();

        public static ObservableCollection<User> DisplayUsers
        {
            get
            {
                return displayUsers;
            }
        }

        static GUIManager()
        {
        }

        /// <summary>
        /// Processes the current user list
        /// </summary>
        /// <param name="users">List of users</param>
        public static void UpdateUsers(UserList users)
        {
            DisplayUsers.Clear();
            foreach (User u in users.users)
                DisplayUsers.Add(u);
        }

        /// <summary>
        /// Processes the current game list
        /// </summary>
        /// <param name="games">List of games</param>
        internal static void UpdateGames(GameList games)
        {
//            throw new NotImplementedException();
        }

        internal static void UpdateChatText(User user, String text)
        {
        }

        internal static void UpdateMOTDText(string text)
        {
        }

        internal static void UpdateGameText(User user, String text)
        {
        }
    }
}
