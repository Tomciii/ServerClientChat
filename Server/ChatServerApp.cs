//-----------------------------------------------------------------------
// <copyright file="ChatServerApp.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the server app logic.</summary>
//-----------------------------------------------------------------------
namespace Server
{
    /// <summary>
    /// The ChatServerApp class.
    /// </summary>
  public class ChatServerApp
    {
        /// <summary>
        /// The Main method of the ChatServerApp class starts a new server.
        /// </summary>
        public static void Main()
        {
            ChatServer server = new ChatServer();
            server.Start();
        }
    }
}