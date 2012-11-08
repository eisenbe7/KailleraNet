using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KailleraNET.Windows;
using KailleraNET.Views;
using System.Net;
using System.IO;
using log4net;


namespace KailleraNET.Util
{
    /// <summary>
    /// Receives information from the master server list
    /// </summary>
    class MasterServerList
    {
        public delegate void serverAdd(Server currServer);

        public serverAdd addServers;

        ServerListView serverView = new ServerListView();
        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Fetches the list of servers and adds them to serverView's server collection
        /// </summary>
        public void fetchServers()
        {
            WebRequest serverReq = WebRequest.Create("http://kaillera.com/raw_server_list2.php?version=0.9");
            WebResponse serverResp = serverReq.GetResponse();
            using (StreamReader sr = new StreamReader(serverResp.GetResponseStream()))
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        Server currServer = new Server();
                        currServer.name = sr.ReadLine();
                        string[] servInfo = sr.ReadLine().Split(';');
                        string[] ipPort = servInfo[0].Split(':');
                        currServer.ip = IPAddress.Parse(ipPort[0]);
                        currServer.port = int.Parse(ipPort[1]);
                        currServer.users = int.Parse(servInfo[1].Split('/')[0]);
                        currServer.numGames = int.Parse(servInfo[2]);
                        currServer.version = servInfo[3];
                        currServer.location = servInfo[4];
                        addServers(currServer);
                    }
                    catch (Exception)
                    {
                        log.Warn("Invalid server detected!");
                    }
                }
            }
        }

    }
}
