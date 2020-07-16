using System;
using System.Numerics;

namespace Client
{
    public class PlayerHandler
    {
        public Guid id;
        public string username;

        public Vector3 position;
        public Quaternion rotation;

        public PlayerHandler(Guid id, string username)
        {
            this.id = id;
            this.username = username;
        }

        public void HandleInput(bool[] inputs, Quaternion rotation)
        {
            this.rotation = rotation;

            if (inputs[0])
                position.Y += 1;
            if (inputs[1])
                position.Y -= 1;
            if (inputs[2])
                position.X -= 1;
            if (inputs[3])
                position.X += 1;
        }
    }
}
