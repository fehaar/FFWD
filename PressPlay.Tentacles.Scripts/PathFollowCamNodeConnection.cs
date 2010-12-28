using System.Collections.Generic;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts
{

    public class PathFollowCamNodeConnection : MonoBehaviour
    {

        //public static PathFollowCamNodeConnection activeNodeConnection;

        public bool isFirstConnection
        {
            get
            {
                return backNode.isFirstNode || frontNode.isFirstNode;
            }
        }
        public bool isLastConnection
        {
            get
            {
                return backNode.isLastNode || frontNode.isLastNode;
            }
        }


        public PathFollowCamNode frontNode;
        public PathFollowCamNode backNode;

        public float connectionDistance;

        private List<PathFollowCamNodeConnection> connectionsToDistanceTest = new List<PathFollowCamNodeConnection>();

        public bool centerFollowObjectInViewPort = false;

        /*public bool isActive{
            get{
                return PathFollowCamNodeConnection.activeNodeConnection == this;
            }
        }*/

        // Use this for initialization
        public void Initialize(PathFollowCamNode _backNode, PathFollowCamNode _frontNode)
        {
            frontNode = _frontNode;
            backNode = _backNode;

            frontNode.backNodeConnection = this;

            backNode.frontNodeConnection = this;


            if (backNode.centerFollowObjectInViewPort && frontNode.centerFollowObjectInViewPort)
            {
                centerFollowObjectInViewPort = true;
            }

            Vector3 direction = frontNode.transform.position - backNode.transform.position;

            connectionDistance = direction.magnitude;

            transform.position = _backNode.transform.position;

            transform.LookAt(transform.position + Vector3.down, new Vector3(-direction.z, 0, direction.x));

            //array of connections to do local distance test
            /*if (frontNode.frontNodeConnection != null && backNode.backNodeConnection != null)
            {
                connectionsToDistanceTest = new PathFollowCamNodeConnection[3];
                connectionsToDistanceTest[0] = this;
                connectionsToDistanceTest[1] = backNode.backNodeConnection;
                connectionsToDistanceTest[2] = frontNode.frontNodeConnection;
            }else if (backNode.backNodeConnection != null)
            {
                connectionsToDistanceTest = new PathFollowCamNodeConnection[2];
                connectionsToDistanceTest[0] = this;
                connectionsToDistanceTest[1] = backNode.backNodeConnection;
            }else if (frontNode.frontNodeConnection != null)
            {
                connectionsToDistanceTest = new PathFollowCamNodeConnection[2];
                connectionsToDistanceTest[0] = this;
                connectionsToDistanceTest[1] = frontNode.frontNodeConnection;
            }else 
            {
                connectionsToDistanceTest = new PathFollowCamNodeConnection[1];
                connectionsToDistanceTest[0] = this;
            }*/
        }

        /*public static void ActivateNodeConnection(PathFollowCamNodeConnection _nodeConnection)
        {
            if (_nodeConnection == null)
            {
                return;
            }
		
            if (!_nodeConnection.isActive)
            {
                DeactivateNode(activeNodeConnection);
                activeNodeConnection = _nodeConnection;
            }
        }
	
        public static void DeactivateNode(PathFollowCamNodeConnection _nodeConnection)
        {
            if (_nodeConnection == null)
            {
                return;
            }
		
            if (_nodeConnection.isActive)
            {
                activeNodeConnection = null;
            }
        }
	
        public void Activate()
        {
            PathFollowCamNodeConnection.ActivateNodeConnection(this);
        }*/

        public PathFollowCamNodeConnection CheckDistanceOnConnections(Vector3 _pos)
        {
            return CheckDistanceOnConnections(_pos, null, 0);
        }
        public PathFollowCamNodeConnection CheckDistanceOnConnections(Vector3 _pos, PathFollowCamNodeConnection _lastConnection, float _changeToLastConnectionThresshold)
        {

            //Gizmos.color = Color.green;
            Debug.DrawLine(backNode.transform.position, frontNode.transform.position, Color.green);


            Vector3 distVec;
            float dist;

            int closestConnectionIndex = 0;

            //Debug.Log("nodeConnections[0]  "+nodeConnections[0]);

            connectionsToDistanceTest.Clear();
            connectionsToDistanceTest.Add(this);
            if (frontNode.frontNodeConnection != null)
            {
                connectionsToDistanceTest.Add(frontNode.frontNodeConnection);
            }
            if (backNode.backNodeConnection != null)
            {
                connectionsToDistanceTest.Add(backNode.backNodeConnection);
            }

            distVec = ((PathFollowCamNodeConnection)connectionsToDistanceTest[0]).GetOrthogonalDistanceVector(_pos);
            float closestDist = distVec.magnitude;
            //Debug.DrawRay(_pos, distVec, Color.Lerp(Color.red, Color.yellow, 0));
            for (int i = 1; i < connectionsToDistanceTest.Count; i++)
            {
                distVec = ((PathFollowCamNodeConnection)connectionsToDistanceTest[i]).GetOrthogonalDistanceVector(_pos);

                dist = distVec.magnitude;

                if (_lastConnection != null && (PathFollowCamNodeConnection)connectionsToDistanceTest[i] != this && (PathFollowCamNodeConnection)connectionsToDistanceTest[i] == _lastConnection)
                {
                    dist += _changeToLastConnectionThresshold;

                    //Debug.DrawRay(_pos, distVec.normalized * dist, Color.Lerp(Color.red, Color.yellow, (float)(i + 1) / (float)connectionsToDistanceTest.Count));

                }
                else
                {
                    Debug.DrawRay(_pos, distVec, Color.Lerp(Color.red, Color.yellow, (float)(i + 1) / (float)connectionsToDistanceTest.Count));
                }

                if (dist <= closestDist)
                {
                    closestConnectionIndex = i;
                    closestDist = dist;
                }
            }

            //((PathFollowCamNodeConnection)connectionsToDistanceTest[closestConnectionIndex]).Activate();

            return ((PathFollowCamNodeConnection)connectionsToDistanceTest[closestConnectionIndex]);
        }

        public Vector3 GetPositionOnCameraPath(Vector3 _pos)
        {
            Vector3 localPos = transform.InverseTransformPoint(_pos);
            localPos.y = 0;

            if (isFirstConnection)
            {
                localPos.x = Mathf.Max(0, localPos.x);
            }

            if (isLastConnection)
            {
                localPos.x = Mathf.Min(connectionDistance, localPos.x);
            }


            return transform.TransformPoint(localPos);
        }

        /** 
         * Returns the position INSIDE the path, that is, allways between the connected nodes. Used to stop the camera in the beginning and end of the entire path
         * */
        /*public Vector3 GetPositionInsideCameraPath(Vector3 _pos)
        {
		
		
            Vector3 localPos = transform.InverseTransformPoint(_pos);
            localPos.y = 0;
		
            Debug.Log("GetPositionInsideCameraPath. local pos before inside : "+localPos);
		
		
            localPos.x = Mathf.Max(0,localPos.x);
            localPos.x = Mathf.Min(connectionDistance,localPos.x);
		
            Debug.Log("GetPositionInsideCameraPath. local pos : "+localPos);
		
            return transform.TransformPoint(localPos);
        }*/

        public Vector3 GetOrthogonalDistanceVector(Vector3 _pos)
        {
            Vector3 localPos = transform.InverseTransformPoint(_pos);
            if (localPos.x < 0)
            {
                return backNode.transform.position - _pos;
            }
            if (localPos.x > connectionDistance)
            {
                return frontNode.transform.position - _pos;
            }

            return GetPositionOnCameraPath(_pos) - _pos;
        }

        public Quaternion GetRotation(Vector3 _pos)
        {
            float progression = GetProgression(_pos);

            if (progression < 0.5f)
            {
                return Quaternion.Lerp(backNode.transform.rotation, transform.rotation, progression * 2);
            }
            else
            {
                return Quaternion.Lerp(transform.rotation, frontNode.transform.rotation, (progression - 0.5f) * 2);
            }

        }

        public float GetProgression(Vector3 _pos)
        {
            Vector3 localPos = transform.InverseTransformPoint(_pos);
            return Mathf.Clamp01(localPos.x / connectionDistance);
        }
    }
}