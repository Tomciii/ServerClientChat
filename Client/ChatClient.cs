//-----------------------------------------------------------------------
// <copyright file="ChatClient.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the chat client logic.</summary>
//-----------------------------------------------------------------------
namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using NetworkComponents;

    /// <summary>
    /// The chat client class.
    /// </summary>
    public class ChatClient
    {
        /// <summary>
        /// The client of the ChatClient.
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// The ChatClientRenderer of the ChatClient.
        /// </summary>
        private ChatClientRenderer renderer;

        /// <summary>
        /// The connection helper of the ChatClient. Helps with user input for connecting to a server.
        /// </summary>
        private ConnectionEstablisher connectionEstablisher;

        /// <summary>
        /// The lock that helps with multithreading and properly displaying messages onto the console.
        /// </summary>
        private object consoleLock;

        /// <summary>
        /// The ID of this ChatClient.
        /// </summary>
        private int id;

        /// <summary>
        /// The nickName of this ChatClient.
        /// </summary>
        private string nickName;

        /// <summary>
        /// The thread that listens to messages from a server.
        /// </summary>
        private Thread serverListenerThread;

        /// <summary>
        /// The thread that watches over the size of the console.
        /// </summary>
        private Thread consoleSizeWatcherThread;

        /// <summary>
        /// The clientsManager that keeps track of online clients.
        /// </summary>
        private ChatClientManager clientsManager;

        /// <summary>
        /// A list containing the received messages from the server.
        /// </summary>
        private List<string> receivedMessages;

        /// <summary>
        /// A list containing the color information of each received messages.
        /// </summary>
        private List<int> receivedMessagesColors;
      
        /// <summary>
        /// The IPEndPoint of this ChatClient's local client.
        /// </summary>
        private IPEndPoint clientEndPoint;

        /// <summary>
        /// The IPEndpoint of the remote server.
        /// </summary>
        private IPEndPoint serverEndPoint;

        /// <summary>
        /// The console color for messages by the user.
        /// </summary>
        private ConsoleColor userColor;

        /// <summary>
        /// The default color for messages.
        /// </summary>
        private ConsoleColor defaultColor;

        /// <summary>
        /// The console color for server messages.
        /// </summary>
        private ConsoleColor serverColor;

        /// <summary>
        /// Initializes a new instance of the ChatClient class.
        /// </summary>
        /// <param name="client">Takes a client as input.</param>
        public ChatClient(TcpClient client)
        {
            this.renderer = new ChatClientRenderer();
            this.consoleLock = new object();
            this.defaultColor = ConsoleColor.Gray;
            this.serverColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = this.defaultColor;
            this.userColor = this.defaultColor;

            this.clientsManager = new ChatClientManager();
            this.receivedMessages = new List<string>();
            this.receivedMessagesColors = new List<int>();
            
            this.tcpClient = client;
            this.clientEndPoint = (IPEndPoint)this.tcpClient.Client.LocalEndPoint;
            
            this.connectionEstablisher = new ConnectionEstablisher();
        }

        /// <summary>
        /// Gets or sets the value of userColor.
        /// </summary>
        /// <value>The value of userColor.</value>
        public ConsoleColor UserColor
        {
            get
            {
                return this.userColor;
            }

            set
            {
                this.userColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of nickName.
        /// </summary>
        /// <value>The value of nickname.</value>
        public string NickName
        {
            get
            {
                return this.nickName;
            }

            set
            {
                this.nickName = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of id.
        /// </summary>
        /// <value>The value of id.</value>
        public int ID
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// Gets the value of client.
        /// </summary>
        /// <value>The value of the client.</value>
        public TcpClient TCPClient
        {
            get
            {
                return this.tcpClient;
            }
        }

        /// <summary>
        /// Gets the value of NetworkStream.
        /// </summary>
        /// <value>The value of the Network stream.</value>
        public NetworkStream NetworkStream
        {
            get
            {
                return this.tcpClient.GetStream();
            }
        }

        /// <summary>
        /// Gets the value of this IPEndPoint.
        /// </summary>
        /// <value>The value of the clientEndPoint.</value>
        public IPEndPoint ClientEndPoint
        {
            get
            {
                return this.clientEndPoint;
            }
        }

        /// <summary>
        /// Starts the chat client.
        /// </summary>
        public void Start()
        {
            Console.WriteLine("Selected to start a new client!\n");
            this.ConnectToServer();
            this.renderer.RenderGUI();
            this.renderer.DisplayServerInfo(this);
            this.CommunicateToServer();
        }

        /// <summary>
        /// This method gets added to the OnClientWroteMessage event invocation List.
        /// </summary>
        /// <param name="sender">The publisher of the event.</param>
        /// <param name="e">The event args e.</param>
        public void OnClientWroteMessage(object sender, OnMessageToClientEventArgs e)
        {
            if (this.tcpClient.Connected)
            {
                e.SendClientMessage(this.tcpClient);
            }
        }

        /// <summary>
        /// This method gets added to the OnClientConnected event invocation List.
        /// </summary>
        /// <param name="sender">The publisher of the event.</param>
        /// <param name="e">The event args e.</param>
        public void OnClientConnected(object sender, OnClientConnectedEventArgs e)
        {
            if (this.tcpClient.Connected)
            {
                e.SendNickName();
                e.SendWelcomeMessage();
                e.SendOnlineClients();
                Console.WriteLine($"{DateTime.Now} Welcome Message sent to user with ID {e.ID}.");
                Console.WriteLine($"{DateTime.Now} Sent all users the updated client list.");
            }
        }

        /// <summary>
        /// Watches over the size of the console.
        /// </summary>
        /// <param name="data">Takes an object as a thread arguments input.</param>
        private void WatchConsoleSize(object data)
        {
            ConsoleSizeWatcherThreadArgs args = (ConsoleSizeWatcherThreadArgs)data;

            while (args.Exit != true)
            {
                Thread.Sleep(100);
                if (Console.WindowWidth < 147 || Console.WindowHeight < this.renderer.ClientHeight)
                {
                    Thread.Sleep(100);
                    lock (this.consoleLock) 
                    {
                        try 
                        { 
                            Console.Clear();
                            Console.SetWindowSize(this.renderer.ClientWidth, this.renderer.ClientHeight);
                            this.renderer.RenderGUI();
                            this.renderer.DisplayServerInfo(this);
                            this.renderer.DisplayOnlineClients(this.clientsManager);
                            this.DisplayMessageHistory();
                            Console.SetCursorPosition(6, this.renderer.ClientHeight);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets up a new Thread for listening to messages from the server.
        /// </summary>
        private void StartServerListenerThread()
        {
            ServerListenerThreadArgs args = new ServerListenerThreadArgs();
            args.Exit = false;

            this.serverListenerThread = new Thread(this.ReadMessagesFromServer);
            this.serverListenerThread.Name = "serverListenerThread";
            this.serverListenerThread.Start(args);
        }

        /// <summary>
        /// Sets up a new console size watcher thread.
        /// </summary>
        private void StartConsoleSizeWatcherThread()
        {
            ConsoleSizeWatcherThreadArgs args = new ConsoleSizeWatcherThreadArgs();
            args.Exit = false;

            this.consoleSizeWatcherThread = new Thread(this.WatchConsoleSize);
            this.consoleSizeWatcherThread.Name = "clientConsoleSizeWatcherThread";
            this.consoleSizeWatcherThread.Start(args);
        }

        /// <summary>
        /// Sets up the ServerListener and ConsoleSizeWatcher Threads for the client and sends the welcome message to the client.
        /// </summary>
        private void InitializeClient()
        {
            try
            {
                Console.SetCursorPosition(3, 4);
                this.ReadWelcomeMessage();
            }
            catch
            {
                this.renderer.DisplayServerInfo(this);
            }

            if (this.serverListenerThread == null)
            {
                this.StartServerListenerThread();
            }

            if (this.consoleSizeWatcherThread == null)
            {
                this.StartConsoleSizeWatcherThread();
            }
        }

        /// <summary>
        /// Handles the write requests to the connected server as well as the read requests from the server.
        /// </summary>
        private void CommunicateToServer()
        {
            bool isConnected = true;

            this.InitializeClient();

            while (isConnected)
            {
                try
                {
                    this.renderer.DisplayOnlineClients(this.clientsManager);
                    string message = string.Empty;

                    while (!message.Equals("get out"))
                    {
                        message = this.WriteMessage();
                        this.renderer.ClearInputBox();

                        if (message.Length < 256)
                        {
                            this.SendMessage(message);
                        }
                    }

                    this.Quit();
                    isConnected = false;
                }
                catch (InvalidOperationException e)
                {
                    Console.Beep(500, 500);
                    this.renderer.DisplayServerInfo(this);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (this.tcpClient.Connected)
                    {
                        this.RefreshGUI();
                    }
                }
                catch (Exception e)
                {
                    Console.Beep(500, 500);
                    this.renderer.DisplayServerInfo(this);
                }
            }
        }

        /// <summary>
        /// Refreshes the GUI.
        /// </summary>
        private void RefreshGUI()
        {
                this.renderer.RenderGUI();
                this.renderer.DisplayServerInfo(this);
                this.renderer.DeleteMessageHistory(this.clientsManager);
                this.DisplayMessageHistory();
                Console.SetCursorPosition(6, this.renderer.ClientHeight);
        }

        /// <summary>
        /// Sends a message through the network stream.
        /// </summary>
        /// <param name="text">Takes a string as input.</param>
        private void SendMessage(string text)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(text);
            this.NetworkStream.Write(sendBuffer, 0, sendBuffer.Length);
        }

        /// <summary>
        /// Handles incoming messages from the server.
        /// </summary>
        /// <param name="data">Takes an object as thread arguments input.</param>
        private void ReadMessagesFromServer(object data)
        {
            ServerListenerThreadArgs args = (ServerListenerThreadArgs)data;
            
            while (args.Exit != true)
            {
               string messageReceived = string.Empty;

                if (this.tcpClient.Connected) 
                {
                    try 
                    { 
                        byte[] readBuffer = new byte[8000];
                        int messageLength = this.NetworkStream.Read(readBuffer, 0, readBuffer.Length);
                        messageReceived = Encoding.UTF8.GetString(readBuffer, 0, messageLength);
                    }
                    catch
                    {
                    }
                }

                lock (this.consoleLock)
                {
                    Thread.Sleep(75);
                    Console.CursorVisible = false;
                   
                    //// Polling for updates to the client list.
                    if (messageReceived.StartsWith("UpdatingClientList:#"))
                        {
                            this.UpdateClientList(messageReceived);
                        }
                    else if (messageReceived.Length > 0 && messageReceived.Length < 2500)
                    {
                        if (this.receivedMessages.Count >= this.renderer.TextBoxHeight - 1)
                        {
                             this.renderer.DeleteMessageHistory(this.clientsManager);
                        }

                    //// Checking if we can add the message or if we need to split it up.
                        if (this.receivedMessages.Count < this.renderer.TextBoxHeight - 1) 
                        { 
                            if (messageReceived.Length < this.renderer.TextBoxWidth - 1)
                            {
                                    this.AddMessageColor(messageReceived);
                                    this.receivedMessages.Add(messageReceived);  
                            }
                        else
                            {
                           ////Splitting the message from the server into a string array if the server message is too big to properly display on the GUI.
                           string[] splitMessage = messageReceived.Split(' ');
                           string result = string.Empty;

                                ////Going through each word of the received message to look at it's length.
                                for (int i = 0; i < splitMessage.Length; i++)
                                    {
                                           if (result.Length + splitMessage[i].Length + 1 < this.renderer.TextBoxWidth - 1)
                                            {
                                                result += splitMessage[i] + " ";
                                            }
                                            else
                                            {
                                                //// Checking if a word itself needs to be split to be properly displayed.
                                                if (splitMessage[i].Length + 1 > 30)
                                                {
                                                    //// Going through each character of a word if it is too long to be properly displayed.
                                                    for (int l = 0; l < splitMessage[i].Length; l++)
                                                    {
                                                        if (result.Length + 2 < this.renderer.TextBoxWidth - 1)
                                                        {
                                                            result += splitMessage[i][l];

                                                            if (l == splitMessage[i].Length - 1)
                                                            {
                                                                result += " ";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //// If the "result" string becomes too long, add it to the received messages list.
                                                            this.AddMessageColor(messageReceived);
                                                            this.receivedMessages.Add(result);
                                                            result = string.Empty;
                                                            result += splitMessage[i][l];
                                                        }
                                                    }
                                                }
                                                else 
                                                {
                                                    ////No need to split the word but to make a new line.
                                                    this.AddMessageColor(messageReceived);
                                                    this.receivedMessages.Add(result);
                                                    result = string.Empty;
                                                    result += splitMessage[i] + " ";
                                                } 
                                            }
                                        } 
                                    
                                    //// Adding the received message into the received messages list.
                                    this.AddMessageColor(messageReceived);
                                    this.receivedMessages.Add(result);
                            }
                        }

                        while (this.receivedMessages.Count >= this.renderer.TextBoxHeight - 1)
                        {
                            this.receivedMessages.RemoveAt(0);
                            this.receivedMessagesColors.RemoveAt(0);
                        }

                        Thread.Sleep(55);
                        this.DisplayMessageHistory();
                        this.CleanUpMessageScreen();
                        Console.SetCursorPosition(4, this.renderer.ClientHeight);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a color to a received message.
        /// </summary>
        /// <param name="messageReceived">Takes a string as input.</param>
        private void AddMessageColor(string messageReceived)
        {
            if (messageReceived.Split(' ')[2].Equals("Server"))
            {
                this.receivedMessagesColors.Add(7);
            }
            else
            {
                int index = this.clientsManager.ClientsNickNames.IndexOf(messageReceived.Split(' ')[2]);
                this.receivedMessagesColors.Add(this.clientsManager.ClientConsoleColors.ElementAt(index));
            }
        }

        /// <summary>
        /// Updates the list of online clients.
        /// </summary>
        /// <param name="messageReceived">Takes a message as input.</param>
        private void UpdateClientList(string messageReceived)
        {
            this.clientsManager.ClearLists();

            string[] clientListInfo = messageReceived.Split('#');
            for (int i = 0; i < int.Parse(clientListInfo[1]); i++)
            {
                this.clientsManager.ClientsIDs.Add(int.Parse(clientListInfo[2 + (4 * i)]));
                this.clientsManager.ClientIPEndPoints.Add(clientListInfo[3 + (4 * i)]);
                this.clientsManager.ClientsNickNames.Add(clientListInfo[4 + (4 * i)]);
                this.clientsManager.ClientConsoleColors.Add(int.Parse(clientListInfo[5 + (4 * i)]));
            }

            this.renderer.DisplayOnlineClients(this.clientsManager);
        }

        /// <summary>
        /// Reads the welcome message from the server.
        /// </summary>
        private void ReadWelcomeMessage()
        {
            byte[] readBuffer = new byte[8000];
            int messageLength = this.NetworkStream.Read(readBuffer, 0, readBuffer.Length);
            string messageReceived = Encoding.UTF8.GetString(readBuffer, 0, messageLength);
            this.WriteWelcomeMessage(messageReceived);
        }

        /// <summary>
        /// Cleans the screen from any unwanted characters.
        /// </summary>
        private void CleanUpMessageScreen()
        {
            for (int i = 0; i < this.receivedMessages.Count; i++)
            {
                for (int j = this.receivedMessages.ElementAt(i).Length; j < this.renderer.TextBoxWidth - 2; j++)
                {
                    try 
                    { 
                  Console.SetCursorPosition(3 + j, i + 4);
                  Console.Write(" ");
                    }
                    catch
                    {
                        this.renderer.RenderGUI();
                        this.renderer.DisplayServerInfo(this);
                        this.renderer.DisplayOnlineClients(this.clientsManager);
                        this.DisplayMessageHistory();
                    }
                }
            }
        }

        /// <summary>
        /// Displays the message history onto the screen.
        /// </summary>
        private void DisplayMessageHistory()
        {
            for (int i = 0; i < this.receivedMessages.Count; i++)
            {
                string[] message = this.receivedMessages.ElementAt(i).Split(' ');
              
                //// Polling for server welcome messages.
                if (this.receivedMessages.ElementAt(i).Equals("Welcome to the ChatServer!") || this.receivedMessages.ElementAt(i).Equals($"Your current nickname is \"{this.NickName}\"") || this.receivedMessages.ElementAt(i).Equals("Your current color is \"grey\"") || this.receivedMessages.ElementAt(i).Equals("Type \"nickname YOUR_NEW_NICKNAME\" to change your nickname") || this.receivedMessages.ElementAt(i).Equals("Type \"color YOUR_NEW_COLOR\" to change your color."))
                {
                    Console.ForegroundColor = this.serverColor;
                    Console.SetCursorPosition(3, 4 + i);
                    Console.Write(this.receivedMessages.ElementAt(i));
                    Console.ForegroundColor = this.defaultColor;
                }
                else if (message.Length > 3) 
                { 
                    //// Polling for who wrote the message. Adds the user's console color to the message.
                            if (message[2].Equals("Server") && message[3].Equals("says:"))
                            {
                                Console.ForegroundColor = this.defaultColor;
                                Console.SetCursorPosition(3, 4 + i);     
                                Console.Write(message[0] + " " + message[1] + " " + message[2] + " " + message[3]);
                                Console.ForegroundColor = this.serverColor;
                                Console.Write(this.receivedMessages.ElementAt(i).Substring(32));
                                Console.ForegroundColor = this.defaultColor;
                            }
                            else
                            {
                                for (int k = 0; k < this.clientsManager.ClientsNickNames.Count; k++)
                                {
                                    if (message[2].Equals(this.clientsManager.ClientsNickNames.ElementAt(k)) && message[3].Equals("says:"))
                                    {
                                        //// Polling for which user wrote the message.
                                        Console.ForegroundColor = this.defaultColor;
                                        Console.SetCursorPosition(3, 4 + i);
                                        Console.Write(message[0] + " " + message[1] + " " + message[2] + " " + message[3] + " ");
                                        Console.ForegroundColor = (ConsoleColor)this.receivedMessagesColors.ElementAt(i);
                                        Console.Write(this.receivedMessages.ElementAt(i).Substring(27 + this.clientsManager.ClientsNickNames.ElementAt(k).Length));
                                        Console.ForegroundColor = this.defaultColor;
                                        break;
                                    }
                                    else
                                    {
                                    //// Polling for longer messages.
                                    if (message[3].Equals("says:"))
                                    {
                                        Console.ForegroundColor = this.defaultColor;
                                        Console.SetCursorPosition(3, 4 + i);
                                        Console.Write(message[0] + " " + message[1] + " " + message[2] + " " + message[3] + " ");
                                        Console.ForegroundColor = (ConsoleColor)this.receivedMessagesColors.ElementAt(i);
                                    
                                       //// Write the message if the username has been changed.
                                        for (int o = 4; o < message.Length; o++)
                                        {
                                            if (o == message.Length - 1)
                                            {
                                                Console.Write(message[o]);
                                            }
                                            else
                                            {
                                                Console.Write(message[o] + " ");
                                            }
                                        }

                                        Console.ForegroundColor = this.defaultColor;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = (ConsoleColor)this.receivedMessagesColors.ElementAt(i);
                                        Console.SetCursorPosition(3, 4 + i);
                                        Console.Write(this.receivedMessages.ElementAt(i));
                                    }
                                }
                             }
                      }
                }
                else
                {
                    Console.ForegroundColor = (ConsoleColor)this.receivedMessagesColors.ElementAt(i);
                    Console.SetCursorPosition(3, 4 + i);
                    Console.Write(this.receivedMessages.ElementAt(i));
                }
            }

            Console.ForegroundColor = this.defaultColor;
            Console.SetCursorPosition(4, this.renderer.ClientHeight);
        }

        /// <summary>
        /// Writes a welcome message onto the screen updates online clients list.
        /// </summary>
        /// <param name="messageReceived">Takes the received message from the server as input for the welcome message.</param>
        private void WriteWelcomeMessage(string messageReceived)
        {
            ////Protocol for welcome message handling and updating client lists is making use of unique beginnings of the string message being sent from the server as well as using '#' for seperating the individual data.
            string[] welcomeMessageArgs = messageReceived.Split('#');
            this.nickName = welcomeMessageArgs[1];

            for (int i = 2; i < 8; i++)
            {
                if (i == 3)
                {
                    Console.ForegroundColor = this.serverColor;
                }

                Console.SetCursorPosition(3, 4 + this.receivedMessages.Count);
                Console.Write(welcomeMessageArgs[i]);
                this.receivedMessages.Add(welcomeMessageArgs[i]);
               
                if (i == 2 || i == 4 || i == 7)
                {
                    Console.SetCursorPosition(3, 4 + this.receivedMessages.Count);
                    Console.Write(" ");
                    this.receivedMessages.Add(" ");
                }
            }

            Console.ForegroundColor = this.defaultColor;
            this.receivedMessagesColors.Add(7);
            this.receivedMessagesColors.Add(0);
            this.receivedMessagesColors.Add(10);
            this.receivedMessagesColors.Add(10);
            this.receivedMessagesColors.Add(0);
            this.receivedMessagesColors.Add(10);
            this.receivedMessagesColors.Add(10);
            this.receivedMessagesColors.Add(10);
            this.receivedMessagesColors.Add(0);
           
            //// Adding all chat clients that are online to the local onlineclients list.
            for (int i = 0; i < int.Parse(welcomeMessageArgs[9]); i++)
                 {
                    this.clientsManager.ClientsIDs.Add(int.Parse(welcomeMessageArgs[10 + (4 * i)]));
                    this.clientsManager.ClientIPEndPoints.Add(welcomeMessageArgs[11 + (4 * i)]);
                    this.clientsManager.ClientsNickNames.Add(welcomeMessageArgs[12 + (4 * i)]);
                    this.clientsManager.ClientConsoleColors.Add(int.Parse(welcomeMessageArgs[13 + (4 * i)]));
                 }
            }

        /// <summary>
        /// Allows the user to write a message.
        /// </summary>
        /// <returns>Returns a message.</returns>
        private string WriteMessage()
        {
            string result = string.Empty;

            if (result.Length < this.renderer.ClientWidth - 5)
            {
                Console.SetCursorPosition(result.Length + 4, this.renderer.ClientHeight);
            }
            else
            {
                Console.SetCursorPosition(this.renderer.ClientWidth - 4, this.renderer.ClientHeight);
            }

            while (true)
            {
                Console.CursorVisible = true;
                ConsoleKeyInfo key = Console.ReadKey();

                lock (this.consoleLock)
                {
                    if (key.Key == ConsoleKey.Enter)
                    {
                        //// Handles the Enter Key being pressed.
                        if (result != string.Empty && result.Length > 0)
                        {
                            Thread.Sleep(15);
                            return result;
                        }
                    }
                    else if (key.Key == ConsoleKey.Backspace && result.Length > 0)
                    {
                        //// Handles the Backspace Key being pressed.
                        if (result.Length > this.renderer.ClientWidth - 5)
                        {
                            result = result.Substring(0, result.Length - 1);
                           
                            for (int i = 0; i < this.renderer.ClientWidth - 5; i++)
                            {
                                Console.SetCursorPosition(i + 4, this.renderer.ClientHeight);
                                Console.Write(result[result.Length - this.renderer.ClientWidth + 5 + i]);
                            }

                            Console.SetCursorPosition(this.renderer.ClientWidth - 1, this.renderer.ClientHeight);
                            Console.Write(" ");
                            Console.SetCursorPosition(this.renderer.ClientWidth - 2, this.renderer.ClientHeight);
                        }
                        else
                        {
                            Console.Write(" ");
                            result = result.Substring(0, result.Length - 1);
                            Console.SetCursorPosition(result.Length + 4, this.renderer.ClientHeight);
                        }
                    }
                    else
                    {
                        if (result.Length > this.renderer.ClientWidth - 5)
                        {
                            if (key.Key == ConsoleKey.Escape)
                            {
                                Console.SetCursorPosition(this.renderer.ClientWidth, this.renderer.ClientHeight);
                                Console.Write(" ");
                            }

                                for (int i = 0; i < this.renderer.ClientWidth - 5; i++)
                                {
                                    Console.SetCursorPosition(i + 4, this.renderer.ClientHeight);
                                    Console.Write(result[result.Length - this.renderer.ClientWidth + 5 + i]);
                                }

                            Console.SetCursorPosition(this.renderer.ClientWidth, this.renderer.ClientHeight);
                            Console.Write(" ");
                            Console.SetCursorPosition(this.renderer.ClientWidth - 1, this.renderer.ClientHeight);
                        }
                       
                        //// As long as the key pressed is not an arrow key or the escape key, add the KeyChar of this key to the result string.
                        if (key.Key != ConsoleKey.Escape && key.Key != ConsoleKey.LeftArrow && key.Key != ConsoleKey.UpArrow && key.Key != ConsoleKey.DownArrow && key.Key != ConsoleKey.RightArrow)
                        {
                            result += key.KeyChar.ToString();
                        }

                        if (key.Key == ConsoleKey.Escape && result.Length < this.renderer.ClientWidth)
                        {
                            Console.SetCursorPosition(result.Length + 4, this.renderer.ClientHeight);
                            Console.Write(" ");
                            Console.SetCursorPosition(result.Length + 4, this.renderer.ClientHeight);
                        }

                        if ((key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.RightArrow) && result.Length < this.renderer.ClientWidth)
                        {
                            Console.SetCursorPosition(result.Length + 4, this.renderer.ClientHeight);
                        }

                        if (Console.CursorLeft < 4)
                        {
                            Console.CursorLeft = 4;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Quits the client.
        /// </summary>
        private void Quit()
        {
            this.tcpClient.Close();
            Environment.Exit(0);
        }

        /// <summary>
        /// Connects the client to a server.
        /// </summary>
        private void ConnectToServer()
        {
            bool isConnected = false;

            do
            {
                try
                {
                    this.serverEndPoint = this.connectionEstablisher.SelectServerEndPoint();
                    this.tcpClient.Connect(this.serverEndPoint);
                    isConnected = true;
                    Console.Clear();
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to connect! Please try again...\n");
                    Console.ForegroundColor = this.defaultColor;
                }
            }
            while (!isConnected);
        }
    }
}