using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET
{


    public class User : IComparable
    {
        public String Name; 
        public int ping; 
        public int status; 
        public int id; 
        public byte connection; 


        public User(String name, int ping, int status, int id, byte connection)
        {
            this.Name = name;
            this.ping = ping;
            this.status = status;
            this.id = id;
            this.connection = connection;
        }

        public User()
        { }

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

    }
}
