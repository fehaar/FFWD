using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PressPlay.FFWD
{
    public class AudioSource : Behaviour
    {
        private AudioClip _clip;
        public AudioClip clip
        {
            get
            {
                return _clip;
            }
            set
            {
                _clip = value;
            }
        }

        private float _minVolume = 0;
        public float minVolume
        {
            get { return _minVolume; }
            set { _minVolume = Mathf.Clamp01(value); }
        }

        private float _maxVolume = 1;
        public float maxVolume
        {
            get { return _maxVolume; }
            set { _maxVolume = Mathf.Clamp01(value); }
        }

        private float _volume = 0;
        public float volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = Mathf.Clamp01(value);
                _volume = Mathf.Max(_volume, _minVolume);
                _volume = Mathf.Min(_volume, minVolume);

                if (soundEffect != null)
                {
                    soundEffect.Volume = _volume;
                }
            }
        }
        public float pitch = 0;
        public bool loop = false; 

        private SoundEffectInstance soundEffect;

        private void SetSoundEffect(SoundEffectInstance sfx)
        {
            soundEffect = sfx;
            soundEffect.IsLooped = loop;
            soundEffect.Volume = volume;
        }
    }
}
