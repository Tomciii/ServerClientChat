//-----------------------------------------------------------------------
// <copyright file="ChatClientManager.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the chat client manager logic.</summary>
//-----------------------------------------------------------------------
namespace NetworkComponents
{
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// This class keeps track of online clients.
    /// </summary>
    public class ChatClientManager
    {
        /// <summary>
        /// Keeps track of the id's of online clients.
        /// </summary>
        private List<int> clientsIDs;

        /// <summary>
        /// Keeps track of the nicknames of online clients.
        /// </summary>
        private List<string> clientsNicknames;

        /// <summary>
        /// Keeps track of the IPEndPoints of online clients.
        /// </summary>
        private List<string> clientsIPEndPoints;

        /// <summary>
        /// Keeps track of the console colors of online clients.
        /// </summary>
        private List<int> clientsConsoleColors;

        /// <summary>
        /// Initializes a new instance of the ChatClientManager class.
        /// </summary>
        public ChatClientManager()
        {
            this.clientsIDs = new List<int>();
            this.clientsNicknames = new List<string>();
            this.clientsIPEndPoints = new List<string>();
            this.clientsConsoleColors = new List<int>();
        }

        /// <summary>
        /// Gets or sets the value of clientsConsoleColors.
        /// </summary>
        /// <value>The value of clientsConsoleColors.</value>
        public List<int> ClientConsoleColors
        {
            get
            {
                return this.clientsConsoleColors;
            }

            set
            {
                this.clientsConsoleColors = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of clientsIDs.
        /// </summary>
        /// <value>The value of clientsIDs.</value>
        public List<int> ClientsIDs
        {
            get
            {
                return this.clientsIDs;
            }

            set
            {
                this.clientsIDs = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of clientsNicknames.
        /// </summary>
        /// <value>The value of clientsNicknames.</value>
        public List<string> ClientsNickNames
        {
            get
            {
                return this.clientsNicknames;
            }

            set
            {
                this.clientsNicknames = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of clientsIPEndPoints.
        /// </summary>
        /// <value>The value of clientsIPEndPoints.</value>
        public List<string> ClientIPEndPoints
        {
            get
            {
                return this.clientsIPEndPoints;
            }

            set
            {
                this.clientsIPEndPoints = value;
            }
        }

        /// <summary>
        /// Adds client data to the lists of the class.
        /// </summary>
        /// <param name="id">Takes an id as input.</param>
        /// <param name="name">Takes a string as input.</param>
        /// <param name="ip">Takes an IPEndPoint as input.</param>
        public void AddClientData(int id, string name, IPEndPoint ip)
        {
            this.clientsIDs.Add(id);
            this.clientsNicknames.Add(name.ToString());
            this.clientsIPEndPoints.Add(ip.ToString());
            this.clientsConsoleColors.Add(7);
        }

        /// <summary>
        /// Removes client data at the index.
        /// </summary>
        /// <param name="index">Takes an index as input.</param>
        public void RemoveAtIndex(int index)
        {
            this.clientsIDs.RemoveAt(index);
            this.clientsNicknames.RemoveAt(index);
            this.clientsIPEndPoints.RemoveAt(index);
            this.clientsConsoleColors.RemoveAt(index);
        }
  
        /// <summary>
        /// Clears the lists of the manager.
        /// </summary>
        public void ClearLists()
        {
            this.ClientsIDs.Clear();
            this.clientsConsoleColors.Clear();
            this.ClientIPEndPoints.Clear();
            this.ClientsNickNames.Clear();
        }
    }
}