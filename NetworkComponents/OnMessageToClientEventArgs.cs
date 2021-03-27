//-----------------------------------------------------------------------
// <copyright file="OnMessageToClientEventArgs.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the EventArgs for the OnMessageToClient event.</summary>
//-----------------------------------------------------------------------
namespace NetworkComponents
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// The EventArgs for the OnMessageToClient event.
    /// </summary>
    public class OnMessageToClientEventArgs : EventArgs
    {
        /// <summary>
        /// The user that sent the message.
        /// </summary>
        private string user;

        /// <summary>
        /// The message sent by the user.
        /// </summary>
        private string message;

        /// <summary>
        /// Initializes a new instance of the OnMessageToClientEventArgs class.
        /// </summary>
        /// <param name="user">Takes a user as input.</param>
        /// <param name="message">Takes a message as input.</param>
        public OnMessageToClientEventArgs(string user, string message)
        {
            this.user = user;
            this.message = message;
        }

        /// <summary>
        /// Sends a message to clients that are listening to the OnMessageToClient event.
        /// </summary>
        /// <param name="client">Takes a client as input.</param>
        public void SendClientMessage(TcpClient client)
        {
            this.SendTextMessage(client, $"{DateTime.Now} {user} says: {message}");
        }

        /// <summary>
        /// Sends a text message through a Network stream.
        /// </summary>
        /// <param name="client">Takes a client as input.</param>
        /// <param name="text">Takes a text message as input.</param>
        private void SendTextMessage(TcpClient client, string text)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(text);
            client.GetStream().Write(sendBuffer, 0, sendBuffer.Length);
        }
    }
}
