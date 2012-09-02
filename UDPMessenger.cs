using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace KailleraNET
{
    /// <summary>
    /// UDPMessenger handles the packing and sending of kaillera messages
    /// Also recieves messages from server
    /// </summary>
    class UDPMessenger
    {

        private static Object mutex = new Object();


        private List<KailleraInstruction> Instructions = new List<KailleraInstruction>();
        private IPEndPoint address;

        public UDPMessenger(IPEndPoint ip, params KailleraInstruction[] instructions)
        {

            foreach (KailleraInstruction k in instructions)
            {
                k.serial = KailleraInstruction.serialNum++;
                Instructions.Add(k);
            }
            address = ip;
        }

        public UDPMessenger(IPEndPoint ip, List<KailleraInstruction> kList)
        {

            Instructions = kList;
            address = ip;
        }

        public UDPMessenger(IPEndPoint ip)
        {
            address = ip;
        }

        public byte[] Recieve(UdpClient client)
        {
            return client.Receive(ref address);
        }

        /// <summary>
        /// Adds messages to the list to be sent
        /// </summary>
        /// <param name="ki"></param>
        public void AddMessages(params KailleraInstruction[] ki)
        {
            foreach (KailleraInstruction k in ki)
            {
                Instructions.Add(k);
            }
        }


        /// <summary>
        /// Sends out Messages - instance method
        /// </summary>
        public void SendMessages(UdpClient client)
        {
            lock (mutex)
            {
                Instructions.Reverse(); //Newest messages are sent first in kaillera protocol

                //Let's first calculate the total size of the byte buffer
                int bytebuffersize = 1;  //Byte for number of instructions
                foreach (KailleraInstruction k in Instructions)
                {
                    bytebuffersize += k.length + k.headerLength;
                }

                //Initiate the byte array
                Byte[] buffer = new Byte[bytebuffersize];

                //This should always be less than 255!  Error otherwise
                buffer[0] = (Byte)Instructions.Count;
                int currIndex = 1;

                //Now copy in the array from each instruction
                foreach (KailleraInstruction k in Instructions)
                {
                    k.serial = KailleraInstruction.serialNum++;
                    Byte[] InstructArray = k.toBytes();
                    InstructArray.CopyTo(buffer, currIndex);
                    currIndex += k.length;
                }

                //Now we are ready to send
                client.Send(buffer, buffer.Length, address);
                Instructions.Clear();
            }
       }

        /// <summary>
        /// Static method for sending out kaillera message - DO NOT USE if connection is already established - it will not work
        /// because multiple UDP clients cannot bind
        /// </summary>
        /// <param name="ki"></param>
        /// <param name="address"></param>
        /// <param name="localAddress"></param>
        public static void SendMessage(KailleraInstruction ki, IPEndPoint address, IPEndPoint localAddress)
        {
            lock (mutex)
            {
                byte[] buffer = new byte[ki.length + 4];
                buffer[0] = 1;
                byte[] instructArray = ki.toBytes();
                instructArray.CopyTo(buffer, 1);
                UdpClient client = new UdpClient(localAddress);
                client.Send(buffer, buffer.Length, address);
                client.Close();
            }
        }


        /// <summary>
        /// Static method for sending out kaillera messages - DO NOT USE if connection is already established - it will not work
        /// </summary>
        /// <param name="Instructions">List of instructions</param>
        /// <param name="address">Server and port to send messages to</param>

        public static void SendMessages(List<KailleraInstruction> Instructions, IPEndPoint address, UdpClient sender)
        {
            lock (mutex)
            {
                Instructions.Reverse(); //Newest messages are sent first in kaillera protocol

                //Let's first calculate the total size of the byte buffer
                int bytebuffersize = 1;  //Byte for number of instructions
                foreach (KailleraInstruction k in Instructions)
                {

                    k.serial = KailleraInstruction.serialNum++;
                    bytebuffersize += k.length + k.headerLength; //Length of body + serialnum + lengthnum + id(included) + username(included)
                }

                //Initiate the byte array
                Byte[] buffer = new Byte[bytebuffersize];

                //This should always be less than 255!  Error otherwise
                buffer[0] = (Byte)Instructions.Count;
                int currIndex = 1;

                //Now copy in the array from each instruction
                foreach (KailleraInstruction k in Instructions)
                {
                    Byte[] InstructArray = k.toBytes();
                    string s = Encoding.ASCII.GetString(InstructArray);
                    InstructArray.CopyTo(buffer, currIndex);
                    currIndex += k.length;
                }

                sender.Send(buffer, buffer.Length, address);
            }
       }



    }
}
