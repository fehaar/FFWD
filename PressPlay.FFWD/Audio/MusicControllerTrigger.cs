using System.Collections.Generic;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public class MusicControllerTrigger : MonoBehaviour
    {

        public enum MusicControllerMode
        {
            inactive,
            isWaitingToTriggerEnter,
            isWaitingToTriggerExit
        }

        public MusicController.BackgroundLoopId[] loopsToTrigger;
        public MusicController.BackgroundLoopId[] loopsToFadeOut;

        public float triggerDelay = 1;
        public float fadeInTime = 1;
        public float fadeOutTime = 1;

        public bool fadeOutOnExitTrigger = false;

        private bool isLemmyInTrigger = false;

        private float lastEnterTime = 0;
        private float lastExitTime = 0;
        private MusicControllerMode mode = MusicControllerMode.inactive;

        private bool hasStartedMusic = false;

        void OnTriggerEnter(Collider collider)
        {
            if (isLemmyInTrigger) return;

            isLemmyInTrigger = true;

            mode = MusicControllerMode.isWaitingToTriggerEnter;
            lastEnterTime = Time.time;
        }

        void OnTriggerExit(Collider collider)
        {
            if (!isLemmyInTrigger) return;

            isLemmyInTrigger = false;

            mode = MusicControllerMode.isWaitingToTriggerExit;
            lastExitTime = Time.time;
        }

        void Update()
        {

            if (mode != MusicControllerMode.inactive)
            {

                switch (mode)
                {
                    case MusicControllerMode.isWaitingToTriggerEnter:
                        if (Time.time > lastEnterTime + triggerDelay)
                        {
                            if (!hasStartedMusic)
                            {
                                PlayMusic();
                            }
                            mode = MusicControllerMode.inactive;
                        }

                        break;

                    case MusicControllerMode.isWaitingToTriggerExit:
                        if (Time.time > lastExitTime + triggerDelay)
                        {
                            if (hasStartedMusic && fadeOutOnExitTrigger)
                            {
                                MuteMusic();
                            }
                            mode = MusicControllerMode.inactive;
                        }

                        break;
                }
            }
        }

        private void PlayMusic()
        {
            if (!LevelHandler.isLoaded) return;

            foreach (MusicController.BackgroundLoopId id in loopsToFadeOut)
            {
                LevelHandler.Instance.musicController.Mute(id, fadeOutTime);
            }

            foreach (MusicController.BackgroundLoopId id in loopsToTrigger)
            {
                LevelHandler.Instance.musicController.Play(id, fadeInTime);
            }

            hasStartedMusic = true;
        }

        private void MuteMusic()
        {
            if (!LevelHandler.isLoaded) return;

            foreach (MusicController.BackgroundLoopId id in loopsToTrigger)
            {
                LevelHandler.Instance.musicController.Mute(id, fadeOutTime);
            }

            hasStartedMusic = false;
        }
    }

    public class LoopControl
    {
        public MusicController.BackgroundLoopId loop;
        public bool on;
    }
}
