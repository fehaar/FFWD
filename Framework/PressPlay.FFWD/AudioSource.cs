using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using PressPlay.FFWD.Interfaces;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{

    public enum AudioVelocityUpdateMode
    {
        Auto,
        Fixed,
        Dynamic
    }

    public class AudioSource : Behaviour, IUpdateable
    {
        [ContentSerializer(ElementName="volume")]
        private float _volume = 1;
        [ContentSerializerIgnore]
        public float volume
        {
            get
            {
                return _volume;
            }
            set
            {
                if (clip == null)
                {
                    return;
                }

                _volume = Mathf.Clamp01(value);

                clip.Volume = _volume;
            }
        }

        public float pitch = 0;

        [ContentSerializer(ElementName = "clip")]
        private AudioClip _clip;
        [ContentSerializerIgnore]
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

        public bool isPlaying
        {
            get
            {
                return (clip != null) && clip.isPlaying;
            }
        }

        [ContentSerializer(ElementName = "loop")]
        private bool _loop = false;
        [ContentSerializerIgnore]
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
                    clip.Loop = _loop;
                }
            }
        }

        public bool playOnAwake = false;
        [ContentSerializerIgnore]
        public float time = 0;
        [ContentSerializerIgnore]
        public AudioVelocityUpdateMode velocityUpdateMode = AudioVelocityUpdateMode.Auto;

        private void SetSoundEffect(AudioClip sfx)
        {
            if (sfx != null)
            {
                sfx.Volume = volume;
                time = 0;
            }
        }

        public void Play()
        {
            if (clip == null)
            {
                return;
            }
            try
            {
                clip.Play();
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

        public override void Awake()
        {
            base.Awake();
            if (playOnAwake)
            {
                clip.Loop = loop;
                clip.Volume = volume;
                // TODO: Create pitch support
                Play();
            }
        }

        public void Stop()
        {
            if (clip == null) return;
            clip.Stop();
            time = 0;
        }

        public void Pause()
        {
            if (clip == null) return;
            clip.Pause();
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
            if (clip == null) return;
            clip.Update();
            if (clip.isPlaying)
            {
                time += Time.deltaTime;
                if (time >= clip.length)
                {
                    time = time - clip.length;
                    if (!loop && clip.Loop)
                    {
                        clip.Stop();
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
