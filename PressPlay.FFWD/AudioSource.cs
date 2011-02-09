using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD
{

    public enum AudioVelocityUpdateMode
    {
        Auto,
        Fixed,
        Dynamic
    }

    public class AudioSource : Behaviour, IUpdateable
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
                SetSoundEffect(_clip);
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

        private float _volume = 1;
        public float volume
        {
            get
            {
                return _volume;
            }
            set
            {
                if (soundEffect != null && soundEffect.IsDisposed)
                {
                    soundEffect = null;
                }

                _volume = Mathf.Clamp01(value);
                _volume = Mathf.Max(_volume, minVolume);
                _volume = Mathf.Min(_volume, maxVolume);


                if (soundEffect != null)
                {
                    soundEffect.Volume = _volume;
                }
            }
        }

        public bool isPlaying
        {
            get
            {
                if (soundEffect == null) { return false; }

                if (soundEffect.IsDisposed) { return false; }

                return (soundEffect.State == SoundState.Playing);
            }
        }

        public float pitch = 0;
        private bool _loop = false;
        public bool loop
        {
            get
            {
                return _loop;
            }
            set
            {
                _loop = value;
                if (soundEffect != null)
                {
                    soundEffect.IsLooped = _loop;
                }
            }
        }

        public bool ignoreListenerVolume = false;
        public bool playOnAwake = false;
        public float time = 0;
        public AudioVelocityUpdateMode velocityUpdateMode = AudioVelocityUpdateMode.Auto;

        private SoundEffectInstance soundEffect;

        private void SetSoundEffect(AudioClip sfx)
        {
            if (sfx == null || sfx.sound == null)
            {
                soundEffect = null;
            }
            else
            {
                soundEffect = sfx.sound.CreateInstance();
                soundEffect.IsLooped = loop;
                soundEffect.Volume = volume;
                time = 0;
            }
        }

        public void Play()
        {
            if (soundEffect == null)
            { 
                return; 
            }

            soundEffect.Play();
        }

        public void PlayOneShot(AudioClip clip, float volumeScale)
        {
            throw new NotImplementedException();
        }

        public void PlayOneShot(AudioClip clip)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            if (soundEffect == null) return;
            soundEffect.Stop();
            time = 0;
        }

        public void Pause()
        {
            if (soundEffect == null) return;
            soundEffect.Pause();
        }

        public static void PlayClipAtPoint(AudioClip clip, Vector3 position)
        {
            PlayClipAtPoint(clip, position, 1);
        }

        public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            if (soundEffect != null && !soundEffect.IsDisposed && soundEffect.State == SoundState.Playing)
            {
                time += Time.deltaTime;
                if (time > clip.length)
                {
                    time = time - clip.length;
                }
            }
        }

        public void LateUpdate()
        {
 
        }

        protected override void Destroy()
        {
            base.Destroy();
            if (soundEffect != null)
            {
                soundEffect.Dispose();
            }
        }
    }
}
