using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET.Instructions
{
    /// <summary>
    /// Instruction sent every minute to
    /// stay connected to the server
    /// </summary>
    class KeepAliveInstruction : KailleraInstruction
    {
        public KeepAliveInstruction() : base(9, false)
        { }

        public override byte[] toBytes()
        {
            byte[] buff = new byte[5];
            byte[] head = HeaderToByte();
            Array.Copy(head, buff, head.Length);
            buff[4] = id;
            return buff;


        /*    int currindex = 0;
            byte[] buff = new byte[5];
            Array.Copy(BitConverter.GetBytes(serialNum), buff, 2);
            currindex += 2;
            Array.Copy(BitConverter.GetBytes(1), 0, buff, currindex, 2);
            currindex += 2;
            buff[currindex++] = id;
            return buff;
         */
        }


    }
}
