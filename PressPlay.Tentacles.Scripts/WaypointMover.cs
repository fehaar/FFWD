using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts
{
    public class WaypointMover : MonoBehaviour
    {

        public Transform movementTarget;
        /*public enum MovementType{
            LOOP,
            PING_PONG
        }*/



        //public MovementType movementType = MovementType.LOOP;

        public float sequenceLengthOverride = -1;

        public float moveSpeed = 1;
        public float sequenceOffset = 0;
        //public float startDelay = 0f;

        //private int nextNodeIndex = 0;
        //private int nodeIncrementValue = 1;

        //private Vector3 fromXYZ;
        //private Vector3 destinationXYZ;
        //private flaot moveTime;

        protected bool isCyclic = false;

        protected Waypoint[] waypoints;

        protected Transform[] bezierPoints;

        protected bool oneShotMovement = false;
        //protected float oneShotMovementStartTime = 0;
        protected float oneShotSequenceTime = 0;
        protected float oneShotSequenceSpeedMod = 1;


        protected float totalPathLength;
        protected float totalSequenceTime;
        private float _sequenceTime;
        public float sequenceTime
        {
            get
            {
                return _sequenceTime;
            }
        }

        protected bool isInitialized = false;

        public GameObject waypointParent;
        public Waypoint startWaypoint;

        public bool rotateToMovementDirection = false;

        protected Vector3 lastPosition;
        //private Quaternion lastRotation;

        public bool automaticMovement = true;

        protected bool isMoving = false;

        private Vector3 zeroVector = Vector3.zero;

        public enum Mode
        {
            straightPath,
            bezier
        }
        public Mode mode = Mode.straightPath;

        public bool autoInitialize = true;

        // Use this for initialization
        public override void Start()
        {

            if (autoInitialize)
            {
                Initialize();
            }
        }

        public virtual void Initialize()
        {
            if (automaticMovement)
            {
                isMoving = true;
            }

            GetAndSetWaypoints();
            isInitialized = true;

            if (movementTarget != null)
            {
                lastPosition = movementTarget.position;
            }
            else
            {
                lastPosition = zeroVector;
            }
        }

        // Update is called once per frame
        public override void Update()
        {


            if (isInitialized && movementTarget != null && isMoving)
            {
                if (oneShotMovement)
                {
                    oneShotSequenceTime += Time.deltaTime * oneShotSequenceSpeedMod;

                    movementTarget.position = GetPositionFromSequenceTime(GetOneShotSequenceTime());
                }
                else
                {
                    movementTarget.position = GetPositionFromGlobalTime(LevelHandler.Instance.globalLevelTime);
                }

                if (rotateToMovementDirection)
                {
                    if (lastPosition != movementTarget.position)
                    {
                        movementTarget.LookAt(movementTarget.position + (movementTarget.position - lastPosition));
                    }

                    lastPosition = movementTarget.position;
                }
            }
        }

        protected Vector3 GetPositionFromGlobalTime(float _globalTime)
        {
            _sequenceTime = (_globalTime - sequenceOffset) % totalSequenceTime;

            //Debug.Log("GetPositionFromGlobalTime  sequenceTime :"+sequenceTime + "     total Sequence Time : "+totalSequenceTime);

            return GetPositionFromSequenceTime(sequenceTime);

        }

        protected Vector3 GetPositionFromSequenceTime(float _sequenceTime)
        {
            switch (mode)
            {
                case Mode.straightPath:
                    return GetLinearPositionFromSequenceTime(_sequenceTime);

                case Mode.bezier:
                    return GetBezierPositionFromSequenceTime(_sequenceTime);
            }

            return zeroVector;
        }

        protected Vector3 GetBezierPositionFromSequenceTime(float _sequenceTime)
        {
            if (_sequenceTime < 0)
            {
                return waypoints[0].transform.position;
            }

            if (_sequenceTime > totalSequenceTime)
            {
                return waypoints[waypoints.Length - 1].transform.position;
            }

            return iTween.PointOnPath(bezierPoints, _sequenceTime / totalSequenceTime);
        }

        protected Vector3 GetLinearPositionFromSequenceTime(float _sequenceTime)
        {
            if (waypoints == null)
            {
                return Vector3.zero;
            }

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (_sequenceTime >= waypoints[i].sequenceStartTime && _sequenceTime <= waypoints[i].sequenceEndTime)
                {
                    return waypoints[i].GetPositionOnPath(_sequenceTime);
                }
            }

            return zeroVector;
        }

        protected void GetAndSetWaypoints()
        {
            if (startWaypoint == null)
            {
                Debug.LogError("Startwaypoint on object: " + name + " is NULL. IT MUST BE SOMETHING!! Or something bad will happen...");
                return;
            }



            // get waypoints from children
            Waypoint[] tmpWaypoints;

            if (waypointParent == null)
            {
                tmpWaypoints = gameObject.GetComponentsInChildren<Waypoint>();
            }
            else
            {
                tmpWaypoints = waypointParent.GetComponentsInChildren<Waypoint>();
            }
            //ArrayList addedWayPointsList = new ArrayList();

            waypoints = new Waypoint[tmpWaypoints.Length];
            waypoints[0] = startWaypoint;
            bezierPoints = new Transform[tmpWaypoints.Length];


            if (waypoints.Length == 0)
            {
                return;
            }

            /*foreach(Waypoint wp in tmpWaypoints)
            {
                if (wp.isFirstWaypoint)
                {
                    waypoints[0] = wp;
                    break;
                }
            }*/

            int currentWaypointIndex = 1;

            Waypoint currentWaypoint = waypoints[0];
            while (currentWaypoint.nextWaypoint != null)
            {

                if (currentWaypoint.nextWaypoint == waypoints[0])
                {
                    isCyclic = true;
                    break;
                }

                waypoints[currentWaypointIndex] = currentWaypoint.nextWaypoint;
                currentWaypoint = currentWaypoint.nextWaypoint;
                currentWaypointIndex++;
            }

            SetWaypoints(waypoints);
        }

        public virtual void SetOneShotMovementSpeedMod(float _speedMod)
        {
            oneShotSequenceSpeedMod = _speedMod;
        }

        public virtual void StartOneShotMovement()
        {
            isMoving = true;
            oneShotMovement = true;
            //oneShotMovementStartTime = LevelHandler.Instance.globalLevelTime;
            oneShotSequenceTime = 0;
            oneShotSequenceSpeedMod = 1;

            Update();
        }

        protected float GetOneShotSequenceTime()
        {
            return Mathf.Min(oneShotSequenceTime, totalSequenceTime);
        }


        /// <summary>
        /// DOES NOT WORK YET !!!!!! Sets the waypoints and do one shot movement.
        /// </summary>
        /// <param name='tmpWaypoints'>
        /// Tmp waypoints.
        /// </param>
        /// <param name='_sequenceTime'>
        /// _sequence time.
        /// </param>
        public void SetWaypointsAndDoOneShotMovement(Waypoint[] tmpWaypoints, float _sequenceTime)
        {


            sequenceLengthOverride = _sequenceTime;
            //totalSequenceTime = sequenceTime;
            SetWaypoints(tmpWaypoints);

            movementTarget.position = GetPositionFromSequenceTime(0);

            StartOneShotMovement();
        }



        public void SetWaypoints(Waypoint[] tmpWaypoints)
        {
            if (tmpWaypoints != waypoints)
            {
                for (int i = 0; i < waypoints.Length; i++)
                {
                    Destroy(waypoints[i]);
                }
            }

            waypoints = tmpWaypoints;
            bezierPoints = new Transform[waypoints.Length];
            //do first initialization and get total path length
            for (int i = 0; i < waypoints.Length; i++)
            {

                waypoints[i].InitializeStart();
                totalPathLength += waypoints[i].lengthToNextWaypoint;

                bezierPoints[i] = waypoints[i].transform;
            }

            if (sequenceLengthOverride == -1)
            {
                totalSequenceTime = totalPathLength / moveSpeed;
            }
            else
            {
                totalSequenceTime = sequenceLengthOverride;

                moveSpeed = totalPathLength / totalSequenceTime;
            }


            float earlierPath = 0;

            for (int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i].InitializeComplete(moveSpeed, earlierPath);
                earlierPath += waypoints[i].lengthToNextWaypoint;
            }
        }
    }
}