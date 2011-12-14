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
                if (clip == null || clip.Instance.IsDisposed)
                {
                    return;
                }

                _volume = Mathf.Clamp01(value);
                _volume = Mathf.Max(_volume, minVolume);
                _volume = Mathf.Min(_volume, maxVolume);

                clip.Instance.Volume = _volume;
            }
        }

        public bool isPlaying
        {
            get
            {
                if (clip == null) { return false; }

                if (clip.Instance.IsDisposed) { return false; }

                return (clip.Instance.State == SoundState.Playing);
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
                if (clip != null)
                {
                    clip.Loop(_loop);
                }
            }
        }

        public bool ignoreListenerVolume = false;
        public bool playOnAwake = false;
        public float time = 0;
        public AudioVelocityUpdateMode velocityUpdateMode = AudioVelocityUpdateMode.Auto;

        private void SetSoundEffect(AudioClip sfx)
        {
            if (sfx != null)
            {
                sfx.Instance.Volume = volume;
                time = 0;
            }
        }

        public void Play()
        {
            if (clip == null)
            {
                return;
            }
            if (clip.Instance.State != SoundState.Stopped)
            {
                clip.Instance.Stop();
            }
            try
            {
                clip.Instance.Play();
            }
            catch (InstancePlayLimitException)
            {
                // We will eat this exception so it does not crash the game. 
                // In order not to get it we should put some limit on playing instances on somewhere else.
#if DEBUG
                PressPlay.FFWD.Debug.LogError("We are trying to play too many sounds at the same time. You cannot play more than 16 sound effects at the same time.");
#endif
            }
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
            if (clip == null || clip.Instance.IsDisposed) return;
            clip.Instance.Stop();
            time = 0;
        }

        public void Pause()
        {
            if (clip == null || clip.Instance.IsDisposed) return;
            clip.Instance.Pause();
        }

        public static void PlayClipAtPoint(AudioClip clip, Vector3 position)
        {
            PlayClipAtPoint(clip, position, 1);
        }

        public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
        {
            throw new NotImplementedException();
        }

        public void LateUpdate()
        {

        }

        public void Update()
        {
            if (clip == null || clip.Instance.IsDisposed) return;
            if (clip.Instance.State == SoundState.Playing)
            {
                time += Time.deltaTime;
                if (time >= clip.length)
                {
                    time = time - clip.length;
                    if (!loop && clip.Instance.IsLooped)
                    {
                        clip.Instance.Stop();
                    }
                }
            }
        }

        public void Play(ulong _uDelay)
        {
            // TODO: Implement the delay
            Play();
        }
    }
}
