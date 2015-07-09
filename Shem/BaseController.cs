﻿using System.Threading;
using Shem.Commands;
using Shem.Sockets;
using Shem.Replies;
using System.Collections.ObjectModel;

namespace Shem
{
    /// <summary>
    /// This class is the first interface to the control port.
    /// If you wanna do things in the right way use the 'TorController'
    /// class instead.
    /// </summary>
    public class BaseController
    {

        protected ControlSocket controlSocket;

        protected int sleep = 10;

        protected uint responseTimeout = 1000;

        /// <summary>
        /// The time the library should wait for a reply.
        /// </summary>
        public uint ResponseTimeout
        {
            get { return responseTimeout; }
            set { responseTimeout = value; }
        }
        /// <summary>
        /// Is the Controller connected to the server.
        /// </summary>
        public bool Connected
        {
            get { return controlSocket.Connected;  }
        }


        /// <summary>
        /// Construct a new TorController, used to control TOR
        /// </summary>
        /// <param name="address">The address where the ControlPort is (usually localhost)</param>
        /// <param name="port">The port where TOR has binded the ControlPort</param>
        /// <param name="connect">If the controller should connect just after the initialization</param>
        public BaseController(string address = "127.0.0.1", uint port = 9051, bool connect = true)
        {
            controlSocket = new ControlSocket(address, port, connect);
        }

        /// <summary>
        /// Send a command and returns the reply as a raw string
        /// </summary>
        /// <param name="command">The command to be sent</param>
        /// <returns>Returns the raw string replied by the server</returns>
        public string SendRawCommand(TCCommand command)
        {
            //Send the command
            controlSocket.Send(command.Raw());
            //Wait for response
            int timeout = (int)ResponseTimeout / sleep;
            int i = 1;
            while (!controlSocket.ResponseAvailable && i < timeout)
            {
                Thread.Sleep(sleep);
                i++;
            }
            //Read Response
            return controlSocket.Receive();
        }

        public Collection<Reply> SendCommand(TCCommand command)
        {
            return ReplyParser.Parse(SendRawCommand(command));
        }

        /// <summary>
        /// Connect to the control port.
        /// </summary>
        public void Connect()
        {
            controlSocket.Connect();
        }

        /// <summary>
        /// Close the socket connection.
        /// Is a good idea to call it before your program exits.
        /// </summary>  
        public virtual void Close()
        {
            SendRawCommand(new Quit());
            controlSocket.Close();
        }

        ~BaseController()
        {
            if(Connected)
                Close();
        }
    }
}
