using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Components
{
    public class AudioSource : Component
    {
        public AudioClip clip;
        public float volume = 1;
        public float pitch = 1;
        public bool playOnAwake;

        public void PlayOneShot(AudioClip clip, float volumeScale)
        {

        }

        public void PlayOneShot(AudioClip clip)
        {

        }
    }
}
