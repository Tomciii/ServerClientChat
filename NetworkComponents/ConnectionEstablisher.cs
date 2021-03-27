//-----------------------------------------------------------------------
// <copyright file="ConnectionEstablisher.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file has useful methods for both servers and clients to connect.</summary>
//-----------------------------------------------------------------------
namespace NetworkComponents
{
    using System;
    using System.Net;

    /// <summary>
    /// This class helps to establish connections between client and server.
    /// </summary>
    public class ConnectionEstablisher
    {
        /// <summary>
        /// Prompts the user to select an IPEndPoint.
        /// </summary>
        /// <returns>Returns an IPEndPoint.</returns>
        public IPEndPoint SelectServerEndPoint()
        {
            Console.WriteLine("Select the server you want to connect to:");

            IPAddress serverIPAddress = this.SelectServerIPAddress();
            int serverPort = this.SelectServerPort();

            IPEndPoint serverEndpoint;

            Console.WriteLine("\nTrying to connect to server... This might take a while...");

            return new IPEndPoint(serverIPAddress, serverPort);
        }

        /// <summary>
        /// Prompts the user to select an IP Address.
        /// </summary>
        /// <returns>Returns an IPAddress.</returns>
        public IPAddress SelectServerIPAddress()
        {
            IPAddress serverIPAddress = null;
            bool parseSuccess;

            do
            {
                Console.Write("Enter a valid IP-Address: ");

                string input = Console.ReadLine();
                parseSuccess = IPAddress.TryParse(input, out serverIPAddress);
            } 
            while (!parseSuccess);

            return serverIPAddress;
        }

        /// <summary>
        /// Prompts the user to select a port.
        /// </summary>
        /// <returns>Returns a port number.</returns>
        public int SelectServerPort()
        {
            int port = 0;
            bool parseSuccess;

            do
            {
                Console.Write("Enter a valid Port: ");
                string input = Console.ReadLine();
                parseSuccess = int.TryParse(input, out port);
            } 
            while (!parseSuccess || port < 1 || port > 65535);

            return port;
        }
    }
}
