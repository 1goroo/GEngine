using Microsoft.Xna.Framework;
using System;

namespace GEngine.Core
{
    public static class EngineTime
    {
        public static TimeSpan TotalTime { get; private set; }
        public static TimeSpan DeltaTime { get; private set; }
        internal static void Update(GameTime time)
        {
            TotalTime = time.TotalGameTime;
            DeltaTime = time.ElapsedGameTime;
        }
    }
}
