using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using NotifyIcon = System.Windows.Forms.NotifyIcon;
using System.Drawing;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Input;

namespace KailleraNET
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        KailleraTrayManager k;
        protected override void  OnStartup(StartupEventArgs e)
        {

            TaskbarIcon tb = (TaskbarIcon)FindResource("KailleraNotifyIcon");

            //Starts the tray notification manager with the default Kaillera Icon
            k = KailleraTrayManager.Instance;
            k.setIcon(tb);

 	        base.OnStartup(e);
        }
    }
}
