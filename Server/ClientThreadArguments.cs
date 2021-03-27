//-----------------------------------------------------------------------
// <copyright file="ClientThreadArguments.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the server app logic.</summary>
//-----------------------------------------------------------------------
namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Client;

    /// <summary>
    /// The arguments for the thread that handles clients.
    /// </summary>
    public class ClientThreadArguments
    {
        /// <summary>
        /// The Boolean flag for exiting the client.
        /// </summary>
        private bool exit = false;

        /// <summary>
        /// The chatClient for this thread.
        /// </summary>
        private ChatClient chatClient;

        /// <summary>
        /// Initializes a new instance of the ClientThreadArguments class.
        /// </summary>
        /// <param name="chatClient">The chatClient for this thread.</param>
        public ClientThreadArguments(ChatClient chatClient)
        {
            this.chatClient = chatClient;
        }

        /// <summary>
        /// Gets the value of chatClient.
        /// </summary>
        /// <value>The value of chatClient.</value>
        public ChatClient ChatClient
        {
            get
            {
                return this.chatClient;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the check box is selected.
        /// </summary>
        /// <value>The value of exit.</value>
        public bool Exit
        {
            get
            {
                return this.exit;
            }

            set
            {
                this.exit = value;
            }
        }
    }
}
