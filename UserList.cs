using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET
{
    public class UserList
    {
        public List<User> users = new List<User>();
        public int numUsers;

        public UserList()
        {
        }
        
        public void AddUser(User user)
        {
            numUsers++;
            users.Add(user);
        }

        public void RemoveUser(User user)
        {
            numUsers--;
            users.Remove(user);
        }


        /// <summary>
        /// Returns a user object from the user's name.
        /// Useful for getting the users in games as only
        /// names are sent by the server
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <returns>The user with the name, or null if not found</returns>
        public User GetUserFromName(string name)
        {
            for (int i = 0; i < numUsers; i++)
            {
                if (users[i].Name.Equals(name))
                    return users[i];
            }
            return null;
        }

        public User GetUserFromID(int id)
        {
            for (int i = 0; i < numUsers; i++)
            {
                if (users[i].id == id)
                    return users[i];
            }
            return null;
        }
    }
}
