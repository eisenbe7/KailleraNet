using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET.Instructions
{
    class UserLogonInstruction : KailleraInstruction
    {
        private string clientName = "KailleraNET 0.8\0";
        private string user;
        public Byte connection;

        public UserLogonInstruction(string user, Byte connection) : base(3, true, user)
        { 
            this.user = user + '\0';
            this.connection = connection;
            userSent = true;
            length += clientName.Length + 1; 
        }

        public override Byte[] toBytes()
        {
            int currindex = 0;
            Byte[] buffer = new Byte[headerLength + length];
            Array.Copy(BitConverter.GetBytes(serial), 0, buffer, 0, 2);    //Serial Number
            currindex += 2;
            Array.Copy(BitConverter.GetBytes(length), 0, buffer, currindex, 2); //Length
            currindex += 2;

            buffer[currindex++] = id;
            Array.Copy(Encoding.UTF8.GetBytes(user), 0, buffer, currindex, user.Length);
            currindex += user.Length-1;
            Array.Copy(Encoding.UTF8.GetBytes(clientName), 0, buffer, currindex, clientName.Length);
            currindex += clientName.Length;
            buffer[currindex] = connection;
            return buffer;
        }
    }
}
