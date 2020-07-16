using System;
using System.Collections.Generic;
using System.Numerics;

namespace Server
{
    /// <summary>Sent from client to server.</summary>
    public enum ClientPackets
    {
        welcomeReceived = 1,
        playerInput,
        sendChat
    }

    public class PacketHandler
    {
        public readonly static Dictionary<ClientPackets, Action<Guid, Packet>> PacketHandlers = new Dictionary<ClientPackets, Action<Guid, Packet>>
        {
            { ClientPackets.welcomeReceived, WelcomeReceived },
            { ClientPackets.playerInput, PlayerInput },
            { ClientPackets.sendChat, SendChat },
        };

        public static void WelcomeReceived(Guid _fromClient, Packet _packet)
        {
            Guid _clientIdCheck = _packet.ReadGuid();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            Server.clients[_fromClient].SendIntoGame(_username);
        }

        public static void PlayerInput(Guid _fromClient, Packet _packet)
        {
            bool[] _inputs = new bool[_packet.ReadInt()];
            for (int i = 0; i < _inputs.Length; i++)
                _inputs[i] = _packet.ReadBool();

            Quaternion _rotation = _packet.ReadQuaternion();

            GameManager.Get.Players[_fromClient].HandleInput(_inputs, _rotation);
        }

        public static void SendChat(Guid _fromClient, Packet _packet)
        {
            Console.Write((char)_packet.ReadByte());
        }
    }
}