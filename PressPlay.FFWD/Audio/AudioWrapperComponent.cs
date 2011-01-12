using System;
using System.Collections.Generic;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public class AudioWrapperComponent : MonoBehaviour
    {
        public AudioWrapper sound;

        // Use this for initialization
        public override void Start()
        {
            sound.PlaySound();
        }

        void OnTurnOffAtDistance()
        {
            sound.Stop();
        }

        void OnTurnOnAtDistance()
        {
            sound.PlaySound();
        }       
    }
}
