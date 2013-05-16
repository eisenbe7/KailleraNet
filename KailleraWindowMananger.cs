using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using KailleraNET.Views;
using KailleraNET.ViewModels;
using System.Windows;

namespace KailleraNET
{
    /// <summary>
    /// Maintains references to all windows, and handles all window events
    /// </summary>
    public class KailleraWindowController
    {
        List<ChatViewModel> ChatWindows = new List<ChatViewModel>();
        List<GameListViewModel> GameListWindows = new List<GameListViewModel>();

        public delegate void ConnectionFailed();
        public ConnectionFailed connectionFailed;

        private readonly int MAINCHAT = 0;

        UsersViewModel usersWindow;

        KailleraManager mgr;

        Mutex mutex = new Mutex();

        public Game currGame;

        private static KailleraWindowController instance;

        public static KailleraWindowController getMgr()
        {
            if (instance != null) return instance;
            else return new KailleraWindowController();
        }

        private KailleraWindowController()
        {
            instance = this;
        }


        /// <summary>
        /// Updates the chat window with a GUIBundle.  At this point there shouldn't
        /// be more than one window receiving the update, but maybe in the future there will be
        /// </summary>
        /// <param name="gb">The UI update - sent to corresponding window based on game number</param>
        public void updateWindow(GUIBundle gb)
        {
            
            int gamenumber = gb.gameNum;
            mutex.WaitOne();

            IEnumerable<ChatViewModel> targwind = from wind in ChatWindows where wind.gameNumber == gamenumber select wind;

            List<ChatViewModel> newList = new List<ChatViewModel>(targwind);
            mutex.ReleaseMutex();


                foreach (ChatViewModel c in newList)
                {
                    c.updater(gb);
                }


                if (gb.gameNum == MAINCHAT && gb.Users != null)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => updateUsersWindow(gb)));
                }
        }
        

        /// <summary>
        /// Updates the users window
        /// </summary>
        /// <param name="gb"></param>
        public void updateUsersWindow(GUIBundle gb)
        {
            if (usersWindow != null)
            {
                usersWindow.updateUsers(gb.Users.users);
            }
        }


        /// <summary>
        /// Begins a connection to the Kaillera server from the connection menu, and opens main chat window
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        internal void BeginNewConnection(System.Net.IPAddress ip, int port, string username)
        {
            //Add one chat window - 0 for main server chat
            initializeServerChatWindow();

            //Initialize user window
            initializeUsersWindow();

            startKailleraManager(ip, port, username);           

            initializeGameList();
        }

        /// <summary>
        /// Creates a new chat window for the main chat
        /// </summary>
        private void initializeServerChatWindow()
        {
            ChatViewModel mainWin = new ChatViewModel(MAINCHAT, this);
            mainWin.winClosed += onMainChatClosed;
            mainWin.sendGameChat = sendGameChat;
            mainWin.addBuddyList += addToBuddyList;
            mainWin.gameNumber = MAINCHAT;

            mainWin.wind.sendPM += sendPM;

            ChatWindows.Add(mainWin);
        }

        /// <summary>
        /// Initializes the users window
        /// </summary>
        private void initializeUsersWindow()
        {
            this.usersWindow = new UsersViewModel();
            this.usersWindow.addBuddyList += addToBuddyList;
            this.usersWindow.wind.sendPM += sendPM;
        }

        /// <summary>
        /// Starts the kaillera manager (model) to send and receive network data
        /// </summary>
        private void startKailleraManager(System.Net.IPAddress ip, int port, string username)
        {
            mgr = new KailleraManager(ip, port, username, KailleraTrayManager.Instance);

            Thread KManager = new Thread(new ParameterizedThreadStart(mgr.Start));
            KManager.Start(this);
        }


        /// <summary>
        /// Starts the game window
        /// </summary>
        private void initializeGameList()
        {
            GameListViewModel gamesList = new GameListViewModel();
            GameListWindows.Add(gamesList);

            gamesList.joinGame += joinSelectedGame;

            gamesList.wind.createChat += createChat;

            //Send the changes in games to the game list
            mgr.gamesChanged += gamesList.onGamesChanged;

            //Set the event to begin the game chat window
            mgr.joinedGameSuccess += joinGameSuccess;
        }

        public void createChat()
        {
            if (currGame != null && MessageBox.Show("Leave your current game and create a chat?", "Make new Game?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                exitGame();
            }

            mgr.createGame();

        }

        /// <summary>
        /// Sets KailleraManager to stop upon main chat window close.  KailleraMananger sends logout packet before terminating.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onMainChatClosed(object sender, EventArgs e)
        {
                mgr.connectionClosed();
                lock (ChatWindows)
                {
                    foreach (var wind in ChatWindows)
                    {
                        wind.closeWindow();
                    }
                }
                lock (GameListWindows)
                {
                    foreach (var wind in GameListWindows)
                    {
                        wind.closeWindow();
                    }
                }
                    usersWindow.close();
        }

        /// <summary>
        /// Sends logout packets, closes the kaillera connection, closes all windows
        /// and ends the application
        /// </summary>
        public void shutDown()
        {
            if (mgr != null)
            {
                mgr.connectionClosed();
                    foreach (var wind in ChatWindows)
                    {
                        wind.closeWindow();
                    }
                    foreach (var wind in GameListWindows)
                    {
                        wind.closeWindow();
                    }

                    usersWindow.close();
                
            }
            KailleraTrayManager.Instance.shutDown();
            Application.Current.Shutdown(0);
        }



        /// <summary>
        /// Sends message to server - if it is 0, it is sent to the main chat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void sendGameChat(int gameNum, string textMessage)
        {
            if (gameNum == 0)
                mgr.SendServerChatText(textMessage);
            else
                mgr.sendGameChatText(textMessage);

        }

        /// <summary>
        /// Messages the KailleraManager to join the selected game
        /// </summary>
        /// <param name="game"></param>
        public void joinSelectedGame(Game game)
        {
            if (game == null) return;
            currGame = game;
            mgr.tryJoinGame(game);
        }

        //Launch new game chat window
        public void joinGameSuccess(Game game)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => joinGameSuccessDispatched(game)));
        }

        /// <summary>
        /// Dispatched version of join game success which can launch the new window
        /// </summary>
        /// <param name="game"></param>
        public void joinGameSuccessDispatched(Game game)
        {
            ChatViewModel gameChat = new ChatViewModel(game.id, this, game);
            gameChat.leaveGame += mgr.leaveGame;
            gameChat.sendGameChat = sendGameChat;
            gameChat.leaveGame += exitGame;

            //Weird error here
                ChatWindows.Add(gameChat);
            gameChat.addBuddyList += addToBuddyList;
            gameChat.wind.sendPM += sendPM;

            //Create a new GUIBundle and update the game chat window with existing players
            GUIBundle newGameBund = new GUIBundle();
            newGameBund.gameNum = game.id;
            newGameBund.Users = game.users;
            updateWindow(newGameBund);
            currGame = game;
        }

        /// <summary>
        /// Removes all of the game windows from the list and
        /// calls the Kaillera Manager to leave the game
        /// </summary>
        /// <param name="gameNum"></param>
        public void exitGame(int gameNum)
        {
            lock (ChatWindows)
            {
                mgr.leaveGame(gameNum);
            }
        }

        /// <summary>
        /// Exits the current game as assigned in the controller
        /// and closes window
        /// </summary>
        public void exitGame()
        {
            if (currGame == null) return;
            exitGame(currGame.id);

            var winds = from windows in ChatWindows
                        where windows.gameNumber == currGame.id
                        select windows;
            foreach (var wind in winds)
            {
                wind.closeWindow();
            }

            lock (ChatWindows)
            {
                ChatWindows.RemoveAll((chatWind) => chatWind.gameNumber == currGame.id);
            }
            currGame = null;

        }

        public void addToBuddyList(User user)
        {
            if (user.Category.Equals("Buddies"))
            {
                user.Category = "Users";
                usersWindow.refreshList();
            }
            else
            {
                user.Category = "Buddies";
                usersWindow.refreshList();
            }
        }

        public void sendPM(User user, string text)
        {
            mgr.sendPM(user, text);
        }


        public void showUsersWindow(bool hide = false)
        {
            if (usersWindow == null) return;
            if (hide)
            {
                usersWindow.wind.Hide();
                return;
            }
            if (this.usersWindow.wind.IsVisible)
            {
                this.usersWindow.wind.Visibility = Visibility.Hidden;
            }
            else
                this.usersWindow.wind.Visibility = Visibility.Visible;
        }

        public void showServerChatWindow(bool hide = false)
        {
            IEnumerable<ChatViewModel> targwind = from wind in ChatWindows where wind.gameNumber == 0 select wind;
            foreach (ChatViewModel c in targwind)
            {
                if (c.wind.Visibility == Visibility.Visible || hide)
                {
                    c.wind.Hide();
                }
                else
                c.wind.Show();               
            }
        }

        public void showGamesWindow(bool hide = false)
        {
            foreach (var w in GameListWindows)
            {
                if (w.wind.IsVisible || hide)
                {
                    w.wind.Hide();
                }
                else
                w.wind.Show();
            }
        }

        public void showCurrGameWindow(bool hide = false)
        {
            IEnumerable<ChatViewModel> targwind = from wind in ChatWindows where wind.gameNumber != 0 select wind;
            foreach (ChatViewModel c in targwind)
            {
                if (c.wind.IsVisible || hide)
                {
                    c.wind.Hide();
                }
                else
                c.wind.Show();
            }
        }
        /// <summary>
        /// Toggles visibility of all windows
        /// </summary>
        internal void toggleMinimize()
        {
            showCurrGameWindow(true);
            showGamesWindow(true);
            showServerChatWindow(true);
            showUsersWindow(true);
        }

        public void closeAllWindows()
        {
            lock (ChatWindows)
            {
                foreach (var wind in ChatWindows)
                {
                    wind.closeWindow();
                }
            }
            lock(GameListWindows)
            {
                foreach (var wind in GameListWindows)
                {
                    wind.closeWindow();
                }
            }

                usersWindow.close();
            }
        }
    }
