using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET.Messages
{
    /// <summary>
    /// Instruction to leave a game
    /// </summary>
    class LeaveGame : KailleraInstruction
    {
        public LeaveGame(string username) : base(11, true, username)
        {
            length += 1 + 2;
        }

        public override byte[] toBytes()
        {
            byte[] head = HeaderToByte();
            byte[] buff = new byte[length];
            Array.Copy(head, buff, head.Length);
            int i = head.Length;
            buff[i++] = id;
            buff[i++] = 0;

            //-1 is used for irrelevant fields
            unchecked
            {
                buff[i++] = (byte)-1;
                buff[i++] = (byte)-1;
            }
            return buff;
        }
    }
}
