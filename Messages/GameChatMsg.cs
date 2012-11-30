using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET.Messages
{
    /// <summary>
    /// The game chat message
    /// </summary>
    class GameChatMsg : KailleraInstruction
    {
        string text;
        public GameChatMsg(string text)
            : base(8, false)
        {
            this.text = text + '\0';
            length += this.text.Length;
        }

        public override byte[] toBytes()
        {
            //ID is listed twice!
            byte[] head = HeaderToByte();
            byte[] buff = new byte[length + 4];
            Array.Copy(head, buff, head.Length);

            int i = head.Length;

            buff[i++] = id;

            buff[i++] = 0;


            Array.Copy(Encoding.UTF8.GetBytes(text), 0, buff, i, text.Length);
            return buff;
        }
    }
}
