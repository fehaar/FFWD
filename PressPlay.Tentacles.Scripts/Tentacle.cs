using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class Tentacle: MonoBehaviour {
	
		public TentacleVisualStats visualStats;
		
		private TentacleJoint[] joints;
		
		
		private TentacleStats stats;
		
		public bool useAnchor = false;
		private TentacleJoint anchor;
		private TentacleJoint body;
		private TentacleJoint tip;

        //private SpriteSheetLineDrawer lineDrawer;
		
		private Vector3 bodyNormal;
		private Vector3 tipNormal;
		
		private Vector3[] jointPositions;
		
		public virtual void Initialize (TentacleStats _stats, TentacleJoint _body, TentacleJoint _tip) {
			Initialize(_stats, _body, null, _tip, false);
		}
		// Use this for initialization
		public virtual void Initialize (TentacleStats _stats, TentacleJoint _body, TentacleJoint _anchor, TentacleJoint _tip, bool _useAnchor) {
			tip = _tip;
			body = _body;
			stats = _stats;	
			
			anchor = _anchor;
			useAnchor = _useAnchor;
			
			CreateJoints();
			
            //lineDrawer = (SpriteSheetLineDrawer)GetComponent(typeof(SpriteSheetLineDrawer));
            //lineDrawer.startWidth = visualStats.startWidth;
            //lineDrawer.endWidth = visualStats.endWidth;
            //lineDrawer.Initialize();
		}
		
		public TentacleJoint GetJoint(int index)
		{
			
			return joints[index];
		}
		
		public void ShowAsAvailable()
		{
			/*if (lineRenderer != null)
			{
				lineRenderer.SetColors(Color.grey,Color.gray);
			}*/
		}
		
		public void ShowAsUnavailable()
		{
			/*if (lineRenderer != null)
			{
				lineRenderer.SetColors(Color.grey,Color.grey);
			}*/
		}
		
		public void SetBodyNormal(Vector3 _normal)
		{
			bodyNormal = _normal;
			body.SetNormal(bodyNormal);
		}
		
		public void SetTipNormal(Vector3 _normal)
		{
			tipNormal = _normal;
			tip.SetNormal(tipNormal);
		}
		
		void DestroyJoints()
		{
			if (joints == null)
			{
				return;
			}
			
			for (int i = 0; i < joints.Length; i++) {
				
				Destroy(joints[i]);
			}
		}
		
		void CreateJoints()
		{
			DestroyJoints();
			
			//at least two joints!
			int jointCount = Mathf.Max(visualStats.joints,2);
			
			joints = new TentacleJoint[jointCount];
			
			jointPositions = new Vector3[jointCount];
			
			for (int i = 0; i < joints.Length; i++) {
				
				GameObject tmpGameObject = new GameObject();//(GameObject)Instantiate(GameObject);
				tmpGameObject.name = "Tentacle Joint "+i;
				TentacleJoint tmpJoint = tmpGameObject.AddComponent(new TentacleJoint());
				joints[i] = tmpJoint;
				
				jointPositions[i] = new Vector3(0,0,0);
			}
			
			for (int i = 0; i < joints.Length; i++) {
				
				if (i == 0)
				{
					joints[0].Initialize(body,joints[1], this, i, visualStats);
				}
				else if (i == joints.Length-1)
				{
					joints[i].Initialize(joints[i-1],tip, this,i, visualStats);
				}
				else
				{
					joints[i].Initialize(joints[i-1],joints[i+1], this,i, visualStats);
				}
			}
		}
		
		// Update is called once per frame
		public override void Update () {
			
			
			for (int n = 0; n < visualStats.physicsIterations; n++) {
				for (int i = 0; i < joints.Length; i++) {
					joints[i].DoUpdate();
				}
				for (int i = 0; i < joints.Length; i++) {
					joints[joints.Length-i-1].DoUpdate();
				}
			}
			
            //if (lineDrawer != null)
            //{
            //    jointPositions = new Vector3[joints.Length+2];
            //    jointPositions[0] = body.transform.position;
            //    jointPositions[joints.Length+1] = tip.transform.position;
				
            //    for (int i = 0; i < joints.Length; i++) {
            //        jointPositions[i+1] = joints[i].transform.position;
            //    }
            //    lineDrawer.DrawLine(jointPositions);
            //}
		}
		
		public void Reset()
		{
			for (int i = 0; i < joints.Length; i++) {
				joints[i].MoveTowardsBackConnection(1);
			}
		}
	}
}