//-----------------------------------------------------------------------
// <copyright file="OnClientConnectedEventArgs.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the EventArgs for the OnClientJoined event.</summary>
//-----------------------------------------------------------------------
namespace NetworkComponents
{
    using System;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// The EventArgs for the OnClientConnected event.
    /// </summary>
    public class OnClientConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// The Network stream used for the event.
        /// </summary>
        private NetworkStream networkStream;

        /// <summary>
        /// The clients manager used for the event.
        /// </summary>
        private ChatClientManager manager;

        /// <summary>
        /// The name of the new client.
        /// </summary>
        private string nickname;

        /// <summary>
        /// The id of the new client.
        /// </summary>
        private int id;

        /// <summary>
        /// Initializes a new instance of the OnClientConnectedEventArgs class.
        /// </summary>
        /// <param name="tcpClient">Takes a client as input.</param>
        /// <param name="manager">Takes a client manager as input.</param>
        public OnClientConnectedEventArgs(TcpClient tcpClient, ChatClientManager manager)
        {
            this.manager = manager;
            this.id = manager.ClientsIDs.Count - 1;
            this.nickname = manager.ClientsNickNames.ElementAt(manager.ClientsIDs.Count - 1);
            this.networkStream = tcpClient.GetStream();
        }

        /// <summary>
        /// Gets the value of id.
        /// </summary>
        /// <value>The value of id.</value>
        public int ID
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// Sends a list of all online clients to the new client.
        /// </summary>
        public void SendOnlineClients()
        {
            ////Protocol for welcome message handling and updating client lists is making use of unique beginnings of the string message as well as using '#' for seperating the individual data.
            string message = "ServerSendingOnlineClients:#" + this.manager.ClientsIDs.Count + "#";
           
            for (int i = 0; i < this.manager.ClientsIDs.Count; i++)
            {
                message += this.manager.ClientsIDs.ElementAt(i) + "#" + this.manager.ClientIPEndPoints.ElementAt(i) + "#" + this.manager.ClientsNickNames.ElementAt(i) + "#" + this.manager.ClientConsoleColors.ElementAt(i) + "#";
            }

            byte[] sendBuffer = Encoding.UTF8.GetBytes(message);
            this.networkStream.Write(sendBuffer, 0, sendBuffer.Length);
        }

        /// <summary>
        /// Sends an assigned name to a new client.
        /// </summary>
        public void SendNickName()
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes("ServerSendingWelcomeMessage:#" + this.ID.ToString() + "#");
            this.networkStream.Write(sendBuffer, 0, sendBuffer.Length);
        }

        /// <summary>
        /// Sends a welcome message to the new client.
        /// </summary>
        public void SendWelcomeMessage()
        {
            this.SendTextMessage($"{DateTime.Now} Server says:#Welcome to the ChatServer!#Your current nickname is \"{this.nickname}\"#Your current color is \"grey\"#Type \"nickname YOUR_NEW_NICKNAME\" to change your nickname#Type \"color YOUR_NEW_COLOR\" to change your color.#Type \"get out\" to exit this chat application.");
        }

        /// <summary>
        /// Sends a text message.
        /// </summary>
        /// <param name="text">Takes a string as input.</param>
        private void SendTextMessage(string text)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(text);
            this.networkStream.Write(sendBuffer, 0, sendBuffer.Length);
        }
    }
}