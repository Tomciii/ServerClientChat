//-----------------------------------------------------------------------
// <copyright file="ChatClientApp.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the chatclient app logic.</summary>
//-----------------------------------------------------------------------
namespace Client
{
    using System.Net.Sockets;

    /// <summary>
    /// The Chat Client App Class.
    /// </summary>
    public class ChatClientApp
    {
        /// <summary>
        /// The main method of the chat client app starts a new chat client.
        /// </summary>
        public static void Main()
        {
            TcpClient tcpclient = new TcpClient();
            ChatClient client = new ChatClient(tcpclient);
            client.Start();
        }
    }
}