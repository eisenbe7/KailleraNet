using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET
{


    public class User : IComparable
    {
        public String Name { get; set; }
        public int ping; 
        public int status; 
        public int id; 
        public byte connection;
        private string category;

        public int sortOrder { get; set; }

        public string Category
        {
            get
            {
                return category;
            }
            set
            {
                if ("Buddies".Equals(value))
                {
                    SettingsManager.getMgr().addBuddy(Name);
                }
                if ("Ignored".Equals(value))
                {
                }
                if ("Users".Equals(value) && category.Equals("Buddies"))
                {
                    SettingsManager.getMgr().removeBuddy(Name);
                }

                category = value;

            }
        }


        public User(String name, int ping, int status, int id, byte connection)
        {
            category = "Users";
            this.Name = name;
            this.ping = ping;
            this.status = status;
            this.id = id;
            this.connection = connection;
        }

        public User()
        {
            category = "Users";
        }

        public static User ParseUserJoined(byte[] msg, int offset)
        {
            User user = new User();
            offset++;
            int endName = offset;
            StringBuilder sb = new StringBuilder();
            while (msg[offset] != 0)
                sb.Append((char)msg[offset++]);
            user.Name = sb.ToString();
            offset++;

            user.id = BitConverter.ToInt16(msg, offset);
            offset += 2;
            user.ping = BitConverter.ToInt32(msg, offset);
            offset += 4;
            user.connection = msg[offset];

            user.assignCategory();

            return user;
        }

        public int CompareTo(Object obj)
        {
            return CompareToName(obj);

        }

        public int CompareToName(Object obj)
        {
            User u = obj as User;
            
            return Name.CompareTo(u.Name);
        }


        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Reads through the buddy and ignore files and properly assigns status
        /// </summary>
        public void assignCategory()
        {
            if (SettingsManager.getMgr().isBuddy(Name))
            {
                Category = "Buddies";
            }

            if (SettingsManager.getMgr().isIgnored(Name))
            {
                Category = "Ignored";
            }
        }
    }
}
