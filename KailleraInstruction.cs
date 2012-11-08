using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using log4net;

namespace KailleraNET
{
    /// <summary>
    /// Base class for a kaillera packet - note that this class is abstract
    /// and cannot be instantiated.  Each packet class inherits from this class
    /// Note:  Instruction classes include the "packed_instruction" header
    /// </summary>
    abstract class KailleraInstruction
    {
        public byte id { get; set; }
        public int length { get; protected set; } //Length in bytes
        public Boolean userSent;
        public static short serialNum = 0;
        protected string username;
        public short serial;
        public int headerLength;
        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int InstructionLength
        {
            get
            {
                return length + headerLength;
            }
            private set {}
        }


        public KailleraInstruction(byte id, Boolean user, string username = "\0")
        {

            log.Info("Creating new instruction with id: " + id.ToString() + "with sequence number: " + serialNum.ToString());
            this.id = id;
            userSent = user;
            length = 1; //id
            this.username = username;
            if (this.username[this.username.Length - 1] != '\0') this.username += '\0';
            length += this.username.Length;
            headerLength = 2 + 2; //user, serial
        }

        public KailleraInstruction() { }

        public void addBytes(string content, ref byte[] buff, int index)
        {
            length += content.Length;
            Array.Copy(Encoding.UTF8.GetBytes(content), 0, buff, index, content.Length);
        }

        public void addByte(byte content, ref byte[] buff, int index)
        {
            length += 1;
            buff[index] = content;
        }

        public void addBytes(byte[] content, ref byte[] buff, int index, int length)
        {
            this.length += content.Length;
            Array.Copy(content, 0, buff, index, content.Length);
        }


        abstract public byte[] toBytes();


        /// <summary>
        /// Adds header to the instruction -
        /// Serial, length, id, and user if required
        /// </summary>
        /// <param name="buff">Array to add buffer to</param>
        /// <returns>current index of array</returns>
        public byte[] HeaderToByte()
        {
            byte[] buff = new byte[4];
            Array.Copy(BitConverter.GetBytes(serialNum), buff, 2);
            Array.Copy(BitConverter.GetBytes(length), 0, buff, 2, 2); //length+2 for 
            return buff;

        }
    }
}
