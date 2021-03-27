//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the chat app logic.</summary>
//-----------------------------------------------------------------------
namespace ChatApp
{
    using System;
    using Client;
    using Server;

    /// <summary>
    /// The ChatApp program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main method starts the chat app.
        /// </summary>
        /// <param name="args">Input args.</param>
        public static void Main(string[] args)
        {
            Console.BufferHeight = 40;
            Console.WindowHeight = 40;

            string input = string.Empty;
            ConsoleColor defaultColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine(" ╔═════════════════════════════════╗");
            Console.WriteLine(" ║   Welcome to our Chatting App!  ║");
            Console.WriteLine(" ╚═════════════════════════════════╝");
            Console.WriteLine();
            Console.ForegroundColor = defaultColor;

            do
            {
                Console.Write("Enter ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\"client\" ");
                Console.ForegroundColor = defaultColor;
                Console.Write("or ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\"server\" ");
                Console.ForegroundColor = defaultColor;
                Console.WriteLine("to start a new client or server!");
                input = Console.ReadLine();
            } 
            while (!input.ToLower().Equals("client") && !input.ToLower().Equals("server"));
            
            Console.Clear();

            if (input.ToLower().Equals("client"))
            {
                ChatClientApp.Main();
            }
            else if (input.ToLower().Equals("server"))
            {
                ChatServerApp.Main();
            }
        }
    }
}