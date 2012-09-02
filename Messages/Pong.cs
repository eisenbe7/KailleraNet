using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET.Instructions
{
    class Pong : KailleraInstruction
    {
        public Pong()
            : base(6, false)
        {
            length += 16;
        }

        public override byte[] toBytes()
        {

            byte[] buff = new byte[length + headerLength];
            Array.Copy(HeaderToByte(), buff, 4);
            buff[4] = 6;
            buff[5] = 0;
            buff[6] = 0;
            buff[7] = 0;
            buff[8] = 0;
            buff[9] = 0;
            buff[10] = 1;
            buff[11] = 0;
            buff[12] = 0;
            buff[13] = 0;
            buff[14] = 2;
            buff[15] = 0;
            buff[16] = 0;
            buff[17] = 0;
            buff[18] = 3;
            buff[19] = 0;
            buff[20] = 0;
            buff[21] = 0;
            return buff;
        }
    }
}
