
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

        /// <summary>
        /// NOTE: This is a bad implementation of this as we statistically get more points on the outside of the circle.
        /// </summary>
        public static Vector3 insideUnitSphere
        {
            get
            {
                Vector3 v = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                if (v.sqrMagnitude > 1.0f)
                {
                    v.Normalize();
                }
                return v;
            }
        }

        /// <summary>
        /// NOTE: This is a bad implementation of this as we statistically get more points on the outside of the circle.
        /// </summary>
        public static Vector2 insideUnitCircle
        {
            get
            {
                Vector2 v = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                if (v.sqrMagnitude > 1.0f)
                {
                    v.Normalize();
                }
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
