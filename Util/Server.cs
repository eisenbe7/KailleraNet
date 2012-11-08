using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace KailleraNET.Util
{
    /// <summary>
    /// Represents a server entry in the master list
    /// </summary>
    public class Server
    {
        public string name { get; set; }
        public int users { get; set; }
        public IPAddress ip { get; set; }
        public int port { get; set; }
        public int numGames { get; set; }
        public string version { get; set; }
        public string location { get; set; }


        public string toString()
        {
            return name + " " + ip.ToString() + " " + users.ToString() + " " + numGames.ToString() +  " " + " " + version + " " + location;

        }

    }
}
