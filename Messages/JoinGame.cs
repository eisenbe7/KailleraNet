using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KailleraNET.Messages
{
    /// <summary>
    /// Message sent from client to server to join a particular game
    /// </summary>
    class JoinGame : KailleraInstruction
    {
        //The game to join
        int gameID;

        public JoinGame(int gameID) : base(12, false)
        {
            this.gameID = gameID;
            length += 4 + 1 + 4 + 2 + 1; //GameID, Irrelevant username, ping, user_id, connection
        }

        /// <summary>
        /// Emulinker treats the packet as a join game request
        /// if username.length == 0 && ping == 0 && userID = 0xFFFF
        /// </summary>
        /// <returns></returns>
        public override byte[] toBytes()
        {
            byte[] head = HeaderToByte();
            byte[] buff = new byte[length + head.Length];
            Array.Copy(head, buff, head.Length);
            int i = head.Length;
            buff[i++] = (byte)id;
            buff[i++] = (byte)0; //Username in header

            Array.Copy(BitConverter.GetBytes(gameID), 0, buff, i, 4);
            i += 4;
            buff[i++] = 0; //username

            //ping
            Array.Copy(BitConverter.GetBytes(0), 0, buff, i, 4);

            i += 4;

            //-1 is used for irrelevant fields
            unchecked
            {

                buff[i++] = (byte)-1; //userID
                buff[i++] = (byte)-1; //userID
                buff[i++] = (byte)1; //connection TODO: make this actual connection
            }

            return buff;            
        }


    }
}
