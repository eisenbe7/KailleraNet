using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET
{
    /// <summary>
    /// Class that represents an update to the UI thread
    /// </summary>
    public class GUIBundle
    {
       private UserList users;
       public GameList games;
       public string ChatText;
       public string GameText;
       public string MOTDText;
       public User TargetUser;
       public string TargetUserString;

       public int gameNum = 0;

       public UserList Users
       {
           get
           {
               return users;
           }
           set
           {
               users = value;
               users.users.Sort();
           }
       }

    }
}
