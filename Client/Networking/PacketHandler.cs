using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;

namespace Client
{
    /// <summary>Sent from server to client.</summary>
    public enum ServerPackets
    {
        welcome = 1,
        spawnPlayer,
        playerTransform,
    }

    public class PacketHandler
    {
        public readonly static Dictionary<ServerPackets, Action<Packet>> PacketHandlers = new Dictionary<ServerPackets, Action<Packet>>
        {
            { ServerPackets.welcome, Welcome },
            { ServerPackets.spawnPlayer, SpawnPlayer },
            { ServerPackets.playerTransform, PlayerTransform }
        };

        public static void Welcome(Packet _packet)
        {
            string _msg = _packet.ReadString();
            Guid _myId = _packet.ReadGuid();

            Console.WriteLine($"Message from server: {_msg}");
            Client.Get.myId = _myId;
            PacketSender.WelcomeReceived();

            // Now that we have the client's id, connect UDP
            Client.Get.udp.Connect(((IPEndPoint)Client.Get.tcp.socket.Client.LocalEndPoint).Port);
        }

        public static void SpawnPlayer(Packet _packet)
        {
            Guid _id = _packet.ReadGuid();
            string _username = _packet.ReadString();
            Vector3 _position = _packet.ReadVector3();
            Quaternion _rotation = _packet.ReadQuaternion();

            PlayerHandler player = new PlayerHandler(_id, _username);
            player.rotation = _rotation;
            player.position = _position;
            GameManager.Get.AddPlayer(_id, player);
        }

        public static void PlayerTransform(Packet _packet)
        {
            Guid _id = _packet.ReadGuid();
            Vector3 _position = _packet.ReadVector3();
            Quaternion _rotation = _packet.ReadQuaternion();

            PlayerHandler player = GameManager.Get.Players[_id];

            if (player != GameManager.Get.LocalPlayer)
                player.rotation = _rotation;
            player.position = _position;
        }

        public static void PlayerDisconnected(Packet _packet)
        {
            Guid _id = _packet.ReadGuid();

            GameManager.Get.Players.Remove(_id);
        }
    }
}