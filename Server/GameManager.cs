using System;
using System.Collections.Generic;

namespace Server
{
    public class GameManager
    {
        public static GameManager Get;
        public Dictionary<Guid, PlayerHandler> Players { get; private set; }

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
            foreach (PlayerHandler player in Players.Values)
                PacketSender.PlayerTransform(player);

            ThreadManager.UpdateMain();
        }
    }
}
