using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts
{
    public class Waypoint : MonoBehaviour
    {

        //public bool useThisWaypoint = true;
        //public bool thisIsTheFirstWaypoint = false;

        public Ease easeToNextWaypoint = Ease.Linear;

        //public bool isFirstWaypoint = false;

        public bool wormholeToNextWaypoint;

        public Waypoint nextWaypoint;

        public float _lengthToNextWaypoint;

        public float lengthToNextWaypoint
        {
            get
            {
                return _lengthToNextWaypoint;
            }
        }

        private float _positionOnPath;
        private float _sequenceStartTime;
        private float _sequenceEndTime;
        private float sequenceTime;

        public float sequenceStartTime
        {
            get { return _sequenceStartTime; }
        }
        public float sequenceEndTime
        {
            get { return _sequenceEndTime; }
        }
        public float positionOnPath
        {
            get { return _positionOnPath; }
        }


        public void InitializeStart()
        {
            if (nextWaypoint == null)
            {
                return;
            }
            if (wormholeToNextWaypoint)
            {
                _lengthToNextWaypoint = 0;
            }
            else
            {
                _lengthToNextWaypoint = (transform.position - nextWaypoint.transform.position).magnitude;
            }
        }

        public void InitializeComplete(float _moveSpeed, float _positionOnPath)
        {
            this._positionOnPath = _positionOnPath;
            this._sequenceStartTime = _positionOnPath / _moveSpeed;
            this._sequenceEndTime = (_positionOnPath + lengthToNextWaypoint) / _moveSpeed;

            sequenceTime = _sequenceEndTime - _sequenceStartTime;
        }

        public Vector3 GetPositionOnPath(float _sequenceTime)
        {

            if (nextWaypoint == null)
            {
                return transform.position;
            }

            float easedFraction = Equations.ChangeFloat(_sequenceTime - sequenceStartTime, 0, 1, sequenceTime, easeToNextWaypoint);

            return Vector3.Lerp(transform.position, nextWaypoint.transform.position, easedFraction);
        }

        public static Waypoint CreateWaypoint(Vector3 _position, Transform _parent, Waypoint _lastWaypoint)
        {
            Waypoint tmpWaypoint = CreateWaypoint(_position, _parent);
            _lastWaypoint.nextWaypoint = tmpWaypoint;

            return tmpWaypoint;
        }
        public static Waypoint CreateWaypoint(Vector3 _position, Transform _parent)
        {
            GameObject tmpGameObject = new GameObject();
            tmpGameObject.transform.position = _position;
            tmpGameObject.transform.parent = _parent;
            Waypoint tmpWaypoint = (Waypoint)(tmpGameObject.AddComponent(typeof(Waypoint)));
            return tmpWaypoint;
        }
    }
}