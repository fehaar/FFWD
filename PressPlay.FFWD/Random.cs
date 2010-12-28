
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

        public static Vector3 insideUnitSphere
        {
            get
            {
                float remain = 1;
                Vector3 v = new Vector3(value, 0, 0);
                remain -= v.x;
                v.y = value * remain;
                v.z = remain - v.y;
                return v;
            }
        }

        public static Vector2 insideUnitCircle
        {
            get
            {
                Vector2 v = new Vector2(value, 0);
                v.y = 1 - v.x;
                return v;
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
