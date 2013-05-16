using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Drawing;
using System.Windows.Input;
using KailleraNET.Util;

namespace KailleraNET
{
    /// <summary>
    /// Handles the tray icon and associated events - implements Singleton pattern
    /// </summary>

    public class KailleraTrayManager
    {

        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        TrayFlags.PopValues currEvent;

        private static KailleraTrayManager instance;

        private string keyword = null;
        private User targUser = null;


        /// <summary>
        /// Accessor that always returns the same instance
        /// </summary>
        public static KailleraTrayManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KailleraTrayManager();
                }
                return instance;
            }
        }



        /// <summary>
        /// Private constructor to prevent multiple instances
        /// </summary>
        private KailleraTrayManager()
        {

        }

        public delegate void onTrayClick();

        public onTrayClick trayClicked;

        /// <summary>
        /// Current window to re-activate when clicked
        /// </summary>
        List<Window> activeWindow = new List<Window>();

        TaskbarIcon kIcon;      

        /// <summary>
        /// Sets/gets the active window for the tray manager.
        /// This will be expanded to multiple windows in the future
        /// </summary>
        public void addActiveWindow(Window value)
        {        
                if (!TrayFlags.containsType(value.GetType())) return;
                activeWindow.Add(value);
        }

        public void clearWindows()
        {
            activeWindow.Clear();
        }

        public void removeWindow(Window w)
        {
            if(activeWindow.Contains(w))
                activeWindow.Remove(w);
        }

        public KailleraWindowController windowMgr;

        public void setIcon(TaskbarIcon currIcon)
        {
            kIcon = currIcon;
            kIcon.TrayLeftMouseDown += handleTrayClick;
            kIcon.TrayBalloonTipClicked += handleTrayClick;
        }

        /// <summary>
        /// Handles the click on the tray button
        /// Maximizes current windows 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleTrayClick(object sender, RoutedEventArgs e)
        {
            //Fire tray click event
            if(trayClicked != null)
                trayClicked();

            if (currEvent != TrayFlags.PopValues.None)
            {
                foreach (var wind in activeWindow)
                {
                    if (TrayFlags.handlesEvent(wind, currEvent))
                    {                      
                            wind.Visibility = Visibility.Visible;
                            wind.WindowState = WindowState.Normal;
                            wind.Show();                        
                    }
                }
            }
            currEvent = TrayFlags.PopValues.None;
        }

        /// <summary>
        /// Handles the tray event if the active window responds to any of the flag
        /// Only one event should be passed in at a time!
        /// </summary>
        /// <param name="flags"></param>
        public void handleTrayEvent(TrayFlags.PopValues flag, User targUser = null, string keyword = null)
        {
            if (!SettingsManager.getMgr().isNotifyEnabled(flag)) return;

            foreach (Window w in activeWindow)
            {
                if (!TrayFlags.handlesEvent(w, flag))
                {
                    log.Info("Received event: " + flag.ToString() + ". Ignoring - " + activeWindow.ToString() + " does not accept event");
                    continue;
                }

                

                currEvent = flag;
                this.targUser = targUser;
                this.keyword = keyword;

                string message = TrayFlags.trayLang[currEvent];
                message = message.Replace("#", targUser.Name);
                message = message.Replace("$", keyword);

                log.Info("Recieved tray event: " + flag.ToString() + ", with targUser: " + targUser.ToString() + ", message: " + message);

                kIcon.ShowBalloonTip("Kaillera Event", message, BalloonIcon.Info);
                return;
            }
        }


        internal void shutDown()
        {
            kIcon.Dispose();
        }
    }
}
