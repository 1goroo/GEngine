using System;

namespace GEngine.Core
{
    public static class EngineRandom
    {
        private static Random BaseRandom { get; set; }
        internal static void Initialize(Random random)
        {
            BaseRandom = random;
        }
        public static int Next() => BaseRandom.Next();
        public static int Next(int maxValue) => BaseRandom.Next(maxValue);
        public static int Next(int minValue, int maxValue) => BaseRandom.Next(minValue, maxValue);
        public static float Range() => (float)BaseRandom.NextDouble();
        public static float Range(float maxValue) => (float)(BaseRandom.NextDouble() * maxValue);
        public static float Range(float minValue, float maxValue) 
            => (float)(BaseRandom.NextDouble() * (maxValue - minValue) + minValue);
        public static T GetItem<T>(T[] array) => array[BaseRandom.Next(array.Length)];
    }
}
