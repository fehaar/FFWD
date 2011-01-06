using System;
using System.Collections.Generic;

namespace PressPlay.FFWD
{
    public class AudioWrapper
    {
        public AudioClip[] sounds;
        public bool random = false;
        public float volume = 1;
        public float pitch = 1;
        public Vector2 pitchVariation = new Vector2(0, 0);
        public bool loop = false;

        public float lockDelay = 0;

        public bool useSpacialAwareness = false;
        public Transform worldPosition;
        public float soundRadius;
        public float fullVolumeRadius;

        private int index = 0;

        private bool isPlaying = false;
        private int currentSound = -1;
        private AudioObject currentSoundObject;

        private float lastPlayTime = 0;

        public void Start()
        {

        }

        public void Stop()
        {
            if (isPlaying && currentSound != -1)
            {
                AudioManager.Instance.Stop(currentSound);
                isPlaying = false;
                currentSound = -1;
            }
        }

        public void PlaySound()
        {
            if (!AudioManager.isLoaded) return;

            if (sounds.Length == 0 || Time.time < lastPlayTime + lockDelay)
            {
                return;
            }

            if (isPlaying && currentSound != -1)
            {
                AudioManager.Instance.Stop(currentSound);
            }

            int sndIndex;
            if (random)
            {
                sndIndex = Random.Range(0, sounds.Length);
                sndIndex = Mathf.Clamp(sndIndex, 0, sounds.Length - 1);

            }
            else
            {

                sndIndex = index;
                index++;
                if (index == sounds.Length)
                {
                    index = 0;
                }
            }

            float tempPitch = Mathf.Clamp(pitch + (Random.Range(-pitchVariation.x, pitchVariation.y)), 0, 1);

            //Debug.Log("sounds[sndIndex] : "+sounds[sndIndex]);

            AudioSettings settings = new AudioSettings(sounds[sndIndex], volume, tempPitch);
            settings.loop = loop;

            if (loop)
            {
                isPlaying = true;
            }
            else
            {
                isPlaying = false;
            }

            if (useSpacialAwareness)
            {
                throw new NotImplementedException("This is not implemented!");
                //settings.EnableSpacialAwareness(fullVolumeRadius, soundRadius, worldPosition, LevelHandler.Instance.cam.transform);
            }

            currentSound = AudioManager.Instance.Play(settings);
            lastPlayTime = Time.time;
        }      
    }
}
