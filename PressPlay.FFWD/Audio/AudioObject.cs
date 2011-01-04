using System;
using System.Collections.Generic;

namespace PressPlay.FFWD
{
    public class AudioObject
    {

        public AudioClip clip;
        public AudioSource source;
        public AudioSettings settings;

        public int id
        {
            get
            {
                return settings.id;
            }
            set
            {
                settings.id = value;
            }
        }

        public int index = -1;

        public string category = "default";
        public int priority = 0;

        public bool isLocked = false;
        public bool isOverridden = false;
        public bool isPaused = false;

        /// <summary>
        /// Determines whether the AudioManager is allowed to overwrite this sound with another sound if the number of available AudioSources is scarce
        /// </summary>	
        public bool isAvailableForOverwrite
        {
            get
            {
                return settings.isAvailableForOverwrite;
            }
            set
            {
                settings.isAvailableForOverwrite = value;
            }
        }

        public bool isLooping
        {
            get
            {
                return source.loop;
            }
        }

        public bool isPlaying
        {
            get
            {
                if (isWaitingToStart)
                {
                    return true;
                }
                else
                {
                    return source.isPlaying;
                }
            }
        }

        public float percentage
        {
            get
            {
                return (source.time / source.clip.length);
            }
        }

        public float volume
        {
            get
            {
                return settings.volume;
            }
            set
            {
                source.volume = value;
                settings.volume = value;
            }
        }

        public float pitch
        {
            get
            {
                return settings.pitch;
            }
            set
            {
                source.pitch = value;
                settings.pitch = value;
            }
        }

        // Variables used to delay start
        private float delayedStart = 0;
        private bool isWaitingToStart = false;
        // -----------------------------

        // Variables used to decrease sound for a period of time
        private float timeToDecreaseSound = 0;
        // ----------------------------------

        // Variables used for fading sound
        private const string STOP = "stop";
        private const string PAUSE = "pause";
        private string onFadeComplete = "";

        private bool isFading = false;
        private float fadeFrom = 0;
        private float fadeTo = 1;
        private float fadeDuration = 0;
        private float fadeTimeCounter = 0;
        private float fadeInTime = 0;
        private float fadeOutTime = 0;
        // --------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings">
        /// The settings for this AudioObject <see cref="AudioSettings"/>
        /// </param>
        /// <param name="source">
        /// The AudioSource that should play this AudioObject <see cref="AudioSource"/>
        /// </param>
        public AudioObject(AudioSettings settings, AudioSource source)
        {
            this.source = source;
            this.settings = settings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings">
        /// The settings for this AudioObject <see cref="AudioSettings"/>
        /// </param>
        /// <param name="source">
        /// The AudioSource that should play this AudioObject <see cref="AudioSource"/>
        /// </param>
        /// <param name="category">
        /// The category to which this sound belongs <see cref="System.String"/>
        /// </param>
        public AudioObject(AudioSettings settings, AudioSource source, string category)
        {
            this.settings = settings;
            this.source = source;
            this.category = category;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings">
        /// The settings for this AudioObject <see cref="AudioSettings"/>
        /// </param>
        /// <param name="source">
        /// The AudioSource that should play this AudioObject <see cref="AudioSource"/>
        /// </param>
        /// <param name="category">
        /// The category to which this AudioObject belongs <see cref="System.String"/>
        /// </param>
        /// <param name="priority">
        /// The priority of this AudioObject <see cref="System.Int32"/>
        /// </param>
        public AudioObject(AudioSettings settings, AudioSource source, string category, int priority)
        {
            this.settings = settings;
            this.source = source;
            this.category = category;
            this.priority = priority;
        }

        // Update
        public void Update()
        {

            if (isPaused) return;

            // If the sound is currently fading
            if (isFading)
            {
                fadeTimeCounter += Time.deltaTime;

                if (fadeTimeCounter < fadeDuration)
                {
                    source.volume = Mathf.Lerp(fadeFrom, fadeTo, fadeTimeCounter / fadeDuration);
                }
                else
                {
                    source.volume = fadeTo;
                    isFading = false;
                    OnFadeComplete();
                }
            }

            // If the sound is waiting to start play
            if (isWaitingToStart)
            {
                delayedStart -= Time.deltaTime;
                if (delayedStart <= 0)
                {
                    source.Play();
                    isWaitingToStart = false;
                }
            }

            // If the sound is overriden by another sound
            if (isOverridden)
            {
                timeToDecreaseSound -= Time.deltaTime;
                if (timeToDecreaseSound <= 0)
                {
                    Fade(volume, fadeInTime);
                    isOverridden = false;
                }
            }

            if (settings.useSpacialAwareness)
            {
                source.volume = settings.spacialVolume;
            }
        }

        // Function to do an override of the sound. 
        // It alters the volume of a sound by a given factor in a given period of time
        public void DoSoundOverride(float duration, float volumeFactor, float timeToFadeIn, float timeToFadeOut)
        {
            timeToDecreaseSound = duration;

            Fade((volume * volumeFactor), fadeOutTime, timeToFadeIn, timeToFadeOut);
            isOverridden = true;
        }

        public void Play()
        {
            settings.CopyToSource(ref source);
            if (settings.useSpacialAwareness)
            {
                source.volume = settings.spacialVolume;
            }

            delayedStart = settings.delay;

            if (delayedStart > 0)
            {
                isWaitingToStart = true;
            }
            else
            {
                source.Play();
            }

            isPaused = false;
        }

        public void Pause()
        {
            source.Pause();
            isPaused = true;
        }

        public void UnPause()
        {
            source.Play();
            isPaused = false;
        }

        public void Stop()
        {

            isFading = false;
            isWaitingToStart = false;
            if (source != null)
            {
                source.Stop();
            }
        }

        public void Fade(float toValue, float duration)
        {
            Fade(toValue, duration, fadeInTime, fadeOutTime);
        }

        public void Fade(float toValue, float duration, float timeToFadeIn, float timeToFadeOut)
        {
            fadeFrom = source.volume;
            fadeTo = toValue;
            fadeDuration = duration;
            fadeInTime = timeToFadeIn;
            fadeOutTime = timeToFadeOut;

            fadeTimeCounter = 0;
            isFading = true;
        }

        private void OnFadeComplete()
        {
            switch (onFadeComplete)
            {
                case AudioObject.STOP:
                    Stop();
                    break;

                case AudioObject.PAUSE:
                    Pause();
                    break;

                default:
                    break;
            }

            onFadeComplete = "";
        }

        public void EnableSpacialAwareness(float fullVolumeRadius, float radius, Transform worldPosition, Transform listenerPosition)
        {
            settings.EnableSpacialAwareness(fullVolumeRadius, radius, worldPosition, listenerPosition);
        }

        public void Destroy()
        {
            Stop();
            clip = null;
            source.clip = null;
            source = null;
        }
    }
}
