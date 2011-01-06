using System;
using System.Collections.Generic;

namespace PressPlay.FFWD
{
    public class AudioSettings
    {
        // Custom variables
        public int id = -1;

        // **************************
        // Custom variables
        // **************************

        /// <summary>
        /// Delay before the AudioObject starts playing
        /// </summary>
        public float delay = 0;

        /// <summary>
        /// Determines whether the AudioManager is allowed to overwrite this sound with another sound if the number of available AudioSources is scarce
        /// </summary>
        public bool isAvailableForOverwrite = true;

        // **************************
        // Variables for AudioSource
        // **************************
        public AudioClip clip;
        public float volume = 1;
        public float pitch = 1;
        public bool loop = false;
        public bool ignoreListenerVolume = false;
        public float minVolume = 0;
        public float maxVolume = 1;
        public float rolloffFactor = 1;
        public AudioVelocityUpdateMode velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;

        // ****************************
        // Variables for spacial volume
        // ****************************
        private bool _useSpacialAwareness = false;
        public bool useSpacialAwareness
        {
            get
            {
                return _useSpacialAwareness;
            }
        }

        public Transform worldPosition;
        public Transform listenerPosition;
        public float soundRadius;
        public float fullVolumeRadius;
        private float distanceToListener = 100000;

        public float spacialVolume
        {
            get
            {
                //added by klaus to remove error when transform is null and sound is still playing after scene load
                if (listenerPosition == null || worldPosition == null)
                {
                    return 0;
                }

                float distance = (listenerPosition.position - worldPosition.position).magnitude;

                if (distance > soundRadius)
                {
                    return 0;
                }
                else if (distance < fullVolumeRadius)
                {
                    return volume;
                }
                else
                {
                    //Debug.Log(string.Format("Distance: {0}, Radius: {1}, Factor: {2}, Volume: {3}", distance, soundRadius, 1 - ((distance - fullVolumeRadius) / (soundRadius - fullVolumeRadius)), (volume * (1 - ((distance - fullVolumeRadius) / (soundRadius - fullVolumeRadius))))));
                    return volume * (1 - ((distance - fullVolumeRadius) / (soundRadius - fullVolumeRadius)));
                }
            }
        }

        public AudioSettings()
        {

        }

        public AudioSettings(AudioClip clip)
        {
            this.clip = clip;
        }

        public AudioSettings(AudioClip clip, float volume, bool loop)
        {
            this.clip = clip;
            this.volume = volume;
            this.loop = loop;
        }

        public AudioSettings(AudioClip clip, float volume, float pitch)
        {
            this.clip = clip;
            this.volume = volume;
            this.pitch = pitch;
        }

        public AudioSettings(AudioClip clip, float volume, float pitch, bool loop)
        {
            this.clip = clip;
            this.volume = volume;
            this.pitch = pitch;
            this.loop = loop;
        }

        public AudioSettings(AudioClip clip, float volume, float pitch, bool loop, float delay)
        {
            this.clip = clip;
            this.volume = volume;
            this.pitch = pitch;
            this.loop = loop;
            this.delay = delay;
        }

        public void EnableSpacialAwareness(float fullVolumeRadius, float radius, Transform worldPosition, Transform listenerPosition)
        {
            this.fullVolumeRadius = fullVolumeRadius;
            this.soundRadius = radius;
            this.worldPosition = worldPosition;
            this.listenerPosition = listenerPosition;

            _useSpacialAwareness = true;
        }

        public void CopyToSource(ref AudioSource source)
        {
            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;
            source.ignoreListenerVolume = ignoreListenerVolume;
            //source.minVolume = minVolume;
            //source.maxVolume = maxVolume;
            //source.rolloffFactor = rolloffFactor;
            source.velocityUpdateMode = velocityUpdateMode;
        }

        public AudioSettings Clone()
        {
            AudioSettings settings = new AudioSettings();
            settings.delay = delay;
            settings.isAvailableForOverwrite = isAvailableForOverwrite;
            settings.volume = volume;
            settings.pitch = pitch;
            settings.loop = loop;
            settings.ignoreListenerVolume = ignoreListenerVolume;
            settings.minVolume = minVolume;
            settings.maxVolume = maxVolume;
            settings.rolloffFactor = rolloffFactor;
            settings.velocityUpdateMode = velocityUpdateMode;
            return settings;
        }
    }
}
