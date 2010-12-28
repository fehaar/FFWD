using PressPlay.FFWD;

namespace PressPlay.Tentacles.Scripts
{
    public class PathFollowCamPosition
    {

        public Vector3 gotoPos;
        public Quaternion gotoRotation;
        public Vector3 childCamPos;

        public float speedMod = 0; //this is not a position, but a measure of how the camera movement should speed up when lemmy moves fast

    }
}