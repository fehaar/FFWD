using PressPlay.FFWD;

namespace PressPlay.Tentacles.Scripts
{
    public class CamState
    {

        public PathFollowCamStats stats;
        public PathFollowCam.State mode;
        public PathFollowCam.State state
        {
            get
            {
                return mode;
            }
        }
        public float stateDuration;

        public Vector3 objectPositionInViewport;
    }
}