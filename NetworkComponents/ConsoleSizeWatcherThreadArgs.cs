//-----------------------------------------------------------------------
// <copyright file="ConsoleSizeWatcherThreadArgs.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the chat client logic.</summary>
//-----------------------------------------------------------------------
namespace NetworkComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The ConsoleSizeWatcherThreadArgs class.
    /// </summary>
    public class ConsoleSizeWatcherThreadArgs
    {
        /// <summary>
        /// The flag of exit.
        /// </summary>
        private bool exit;

        /// <summary>
        /// Initializes a new instance of the ConsoleSizeWatcherThreadArgs class.
        /// </summary>
        public ConsoleSizeWatcherThreadArgs()
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
