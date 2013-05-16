using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET.Messages
{
    class CreateGame : KailleraInstruction
    {
        string gameName;
        public CreateGame(string name = "*Chat (not game)\0")
            : base(10, false)
        {
            this.gameName = name;
            length += gameName.Length  + 1 + 4; //emu name, id
        }

        public override byte[] toBytes()
        {
            byte[] head = HeaderToByte();
            byte[] buff = new byte[length + head.Length];
            Array.Copy(head, buff, head.Length);
            int i = head.Length;
            buff[i++] = id;
            buff[i++] = 0; //Username
            Array.Copy(Encoding.UTF8.GetBytes(gameName), 0, buff, i, gameName.Length);
            i += gameName.Length;
            buff[i++] = 0;

            unchecked
            {
                buff[i++] = (byte)-1;
                buff[i++] = (byte)-1;
                buff[i++] = (byte)-1;
                buff[i++] = (byte)-1;
            }
            return buff;


        }
    }
}
