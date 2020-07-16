using System;
using System.Collections.Generic;

namespace Client
{
    public class GameManager
    {
        public static GameManager Get;
        public Dictionary<Guid, PlayerHandler> Players { get; private set; }
        public PlayerHandler LocalPlayer { get { return Players[Client.Get.myId]; } }

        public static void Init()
        {
            Get = new GameManager();
        }
        private GameManager()
        {
            Players = new Dictionary<Guid, PlayerHandler>();
        }

        public void AddPlayer(Guid id, PlayerHandler player)
        {
            Players.Add(id, player);
        }

        public void Update()
        {
            if (Console.KeyAvailable)
                PacketSender.SendChat(Console.ReadKey().KeyChar);
            if (Client.Get.Connected() && Players.ContainsKey(Client.Get.myId))
                PacketSender.PlayerInput(new bool[] { true, false, false, false });
            ThreadManager.UpdateMain();
        }
    }
}
