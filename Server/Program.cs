using System;
using System.Threading;

namespace Server
{
    class Program
    {
        private static bool isRunning = false;
        private const int TICKS_PER_SEC = 30; // How many ticks per second
        private const float MS_PER_TICK = 1000f / TICKS_PER_SEC; // How many milliseconds per tick

        static void Main(string[] args)
        {
            Console.Title = "Game Server";
            isRunning = true;
            GameManager.Init();

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(50, 26950);
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at { TICKS_PER_SEC} ticks per second.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    // If the time for the next loop is in the past, aka it's time to execute another tick
                    GameManager.Get.Update(); // Execute game logic

                    _nextLoop = _nextLoop.AddMilliseconds( MS_PER_TICK); // Calculate at what point in time the next tick should be executed

                    if (_nextLoop > DateTime.Now)
                    {
                        // If the execution time for the next tick is in the future, aka the server is NOT running behind
                        Thread.Sleep(_nextLoop - DateTime.Now); // Let the thread sleep until it's needed again.
                    }
                }
            }
        }
    }
}
