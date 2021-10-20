using System;
namespace Boggle.Helpers
{
    public static class RNG
    {
        private static Random _random = null;

        public static Random Random
        {
            get
            {
                if (_random == null) _random = new Random((int)DateTime.Now.Ticks);
                return _random;
            }
        }

        public static int Rand(int min, int max) { return Random.Next(min, max + 1); }
    }
}
