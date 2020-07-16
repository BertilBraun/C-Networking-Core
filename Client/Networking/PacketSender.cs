using System;

namespace Client
{
    /// <summary>Sent from client to server.</summary>
    public enum ClientPackets
    {
        welcomeReceived = 1,
        playerInput,
        sendChat
    }

    public class PacketSender
    {
        #region Packets
        /// <summary>Lets the server know that the welcome message was received.</summary>
        public static void WelcomeReceived()
        {
            using (Packet _packet = new Packet(ClientPackets.welcomeReceived))
            {
                _packet.Write(Client.Get.myId);
                _packet.Write(Program.username);

                SendTCPData(_packet);
            }
        }

        /// <summary>Sends player input to the server.</summary>
        /// <param name="_inputs"></param>
        public static void PlayerInput(bool[] inputs)
        {
            using (Packet _packet = new Packet(ClientPackets.playerInput))
            {
                _packet.Write(inputs.Length);
                foreach (bool _input in inputs)
                    _packet.Write(_input);

                _packet.Write(GameManager.Get.LocalPlayer.rotation);

                SendUDPData(_packet);
            }
        }

        /// <summary>Sends player input to the server.</summary>
        /// <param name="_inputs"></param>
        public static void SendChat(char c)
        {
            using (Packet _packet = new Packet(ClientPackets.sendChat))
            {
                _packet.Write(c);
                SendTCPData(_packet);
            }
        }

        #endregion

        #region Functionality

        /// <summary>Sends a packet to the server via TCP.</summary>
        /// <param name="_packet">The packet to send to the sever.</param>
        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.Get.tcp.SendData(_packet);
        }

        /// <summary>Sends a packet to the server via UDP.</summary>
        /// <param name="_packet">The packet to send to the sever.</param>
        private static void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.Get.udp.SendData(_packet);
        }

        #endregion
    }
}