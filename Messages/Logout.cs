using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET.Instructions
{
    class Logout : KailleraInstruction
    {
        string exitMessage = "Using KailleraNET 0.1!";

        public Logout() 
            : base(1, false)
        {
            length += 2 + exitMessage.Length + 1; //Irrelevant id field, exit message length, null character
        }

        public override byte[] toBytes()
        {
            //A value of -1 is used to denote a unnecessary field
            unchecked
            {
                byte[] buff = new byte[length + 4];
                Array.Copy(HeaderToByte(), buff, 4);
                int i = headerLength++;
                buff[i++] = id;
                buff[i++] = 0;

                buff[i++] = (byte)-1;
                buff[i++] = (byte)-1;

                Array.Copy(Encoding.UTF8.GetBytes(exitMessage), 0, buff, i, exitMessage.Length);
                buff[i + exitMessage.Length] = 0;
                return buff;
            }
        }


    }
}
