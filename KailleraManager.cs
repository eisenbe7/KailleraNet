using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using KailleraNET.Instructions;
using System.Windows;
using System.ComponentModel;
using log4net;
using System.Timers;
using KailleraNET.Messages;
using System.Text.RegularExpressions;
using KailleraNET.Util;

namespace KailleraNET
{
    /// <summary>
    /// Class that handles the main kaillera instance.
    /// Connects and mananges sending and recieving threads
    /// And also maintains user and game list
    /// </summary>
    class KailleraManager
    {
        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public delegate void OnUsersChanged(UserList users);
        public delegate void OnGamesChanged(GameList games);
        public delegate void OnGameJoinSuccess(Game game);
        

        public OnGameJoinSuccess joinedGameSuccess;

        public OnGamesChanged gamesChanged;

        UdpClient client = new UdpClient();
        
        public delegate void CloseInstance();

        //Contains only "End Connection" by default
        public CloseInstance connectionClosed;

        KailleraWindowController WindowMngr;

        KailleraTrayManager TrayMgr;

        Thread alive;

        //Currently implemented instructions
        const byte UserLoginSuccess = 4;
        const byte UserLoginStatus = 22;
        const byte Ping = 5;
        const byte UserJoined = 2;
        const byte UserLeave = 1;
        const byte GameCreate = 10;
        const byte GameClose = 16;
        const byte GameStatus = 14;
        const byte PlayerJoin = 12;
        const byte PlayerLeave  = 11;
        const byte ExistingPlayers = 13;
        const byte ServerChat = 7;
        const byte GameChat = 8;
        const byte MOTD = 23;

        //Set to true when main chat is closed
        public Boolean stop = false;


        private UserList users = new UserList();
        public GameList games = new GameList();
        public IPEndPoint ip;
        IPAddress ipaddr;
        public int port;
        public int CurSeqNum = -1;
        private UDPMessenger messager;
        private string username;

        //The current game the user is in
        private Game currGame = null;
        private Game tempCurrGame;


        public KailleraManager(IPAddress ips, int port, string Username, KailleraTrayManager mgr = null)
        {
            TrayMgr = mgr;
            this.ipaddr = ips;
            this.port = port;
            username = Username;
        }

        public void Start(Object o)
        {
            connectionClosed += EndConnection;
            WindowMngr = o as KailleraNET.KailleraWindowController;
            if (initConnection())
            {
                //Send Logon
                messager = new UDPMessenger(ip);
                messager.AddMessages(new UserLogonInstruction(username + "\0", 1));
                messager.SendMessages(client);
                //Begin recieving and processing loop
                alive = new Thread(KeepAlive);
                //alive.Start(ip);
                /*     System.Timers.Timer KeepAliveTimer = new System.Timers.Timer();
                     KeepAliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(KeepAliveSameThread);
                     KeepAliveTimer.Interval = 60000;
                     KeepAliveTimer.AutoReset = true;
                     KeepAliveTimer.Enabled = true;
                 */
                alive.Start(ip);
                Recieve();
            }
            else //If we didn't connect successfully, return to the main window
            {
                KailleraWindowController.getMgr().connectionFailed();
            }

            
        }

        private void EndConnection()
        {
            stop = true;
            if (messager == null) return;
            Logout l = new Logout();
            messager.AddMessages(l);
            messager.SendMessages(client);
            //There are better ways to stop this thread.  Ok for now.
            alive.Interrupt();
            client.Close();
            
        }


        /// <summary>
        /// Only here for debugging purposes, not used otherwise.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KeepAliveSameThread(object sender, ElapsedEventArgs e)
        {
            KeepAliveInstruction ki = new KeepAliveInstruction();
            List<KailleraInstruction> alive = new List<KailleraInstruction>();
            alive.Add(ki);
            log.Info("Sending keepalive packer to " + ip.ToString() + "with seq number " + ki.serial.ToString());
            UDPMessenger.SendMessages(alive, ip, client);
        }

        /// <summary>
        /// Thread to handle recieving of messages from the server
        /// and dispatches to correct handler
        /// </summary>
        private void Recieve()
        {
            while (!stop )
            {
                byte[] msg;
                try
                {
                    msg = messager.Recieve(client);
                }
                catch (SocketException)
                {
                    break;
                }

                int count = msg[0];
                Parse(msg, count);
            }

        }


        /// <summary>
        /// Main kaillera message parser.
        /// Iterates through received packet and 
        /// calls parser method depending on ID
        /// </summary>
        /// <param name="msg">The recieved packet</param>
        /// <param name="count">Number of messages in packet</param>
        private void Parse(byte[] msg, int count)
        {
            int currIndex = 1;
            for (int i = 0; i < count; i++)
            {
                short serial = BitConverter.ToInt16(msg, currIndex);
                //Emulinker sends the last 5 instructions
                if (serial <= CurSeqNum)
                {
                    //If this is an old instruction, add insturcion length to message
                    //plus 4 for serial+length fields
                    currIndex += BitConverter.ToInt16(msg, currIndex + 2) + 4;
                    continue;
                }
                CurSeqNum = serial;
                currIndex += 2;
                short InstructionLength = BitConverter.ToInt16(msg, currIndex);
                currIndex += 2;
                byte id = msg[currIndex];

                //Process methods called with offset of index of id of message
                switch (id)
                {
                    case UserLoginSuccess:
                        ParseLoginSuccess(msg, currIndex);
                        break;
                    case Ping:
                        ProcessPong();
                        break;
                    case UserJoined:
                        ProcessUserJoin(msg, currIndex);
                        break;
                    case UserLeave:
                        ProcessUserLeave(msg, currIndex);
                        break;
                    case GameCreate:
                        ProcessGameCreate(msg, currIndex);
                        break;
                    case GameClose:
                        ProcessGameClose(msg, currIndex);
                        break;
                    case GameStatus:
                        ProcessGameStatus(msg, currIndex);
                        break;
                    case PlayerJoin:
                        ProcessPlayerJoin(msg, currIndex);
                        break;
                    case PlayerLeave:
                        ProcessPlayerLeave(msg, currIndex);
                        break;
                    case ExistingPlayers:
                        ProcessExistingPlayers(msg, currIndex);
                        break;
                    case ServerChat:
                        ProcessServerChat(msg, currIndex);
                        break;
                    case GameChat:
                        ProcessGameChat(msg, currIndex);
                        break;
                    case MOTD:
                        ProcessMOTD(msg, currIndex);
                        break;
                }

                //Add instruction length to current index
                currIndex += BitConverter.ToInt16(msg, currIndex - 2);
            }
        }


        /// <summary>
        /// Processes message of the day
        /// MOTD has string "server" first, so
        /// we will skip over it
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="currIndex"></param>
        private void ProcessMOTD(byte[] msg, int currIndex)
        {
            currIndex++;
            StringBuilder s = new StringBuilder();
            while (msg[currIndex++] != 0)
            currIndex++;
            while (msg[currIndex] != 0)
                s.Append((char)msg[currIndex++]);
            GUIBundle MOTDbund = new GUIBundle();
            MOTDbund.MOTDText = s.ToString();
            MOTDbund.gameNum = 0;
            WindowMngr.updateWindow(MOTDbund);
            handlePM(s.ToString());
            
        }

        /// <summary>
        /// Processes a PM - writes to file and triggers balloon popup
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="msg"></param>
        private void processPM(string userInfo, string msg, string completeStr)
        {
            var settingsMgr = SettingsManager.getMgr();
            int userEndInd = userInfo.LastIndexOf('>');

            User targUser = users.GetUserFromName(userInfo.Substring(1, userEndInd-1));
            settingsMgr.writePM(DateTime.Now.ToString() + ": " + completeStr);
            if (TrayMgr != null)
            {
                TrayMgr.handleTrayEvent(Util.TrayFlags.PopValues.pmRecieved, targUser, msg);
            }
        }

        /// <summary>
        /// Checks whether the incoming message is a private message
        /// PMs are MOTD with "/username\ (ID): message"
        /// with slashes replace with lt/gt
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private void handlePM(string p)
        {            
            if (p[0] != '<') return;
            Match pmMatch = Regex.Match(p, @"<(.*?)>\s\(\d*\):\s(.*?)");
            if (pmMatch.Success)
            {
                string userID = pmMatch.Groups[0].Value;
                string pmMsg = p.Substring(userID.Length);
                log.Info("Private message recieved: " + p);
                processPM(userID, pmMsg, p);
            }
        }

        /// <summary>
        /// Handles the game chat and calls the window manager to update the window
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="currIndex"></param>
        private void ProcessGameChat(byte[] msg, int currIndex)
        {
            currIndex++;
            StringBuilder s = new StringBuilder();

            //Get sender
            while(msg[currIndex] != 0)
            {
                s.Append((char)msg[currIndex++]);
            }
            string username = s.ToString();
            s.Clear();
            currIndex++;

            //Get text
            while (msg[currIndex] != 0)
            {
                s.Append((char)msg[currIndex++]);
            }
            string text = s.ToString();
            //Get user instances
            User user = users.GetUserFromName(username);
            GUIBundle gameChatBundle = new GUIBundle();
            gameChatBundle.ChatText = text;
            gameChatBundle.gameNum = currGame.id;
            gameChatBundle.TargetUser = user;
            gameChatBundle.TargetUserString = username;

            WindowMngr.updateWindow(gameChatBundle);
        }

        private void ProcessServerChat(byte[] msg, int currIndex)
        {
            currIndex++;
            StringBuilder s = new StringBuilder();

            //Get sender
            while (msg[currIndex] != 0)
            {
                s.Append((char)msg[currIndex++]);
            }
            string username = s.ToString();
            s.Clear();
            currIndex++;

            //Get text
            while (msg[currIndex] != 0)
            {
                s.Append(Encoding.UTF8.GetString(msg, currIndex++, 1));
            }
            string text = s.ToString();
            //Get user instances
            User user = users.GetUserFromName(username);

            GUIBundle bund = new GUIBundle();
            bund.ChatText = text;
            bund.TargetUser = users.GetUserFromName(username);
            bund.TargetUserString = username;
            WindowMngr.updateWindow(bund);
        }

        private void ProcessExistingPlayers(byte[] msg, int currIndex)
        {
            currIndex+=2;
            int numUsers = BitConverter.ToInt32(msg, currIndex);
            currIndex += 4;

            //The server sends the name, ping, id, and connection
            //of each user in game - but we should already have
            //this information if we match the name to the user instance.
            //If game contains a user that server has not sent yet,
            //this fails, but we will try without it
            for (int i = 0; i < numUsers; i++)
            {

                StringBuilder s = new StringBuilder();

                while (msg[currIndex] != 0)
                    s.Append((char)msg[currIndex++]);
                string username = s.ToString();
                currGame = tempCurrGame;
                currGame.users.AddUser(users.GetUserFromName(username));

                currIndex += 1 + 4 + 2 + 1; //null, ping, id, connection
            }

                joinedGameSuccess(currGame);
        }

        private void ProcessPlayerLeave(byte[] msg, int currIndex)
        {
            currIndex+=1;
            StringBuilder s = new StringBuilder();
            while (msg[currIndex] != 0)
                s.Append((char)msg[currIndex++]);
            //Server sends a player left instruction to you when you leave a game
            //if the current game is null it means you have already left
            if (currGame != null)
            {
                var leftPlayer = users.GetUserFromName(s.ToString());

                currGame.users.RemoveUser(leftPlayer);

                if (leftPlayer.Category.Equals("Buddies"))
                {
                    KailleraTrayManager.Instance.handleTrayEvent(TrayFlags.PopValues.buddyLeftGame, leftPlayer);
                }

                GUIBundle playerLeftBundle = new GUIBundle();
                playerLeftBundle.Users = new UserList(currGame.users);
                playerLeftBundle.gameNum = currGame.id;
                WindowMngr.updateWindow(playerLeftBundle);
            }
        }

        /// <summary>
        /// Parses the player join message and adds the user to the current game
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="currIndex"></param>
        private void ProcessPlayerJoin(byte[] msg, int currIndex)
        {
            currIndex += 2;
            int gameId = BitConverter.ToInt32(msg, currIndex);
            currIndex += 4;
            StringBuilder s = new StringBuilder();
            while (msg[currIndex] != 0)
            {
                s.Append((char)msg[currIndex++]);
            }

            currIndex++;

            int ping = BitConverter.ToInt32(msg, currIndex);
            currIndex += 4;
            int userID = BitConverter.ToInt16(msg, currIndex);

            currIndex += 2;
            byte connection = msg[currIndex];

            User newPlayer = users.GetUserFromID(userID);

            //This is bad!
            if (newPlayer == null) return;

            if (newPlayer.Category.Equals("Buddies"))
            {
                KailleraTrayManager.Instance.handleTrayEvent(TrayFlags.PopValues.buddyJoinedGame, newPlayer);
            }

            if(currGame != null)
                currGame.users.AddUser(newPlayer);
            GUIBundle playerJoinedBundle = new GUIBundle();
            playerJoinedBundle.Users = new UserList(currGame.users);
            playerJoinedBundle.gameNum = currGame.id;
            WindowMngr.updateWindow(playerJoinedBundle);
        }

        private void ProcessGameStatus(byte[] msg, int currIndex)
        {
            currIndex+=2;
            Game thisgame = games.GetGameFromID(BitConverter.ToInt32(msg, currIndex));
            currIndex += 4;
            if (thisgame != null)
            {
                thisgame.status = msg[currIndex++];
                thisgame.numPlayers = Int32.Parse(msg[currIndex++].ToString());
                thisgame.maxPlayers = Int32.Parse(msg[currIndex].ToString());
                thisgame.Users_count = thisgame.numPlayers.ToString() + '/' + thisgame.maxPlayers.ToString();
            }
            gamesChanged(games);
        }

        private void ProcessGameClose(byte[] msg, int currIndex)
        {
            currIndex+=2;
            games.RemoveGame(games.GetGameFromID(BitConverter.ToInt32(msg, currIndex)));
            gamesChanged(games);

        }

        private void ProcessGameCreate(byte[] msg, int currIndex)
        {
            string userHost;
            Game newGame = new Game();
            currIndex++;
            StringBuilder s = new StringBuilder();
            while (msg[currIndex] != 0)
                s.Append((char)msg[currIndex++]);
            userHost = s.ToString();
            User host = users.GetUserFromName(userHost);

            newGame.host = host;

            newGame.gameHost = s.ToString();

            currIndex++;
            s.Clear();

            while (msg[currIndex] != 0)
                s.Append((char)msg[currIndex++]);
            newGame.name = s.ToString();

            if (host == null) return;

            if (host.Category.Equals("Buddies"))
                KailleraTrayManager.Instance.handleTrayEvent(TrayFlags.PopValues.gameCreated, host, newGame.name);

            if (users.GetUserFromName(username).Equals(host))
            {
                currGame = newGame;
            }
            
            s.Clear();
            currIndex++;

            while (msg[currIndex] != 0)
                s.Append((char)msg[currIndex++]);
            newGame.emuName = s.ToString();

            currIndex++;
            newGame.id = BitConverter.ToInt32(msg, currIndex);
            games.AddGame(newGame);
            gamesChanged(games);
            GUIManager.UpdateGames(games);


        }

        private void ProcessUserLeave(byte[] msg, int currIndex)
        {
            currIndex++;

            while (msg[currIndex++] != 0) ;
            int userID = BitConverter.ToInt16(msg, currIndex);

            var leftUser = users.GetUserFromID(userID);

            if (leftUser == null)
            {
                return;
            }

            if (leftUser.Category.Equals("Buddies"))
                KailleraTrayManager.Instance.handleTrayEvent(TrayFlags.PopValues.buddyLeftServer, leftUser);

            users.RemoveUser(leftUser);
            GUIBundle bund = new GUIBundle();
            bund.Users = new UserList(users);
            WindowMngr.updateWindow(bund);
        }

        private void ProcessUserJoin(byte[] msg, int currIndex)
        {
            var newUser = User.ParseUserJoined(msg, currIndex);

            if (newUser.Category.Equals("Buddies"))
                KailleraTrayManager.Instance.handleTrayEvent(TrayFlags.PopValues.buddyJoinedServer, newUser);

            users.AddUser(newUser);
            GUIBundle bund = new GUIBundle();
            bund.Users = new UserList(users);

            WindowMngr.updateWindow(bund);
        }

        private void ParseLoginSuccess(byte[] msg, int currIndex)
        {
            LoginSuccess.parseUsersGames(msg, ref users, ref games, currIndex);

            GUIBundle bund = new GUIBundle();
            bund.Users = new UserList(users);
            gamesChanged(games);
            WindowMngr.updateWindow(bund);
        }

        private void ProcessPong()
        {
            Pong pong = new Pong();
            messager.AddMessages(pong);
            messager.SendMessages(client);
        }

        /// <summary>
        /// Handles the keepalive packets every minute to remain in the server
        /// </summary>
        private void KeepAlive(Object adrr)
        {
            while (!stop)
            {
                try
                {
                    Thread.Sleep(1000 * 30);
                    KeepAliveInstruction ki = new KeepAliveInstruction();
                    List<KailleraInstruction> alive = new List<KailleraInstruction>();
                    alive.Add(ki);
                    IPEndPoint adr = (IPEndPoint)adrr;
                    log.Info("Sending keepalive packer to " + adr.ToString() + "with seq number " + ki.serial.ToString());
                    //Race condition here, should lock to prevent it
                    if (stop) break;
                    UDPMessenger.SendMessages(alive, adr, client);
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
            }

        }


        /// <summary>
        ///Logs on to the kaillera server
        ///and sets future port
        ///return true if connection was successful
        /// </summary>
        /// <returns></returns>
        public Boolean initConnection()
        {
            ip = new IPEndPoint(ipaddr, port);
            byte[] buff = Encoding.UTF8.GetBytes("HELLO0.83\0");
            client.Send(buff, buff.Length, ip);
            log.Info("Original Local end point is: " + client.Client.LocalEndPoint);
            IPEndPoint p = client.Client.LocalEndPoint as IPEndPoint;
            client.Client.ReceiveTimeout = 1500;
            byte[] rec;
            try
            {
                rec = client.Receive(ref ip);
            }
            catch(SocketException)
            {
                MessageBox.Show("Error logging into server.  Do you have the right ip?  Are you banned?", "Server Error");
                connectionClosed();
                return false;
            }
            string response = new string(Encoding.UTF8.GetChars(rec));
            //Parse the response and switch to the correct port
            if (response.Contains("HELLOD00D"))
            {
                this.port = Int32.Parse(response.Substring(9));
                this.ip = new IPEndPoint(ipaddr, port);
                client.Close();
                client = new UdpClient(p);
            }
            else
            {
                MessageBox.Show("Error logging into server.", "Server Error");
                return false;
            }
            log.Info("New local end point is: " + client.Client.LocalEndPoint);
            return true;
        }

        /// <summary>
        /// Sends multiple messages for chat messages
        /// </summary>
        /// <param name="text"></param>
        public void SendServerChatText(string text)
        {
            while(text.Length >= 127)
            {
                string sendtext = text.Substring(0, 127);
                string txt = text.Substring(127, text.Length - 127);
                ServerChatMsg msg = new ServerChatMsg(sendtext);
                messager.AddMessages(msg);
                messager.SendMessages(client);
                text = txt;
            }
            ServerChatMsg lastmsg = new ServerChatMsg(text);
            messager.AddMessages(lastmsg);
            messager.SendMessages(client);
        }

        /// <summary>
        /// Sends multiple messages for game chat messages
        /// </summary>
        /// <param name="text"></param>
        public void sendGameChatText(string text)
        {
            while (text.Length >= 127)
            {
                string sendtext = text.Substring(0, 127);
                string txt = text.Substring(127, text.Length - 127);
                GameChatMsg msg = new GameChatMsg(sendtext);
                messager.AddMessages(msg);
                messager.SendMessages(client);
                text = txt;
            }
            GameChatMsg lastmsg = new GameChatMsg(text);
            messager.AddMessages(lastmsg);
            messager.SendMessages(client);
        }

        /// <summary>
        /// Sends the join game message
        /// </summary>
        /// <param name="game"></param>
        public void tryJoinGame(Game game)
        {
            messager.AddMessages(new JoinGame(game.id));
            tempCurrGame = game;
            messager.SendMessages(client);
        }
        
        /// <summary>
        /// Joins a game and launches the game window
        /// </summary>
        public void createGame()
        {
            messager.AddMessages(new CreateGame());
            messager.SendMessages(client);
            //joinedGameSuccess(myGame);
        }

        /// <summary>
        /// Leaves the current game
        /// </summary>
        public void leaveGame(int gameNum)
        {
            messager.AddMessages(new LeaveGame(username));
            messager.SendMessages(client);
            currGame = tempCurrGame = null;
        }

        public void sendPM(User user, string text)
        {
            while (text.Length >= 110)
            {
                string sendtext = text.Substring(0, 110);
                string txt = text.Substring(110, text.Length - 110);
                SendServerChatText("/msg " + user.id + " " + sendtext);
                text = txt;
            }
            SendServerChatText("/msg " + user.id + " " + text);
        }
    }
}
