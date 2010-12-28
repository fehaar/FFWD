using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class PathFollowCam : MonoBehaviour {
		
		//private CamState currentCamState;
		
		private GameObject automaticMoveObject;
		private WaypointMover automaticFollowObjectMover;
		private Waypoint[] currentAutomaticMovementWaypoints;
		
		private PathFollowCamStats defaultStats;
		
		public PathFollowCamStats stats;
		
		public float[] followObjectWeights;
		public Transform[] followObjects;
		
		private GameObject rotationAnchor;
		public Transform rotationAnchorPosition;
		
		public GameObject followObject;
		//public Rigidbody followObjectRigidbody;
	
		// Used for shaking the Camera!
		public GameObject camShakeObject;
		//private float shake = 0;
		//public float shakeAmount = 0.7f;
		//public float shakeDecrease = 1.0f;
		//private bool shaking = false;
		//private bool isCameraDisplaced = false;
		
		public GameObject childCam;
		
		public Camera backgroundCamera;
		
	    public Camera raycastCamera;
	
	    public Camera GUICamera;

		[ContentSerializerIgnore]
		public PathFollowCamNodeConnection getCurrentActiveConnection{
			get{return currentActiveConnection;}
		}
	
	    public string getCurrentNodeName
	    {
	        get
	        {
	            if (currentActiveConnection.backNode == null)
	            {
	                return "NULL";
	            }
	            else
	            {
	                return currentActiveConnection.backNode.name;
	            }
	        }
	    }
	
		private PathFollowCamNodeConnection currentActiveConnection;
		private PathFollowCamNodeConnection lastActiveConnection;
		
		
		private PathFollowCamNode[] nodes;
		private PathFollowCamNodeConnection[] nodeConnections;
		
		public static bool isLoaded = false;
		private static PathFollowCam instance;
		
		private Vector3 childCamPos;
		private Vector3 gotoPos;
		private Quaternion gotoRotation;
		
		private bool isLocked = false;
	
		
		/*public bool automaticMovement{
			get{
				return state == PathFollowCam.State.followPathAutomatic;
			}
		}*/
		
		//public float moveSpeed = 4;
		
		private Vector3 followObjectPositionInViewPort = Vector3.zero;
	
	    private PathFollowCam.State state = PathFollowCam.State.followPath;
		public enum State{
			followPath,
			placeFollowObjectInViewPort,
			placeBetweenFollowObjects
		}
		
		
		//private float currentConnectionTraversalFraction;
		//private GameObject autoMoveCamMover;
		
		public static PathFollowCam Instance
	    {
	        get
	        {
	            if (instance == null)
	            {
	                Debug.LogError("Attempt to access instance of PathFollowCamHandler singleton earlier than Start or without it being attached to a GameObject.");
	            }
	
	            return instance;
	        }
	    }
		
		
		/**
		 * Sets the camera state
		 * */
		public void SetCamState(CamState _camState)
		{
			stats = _camState.stats;
			ChangeState(_camState.state);
			
			followObjectPositionInViewPort = _camState.objectPositionInViewport;
			
			
		}
		
		void ChangeState(State _state)
		{
			state = _state;
			switch(_state)
			{
			case State.followPath:
				break;
			}
	
		}
		
		public void SetBackgroundColor(Color _color)
		{
			backgroundCamera.backgroundColor = _color;
		}
		
		public Vector3 GetForwardDirection()
		{
			return currentActiveConnection.transform.right;
			//return childCam.transform.right;
		}
		
		public override void Awake()
	    {
	        if (instance != null)
	        {
	            Debug.LogError("Cannot have two instances of PathFollowCamHandler. Self destruction in 3...");
	            Destroy(this);
	            return;
	        }
			
			isLoaded = true;
	        instance = this;
	
			Initialize();	
	    }
		
		// Use this for initialization
		public void Initialize () 
		{
			/*if (automaticMovement)
			{
				//followObject = (GameObject)(Instantiate(new GameObject()));
				followObject = new GameObject();
				followObject.name = "Camera mover target";
				
				Debug.Log("Followobject Created");
			}else*/
			//{
				if (followObject == null)
				{
					followObject = ((Lemmy)FindObjectOfType(typeof(Lemmy))).gameObject;
					//followObjectRigidbody = followObject.rigidbody;
				}
			//}
			GameObject tmpGameObject =  new GameObject();
			automaticFollowObjectMover = (WaypointMover)tmpGameObject.AddComponent(typeof(WaypointMover));
			automaticFollowObjectMover.name = "_Path Follow Cam Automatic FollowObject Mover";
			automaticMoveObject = new GameObject();
			automaticMoveObject.transform.parent = automaticFollowObjectMover.transform;
			automaticMoveObject.name = "Automatic Move Object";
			currentAutomaticMovementWaypoints = CreateWaypointPathForCamMovement(automaticFollowObjectMover.gameObject.transform,Vector3.zero,Vector3.zero,new PathFollowCamNode[0]);
			automaticFollowObjectMover.startWaypoint = currentAutomaticMovementWaypoints[0];
			
			
			CreateArrayOf_PathFollowCamNodes();
			
			defaultStats = stats;
			
			//ForcedMoveToFollowObjectPosition();
		}
		
		public void ChangeToDefaultStats()
		{
			stats = defaultStats;
		}
		
		/*public void ForcedMoveToFollowObjectPosition()
		{
			childCam.transform.localPosition = new Vector3(0,0,0);
			transform.position = followObject.transform.position;///currentActiveConnection.GetPositionOnCameraPath(followObject.transform.position);
			transform.rotation = currentActiveConnection.transform.rotation;
		}*/
		
		public void PlaceObjectInViewPort(Vector3 _viewPortPosition)
		{
			followObjectPositionInViewPort = _viewPortPosition;
	
			ChangeState(PathFollowCam.State.placeFollowObjectInViewPort);
			
		}
		
		public void FollowPathDefaultStats()
		{
			ChangeToDefaultStats();
			ChangeState(PathFollowCam.State.followPath);
		}
		
		public void PlaceBetweenFollowObjects(Transform[] _followObjects, Transform _rotationAnchor)
		{
			float[] tmpWeights = new float[_followObjects.Length];
			for (int i = 0; i < tmpWeights.Length; i++) {
				tmpWeights[i] = 1;
			}
			
			PlaceBetweenFollowObjects(_followObjects, tmpWeights, _rotationAnchor);
		}
		
		public void PlaceBetweenFollowObjects(Transform[] _followObjects, float[] _weights, Transform _rotationAnchor)
		{
			if (_followObjects.Length != _weights.Length)
			{
				Debug.LogError("_followObjects and _weights have different length. They MUSY have same length");
				return;
			}
			
			rotationAnchorPosition = _rotationAnchor;
			
			followObjects = _followObjects;
			followObjectWeights = _weights;
			
			ChangeState(State.placeBetweenFollowObjects);
		}
		
		
		/*public void ShakeCamera(float shakeTime){
			this.shake = shakeTime;
			shaking = true;
		}*/
		
		public Quaternion GetRotationFromAnchor(Vector3 _camPosition)
		{
			if (rotationAnchorPosition == null)
			{
				return transform.rotation;
			}
			
			if (rotationAnchor == null)
			{
				rotationAnchor = new GameObject();
				rotationAnchor.name = "Camera rotation anchor";
			}
			
			rotationAnchor.transform.position = rotationAnchorPosition.position;
			
			//rotationAnchor.transform.LookAt(_camPosition);
			
			Vector3 direction = rotationAnchor.transform.position - _camPosition;
			
			rotationAnchor.transform.LookAt( rotationAnchor.transform.position + Vector3.down, new Vector3(-direction.x, 0, -direction.z));
			
			return rotationAnchor.transform.rotation;
		}
		
		/*private void DoShake()
		{
			// This shakes the Camera
			if (shaking && shake > 0) { 			
				camShakeObject.transform.localPosition += Random.insideUnitSphere * shakeAmount;
				shake -= Time.deltaTime * shakeDecrease; 
				isCameraDisplaced = true;
			} else if(isCameraDisplaced) { 
				shake = 0.0f;
				shaking = false;
				
				Vector3 returnToStartPosition = Vector3.zero - camShakeObject.transform.localPosition;
				
				// This returns the position of the camera.
				if(returnToStartPosition.magnitude > 0.1f){
					camShakeObject.transform.localPosition += returnToStartPosition.normalized * 0.1f;
				}else{
					camShakeObject.transform.localPosition = Vector3.zero;
					isCameraDisplaced = false;
				}
			}
		}*/
	
	    // This is the iTween version!
	    public void ShakeCamera(Vector3 amount, float time)
	    {
	        iTween.ShakeRotation(camShakeObject, iTween.Hash("amount", amount, "time", time));
	    }
	
		private void CreateArrayOf_PathFollowCamNodes()
		{
			Object[] tmpNodeArray = FindObjectsOfType(typeof(PathFollowCamNode));
			
			nodes = new PathFollowCamNode[tmpNodeArray.Length];
	
			for (int i = 0; i < tmpNodeArray.Length; i++) {
				nodes[i] = (PathFollowCamNode)(tmpNodeArray[i]);
				
				if (((PathFollowCamNode)tmpNodeArray[i]).nextNode != null)
				{
					((PathFollowCamNode)tmpNodeArray[i]).nextNode.previousNode = (PathFollowCamNode)(tmpNodeArray[i]);
				}
			}
			
			PathFollowCamNode firstNode = nodes[0];
			for (int i = 0; i < nodes.Length; i++) {
				if (nodes[i].previousNode == null)
				{
					firstNode = nodes[i];
				}
			}

            List<PathFollowCamNode> tmpSortedNodes = new List<PathFollowCamNode>();
			PathFollowCamNode curNode = firstNode;
			while(curNode.nextNode != null)
			{
				tmpSortedNodes.Add(curNode);
				curNode = curNode.nextNode;
			}
			tmpSortedNodes.Add(curNode);
			
			nodes = new PathFollowCamNode[tmpSortedNodes.Count];
			for (int i = 0; i < nodes.Length; i++) {
				nodes[i] = (PathFollowCamNode)(tmpSortedNodes[i]);
			}
			
			
			//create node connections
			nodeConnections = new PathFollowCamNodeConnection[nodes.Length-1];
			PathFollowCamNodeConnection tmpNodeConnection;
			GameObject tmpGameObject;
			int nodeConnectionIndex = 0;
			for (int i = 0; i <nodeConnections.Length ; i++) {
				
				/*if (nodes[i].nextNode == null)
				{
					continue;
				}*/	
				
				tmpGameObject = new GameObject();
				tmpNodeConnection = (PathFollowCamNodeConnection)(tmpGameObject.AddComponent(typeof(PathFollowCamNodeConnection)));
				tmpNodeConnection.Initialize(nodes[i], nodes[i].nextNode);
				
				tmpNodeConnection.name = ("Node Connection "+i).ToString();
				
				//Debug.Log("new connection created "+tmpNodeConnection.name);
				
				nodeConnections[i] = tmpNodeConnection;
				
				//nodeConnectionIndex++;
			}
			
					
			//turn nodes
			foreach(PathFollowCamNodeConnection _connection in nodeConnections)
			{
				if (_connection.backNode.backNodeConnection != null)
				{
					_connection.backNode.transform.rotation = Quaternion.Lerp(_connection.backNode.backNodeConnection.transform.rotation, _connection.transform.rotation, 0.5f);
				}else
				{
					_connection.backNode.isFirstNode = true;
					_connection.backNode.transform.rotation = _connection.transform.rotation;
				}
				
				if (_connection.frontNode.frontNodeConnection == null)
				{
					_connection.backNode.isLastNode = true;
					_connection.frontNode.transform.rotation = _connection.transform.rotation;
				}
			}
	
			
			
			currentActiveConnection = GetClosestConnection(followObject.transform.position);
			
		}
	
		public void ActivateClosestConnection(Vector3 _pos)
		{
			lastActiveConnection = currentActiveConnection;
			currentActiveConnection = GetClosestConnection(_pos);
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
	
			for (int i = 0; i < nodeConnections.Length; i++) {
				distVec = nodeConnections[i].GetOrthogonalDistanceVector(_pos);
				sqrtDist = distVec.sqrMagnitude;
				
				if (sqrtDist < closestDist)
				{
					closestConnectionIndex = i;
					closestDist = sqrtDist;
				}
			}
	
			Debug.DrawRay(_pos, nodeConnections[closestConnectionIndex].GetOrthogonalDistanceVector(_pos), Color.green);
			
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
		public override void Update () {
			
			if (isLocked)
			{
				return;
			}
			
			//DoShake();
			
			
			//update active node connection
			PathFollowCamNodeConnection tmpConnection = currentActiveConnection;
			currentActiveConnection = currentActiveConnection.CheckDistanceOnConnections(followObject.transform.position, lastActiveConnection, stats.changeToLastConnectionThresshold);
			
			
			//currentActiveConnection
			
			
			if (currentActiveConnection != tmpConnection)
			{lastActiveConnection = tmpConnection;}
			
			switch(state)
			{
			
			case State.followPath:
				MoveCameraTowards(GetFollowPathPosition(currentActiveConnection));
				break;
			
			case State.placeBetweenFollowObjects:
				PathFollowCamPosition pos = new PathFollowCamPosition();
				pos.gotoPos = GetPositionBetweenFollowObjects();
				pos.childCamPos = new Vector3(0,0,-stats.defaultHeight);
				
				pos.gotoRotation = GetRotationFromAnchor(pos.gotoPos);
				
				MoveCameraTowards(pos);
				break;
			
			case State.placeFollowObjectInViewPort:
				MoveCameraTowards(GetPositionForPlaceInViewport(currentActiveConnection));
				break;
			}
			
			/*if (automaticMovement)
			{
				MoveFollowObjectInAutomaticMoveMode();
				MoveCameraAccordingToNodeConnection(currentActiveConnection);
			}
			else
			{
				//Debug.DrawLine(lastActiveConnection.frontNode.transform.position, lastActiveConnection.backNode.transform.position, Color.yellow);
				
				//Debug.DrawLine(currentActiveConnection.frontNode.transform.position, currentActiveConnection.backNode.transform.position, Color.green);
				
				PathFollowCamNodeConnection tmpConnection = currentActiveConnection;
				currentActiveConnection = currentActiveConnection.CheckDistanceOnConnections(followObject.transform.position, lastActiveConnection, stats.changeToLastConnectionThresshold);
				
				if (currentActiveConnection != tmpConnection)
				{lastActiveConnection = tmpConnection;}
							
				MoveCameraAccordingToNodeConnection(currentActiveConnection);
			}*/
		}
		
		public void MoveToStablePosition()
		{
			PathFollowCamNodeConnection tmpConnection = currentActiveConnection;
			currentActiveConnection = currentActiveConnection.CheckDistanceOnConnections(followObject.transform.position, lastActiveConnection, stats.changeToLastConnectionThresshold);
			
			if (currentActiveConnection != tmpConnection)
			{lastActiveConnection = tmpConnection;}
			
			switch(state)
			{
			
			case State.followPath:
				MoveCameraTo(GetFollowPathPosition(currentActiveConnection));
				break;
			
			case State.placeBetweenFollowObjects:
				PathFollowCamPosition pos = new PathFollowCamPosition();
				pos.gotoPos = GetPositionBetweenFollowObjects();
				pos.childCamPos = new Vector3(0,0,-stats.defaultHeight);
				
				pos.gotoRotation = GetRotationFromAnchor(pos.gotoPos);
				
				MoveCameraTo(pos);
				break;
			
			case State.placeFollowObjectInViewPort:
				MoveCameraTo(GetPositionForPlaceInViewport(currentActiveConnection));
				break;
			}
			
					
			//Debug.Break();
		}
		
		/*private void MoveFollowObjectInAutomaticMoveMode()
		{
			if (followObject == null)
			{
				return;
			}
			
			Vector3 dir = currentActiveConnection.frontNode.transform.position - followObject.transform.position; 
			Debug.DrawRay(followObject.transform.position, dir.normalized * Time.deltaTime * moveSpeed, Color.magenta);
			followObject.transform.position += dir.normalized * Time.deltaTime * moveSpeed;
			
			//HACK!!!
			if (currentActiveConnection.transform.InverseTransformPoint(followObject.transform.position).x > currentActiveConnection.connectionDistance-0.1f)
			{
				currentActiveConnection = currentActiveConnection.frontNode.frontNodeConnection;
			}
		}*/
		
		private Vector3 GetPositionBetweenFollowObjects()
		{
			float weightTotal = 0;
			Vector3 averagePos = Vector3.zero;
			for (int i = 0; i < followObjects.Length; i++) {
				//Debug.Log("followObjects[i].position : "+followObjects[i].position);
				averagePos += followObjects[i].position;
				weightTotal += 1;//followObjectWeights[i];
			}
			averagePos /= weightTotal;
			
			//Debug.Log("GetPositionBetweenFollowObjects : "+averagePos);
			
			return averagePos;
		}
		
		
		
		private PathFollowCamPosition GetFollowPathPosition(PathFollowCamNodeConnection _nodeConnection)
		{
			PathFollowCamPosition pos = new PathFollowCamPosition();
			
			Vector3 lookAhead = Vector3.zero;
			float speedMod = 0;
			if (followObject.rigidbody != null)
			{
				speedMod = Mathf.Pow((followObject.rigidbody.velocity.sqrMagnitude+1),stats.speedCurvePow)-1 - stats.speedModThresshold;
				if (speedMod < 0){speedMod = 0;}
				
				lookAhead = stats.speedLookAhead * speedMod * followObject.rigidbody.velocity.normalized;
			}
			
			
			if (currentActiveConnection.centerFollowObjectInViewPort)
			{
				gotoPos = followObject.transform.position;
			}else
			{
				gotoPos = _nodeConnection.GetPositionOnCameraPath(followObject.transform.position + lookAhead);
				/*if (_nodeConnection.isLastConnection || _nodeConnection.isFirstConnection)
				{
					gotoPos = _nodeConnection.GetPositionInsideCameraPath(followObject.transform.position + lookAhead);
				}else
				{
					gotoPos = _nodeConnection.GetPositionOnCameraPath(followObject.transform.position + lookAhead);
				}*/
			}
			
			float distToFront = (gotoPos - _nodeConnection.frontNode.transform.position).magnitude;
			float distToBack = (gotoPos - _nodeConnection.backNode.transform.position).magnitude;
			
			float connectionDist = distToBack + distToFront;
			
			if (_nodeConnection.frontNode.affectCameraRotation)
			{
				gotoRotation = _nodeConnection.transform.rotation;
				
				float blendRotationDist = 3;
				
				if (distToBack < blendRotationDist)
				{
					gotoRotation = Quaternion.Lerp(_nodeConnection.backNode.transform.rotation,_nodeConnection.transform.rotation,distToBack/blendRotationDist);
				}
				if (distToFront < blendRotationDist)
				{
					gotoRotation = Quaternion.Lerp(_nodeConnection.frontNode.transform.rotation,_nodeConnection.transform.rotation,distToFront/blendRotationDist);
				}
				
			}
			
			pos.gotoPos = gotoPos;
			pos.gotoRotation = gotoRotation;
			pos.speedMod = speedMod;
			
			float lerpedCamHeight = -(_nodeConnection.frontNode.camHeight - (distToFront/connectionDist)*(_nodeConnection.frontNode.camHeight - _nodeConnection.backNode.camHeight));	
			
			
			
			childCamPos.x = stats.defaultLookAhead;
			childCamPos.y = _nodeConnection.frontNode.yOffset;
			childCamPos.z = -stats.defaultHeight - speedMod*stats.speedZoomOut;
			if (_nodeConnection.frontNode.forceCamLookAhead)
			{
				//childCamPos.x = _nodeConnection.frontNode.camLookAhead;
				childCamPos.x = _nodeConnection.frontNode.camLookAheadFraction * (-lerpedCamHeight);
			}
			if (_nodeConnection.frontNode.forceCamHeight)
			{
				childCamPos.z = lerpedCamHeight;
			}
			
			pos.childCamPos = childCamPos;
			
			//Debug.LogWarning("childCamPos "+childCamPos);
			
			return pos;
		}
		
		private PathFollowCamPosition GetPositionForPlaceInViewport(PathFollowCamNodeConnection _nodeConnection)
		{
			PathFollowCamPosition pos = new PathFollowCamPosition();
			Vector3 lookAhead = Vector3.zero;
			float speedMod = 0;
			if (followObject.rigidbody != null)
			{
				speedMod = Mathf.Pow((followObject.rigidbody.velocity.sqrMagnitude+1),stats.speedCurvePow)-1 - stats.speedModThresshold;
				if (speedMod < 0){speedMod = 0;}
				
				lookAhead = stats.speedLookAhead * speedMod * followObject.rigidbody.velocity.normalized;
			}
			
			gotoPos = followObject.transform.position;
	
			float distToFront = (gotoPos - _nodeConnection.frontNode.transform.position).magnitude;
			float distToBack = (gotoPos - _nodeConnection.backNode.transform.position).magnitude;
			
			float connectionDist = distToBack + distToFront;
			
			if (_nodeConnection.frontNode.affectCameraRotation)
			{
				gotoRotation = _nodeConnection.transform.rotation;
				
				float blendRotationDist = 3;
				
				if (distToBack < blendRotationDist)
				{
					gotoRotation = Quaternion.Lerp(_nodeConnection.backNode.transform.rotation,_nodeConnection.transform.rotation,distToBack/blendRotationDist);
				}
				if (distToFront < blendRotationDist)
				{
					gotoRotation = Quaternion.Lerp(_nodeConnection.frontNode.transform.rotation,_nodeConnection.transform.rotation,distToFront/blendRotationDist);
				}
				
			}
			
			childCamPos.x = -followObjectPositionInViewPort.x;
			childCamPos.y = -followObjectPositionInViewPort.y;
			childCamPos.z = -followObjectPositionInViewPort.z;
			
			pos.gotoPos = gotoPos;
			pos.gotoRotation = gotoRotation;
			pos.childCamPos = childCamPos;
			
			return pos;
		}
		
		
		/**
		 * This moves camera towards position and rotation using the speed in stats
		 * */
		void MoveCameraTowards(PathFollowCamPosition pos)
		{
			//Debug.Log("MoveCameraTowards child pos : "+pos.childCamPos);
			
			
			if (pos.gotoPos != null)
			{
				transform.position = Vector3.Lerp(transform.position, pos.gotoPos,(stats.moveStiffness + (pos.speedMod*stats.speedMoveStiffnessMod)) * Time.deltaTime);
			}
			
			if (pos.gotoRotation != null)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, pos.gotoRotation, (stats.turnStiffness + (pos.speedMod*stats.speedTurnStiffnessMod)) * Time.deltaTime);
			}
			
			if (pos.childCamPos != null)
			{
				childCam.transform.localPosition = Vector3.Lerp(childCam.transform.localPosition, pos.childCamPos, stats.lookAheadAndHeightStiffnes * Time.deltaTime);
			}
		}
		
		/**
		 * This moves camera TO position and rotation using the speed in stats
		 */
		void MoveCameraTo(PathFollowCamPosition pos)
		{
			if (pos.gotoPos != null)
			{
				transform.position = pos.gotoPos;
			}
			if (pos.gotoRotation != null)
			{
				transform.rotation = pos.gotoRotation;
			}
			if (pos.childCamPos != null)
			{
				childCam.transform.localPosition = pos.childCamPos;
			}
		}
		
		/// <summary>
		/// DOES NOT WORK YET!!!!! Starts the respawn movement.
		/// </summary>
		/// <param name='_startPosition'>
		/// _start position.
		/// </param>
		/// <param name='_endPosition'>
		/// _end position.
		/// </param>
		/// <param name='_movementTime'>
		/// _movement time.
		/// </param>
		/*public void StartRespawnMovement(Vector3 _startPosition, Vector3 _endPosition, float _movementTime)
		{
			//DestroyCurrentAutomaticMovementWaypoints();
			
			ArrayList nodesBetweenList = new ArrayList();
			
			PathFollowCamNodeConnection startConnection = GetClosestConnection(_startPosition);
			PathFollowCamNodeConnection endConnection = GetClosestConnection(_endPosition);
			
			
			//if the connections are NOT the same add nodes in between
			PathFollowCamNode currentNode = startConnection.backNode;
			if (startConnection != endConnection)
			{
				while(currentNode != endConnection.frontNode)
				{
					nodesBetweenList.Add(currentNode);
					currentNode = currentNode.backNodeConnection.backNode;
				}
				nodesBetweenList.Add(currentNode);
			}
			
			PathFollowCamNode[] tmpNodes = (PathFollowCamNode[])(nodesBetweenList.ToArray(typeof(PathFollowCamNode)));
	
			currentAutomaticMovementWaypoints = CreateWaypointPathForCamMovement(automaticFollowObjectMover.gameObject.transform,_startPosition, _endPosition, tmpNodes);
			
			automaticFollowObjectMover.movementTarget = automaticMoveObject.transform;
			
			//automaticFollowObjectMover.sequenceLengthOverride = _movementTime;
			
			automaticFollowObjectMover.SetWaypointsAndDoOneShotMovement(currentAutomaticMovementWaypoints, _movementTime);
			
			
			
			//stats.moveStiffness = 20;
			//stats.turnStiffness = 20;
			
			followObject = automaticMoveObject;
			
			//Debug.Break();
		}*/
		
		/*void DestroyCurrentAutomaticMovementWaypoints()
		{
			for (int i = 0; i < currentAutomaticMovementWaypoints.Length; i++) {
				Destroy(currentAutomaticMovementWaypoints[i].gameObject);
			}
			
			currentAutomaticMovementWaypoints = new Waypoint[0];
		}*/
		
		Waypoint[] CreateWaypointPathForCamMovement(Transform _parent, Vector3 _startPosition, Vector3 _endPosition, PathFollowCamNode[] inBetweenCamNodes)
		{
			Waypoint[] tmpWaypoints = new Waypoint[2+inBetweenCamNodes.Length];
			
			tmpWaypoints[0] = Waypoint.CreateWaypoint(_startPosition, _parent);
			for (int i = 0; i < inBetweenCamNodes.Length; i++) {
				tmpWaypoints[1+i] = Waypoint.CreateWaypoint(inBetweenCamNodes[i].transform.position, _parent);
				
				//connect earlier wapoint to the recently created one
				tmpWaypoints[i].nextWaypoint = tmpWaypoints[i+1];
			}
			
			tmpWaypoints[tmpWaypoints.Length-1] = Waypoint.CreateWaypoint(_endPosition, _parent);
			tmpWaypoints[tmpWaypoints.Length-2].nextWaypoint = tmpWaypoints[tmpWaypoints.Length-1];
			
			return tmpWaypoints;
		}
		
		
	}
	
		
}