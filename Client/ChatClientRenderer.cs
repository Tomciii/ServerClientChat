//-----------------------------------------------------------------------
// <copyright file="ChatClientRenderer.cs" company="FH WN">
//     Copyright (c) Thomas Horvath. All rights reserved.
// </copyright>
// <summary>This file contains the chatclient renderer logic.</summary>
//-----------------------------------------------------------------------
namespace Client
{
    using System;
    using System.Net.Sockets;
    using NetworkComponents;

    /// <summary>
    /// The chat client renderer class.
    /// </summary>
    public class ChatClientRenderer
    {
        /// <summary>
        /// The width of the console window.
        /// </summary>
        private int consoleWindowWidth;

        /// <summary>
        /// The height of the console window.
        /// </summary>
        private int consoleWindowHeight;

        /// <summary>
        /// The width of the chat client.
        /// </summary>
        private int clientWidth;

        /// <summary>
        /// The height of the chat client.
        /// </summary>
        private int clientHeight;

        /// <summary>
        /// The width of the textbox for received messages.
        /// </summary>
        private int textBoxWidth;

        /// <summary>
        /// The height of the textbox for received messages.
        /// </summary>
        private int textBoxHeight;

        /// <summary>
        /// A line adjustment offset for precise rendering.
        /// </summary>
        private int lineAdjustmentOffset;

        /// <summary>
        /// The char that is used for bold vertical lines.
        /// </summary>
        private char boldVerticalLine;

        /// <summary>
        /// The char that is used for bold horizontal lines.
        /// </summary>
        private char boldHorizontalLine;

        /// <summary>
        /// The char that is used for light vertical lines.
        /// </summary>
        private char lightVerticalLine;

        /// <summary>
        /// The char that is used for light horizontal lines.
        /// </summary>
        private char lightHorizontalLine;

        /// <summary>
        /// The char that is used for the top left corner line.
        /// </summary>
        private char topLeftCornerLine;

        /// <summary>
        /// The char that is used for the top right corner line.
        /// </summary>
        private char topRightCornerLine;

        /// <summary>
        /// The char that is used for the bottom left corner line.
        /// </summary>
        private char bottomLeftCornerLine;

        /// <summary>
        /// The char that is used for the bottom right corner line.
        /// </summary>
        private char bottomRightCornerLine;

        /// <summary>
        /// The X coordinate for the input field.
        /// </summary>
        private int inputX;
        
        /// <summary>
        /// The Y coordinate for the input field.
        /// </summary>
        private int inputY;

        /// <summary>
        /// The X coordinate for the server end point field.
        /// </summary>
        private int serverEndPointX;

        /// <summary>
        /// The Y coordinate for the server end point field.
        /// </summary>
        private int serverEndPointY;

        /// <summary>
        /// The X coordinate for the server state field.
        /// </summary>
        private int serverStateX;

        /// <summary>
        /// The Y coordinate for the server state field.
        /// </summary>
        private int serverStateY;

        /// <summary>
        /// The X coordinate for the clients list field.
        /// </summary>
        private int clientsListX;

        /// <summary>
        /// The Y coordinate for the clients list field.
        /// </summary>
        private int clientsListY;

        /// <summary>
        /// Initializes a new instance of the ChatClientRenderer class.
        /// </summary>
        public ChatClientRenderer()
        {
            this.clientWidth = 130;
            this.clientHeight = 33;

            this.consoleWindowHeight = this.clientHeight + 10;
            this.consoleWindowWidth = this.clientWidth + 20;

            this.lineAdjustmentOffset = 3;

            this.textBoxWidth = (this.clientWidth / 2) + 20;
            this.textBoxHeight = this.clientHeight - 4;

            this.inputX = 4;
            this.inputY = this.clientHeight;

            this.serverEndPointX = this.textBoxWidth + 13;
            this.serverEndPointY = this.clientHeight - 3;

            this.serverStateX = this.textBoxWidth + 13;
            this.serverStateY = this.clientHeight - 2;

            this.clientsListX = this.textBoxWidth + 4;
            this.clientsListY = 4;

            this.boldHorizontalLine = '═';
            this.boldVerticalLine = '║';
            this.lightHorizontalLine = '─';
            this.lightVerticalLine = '│';

            this.topRightCornerLine = '╗';
            this.topLeftCornerLine = '╔';
            this.bottomLeftCornerLine = '╚';
            this.bottomRightCornerLine = '╝';
        }

        /// <summary>
        /// Gets the value of clientHeight.
        /// </summary>
        /// <value>The value of clientHeight.</value>
        public int ClientHeight
        {
            get
            {
                return this.clientHeight;
            }
        }

        /// <summary>
        /// Gets the value of clientWidth.
        /// </summary>
        /// <value>The value of clientWidth.</value>
        public int ClientWidth
        {
            get
            {
                return this.clientWidth;
            }
        }

        /// <summary>
        /// Gets the value of serverEndPointY.
        /// </summary>
        /// <value>The value of serverEndPointY.</value>
        public int ServerEndPointY
        {
            get
            {
                return this.serverEndPointY;
            }
        }

        /// <summary>
        /// Gets the value of serverEndPointX.
        /// </summary>
        /// <value>The value of serverEndPointX.</value>
        public int ServerEndPointX
        {
            get
            {
                return this.serverEndPointX;
            }
        }

        /// <summary>
        /// Gets the value of clientsListY.
        /// </summary>
        /// <value>The value of clientsListY.</value>
        public int ClientsListY
        {
            get
            {
                return this.clientsListY;
            }
        }

        /// <summary>
        /// Gets the value of clientsListX.
        /// </summary>
        /// <value>The value of clientsListX.</value>
        public int ClientsListX
        {
            get
            {
                return this.clientsListX;
            }
        }

        /// <summary>
        /// Gets the value of serverStateY.
        /// </summary>
        /// <value>The value of serverStateY.</value>
        public int ServerStateY
        {
            get
            {
                return this.serverStateY;
            }
        }

        /// <summary>
        /// Gets the value of serverStateX.
        /// </summary>
        /// <value>The value of serverStateX.</value>
        public int ServerStateX
        {
            get
            {
                return this.serverStateX;
            }
        }

        /// <summary>
        /// Gets the value of textBoxWith.
        /// </summary>
        /// <value>The value of textboxWidth.</value>
        public int TextBoxWidth
        {
            get
            {
                return this.textBoxWidth;
            }
        }

        /// <summary>
        /// Gets the value of textBoxHeight.
        /// </summary>
        /// <value>The value of textBoxHeight.</value>
        public int TextBoxHeight
        {
            get
            {
                return this.textBoxHeight;
            }
        }

        /// <summary>
        /// Gets the value of inputX.
        /// </summary>
        /// <value>The value of inputX.</value>
        public int InputX
        {
            get
            {
                return this.inputX;
            }
        }

        /// <summary>
        /// Gets the value of inputY.
        /// </summary>
        /// <value>The value of inputY.</value>
        public int InputY
        {
            get
            {
                return this.inputY;
            }
        }

        /// <summary>
        /// Renders the GUI.
        /// </summary>
        public void RenderGUI()
        {
            try 
            { 
            Console.Clear();
            }
            catch
            {
                Console.SetBufferSize(this.consoleWindowWidth - this.lineAdjustmentOffset, this.consoleWindowHeight - this.lineAdjustmentOffset);
                Console.SetWindowSize(this.consoleWindowWidth - this.lineAdjustmentOffset, this.consoleWindowHeight - this.lineAdjustmentOffset);
            }

            this.RenderOuterGUI();
            this.RenderInteriorGUI();
        }

        /// <summary>
        /// Displays the Server info onto the screen.
        /// </summary>
        /// <param name="client">Takes a client as input.</param>
        public void DisplayServerInfo(object client)
        {
            ChatClient chatClient = (ChatClient)client;
            TcpClient tcpClient = chatClient.TCPClient;

            string serverEndPoint = tcpClient.Client.RemoteEndPoint.ToString();

                Console.SetCursorPosition(this.serverEndPointX, this.serverEndPointY);
                Console.Write($"{serverEndPoint}");

                for (int i = 0; i < 12; i++)
                {
                Console.SetCursorPosition(this.serverStateX + i, this.serverStateY);
                Console.Write(" ");
                }
           
            Console.SetCursorPosition(this.serverStateX, this.serverStateY);
              
            if (tcpClient.Connected)
                {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Connected");
                }
            else
                {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Disconnected");
                }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Displays online clients onto the screen.
        /// </summary>
        /// <param name="clientList">Takes a chat client manager as input.</param>
        public void DisplayOnlineClients(ChatClientManager clientList)
        {
            for (int i = this.clientsListX; i < this.clientWidth + 1; i++)
            {
                for (int j = this.clientsListY; j < this.clientHeight - 6; j++)
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write(" ");
                }
            }

            for (int i = 0; i < clientList.ClientsIDs.Count; i++)
            {
                int offset = 0;

                if (clientList.ClientsIDs.Count > 23)
                {
                    offset = clientList.ClientsIDs.Count - 23;
                }

                if (i < 23)
                {
                    Console.SetCursorPosition(this.clientsListX, this.clientsListY + i);
                    Console.Write($"{clientList.ClientIPEndPoints[i + offset]}\t{"|"} ");

                    Console.ForegroundColor = (ConsoleColor)clientList.ClientConsoleColors[i + offset];

                    Console.Write($"{clientList.ClientsNickNames[i + offset]}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            Console.SetCursorPosition(4, this.clientHeight);
        }

        /// <summary>
        /// Clears the input of any characters.
        /// </summary>
        public void ClearInputBox()
        {
            for (int i = 0; i < this.clientWidth - 3; i++)
            {
                Console.SetCursorPosition(4 + i, this.clientHeight);
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, this.clientHeight);
            Console.Write(" ");
        }

        /// <summary>
        /// Deletes the message history on the screen.
        /// </summary>
        /// <param name="clientsManager">Takes a client manager as input.</param>
        public void DeleteMessageHistory(ChatClientManager clientsManager)
        {
            Console.CursorVisible = false;
            for (int i = 0; i < this.textBoxWidth - 1; i++)
            {
                for (int j = 0; j < this.textBoxHeight - 1; j++)
                {
                    try
                    {
                        Console.SetCursorPosition(3 + i, 4 + j);
                        Console.Write(" ");
                    }
                    catch
                    {
                        this.RenderGUI();
                        this.DisplayOnlineClients(clientsManager);
                        this.DisplayServerInfo(this);
                    }
                }
            }

            Console.SetCursorPosition(4, this.clientHeight);
        }

        /// <summary>
        /// Renders the interior part of the GUI.
        /// </summary>
        private void RenderInteriorGUI()
        {
            this.RenderChatBoxHeader();
            this.RenderInputBox();
            this.RenderServerInfoBox();
            this.RenderClientsBox();
        }

        /// <summary>
        /// Renders the server info box onto the screen.
        /// </summary>
        private void RenderServerInfoBox()
        {
            Console.SetCursorPosition(this.textBoxWidth + 3, this.clientHeight - 5);
            Console.Write("Server");

            Console.SetCursorPosition(this.textBoxWidth + 3, this.clientHeight - 4);
            this.RenderHorizontalLine(6);

            Console.SetCursorPosition(this.textBoxWidth + 3, this.clientHeight - 3);
            Console.Write("EndPoint: ");

            Console.SetCursorPosition(this.textBoxWidth + 6, this.clientHeight - 2);
            Console.Write("State: ");

            Console.SetCursorPosition(this.textBoxWidth + 2, this.clientHeight - 6);
            this.RenderHorizontalLine(this.clientWidth - this.textBoxWidth - 1);
        }

        /// <summary>
        /// Renders the online clients box.
        /// </summary>
        private void RenderClientsBox()
        {
            this.RenderVerticalLine(this.clientHeight - 1);

            Console.SetCursorPosition(this.textBoxWidth + 3, 2);
            Console.Write("Clients");

            Console.SetCursorPosition(this.textBoxWidth + 3, 3);
            this.RenderHorizontalLine(7);
        }

        /// <summary>
        /// Renders the input box.
        /// </summary>
        private void RenderInputBox()
        {
            Console.SetCursorPosition(2, this.clientHeight);
            Console.Write(">");
            Console.SetCursorPosition(2, this.clientHeight - 1);
            this.RenderHorizontalLine(this.clientWidth);
        }

        /// <summary>
        /// Renders the chat box header.
        /// </summary>
        private void RenderChatBoxHeader()
        {
            Console.SetCursorPosition(3, 2);
            Console.Write("Messages");

            Console.SetCursorPosition(2, 3);
            this.RenderHorizontalLine(this.textBoxWidth);
        }

        /// <summary>
        /// Renders the outer GUI.
        /// </summary>
        private void RenderOuterGUI()
        {
            try 
            { 
            Console.SetWindowSize(this.consoleWindowWidth - this.lineAdjustmentOffset, this.consoleWindowHeight - this.lineAdjustmentOffset);
            Console.SetBufferSize(this.consoleWindowWidth - this.lineAdjustmentOffset, this.consoleWindowHeight - this.lineAdjustmentOffset);
            }
            catch
            {
            }

            this.RenderTopOuterline();
            this.RenderLeftOuterLine();
            this.RenderRightOuterLine();
            this.RenderBottomOuterline();
        }

        /// <summary>
        /// Renders the left outer line of the GUI.
        /// </summary>
        private void RenderLeftOuterLine()
        {
            for (int i = 0; i < this.clientHeight; i++)
            {
                Console.SetCursorPosition(1, 2 + i);
                Console.Write(this.boldVerticalLine);
            }
        }

        /// <summary>
        /// Renders a horizontal line.
        /// </summary>
        /// <param name="length">Takes a length as input.</param>
        private void RenderHorizontalLine(int length)
        {
            for (int i = 0; i < length; i++)
            {
                Console.Write(this.lightHorizontalLine);
            }
        }

        /// <summary>
        /// Renders a vertical line.
        /// </summary>
        /// <param name="length">Takes a length as input.</param>
        private void RenderVerticalLine(int length)
        {
            for (int i = 0; i < length - 2; i++)
            {
                Console.SetCursorPosition(this.textBoxWidth + 1, 2 + i);
                Console.Write(this.lightVerticalLine);
            }
        }

        /// <summary>
        /// Renders the right outer line of the GUI.
        /// </summary>
        private void RenderRightOuterLine()
        {
            for (int i = 0; i < this.clientHeight; i++)
            {
                Console.SetCursorPosition(this.clientWidth + 2, 2 + i);
                Console.Write(this.boldVerticalLine);
            }
        }

        /// <summary>
        /// Renders the top outer line of the GUI.
        /// </summary>
        private void RenderTopOuterline()
        {
            Console.SetCursorPosition(1, 1);
            Console.Write(this.topLeftCornerLine);

            for (int i = 0; i < this.clientWidth; i++)
            {
                Console.Write(this.boldHorizontalLine);
            }

            Console.Write(this.topRightCornerLine);
        }

        /// <summary>
        /// Renders the bottom outer line of the GUI.
        /// </summary>
        private void RenderBottomOuterline()
        {
            Console.SetCursorPosition(1, this.clientHeight + 1);
            Console.Write(this.bottomLeftCornerLine);

            for (int i = 0; i < this.clientWidth; i++)
            {
                Console.Write(this.boldHorizontalLine);
            }

            Console.Write(this.bottomRightCornerLine);
        }
    }
}