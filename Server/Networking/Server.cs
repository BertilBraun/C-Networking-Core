﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }

        public static Dictionary<Guid, ClientConnection> clients = new Dictionary<Guid, ClientConnection>();

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        /// <summary>Starts the server.</summary>
        /// <param name="_maxPlayers">The maximum players that can be connected simultaneously.</param>
        /// <param name="_port">The port to start the server on.</param>
        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on port {Port}.");
        }

        /// <summary>Handles new TCP connections.</summary>
        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            if (clients.Count < MaxPlayers)
            {
                Guid i = Guid.NewGuid();
                clients.Add(i, new ClientConnection(i));

                clients[i].tcp.Connect(_client);
                return;
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        /// <summary>Receives incoming UDP data.</summary>
        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    Guid _clientId = _packet.ReadGuid();

                    if (_clientId == Guid.Empty)
                    {
                        return;
                    }

                    if (clients[_clientId].udp.endPoint == null)
                    {
                        // If this is a new connection
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        // Ensures that the client is not being impersonated by another by sending a false clientID
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
        /// <param name="_clientEndPoint">The endpoint to send the packet to.</param>
        /// <param name="_packet">The packet to send.</param>
        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }

        public static void Stop()
        {
            tcpListener.Stop();
            udpListener.Close();
        }
    }
}