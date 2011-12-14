using System;
using System.Collections.Generic;
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

        public AudioClip(SoundEffect sound)
        {
            this.sound = sound;
            if (sound != null)
            {
                Instance = sound.CreateInstance();
                loopSet = false;
            }
        }

        protected override void DoLoadAsset(AssetHelper assetHelper)
        {
            if (sound == null)
            {
                sound = assetHelper.Load<SoundEffect>("Sounds/" + clip);
                loopSet = false;
                name = clip;
                if (sound != null)
                {
                    Instance = sound.CreateInstance();
                }
            }
        }

        internal SoundEffectInstance Instance { get; private set; }

        private bool loopSet = false;
        internal void Loop(bool loop)
        {
            if (!loopSet && Instance != null)
            {
                loopSet = true;
                Instance.IsLooped = loop;
            }
        }

        public bool isReadyToPlay 
        {
            get
            {
                return Instance != null;
            }
        }
    }
}
