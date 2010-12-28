using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class PathFollowCamNode : MonoBehaviour {
	
		[ContentSerializerIgnore]
		public bool isFirstNode = false;
		[ContentSerializerIgnore]
		public bool isLastNode = false;

        [ContentSerializerIgnore]
        public PathFollowCamNode nextNode;
		
		[ContentSerializerIgnore]
		public PathFollowCamNode previousNode;
		
		
		[ContentSerializerIgnore]
		public PathFollowCamNodeConnection frontNodeConnection;
		[ContentSerializerIgnore]
		public PathFollowCamNodeConnection backNodeConnection;
		
		public bool forceCamHeight = false;
		public bool forceCamLookAhead = false;
		public float camHeight = 15;
		
		/// <summary>
		/// The cam look ahead. THIS IS OBSOLETE!!!
		/// </summary>
		//public float camLookAhead = 5;
		
		public float camLookAheadFraction = 0.6f;
		public bool useCamLookAheadFraction = true;
		
		public float yOffset = 0;
		
		public bool affectCameraRotation = true;
		
		public bool centerFollowObjectInViewPort = false; 
		
		// Use this for initialization
		/*public override void Start () {
		
		}*/
		
		// Update is called once per frame
		/*public override void Update () {
		
		}*/
		
		//private CapsuleCollider capsuleCollider;
		
			
		/*void OnTriggerEnter(Collider other)
		{
			if (other.gameObject != null && other.gameObject == PathFollowCam.Instance.followObject)
			{
				PathFollowCam.Instance.ActivateNode(this);
			}
		}
		
		void OnTriggerExit(Collider other)
		{
			if (other.gameObject != null && other.gameObject == PathFollowCam.Instance.followObject)
			{
				PathFollowCam.Instance.DeactivateNode(this);
			}
		}*/
		
		public float SqrtDistanceTo(Vector3 _pos)
		{
			return (transform.position -_pos).sqrMagnitude;
		}
		
		/*public float GetNormalizedDistanceToBounds(Vector3 _pos)
		{
			return (collider.ClosestPointOnBounds(_pos) - _pos).magnitude/capsuleCollider.radius;
		}*/
		
		public Vector3 GetPosition(Vector3 _followObjectPos)
		{
			Vector3 localPos = transform.InverseTransformPoint(_followObjectPos);
			localPos.y = 0;
			//localPos.z = 
			
			return transform.TransformPoint(localPos);
		}
	}
}