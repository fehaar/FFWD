using System;
using System.Collections.Generic;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public class AudioManager : MonoBehaviour
    {

        private static int id = 0;
        public static int nextId
        {
            get
            {
                id++;
                return id - 1;
            }
        }

        private static AudioManager instance;

        private GameObject listenerObject;
        private Transform followObject;

        public int numberOfSoundChannels = 16;

        public float defaultVolume = 0.5f;
        public bool soundIsOn = true;

        public bool autoExpandCapacity = true;
        public int channelsToIncrementOnExpand = 16;
        public int maxSoundChannelCapacity = 256;

        // At which percent of played sound it is allowed to overwrite it with another sound
        public float allowedOverwritePercentage = 90;

        private AudioSource[] sources;
        private AudioObject[] sounds;

        private List<AudioObject> soundsReadyToDie = new List<AudioObject>();

        public int soundsPlaying = 0;

        public static bool isLoaded = false;

        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogError("Attempt to access instance of AudioManager earlier than Start or without it being attached to a GameObject.");
                }

                return instance;
            }
        }

        public override void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Cannot have two instances of AudioManager. Self destruction in 3...");
                Destroy(this);
                return;
            }

            Init();
            instance = this;
            isLoaded = true;
        }

        private void Init()
        {

            listenerObject = new GameObject("AudioManager Listener");
            listenerObject.transform.parent = transform;

            sources = new AudioSource[numberOfSoundChannels];
            sounds = new AudioObject[numberOfSoundChannels];

            for (int i = 0; i < numberOfSoundChannels; i++)
            {
                sources[i] = (AudioSource)listenerObject.AddComponent(new AudioSource());
                sources[i].volume = defaultVolume;
                sounds[i] = null;
            }
        }

        public override void Update()
        {
            if (followObject != null)
            {
                listenerObject.transform.position = followObject.position;
            }

            MaintainSounds();
        }

        public void SetFollowObject(Transform followObject)
        {
            this.followObject = followObject;
        }

        public void ClearFollowObject()
        {
            followObject = null;
        }

        private void MaintainSounds()
        {
            soundsReadyToDie.Clear();
            soundsPlaying = 0;

            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null)
                {
                    continue;
                }

                if (sounds[i].isPlaying) soundsPlaying++;

                // Updating sound
                sounds[i].Update();

                // We check if the sound is playing
                if (!sounds[i].isLocked)
                {
                    if (!sounds[i].isPlaying)
                    {
                        sounds[i].Destroy();
                        sounds[i] = null;
                    }
                    else
                    {

                        if (sounds[i].isAvailableForOverwrite && (sounds[i].percentage * 100) > allowedOverwritePercentage)
                        {
                            soundsReadyToDie.Add(sounds[i]);
                        }
                    }
                }
            }
        }

        public void OnApplicationQuit()
        {
            instance = null;
        }

        // Overrides all sounds of lower priority
        public void DoOverride(int priority, float duration, float factor)
        {
            DoOverride(priority, duration, factor, 0.3f, 0.3f);
        }

        // Overrides all sounds of lower priority
        public void DoOverride(int priority, float duration, float factor, float fadeInTime, float fadeOutTime)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null) continue;

                if (sounds[i].priority < priority)
                {
                    sounds[i].DoSoundOverride(duration, factor, fadeInTime, fadeOutTime);
                }
            }
        }

        // Overrides all sounds of other category
        public void DoOverride(string ignoreCategory, float duration, float factor)
        {
            DoOverride(ignoreCategory, duration, factor, 0.3f, 0.3f);
        }

        // Overrides all sounds of other category
        public void DoOverride(string ignoreCategory, float duration, float factor, float fadeInTime, float fadeOutTime)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null) continue;

                if (sounds[i].category != ignoreCategory)
                {
                    sounds[i].DoSoundOverride(duration, factor, fadeInTime, fadeOutTime);
                }
            }
        }

        // Returns the first AudioObject from category
        public AudioObject Get(string category)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null) continue;

                if (sounds[i].category == category)
                {
                    return sounds[i];
                }
            }

            return null;
        }

        public bool IsCategoryPlaying(string category)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null) continue;

                if (sounds[i].category == category && sounds[i].isPlaying)
                {
                    return true;
                }
            }

            return false;
        }

        public AudioObject Get(int id)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null) continue;

                if (sounds[i].id == id)
                {
                    return sounds[i];
                }
            }

            Debug.LogWarning("AudioManager::The Audio with ID: " + id + " doesn't exist anymore");
            return null;
        }

        public AudioObject Add(AudioSettings settings, string _category, int _priority)
        {
            if (settings.clip == null)
            {
                Debug.Log("The AudioClip you are trying to play is null (" + settings.clip.name + ")");
                return null;
            }

            int index = GetReadyAudioSource();

            if (index != -1)
            {
                settings.id = AudioManager.nextId;
                sounds[index] = new AudioObject(settings, sources[index], _category, _priority);
                sounds[index].isLocked = true;
                sounds[index].index = index;
                return sounds[index];
            }
            else
            {
                Debug.Log("AudioManager couldn't find available AudioSource. Aborting play");
                return null;
            }
        }

        public int Play(AudioClip _clip)
        {
            if (!soundIsOn) return -1;

            AudioSettings settings = new AudioSettings(_clip, defaultVolume, false);

            return Play(settings, "default", 0);
        }

        public int Play(AudioSettings settings)
        {
            if (!soundIsOn) return -1;

            return Play(settings, "default", 0);
        }

        public int Play(AudioSettings settings, string _category, int _priority)
        {
            if (settings.clip == null)
            {
                Debug.LogWarning("The AudioClip you are trying to play is null (" + settings + ")");
                return -1;
            }

            if (!soundIsOn)
            {
                return -1;
            }

            int index = GetReadyAudioSource();

            if (index != -1)
            {
                settings.id = AudioManager.nextId;
                sounds[index] = new AudioObject(settings, sources[index], _category, _priority);
                sounds[index].index = index;
                sounds[index].Play();
                return settings.id;
            }
            else
            {
                Debug.LogWarning("AudioManager couldn't find available AudioSource. Aborting play");
                return -1;
            }
        }

        public void Stop(int id)
        {

            AudioObject snd = Get(id);

            if (snd != null)
            {
                snd.Stop();
            }
        }

        public void Stop(string category)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null) continue;

                if (sounds[i].category == category)
                {
                    sounds[i].Stop();
                }
            }
        }

        public void PauseAllSounds()
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null) continue;

                if (sounds[i].isPlaying)
                {
                    sounds[i].Pause();
                }
            }
        }


        public void UnPauseAllSounds()
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null) continue;

                if (sounds[i].isPlaying)
                {
                    sounds[i].UnPause();
                }
            }
        }

        public void StopAllSounds()
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null) continue;

                if (sounds[i].isPlaying)
                {
                    sounds[i].Stop();
                }
            }
        }

        public void DestroyAllSounds()
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i] == null) continue;
                sounds[i].Destroy();
                sounds[i] = null;
            }
            Reset();
        }

        private void Reset()
        {

            id = 0;
            foreach (AudioSource item in sources)
            {
                Destroy(item);
            }
            Init();
        }

        private int GetReadyAudioSource()
        {
            for (int i = 0; i < sounds.Length; i++)
            {

                // If sound is NULL
                if (sounds[i] == null)
                {
                    return i;
                }

                // We skip if this sound is locked	
                if (sounds[i].isLocked)
                {
                    continue;
                }

                // If the sound is finished playing
                if (!sounds[i].isPlaying)
                {
                    return i;
                }
            }

            if (autoExpandCapacity && sources.Length < maxSoundChannelCapacity)
            {
                AddSources();

                return GetReadyAudioSource();
            }
            else
            {
                Debug.Log("I am getting a sound from overwrite list");
                return GetAudioSourceFromOverwriteList();
            }
        }

        private int GetAudioSourceFromOverwriteList()
        {
            AudioObject sound;
            float percentage = allowedOverwritePercentage;
            int index = -1;

            for (int i = 0; i < soundsReadyToDie.Count; i++)
            {
                sound = (AudioObject)soundsReadyToDie[i];

                if (sound.percentage > percentage)
                {
                    percentage = sound.percentage;
                    index = sound.index;
                }
            }

            return index;
        }

        private void AddSources()
        {
            int newSize = numberOfSoundChannels + channelsToIncrementOnExpand;
            newSize = Mathf.Min(newSize, maxSoundChannelCapacity);

            AudioSource[] newSources = new AudioSource[newSize];
            AudioObject[] newSounds = new AudioObject[newSize];

            for (int i = 0; i < newSize; i++)
            {
                if (i < numberOfSoundChannels)
                {
                    newSources[i] = sources[i];
                    newSounds[i] = sounds[i];
                }
                else
                {
                    newSources[i] = (AudioSource)gameObject.AddComponent(new AudioSource());
                    newSources[i].volume = defaultVolume;
                    newSounds[i] = null;
                }
            }

            sources = newSources;
            sounds = newSounds;
            numberOfSoundChannels = newSize;
        }

    }
}
