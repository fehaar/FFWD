using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts
{
    public class PathFollowObject : MonoBehaviour
    {

        //public PathFollowCamStats stats;

        public GameObject followObject;

        private PathFollowCamNode[] nodes;
        private PathFollowCamNodeConnection[] nodeConnections;

        private Vector3 gotoPos;
        private Quaternion gotoRotation;

        private bool isLocked = false;

        public bool automaticMovement = false;
        public float moveSpeed = 4;

        //private float currentConnectionTraversalFraction;

        private PathFollowCamNodeConnection currentActiveConnection;

        private PathFollowCamNodeConnection startConnection;

        public override void Awake()
        {


            Initialize();
        }

        // Use this for initialization
        public void Initialize()
        {


            if (automaticMovement)
            {
                followObject = (GameObject)(Instantiate(new GameObject(), transform.position, transform.rotation));
                followObject.name = "Object mover target";

                Debug.Log("Followobject Created");
            }
            else
            {
                followObject = ((Lemmy)FindObjectOfType(typeof(Lemmy))).gameObject;
            }

            CreateArrayOf_PathFollowCamNodes();
        }


        private void CreateArrayOf_PathFollowCamNodes()
        {
            UnityObject[] tmpNodeArray = FindObjectsOfType(typeof(PathFollowCamNode));

            nodes = new PathFollowCamNode[tmpNodeArray.Length];

            for (int i = 0; i < tmpNodeArray.Length; i++)
            {

                /*int index = int.Parse(tmpNodeArray[i].name.Split('_')[1]);
			
                if (index > nodes.Length-1)
                {
                    Debug.LogError("The named index is too large for number of nodes : "+index + " the new index is " + (nodes.Length-1).ToString());
                    index = nodes.Length-1;
                }
			
                if (nodes[index] != null)
                {
                    Debug.LogError("There are two nodes with named index : "+index);
                }*/

                nodes[i] = (PathFollowCamNode)(tmpNodeArray[i]);
            }

            //create node connections
            nodeConnections = new PathFollowCamNodeConnection[nodes.Length - 1];
            PathFollowCamNodeConnection tmpNodeConnection;
            GameObject tmpGameObject;
            int nodeConnectionIndex = 0;
            for (int i = 0; i < nodes.Length; i++)
            {

                if (nodes[i].nextNode == null)
                {
                    continue;
                }


                tmpGameObject = (GameObject)Instantiate(new GameObject());
                tmpNodeConnection = (PathFollowCamNodeConnection)(tmpGameObject.AddComponent(typeof(PathFollowCamNodeConnection)));
                tmpNodeConnection.Initialize(nodes[i], nodes[i].nextNode);

                tmpNodeConnection.name = ("Node Connection " + i).ToString();

                nodeConnections[nodeConnectionIndex] = tmpNodeConnection;

                nodeConnectionIndex++;
            }

            currentActiveConnection = GetClosestConnection(followObject.transform.position);
            startConnection = currentActiveConnection;
        }

        public PathFollowCamNodeConnection GetClosestConnection(Vector3 _pos)
        {
            if (nodeConnections.Length == 0)
            {
                return null;
            }

            Vector3 distVec;
            float sqrtDist;

            int closestConnectionIndex = 0;

            //Debug.Log("nodeConnections[0]  "+nodeConnections[0]);

            float closestDist = nodeConnections[0].GetOrthogonalDistanceVector(_pos).sqrMagnitude;

            for (int i = 0; i < nodeConnections.Length; i++)
            {
                distVec = nodeConnections[i].GetOrthogonalDistanceVector(_pos);
                sqrtDist = distVec.sqrMagnitude;

                if (sqrtDist < closestDist)
                {
                    closestConnectionIndex = i;
                    closestDist = sqrtDist;
                }
            }

            //Debug.DrawRay(_pos, nodeConnections[closestConnectionIndex].GetOrthogonalDistanceVector(_pos), Color.green);

            return nodeConnections[closestConnectionIndex];
        }

        public void Lock()
        {
            isLocked = true;
        }

        public void Unlock()
        {
            isLocked = false;
        }

        // Update is called once per frame
        public override void Update()
        {

            if (isLocked)
            {
                return;
            }

            if (automaticMovement)
            {
                MoveFollowObjectInAutomaticMoveMode();
                MoveCameraAccordingToNodeConnection(currentActiveConnection);
            }
            else
            {
                currentActiveConnection = currentActiveConnection.CheckDistanceOnConnections(followObject.transform.position);
                MoveCameraAccordingToNodeConnection(currentActiveConnection);
            }
        }

        private void MoveFollowObjectInAutomaticMoveMode()
        {
            if (followObject == null)
            {
                return;
            }

            Vector3 dir = currentActiveConnection.frontNode.transform.position - followObject.transform.position;
            Debug.DrawRay(followObject.transform.position, dir.normalized * Time.deltaTime * moveSpeed, Color.magenta);
            followObject.transform.position += dir.normalized * Time.deltaTime * moveSpeed;

            if (currentActiveConnection.transform.InverseTransformPoint(followObject.transform.position).x > currentActiveConnection.connectionDistance - 0.2f)
            {
                currentActiveConnection = currentActiveConnection.frontNode.frontNodeConnection;
            }
        }

        private void MoveCameraAccordingToNodeConnection(PathFollowCamNodeConnection _nodeConnection)
        {
            gotoPos = _nodeConnection.GetPositionOnCameraPath(followObject.transform.position);
            //gotoRotation = _nodeConnection.GetRotation(followObject.transform.position);
            gotoRotation = _nodeConnection.transform.rotation;

            transform.position = Vector3.Lerp(transform.position, gotoPos, 0.04f);
            transform.rotation = Quaternion.Lerp(transform.rotation, gotoRotation, 0.02f);
        }

        public void Reset()
        {
            currentActiveConnection = startConnection;
            transform.position = startConnection.backNode.transform.position;
            followObject.transform.position = startConnection.backNode.transform.position;
            followObject.transform.rotation = startConnection.backNode.transform.rotation;
        }

        public void GotoNode(PathFollowCamNode _node)
        {
            currentActiveConnection = _node.frontNodeConnection;
            transform.position = _node.transform.position;
            followObject.transform.position = _node.frontNodeConnection.transform.position;
            followObject.transform.rotation = _node.frontNodeConnection.transform.rotation;
        }
    }
}