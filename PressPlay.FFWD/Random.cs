using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public static class Random
    {
        private static System.Random _rnd;
        private static System.Random rnd
        {
            get
            {
                if (_rnd == null)
                {
                    _rnd = new System.Random();
                }
                return _rnd;
            }
        }

        private static int _seed;
        public static int seed 
        { 
            get
            {
                return _seed;
            }
            set
            {
                _seed = value;
                _rnd = new System.Random(_seed);
            }
        }

        public static float value
        {
            get
            {
                return (float)rnd.NextDouble();
            }
        }

        public static int Range(int min, int max)
        {
            return rnd.Next(min, max);
        }

        public static float Range(float min, float max)
        {
            return (float)rnd.NextDouble() * (max - min) + min;
        }
    }
}
