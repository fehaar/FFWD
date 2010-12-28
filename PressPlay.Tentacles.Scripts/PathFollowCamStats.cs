
namespace PressPlay.Tentacles.Scripts
{
    public class PathFollowCamStats
    {

        public float maxRotationSpeed;
        public float maxMovementSpeed;

        public float moveStiffness = 0.8f;
        public float turnStiffness = 0.8f;
        public float lookAheadAndHeightStiffnes = 0.8f;

        public float placeObjectInViewPortStiffness = 2.5f;

        public float defaultHeight = 12;
        public float defaultLookAhead = 4;

        public float changeToLastConnectionThresshold = 4f;


        public float speedCurvePow = 0.4f;
        public float speedLookAhead = 4;
        public float speedZoomOut = 0.2f;
        public float speedMoveStiffnessMod = 2;
        public float speedTurnStiffnessMod = 0.5f;
        public float speedModThresshold = 2;

    }
}