using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class UnityAudioListener : Behaviour
    {
        private static float _volume;
        public static float volume
        {
            get { return _volume; }
            set { _volume = Mathf.Clamp01(value); }
        }

        private static bool _pause;
        public static bool pause
        {
            get { return _pause; }
            set { _pause = value; }
        }

        public static AudioVelocityUpdateMode velocityUpdateMode = AudioVelocityUpdateMode.Auto;
        
        public static float[] GetOutputData(int numSamples, int channel)
        {
            throw new NotImplementedException("GetOutputData is not implemented!");
        }

        public static void GetOutputData(float[] samples, int channel)
        {
            throw new NotImplementedException("GetOutputData is not implemented!");
        }

        // TODO:
        //public static float[] GetSpectrumData(int numSamples, int channel, FFTWindow window)
        //public static void GetSpectrumData(float[] samples, int channel, FFTWindow window)
    }
}
