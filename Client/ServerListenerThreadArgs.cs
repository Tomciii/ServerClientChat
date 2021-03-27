//-----------------------------------------------------------------------
// <copyright file="ServerListenerThreadArgs.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the chat client logic.</summary>
//-----------------------------------------------------------------------
namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The ServerListenerThreadArgs class.
    /// </summary>
    public class ServerListenerThreadArgs
    {
        /// <summary>
        /// The flag of exit.
        /// </summary>
        private bool exit;

        /// <summary>
        /// Initializes a new instance of the ServerListenerThreadArgs class.
        /// </summary>
        public ServerListenerThreadArgs()
        {
            this.exit = false;
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
