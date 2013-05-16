using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace KailleraNET.Hotkeys
{
    class BeginDisconnectHotkey : HotKey
    {
        public BeginDisconnectHotkey(string name, Key key, ModifierKeys modifiers, bool enabled)
            : base(key, modifiers, enabled)
        {
            Name = name;
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged(name);
                }
            }
        }

        protected override void OnHotKeyPress()
        {
            if (MessageBox.Show("Disconnect and close Application?  Note that you can continue running Kaillera in your system tray.", "Disconnect?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            KailleraWindowController.getMgr().shutDown();
        }


        protected BeginDisconnectHotkey(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            Name = info.GetString("BeginDisconnectHotkey");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("BeginDisconnectHotkey", Name);
        }
    }
}
