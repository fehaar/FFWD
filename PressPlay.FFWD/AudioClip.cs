using System;using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace PressPlay.FFWD
{
    public class AudioClip : Asset
    {
        [ContentSerializer(Optional = true)]
        public string clip;

        [ContentSerializerIgnore]
        private SoundEffect sound;

        private bool startNextFrame = false;

        public float length
        {
            get
            {
                if (sound == null) return 0.0f;
                return (float)sound.Duration.TotalSeconds;
            }
        }

        public AudioClip()
        {

        }

        internal AudioClip(SoundEffect sound)
        {
            this.sound = sound;
            if (sound != null)
            {
                Instance = sound.CreateInstance();
                name = sound.Name + " (resource)";
            }
        }

        protected override void DoLoadAsset(AssetHelper assetHelper)
        {
            if (sound == null)
            {
                sound = assetHelper.Load<SoundEffect>("Sounds/" + clip);
                name = clip;
                if (sound != null)
                {
                    Instance = sound.CreateInstance();
                }
            }
        }

        private SoundEffectInstance _instance;
        private SoundEffectInstance Instance 
        { 
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
                if (_instance != null)
                {
                    _instance.Volume = Volume;
                    _instance.IsLooped = Loop;
                }
            }
        }

        public bool isReadyToPlay 
        {
            get
            {
                return Instance != null;
            }
        }

        internal bool _loop = false;
        internal bool Loop
        {
            get
            {
                return _loop;
            }
            set
            {
                _loop = value;
                if (Instance != null && !Instance.IsDisposed)
                {
                    Instance.IsLooped = value;
                }
            }
        }

        internal float _volume = 1;
        internal float Volume 
        { 
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
                if (Instance != null && !Instance.IsDisposed)
                {
                    Instance.Volume = value;
                }
            }
        }

        internal bool isPlaying
        {
            get
            {
                return (Instance != null && Instance.State == SoundState.Playing);
            }
        }

        internal void Play()
        {
            if (Instance != null)
            {
                if (Instance.State == SoundState.Playing)
                {
                    Instance.Stop();
                    startNextFrame = true;
                    return;
                }
                Instance.Play();
                startNextFrame = false;
            }
            else
            {
                startNextFrame = true;
            }
        }

        internal void Stop()
        {
            if (Instance != null && Instance.State != SoundState.Stopped)
            {
                Instance.Stop();
            }
        }

        internal void Pause()
        {
            if (Instance != null && Instance.State != SoundState.Paused)
            {
                Instance.Pause();
            }
        }

        internal void Update()
        {
            if (startNextFrame)
            {
                Play();
            }
        }
    }
}
