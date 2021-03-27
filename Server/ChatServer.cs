//-----------------------------------------------------------------------
// <copyright file="ChatServer.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the chatserver logic.</summary>
//-----------------------------------------------------------------------
namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using Client;
    using NetworkComponents;

    /// <summary>
    /// The ChatServer class.
    /// </summary>
    public class ChatServer
    {
        /// <summary>
        /// The port of the server.
        /// </summary>
        private int port;

        /// <summary>
        /// The thread for the Listener.
        /// </summary>
        private Thread clientListenerThread;

        /// <summary>
        /// The thread that watches over keyboard input.
        /// </summary>
        private Thread keyboardWatcherThread;

        /// <summary>
        /// The thread that watches over the size of the console.
        /// </summary>
        private Thread consoleSizeWatcherThread;

        /// <summary>
        /// The TCP listener of the server that listens to incoming connection requests.
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// The list that adds all clients that connected to the server.
        /// </summary>
        private List<ChatClient> registeredChatClients;

        /// <summary>
        /// The connection helper of the server that helps to select a port.
        /// </summary>
        private ConnectionEstablisher connectionHelper;

        /// <summary>
        /// The clients manager that keeps track of the data of online clients.
        /// </summary>
        private ChatClientManager clientsManager;

        /// <summary>
        /// Initializes a new instance of the ChatServer class.
        /// </summary>
        public ChatServer()
        {
            this.clientsManager = new ChatClientManager();
            this.registeredChatClients = new List<ChatClient>();
            this.connectionHelper = new ConnectionEstablisher();
        }

        /// <summary>
        /// The OnClientWroteMessage event handles the broadcasting of messages to all clients.
        /// </summary>
        public event EventHandler<OnMessageToClientEventArgs> OnClientWroteMessage;

        /// <summary>
        /// The OnClientJoined event sends the new client a welcome message.
        /// </summary>
        public event EventHandler<OnClientConnectedEventArgs> OnClientConnected;

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            Console.Clear();
            this.StartListener();

            this.clientListenerThread = new Thread(this.ListenToConnectionRequests);
            this.clientListenerThread.Name = "ClientListenerThread";
            this.clientListenerThread.Start();

            this.StartConsoleSizeWatcherThread();
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <param name="chatClients">Takes a client as input.</param>
        public void Stop(List<ChatClient> chatClients)
        {
            string message = $"{DateTime.Now} Server says: Server went offline.";
            Console.WriteLine(message);

            this.BroadcastMessage(message);
            this.CloseAllConnections(chatClients);

            Environment.Exit(0);
        }

        /// <summary>
        /// Fires the OnMessageToClient event.
        /// </summary>
        /// <param name="chatClient">Takes a chatClient as input.</param>
        /// <param name="name">Takes a name as input.</param>
        /// <param name="message">Takes a message as input.</param>
        protected virtual void FireOnMessageToClient(ChatClient chatClient, string name, string message)
        {
            this.OnClientWroteMessage?.Invoke(this, new OnMessageToClientEventArgs(name, message));
        }

        /// <summary>
        /// Fires the OnClientJoined event.
        /// </summary>
        /// <param name="client">Takes a client as input.</param>
        /// <param name="manager">Takes a client manager as input.</param>
        protected virtual void FireOnClientJoined(TcpClient client, ChatClientManager manager)
        {
            this.OnClientConnected?.Invoke(this, new OnClientConnectedEventArgs(client, manager));
        }

        /// <summary>
        /// Sets up a new console size watcher thread.
        /// </summary>
        private void StartConsoleSizeWatcherThread()
        {
            this.consoleSizeWatcherThread = new Thread(this.WatchConsoleSize);
            this.consoleSizeWatcherThread.Name = "serverConsoleSizeWatcherThread";
            this.consoleSizeWatcherThread.Start();
        }

        /// <summary>
        /// Resizes the console if the size becomes too small.
        /// </summary>
        private void WatchConsoleSize()
        {
            while (true)
            {
                Thread.Sleep(100);
                if (Console.WindowWidth < 100 || Console.WindowHeight < 20)
                {
                    try 
                    {
                        Console.SetWindowSize(140, 40);
                    }
                    catch
                    {
                        Console.SetWindowSize(140, 40);
                    }
                    }
            }
        }

        /// <summary>
        /// Starts the TCPListener.
        /// </summary>
        private void StartListener()
        {
            Console.WriteLine("Selected to start a new server!");
            bool isPortAvailable = false;

            do
            {
                try
                {
                    this.port = this.connectionHelper.SelectServerPort();
                    this.tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, this.port));
                    this.tcpListener.Start();

                    isPortAvailable = true;
                    Console.WriteLine($"\n{DateTime.Now} Starting server on port {this.port}");
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nPort already taken! Please use another port.");
                }
            } 
            while (!isPortAvailable);
        }
      
        /// <summary>
        /// Listens to incoming connection requests.
        /// </summary>
        private void ListenToConnectionRequests()
        {
            this.StartKeyboardWatcherThread();

            Console.WriteLine("...Listening for clients to connect...\n");
            while (true)
            {
                this.AcceptClient();
            }
        }

        /// <summary>
        /// Starts the KeyboardWatcher Thread.
        /// </summary>
        private void StartKeyboardWatcherThread()
        {
            this.keyboardWatcherThread = new Thread(this.WatchKeyboard);
            this.keyboardWatcherThread.Name = "serverKeyboardWatcherThread";
            this.keyboardWatcherThread.Start();
        }

        /// <summary>
        /// Watches for keyboard input.
        /// </summary>
        private void WatchKeyboard()
        {
            ConsoleKey input;
            do
            {
                input = Console.ReadKey(true).Key;
            } 
            while (input != ConsoleKey.Escape);
           
            this.Stop(this.registeredChatClients);
        }

        /// <summary>
        /// Broadcasts a message to all connected clients.
        /// </summary>
        /// <param name="message">Takes a message as input.</param>
        private void BroadcastMessage(string message)
        {
            for (int i = 0; i < this.clientsManager.ClientsIDs.Count; i++)
            {
                try
                {
                    byte[] sendBuffer = Encoding.UTF8.GetBytes(message);
                    this.registeredChatClients.ElementAt(this.clientsManager.ClientsIDs.ElementAt(i)).TCPClient.GetStream().Write(sendBuffer, 0, sendBuffer.Length);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Closes all connections to clients.
        /// </summary>
        /// <param name="chatClients">Takes a list of clients as input.</param>
        private void CloseAllConnections(List<ChatClient> chatClients)
        {
            if (chatClients.Count > 0)
            {
                foreach (ChatClient client in chatClients)
                {
                    try
                    {
                        client.TCPClient.Close();
                    }
                    catch
                    {
                    }
                }

                this.tcpListener.Stop();
            }
        }

        /// <summary>
        /// Handles the incoming messages of a client.
        /// </summary>
        /// <param name="data">Takes a object as input.</param>
        private void HandleMessages(object data)
        {
            ClientThreadArguments args;
            ChatClient chatClient = null;

            try
            {
                args = (ClientThreadArguments)data;
                chatClient = args.ChatClient;

                byte[] receiveBuffer = new byte[8000];
                byte[] sendBuffer = new byte[8000];

                if (sendBuffer.Length < 255)
                {
                    chatClient.NetworkStream.Write(sendBuffer, 0, sendBuffer.Length);
                }

                string messageReceived = string.Empty;
                while (args.Exit != true)
                {
                    Thread.Sleep(40); 

                    int messageLength = chatClient.NetworkStream.Read(receiveBuffer, 0, receiveBuffer.Length);
                    messageReceived = Encoding.UTF8.GetString(receiveBuffer, 0, messageLength);

                    string[] receivedWords = messageReceived.Split(' ');

                    if (receivedWords[0].ToLower().Equals("color") && receivedWords.Length == 2)
                    {
                        this.HandleColorChangeRequest(chatClient, receivedWords);
                    }
                    else if (receivedWords[0].ToLower().Equals("nickname") && receivedWords.Length == 2)
                    {
                        this.HandleNameChangeRequest(chatClient, receivedWords, sendBuffer);
                    }
                    else
                    {
                        if (!messageReceived.Equals("get out"))
                        {
                            Console.WriteLine($"{DateTime.Now} Received message from user {this.registeredChatClients.IndexOf(chatClient)} with a length of {messageLength} bytes.");
                            this.FireOnMessageToClient(chatClient, chatClient.NickName, messageReceived);
                            Console.WriteLine($"{DateTime.Now} Sent message to all users."); 
                        }
                    }

                    if (messageReceived.Equals("get out"))
                    {
                        Console.WriteLine($"{DateTime.Now} Received \"get out\" request from user with ID {chatClient.ID}.");
                        args.Exit = true;
                        this.CloseClientConnection(chatClient);
                    }
                }
            }
            catch (Exception e)
            {
                if (chatClient != null)
                {
                    this.CloseClientConnection(chatClient);
                }
            }
        }

        /// <summary>
        /// Checks if a client name is already taken.
        /// </summary>
        /// <param name="name">Takes a name as input.</param>
        /// <returns>Returns true or false.</returns>
        private bool CheckIfNameTaken(string name)
        {
            if (name.ToLower().Equals("server"))
            {
                return true;
            }
            
            if (name.Contains('#'))
            {
                return true;
            }

            for (int i = 0; i < this.clientsManager.ClientsNickNames.Count; i++)
            {
                if (this.clientsManager.ClientsNickNames.ElementAt(i).Equals(name))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles an incoming name change request.
        /// </summary>
        /// <param name="chatClient">Takes a client as input.</param>
        /// <param name="receivedWords">Takes the message of the client as input.</param>
        /// <param name="sendBuffer">Takes a byte array as input.</param>
        private void HandleNameChangeRequest(ChatClient chatClient, string[] receivedWords, byte[] sendBuffer)
        {
            Console.WriteLine($"{DateTime.Now} Received nickname change request from user with ID {chatClient.ID}.");

            bool isNameTaken = this.CheckIfNameTaken(receivedWords[1]);
            
            if (receivedWords[1].Length < 18 && !isNameTaken)
            {
                Console.WriteLine($"{DateTime.Now} New nickname for user with ID {chatClient.ID} should be ok. Notifying users.");
                string newNickName = receivedWords[1];
                int index = this.clientsManager.ClientsIDs.IndexOf(chatClient.ID);

                string userChangedNameMessage = $"\"{chatClient.NickName}\" changed nickname to \"{newNickName}\".";
                this.FireOnMessageToClient(chatClient, "Server", userChangedNameMessage);

                this.clientsManager.ClientsNickNames[index] = newNickName;
                chatClient.NickName = newNickName;

                string message = this.CreateClientList();
                this.BroadcastMessage(message);
            }
            else
            {
                if (isNameTaken)
                {
                    sendBuffer = Encoding.UTF8.GetBytes($"{DateTime.Now} Server says: Please pick a unique and valid name.");
                }
                else
                {
                    sendBuffer = Encoding.UTF8.GetBytes($"{DateTime.Now} Server says: Please pick a name under 18 characters.");
                }
               
                chatClient.TCPClient.GetStream().Write(sendBuffer, 0, sendBuffer.Length);
                Console.WriteLine($"{DateTime.Now} Denied nickname change for user with ID {chatClient.ID}.");
            }
        }

        /// <summary>
        /// Handles am incoming color change request.
        /// </summary>
        /// <param name="chatClient">Takes a client as input.</param>
        /// <param name="receivedWords">Takes the message of the client as input.</param>
        private void HandleColorChangeRequest(ChatClient chatClient, string[] receivedWords)
        {
            Console.WriteLine($"{DateTime.Now} Received color change request from user with ID {chatClient.ID}.");

            if (receivedWords[1].ToLower() == "darkblue")
            {
                this.SetClientColor(chatClient, 1, "darkblue");
            }
            else if (receivedWords[1].ToLower() == "darkcyan")
            {
                this.SetClientColor(chatClient, 3, "darkcyan");
            }
            else if (receivedWords[1].ToLower() == "darkred")
            {
                this.SetClientColor(chatClient, 4, "darkred");
            }
            else if (receivedWords[1].ToLower() == "darkmagenta")
            {
                this.SetClientColor(chatClient, 5, "darkmagenta");
            }
            else if (receivedWords[1].ToLower() == "darkyellow")
            {
                this.SetClientColor(chatClient, 6, "darkyellow");
            }
            else if (receivedWords[1].ToLower() == "gray")
            {
                this.SetClientColor(chatClient, 7, "gray");
            }
            else if (receivedWords[1].ToLower() == "darkgray")
            {
                this.SetClientColor(chatClient, 8, "darkgray");
            }
            else if (receivedWords[1].ToLower() == "blue")
            {
                this.SetClientColor(chatClient, 9, "blue");
            }
            else if (receivedWords[1].ToLower() == "cyan")
            {
                this.SetClientColor(chatClient, 11, "cyan");
            }
            else if (receivedWords[1].ToLower() == "red")
            {
                this.SetClientColor(chatClient, 12, "red");
            }
            else if (receivedWords[1].ToLower() == "magenta")
            {
                this.SetClientColor(chatClient, 13, "magenta");
            }
            else if (receivedWords[1].ToLower() == "yellow")
            {
                this.SetClientColor(chatClient, 14, "yellow");
            }
            else if (receivedWords[1].ToLower() == "white")
            {
                this.SetClientColor(chatClient, 15, "white");
            }
            else
            {
                Console.WriteLine($"{DateTime.Now} Denied color change for user with ID {chatClient.ID}.");
                byte[] sendBuffer = Encoding.UTF8.GetBytes($"{DateTime.Now} Server says: Not a valid input for a color.");
                chatClient.TCPClient.GetStream().Write(sendBuffer, 0, sendBuffer.Length);
            }
        }

        /// <summary>
        /// Closes the connection to a client.
        /// </summary>
        /// <param name="chatClient">Takes a client as input.</param>
        private void CloseClientConnection(ChatClient chatClient)
        {
            int index = this.clientsManager.ClientsIDs.IndexOf(chatClient.ID);
            this.clientsManager.RemoveAtIndex(index);
            chatClient.TCPClient.Close();

            string message = "UpdatingClientList:#" + this.clientsManager.ClientsIDs.Count + "#";

            for (int i = 0; i < this.clientsManager.ClientsIDs.Count; i++)
            {
                message += this.clientsManager.ClientsIDs.ElementAt(i) + "#" + this.clientsManager.ClientIPEndPoints.ElementAt(i) + "#" + this.clientsManager.ClientsNickNames.ElementAt(i) + "#" + this.clientsManager.ClientConsoleColors.ElementAt(i) + '#';
            }

            Console.WriteLine($"{DateTime.Now} User with ID {chatClient.ID} was disconnected. Notifying users."); 
            
            for (int i = 0; i < this.clientsManager.ClientsIDs.Count; i++)
            {
                try 
                { 
                    byte[] sendBuffer = Encoding.UTF8.GetBytes($"{DateTime.Now} Server says: {chatClient.NickName} was disconnected.");
                    this.registeredChatClients.ElementAt(this.clientsManager.ClientsIDs.ElementAt(i)).TCPClient.GetStream().Write(sendBuffer, 0, sendBuffer.Length);
                    sendBuffer = Encoding.UTF8.GetBytes(message);
                    this.registeredChatClients.ElementAt(this.clientsManager.ClientsIDs.ElementAt(i)).TCPClient.GetStream().Write(sendBuffer, 0, sendBuffer.Length);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Sets the color of a client.
        /// </summary>
        /// <param name="chatClient">Takes a client as input.</param>
        /// <param name="requestedColor">Takes the console color Integer value as input.</param>
        /// <param name="colorName">Takes the color name as input.</param>
        private void SetClientColor(ChatClient chatClient, int requestedColor, string colorName)
        {
            Console.WriteLine($"{DateTime.Now} New color for user {chatClient.NickName} should be ok. Notifying users.");
            int newColor = requestedColor;

            int index = this.clientsManager.ClientsIDs.IndexOf(chatClient.ID);
            this.clientsManager.ClientConsoleColors[index] = newColor;
            chatClient.UserColor = (ConsoleColor)newColor;

            string message = $"\"{chatClient.NickName}\" changed color to \"{colorName}\".";
            this.FireOnMessageToClient(chatClient, "Server", message);

            string newClientList = this.CreateClientList();
            this.BroadcastMessage(newClientList);
        }

        /// <summary>
        /// Creates a list of all connected clients.
        /// </summary>
        /// <returns>Returns a string message.</returns>
        private string CreateClientList()
        { 
            ////Protocol for updating client lists is making use of unique beginnings of a string message as well as using '#' for seperating the individual data.
            string result = "UpdatingClientList:#" + this.clientsManager.ClientsIDs.Count + "#";

            for (int i = 0; i < this.clientsManager.ClientsIDs.Count; i++)
            {
                result += this.clientsManager.ClientsIDs.ElementAt(i) + "#" + this.clientsManager.ClientIPEndPoints.ElementAt(i) + "#" + this.clientsManager.ClientsNickNames.ElementAt(i) + "#" + this.clientsManager.ClientConsoleColors.ElementAt(i) + '#';
            }

            return result;
        }

        /// <summary>
        /// Accepts incoming client connection requests.
        /// </summary>
        private void AcceptClient()
        {
            TcpClient tcpClient = this.tcpListener.AcceptTcpClient();
            ChatClient chatClient = new ChatClient(tcpClient);
            this.AddClient(chatClient);

            string message = this.CreateClientList();
            this.BroadcastUserJoinedMessage(message, chatClient);
            this.StartClientThread(chatClient);
        }

        /// <summary>
        /// Broadcasts the user joined message to all connected clients.
        /// </summary>
        /// <param name="message">Takes a message as input.</param>
        /// <param name="chatClient">Takes a client as input.</param>
        private void BroadcastUserJoinedMessage(string message, ChatClient chatClient)
        {
            for (int i = 0; i < this.clientsManager.ClientsIDs.Count; i++)
            {
                try
                {
                    byte[] sendBuffer = Encoding.UTF8.GetBytes($"{DateTime.Now} Server says: User \"{chatClient.NickName}\" joined the server.");
                    this.registeredChatClients.ElementAt(this.clientsManager.ClientsIDs.ElementAt(i)).TCPClient.GetStream().Write(sendBuffer, 0, sendBuffer.Length);

                    sendBuffer = Encoding.UTF8.GetBytes(message);
                    this.registeredChatClients.ElementAt(this.clientsManager.ClientsIDs.ElementAt(i)).TCPClient.GetStream().Write(sendBuffer, 0, sendBuffer.Length);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Starts the thread for a new connected client.
        /// </summary>
        /// <param name="chatClient">Takes a client as input.</param>
        private void StartClientThread(ChatClient chatClient)
        {
            ClientThreadArguments threadArguments = new ClientThreadArguments(chatClient);
            threadArguments.Exit = false;

            Thread clientThread = new Thread(this.HandleMessages);
            clientThread.Name = $"ClientThread ID: {chatClient.ID}";
            clientThread.Start(threadArguments);
        }

        /// <summary>
        /// Adds a client to the list of online clients.
        /// </summary>
        /// <param name="chatClient">Takes a client as input.</param>
        private void AddClient(ChatClient chatClient)
        {
            chatClient.ID = this.registeredChatClients.Count;
            chatClient.NickName = this.CreateNickName(chatClient);
            chatClient.UserColor = (ConsoleColor)7;

            this.clientsManager.AddClientData(this.registeredChatClients.Count, chatClient.NickName, (IPEndPoint)chatClient.TCPClient.Client.RemoteEndPoint);
            this.registeredChatClients.Add(chatClient);

            Console.WriteLine($"{DateTime.Now} New client successfully connected to server! I'll give him the ID {chatClient.ID}");

               this.OnClientConnected = chatClient.OnClientConnected;
               this.OnClientWroteMessage += chatClient.OnClientWroteMessage;

            this.FireOnClientJoined(chatClient.TCPClient, this.clientsManager);
        }

       /// <summary>
       /// Gives a new client a unique nickname.
       /// </summary>
       /// <param name="chatClient">Takes the new chatClient as input.</param>
       /// <returns>Returns a unique nickname.</returns>
        private string CreateNickName(ChatClient chatClient)
        {
            if (this.clientsManager.ClientsNickNames.Contains(chatClient.ID.ToString()))
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (!this.clientsManager.ClientsNickNames.Contains(i.ToString()))
                    {
                        return i.ToString();
                    }
                }
            }

            return chatClient.ID.ToString();
        }
    }
}