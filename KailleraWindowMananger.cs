using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using KailleraNET.Views;
using KailleraNET.ViewModels;

namespace KailleraNET
{
    /// <summary>
    /// Maintains references to all windows, and handles all window events
    /// </summary>
    public class KailleraWindowMananger
    {
        List<ChatViewModel> ChatWindows = new List<ChatViewModel>();
        List<GameListViewModel> GameListWindows = new List<GameListViewModel>();


        UsersViewModel usersWindow;

        KailleraManager mgr;

        Game currGame;

        Object mutex = new Object();


        /// <summary>
        /// Updates the chat window with a GUIBundle.  At this point there shouldn't
        /// be more than one window receiving the update, but maybe in the future there will be
        /// </summary>
        /// <param name="gb">The UI update - sent to corresponding window based on game number</param>
        public void updateWindow(GUIBundle gb)
        {
            
            int gamenumber = gb.gameNum;
            IEnumerable<ChatViewModel> targwind = from wind in ChatWindows where wind.gameNumber == gamenumber select wind;
            foreach (ChatViewModel c in targwind)
            {
                c.updater(gb);
            }

            if (gb.gameNum == 0 && gb.Users != null)
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
            ChatViewModel mainWin = new ChatViewModel(0, this);
            mainWin.winClosed += onMainChatClosed;
            mainWin.sendGameChat = sendGameChat;
            mainWin.addBuddyList += addToBuddyList;
            mainWin.gameNumber = 0;

            ChatWindows.Add(mainWin);

            //Initialize user window

            usersWindow = new UsersViewModel();
            usersWindow.addBuddyList += addToBuddyList;


            mgr = new KailleraManager(ip, port, username, KailleraTrayManager.Instance);

            Thread KManager = new Thread(new ParameterizedThreadStart(mgr.Start));
            KManager.Start(this);

            GameListViewModel gamesList = new GameListViewModel();
            GameListWindows.Add(gamesList);

            gamesList.joinGame += joinSelectedGame;

            //Send the changes in games to the game list
            mgr.gamesChanged += gamesList.onGamesChanged;

            //Set the event to begin the game chat window
            mgr.joinedGameSuccess += joinGameSuccess;

            //Initialize the users window

        }

        /// <summary>
        /// Sets KailleraManager to stop upon main chat window close.  KailleraMananger sends logout packet before terminating.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onMainChatClosed(object sender, EventArgs e)
        {
                mgr.connectionClosed();
                lock (mutex)
                {
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
            ChatViewModel gameChat = new ChatViewModel(currGame.id, this, game);
            gameChat.leaveGame += mgr.leaveGame;
            gameChat.sendGameChat = sendGameChat;
            gameChat.leaveGame += exitGame;
            ChatWindows.Add(gameChat);
            gameChat.addBuddyList += addToBuddyList;
            //Create a new GUIBundle and update the game chat window with existing players
            GUIBundle newGameBund = new GUIBundle();
            newGameBund.gameNum = game.id;
            newGameBund.Users = game.users;
            updateWindow(newGameBund);
        }

        /// <summary>
        /// Removes all of the game windows from the list and
        /// calls the Kaillera Manager to leave the game
        /// </summary>
        /// <param name="gameNum"></param>
        public void exitGame(int gameNum)
        {
            lock (mutex)
            {
                //ChatWindows.RemoveAll((chatWind) => chatWind.gameNumber == gameNum);
                mgr.leaveGame(gameNum);
            }
        }

        public void addToBuddyList(User user)
        {
            user.Category ="Buddies";
            usersWindow.refreshList();
        }
    }
}
