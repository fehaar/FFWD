using System.Collections.Generic;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public class MusicController : MonoBehaviour
    {
        public enum BackgroundLoopId
        {
            loop1,
            loop2,
            loop3
        }

        private Dictionary<BackgroundLoopId, AudioObject> loops = new Dictionary<BackgroundLoopId, AudioObject>();
        private List<AudioObject> musicToBeRemoved = new List<AudioObject>();

        public AudioSettings defaultTrackSetting;

        public bool playMusicOnStart;
        public BackgroundLoopId musicToPlayOnStart = BackgroundLoopId.loop1;

        public float defautFadeIn = 1;
        public float defaultFadeOut = 1;

        private bool _isInitialized = false;
        public bool isInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        private AudioObject currentMusic;

        void Start()
        {

        }

        public void Init(AudioClip loop1, AudioClip loop2, AudioClip loop3)
        {

            AudioSettings settings;
            if (loop1 != null)
            {
                settings = defaultTrackSetting.Clone();
                settings.clip = loop1;
                settings.loop = true;
                settings.volume = 0;
                loops.Add(BackgroundLoopId.loop1, AudioManager.Instance.Add(settings, "music", 1));
                loops[BackgroundLoopId.loop1].Play();
            }

            if (loop2 != null)
            {
                settings = defaultTrackSetting.Clone();
                settings.clip = loop2;
                settings.loop = true;
                settings.volume = 0;
                loops.Add(BackgroundLoopId.loop2, AudioManager.Instance.Add(settings, "music", 1));
                loops[BackgroundLoopId.loop2].Play();
            }

            if (loop3 != null)
            {
                settings = defaultTrackSetting.Clone();
                settings.clip = loop3;
                settings.loop = true;
                settings.volume = 0;
                loops.Add(BackgroundLoopId.loop3, AudioManager.Instance.Add(settings, "music", 1));
                loops[BackgroundLoopId.loop3].Play();
            }

            if (playMusicOnStart)
            {
                Play(musicToPlayOnStart);
            }
        }

        public void MuteAll(float fadeDuration)
        {
            foreach (KeyValuePair<BackgroundLoopId, AudioObject> kvp in loops)
            {
                kvp.Value.Fade(0, defaultFadeOut);
            }
        }

        public void MuteAll()
        {
            MuteAll(defaultFadeOut);
        }

        public void Mute(BackgroundLoopId id, float fadeDuration)
        {
            if (!loops.ContainsKey(id)) return;

            loops[id].Fade(0, fadeDuration);
        }

        public void Mute(BackgroundLoopId id)
        {
            Mute(id, defaultFadeOut);
        }

        public void Play(BackgroundLoopId id)
        {
            Play(id, defautFadeIn);
        }

        public void Play(BackgroundLoopId id, float fadeDuration)
        {
            if (!loops.ContainsKey(id)) return;

            loops[id].Fade(1, fadeDuration);
        }
    }

    public class BackgroundTrack
    {
        public string id;
        public AudioClip track;
    }
}
