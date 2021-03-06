﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using Shem.Utils;
using Shem.Exceptions;

namespace Shem.Sockets
{
    /// <summary>
    /// This class manage to communicate with TOR, low level way.
    /// </summary>
    public class ControlSocket
    {
        private Socket _socket;
        public IPAddress Address { get; private set; }
        public uint Port { get; private set; }
        public bool Connected
        {
            get
            {
                return !(_socket == null || !_socket.Connected || (_socket.Poll(1000, SelectMode.SelectRead) && (_socket.Available == 0)));
            }
        }
        public bool ResponseAvailable { get { return _socket.Available > 0; } }

        /// <summary>
        /// Create a ControlSocket instance from a specified control port.
        /// </summary>
        /// <param name="address">The IP address or the hostname to connect-to (E.G. 192.168.1.1, youporn.com).</param>
        /// <param name="port">The Port number where TOR binded his ControlPort (E.G. 666).</param>
        /// <param name="connect">Connect automatically after the initialization.</param>
        public ControlSocket(string address = "127.0.0.1", uint port = 9051, bool connect = true)
        {
            IPAddress _addr;

            if (!IPAddress.TryParse(address, out _addr))
            {
                IPAddress[] tmp = Dns.GetHostAddresses(address);
                if (tmp.Length < 1)
                    throw new ServerNotFoundException();
                _addr = tmp[0];
            }
            this.Address = _addr;
            this.Port = port;
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            if (connect)
                this.Connect();
        }

        /// <summary>
        /// Connect to the parameters specified before.
        /// </summary>
        public void Connect()
        {
            Logger.LogDebug(string.Format("Connecting to the server \"{0}:{1}\".", this.Address, this.Port));
            _socket.Connect(Address, (int)Port);
        }

        /// <summary>
        /// Close the current connection.
        /// </summary>
        public void Close()
        {
            if (Connected)
            {
                Logger.LogDebug(string.Format("Closing the connection to \"{0}:{1}\".", this.Address, this.Port));
                _socket.Close();
            }
        }

        /// <summary>
        /// Send a message to the TOR
        /// </summary>
        /// <param name="message">The message to send-to TOR.</param>
        public void Send(string message)
        {
            Logger.LogDebug(string.Format("Sent message to the server: \"{0}\".", message.Replace("\r\n", "\\r\\n")));
            _socket.Send(Encoding.ASCII.GetBytes(message));
        }

        public string Receive()
        {
            byte[] buffer = new byte[_socket.Available];
            string reply;
            _socket.Receive(buffer);
            reply = Encoding.ASCII.GetString(buffer);
            Logger.LogDebug(string.Format("Received a reply from the server: \"{0}\".", reply.Replace("\r\n", "\\r\\n")));
            return reply;
        }

        //private string Format(string message)
        //{
        //    //From control-spec section 2.2...
        //    //  Command = Keyword OptArguments CRLF / "+" Keyword OptArguments CRLF CmdData
        //    //  Keyword = 1*ALPHA
        //    //  OptArguments = [ SP *(SP / VCHAR) ]

        //    //A command is either a single line containing a Keyword and arguments, or a
        //    //multiline command whose initial keyword begins with +, and whose data
        //    //section ends with a single "." on a line of its own.

        //    //if we already have \r\n entries then standardize on \n to start with

        //    message = message.Replace("\r\n", "\n");

        //    if (message.Contains("\n"))
        //        return "+" + message.Replace("\n", "\r\n") + "\r\n.\r\n";
        //    else
        //        return message + "\r\n";
        //}

        ~ControlSocket()
        {
            if (Connected)
                this.Close();
        }
    }
}
