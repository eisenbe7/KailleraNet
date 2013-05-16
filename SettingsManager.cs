using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System.Windows;
using Nini.Config;
using KailleraNET.Util;

//http://kaillera.com/raw_server_list2.php?version=0.9

namespace KailleraNET
{
    /// <summary>
    /// Manages all the settings for the client, including:
    /// Server List (stored in servers.txt)
    /// Buddy List (stored in buddy.txt)
    /// UserNames (stored in username.txt)
    ///  
    /// </summary>    
    /// 

    public class SettingsManager
    {
        public class iniSettings
        {
            public bool NotifyBuddyJoinServer = true;
            public bool NotifyBuddyLeaveServer = true;
            public bool NotifyBuddyJoinedGame = true;
            public bool NotifyBuddyLeftGame = true;
            public bool NotifyPM = true;

            public bool UserJoinedMessage = true;
            public bool UserLeftMessage = true;
        }

        Dictionary<TrayFlags.PopValues, Func<bool>> flagFunctor = new Dictionary<TrayFlags.PopValues, Func<bool>>();

        private void initializeDict()
        {
            flagFunctor.Add(TrayFlags.PopValues.buddyJoinedGame, () => { return getSettings().NotifyBuddyJoinedGame; });
            flagFunctor.Add(TrayFlags.PopValues.buddyLeftGame, () => { return getSettings().NotifyBuddyLeftGame; });
            flagFunctor.Add(TrayFlags.PopValues.buddyJoinedServer, () => { return getSettings().NotifyBuddyJoinServer; });
            flagFunctor.Add(TrayFlags.PopValues.buddyLeftServer, () => { return getSettings().NotifyBuddyLeaveServer; });
            flagFunctor.Add(TrayFlags.PopValues.pmRecieved, () => { return getSettings().NotifyPM; });

        }




        string servers;
        string buddy;
        string username;
        string privMessages;
        string ignore;
        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IConfigSource settings = new IniConfigSource("settings.ini");

        private static SettingsManager instance;

        public static SettingsManager getMgr(string servers = "servers.txt", string buddy = "buddy.txt", string username = "usernames.txt", string privMessages = "pm.txt", string ignore = "ignore.txt")
        {
            if (instance != null) return instance;
            else return new SettingsManager(servers, buddy, username, privMessages, ignore);
        }

        private SettingsManager()
        {
            initializeDict();
        }

        /// <summary>
        /// Initializes the settings manager.  The default params are the default location of the files
        /// </summary>
        /// <param name="servers"></param>
        /// <param name="buddy"></param>
        /// <param name="username"></param>
        private SettingsManager(string servers = "servers.txt", string buddy = "buddy.txt", string username = "usernames.txt", string privMessages = "pm.txt", string ignore = "ignore.txt")
        {
            initializeDict();
            log.Debug("Initializing settings manager");
            this.servers = servers;
            this.buddy = buddy;
            this.username = username;
            this.privMessages = privMessages;
            this.ignore = ignore;
            instance = this;
        }

        /// <summary>
        /// Loads all saved servers
        /// Throws IOException, ArrayOutOfBounds on failure - should be handled in caller class
        /// </summary>
        /// <returns>A list of server, ip string pairs</returns>
        public List<KeyValuePair<string, string>> getServers()
        {
            List<KeyValuePair<string, string>> serverList = new List<KeyValuePair<string, string>>();
            using (StreamReader serverReader = new StreamReader(servers))
            {
                while (!serverReader.EndOfStream)
                {
                    //<SERVER_NAME>,<ip>
                    string line = serverReader.ReadLine();
                    string[] entry = line.Split(',');
                    if (entry.Length != 2 || line.Contains('#'))
                    {
                        log.Info("Skipping line in server list: \"" + line + "\"");
                        continue;
                    }
                    serverList.Add(new KeyValuePair<string, string>(entry[0], entry[1]));
                }
            }
            return serverList;
        }

        /// <summary>
        /// Loads all saved usernames
        /// Throws IOException, ArrayOutOfBounds on failure - should be handled in caller class
        /// </summary>
        /// <returns>List of username strings</returns>
        public List<string> getUsernames()
        {
            List<string> usernameList = new List<string>();
            using (StreamReader usernameReader = new StreamReader(username))
            {
                while (!usernameReader.EndOfStream)
                {
                    string userName = usernameReader.ReadLine();
                    if (String.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName)) continue;
                    if (userName.Contains('#'))
                    {
                        log.Info("Skipping line in userName list: \"" + userName + "\"");
                        continue;
                    }

                    if (userName.Length > 30)
                    {
                        log.Warn("Username too long!  Skipping: " + userName);
                        continue;
                    }
                    usernameList.Add(userName);
                }
            }
            return usernameList;
        }

        /// <summary>
        /// Checks for duplicate server entries
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public Boolean isDuplicateServer(string serverName)
        {
            using (StreamReader serverReader = new StreamReader(servers))
            {
                while (!serverReader.EndOfStream)
                {
                    if (serverReader.ReadLine().Split(',')[0].Equals(serverName))
                    {
                        return true;
                    }
                }
                serverReader.Close();
            }
            return false;
        }

        /// <summary>
        /// Checks for duplicate usernames
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool isDuplicateUserName(string name)
        {
            using (StreamReader usernameReader = new StreamReader(username))
            {
                while (!usernameReader.EndOfStream)
                {
                    if (usernameReader.ReadLine().Equals(name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a server entry to the default file
        /// </summary>
        /// <param name="name">Name of server</param>
        /// <param name="ip">ip:port string</param>
        public void addServer(string name, string ip)
        {
            if (isDuplicateServer(name))
            {
                return;
            }

            using (StreamWriter serverWriter = File.AppendText(servers))
            {
                serverWriter.WriteLine(name + "," + ip);
                serverWriter.Flush();
                serverWriter.Close();
            }
        }

        public void addUsername(string name)
        {
            if (isDuplicateUserName(name))
            {
                return;
            }


            using (StreamWriter usernameWriter = File.AppendText(username))
            {
                usernameWriter.WriteLine(name);
                usernameWriter.Flush();
                usernameWriter.Close();
            }
        }

        /// <summary>
        /// Writes the pm to the file
        /// </summary>
        /// <param name="pm"></param>
        public void writePM(string pm)
        {
            using (StreamWriter pmWriter = File.AppendText(privMessages))
            {
                pmWriter.WriteLine(pm);
                pmWriter.Flush();
                pmWriter.Close();
            }
        }

        public List<string> getPMs()
        {
            var pms = new List<string>();
            using (StreamReader pmReader = new StreamReader(privMessages))
            {
                while (!pmReader.EndOfStream)
                {
                    pms.Add(pmReader.ReadLine());
                }
            }
            return pms;
        }

        public Boolean isBuddy(string name)
        {
            using (StreamReader buddyReader = new StreamReader(buddy))
            {
                while (!buddyReader.EndOfStream)
                {
                    if (name.Equals(buddyReader.ReadLine()))
                        return true;
                }
            }
            return false;
        }

        public Boolean isIgnored(string name)
        {
            using (StreamReader ignoreReader = new StreamReader(ignore))
            {
                while (!ignoreReader.EndOfStream)
                {
                    if (name.Equals(ignoreReader.ReadLine()))
                        return true;
                }
            }
            return false;
        }

        public void addBuddy(string name)
        {
            if (isBuddy(name)) return;

            using (StreamWriter buddyWriter = File.AppendText(buddy))
            {
                buddyWriter.WriteLine(name);
                buddyWriter.Flush();
                buddyWriter.Close();
            }
        }

        /// <summary>
        /// Removes the buddy from the list.
        /// Note: Overwrites previous file
        /// </summary>
        /// <param name="name"></param>
        public void removeBuddy(string name)
        {
            if (!isBuddy(name)) return;
            
            List<string> buddies = new List<string>(File.ReadAllLines(buddy));

            buddies.Remove(name);
            File.WriteAllLines(buddy, buddies);

        }

        public iniSettings getSettings()
        {
            iniSettings currSettings = new iniSettings();
            currSettings.NotifyBuddyJoinServer = settings.Configs["Notifications"].GetInt("NotifyBuddyJoinServer", 1) == 1;
            currSettings.NotifyBuddyLeftGame = settings.Configs["Notifications"].GetInt("NotifyBuddyLeftServer", 1) == 1;
            currSettings.NotifyBuddyJoinedGame = settings.Configs["Notifications"].GetInt("NotifyBuddyJoinedGame", 1) == 1;
            currSettings.NotifyBuddyLeftGame = settings.Configs["Notifications"].GetInt("NotifyBuddyLeftGame", 1) == 1;
            currSettings.NotifyPM = settings.Configs["Notifications"].GetInt("NotifyPM", 1) == 1;
            currSettings.UserLeftMessage = settings.Configs["Server Chat"].GetInt("UserJoinedMessage", 1) == 1;
            currSettings.UserLeftMessage = settings.Configs["Server Chat"].GetInt("UserLeftMessage", 1) == 1;

            return currSettings;
        }

        internal bool isNotifyEnabled(TrayFlags.PopValues flag)
        {
            if(!flagFunctor.ContainsKey(flag)) return false;
            return flagFunctor[flag].Invoke();
        }
    }
}
