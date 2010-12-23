using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

/// <summary>
/// 2d xz distance greater than distance turns of collider
/// </summary>

namespace PressPlay.Tentacles.Scripts {
	public class TurnOffAtDistance : MonoBehaviour {
	
		private static float lLeft;
		private static float rLeft;
		private static float rRight;
		private static float lRight;
		private static float lTop;
		private static float rTop;
		private static float lBottom;
		private static float rBottom;
	
	    public bool notifyGameObjectOnStatusChange = false;
		
		public bool useColliderBounds = false;
		
		public float distanceMod = 0;
	
		private Bounds ownBounds;
		private bool boundsAreImportant = false;
		
		//private GameObject distanceObject;
		
		private Vector2 distanceVector = new Vector2();
		
		//private int framesSinceLastCheck;
		//private int frameSkip = 30;
		
		public bool markedForDestruction = false;
		
		private bool isInitialized = false;
		
		// Use this for initialization
		public void Initialize () {
	
			if (isInitialized)
			{
				return;
			}
			isInitialized = true;
			
			ownBounds = new Bounds(transform.position, Vector3.one * distanceMod);
			
			if (distanceMod > 0)
			{
				boundsAreImportant = true;
			}
			
			//framesSinceLastCheck = Random.Range(0,frameSkip);
			//framesSinceLastCheck = frameSkipSeed%frameSkip;
			
			//distanceObject = _distanceObject;
			
			//distanceModSqrt = distanceMod * distanceMod;
			
			Component[] childTurnOffScripts = GetComponentsInChildren(typeof(TurnOffAtDistance));
			
			for (int i = 0; i < childTurnOffScripts.Length; i++) 
			{
				//Debug.Log(" marked for destruction");
				if (childTurnOffScripts[i] != this)
				{
					((TurnOffAtDistance)childTurnOffScripts[i]).markedForDestruction = true;
				}
			}
			
			if (useColliderBounds || childTurnOffScripts.Length > 1)
			{
				if (collider)
				{
					ownBounds = collider.bounds;
				}
				
				Vector3 max = ownBounds.max;
				Vector3 min = ownBounds.min;
				
				for (int i = 0; i < childTurnOffScripts.Length; i++) {
					
					if (!((TurnOffAtDistance)childTurnOffScripts[i]).isInitialized)
					{
						((TurnOffAtDistance)childTurnOffScripts[i]).Initialize();
					}
					//Debug.Log("child turn offscript "+((TurnOffAtDistance)childTurnOffScripts[i]).name+" bounds : "+((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds+"   is Inlitialized : "+((TurnOffAtDistance)childTurnOffScripts[i]).isInitialized);
					
					if (((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.max.x > ownBounds.max.x)
					{max.x = ((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.max.x;}
					if (((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.max.y > ownBounds.max.y)
					{max.y = ((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.max.y;}
					if (((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.max.z > ownBounds.max.z)
					{max.z =((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.max.z;}
					
					if (((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.min.x < ownBounds.min.x)
					{min.x = ((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.min.x;}
					if (((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.min.y < ownBounds.min.y)
					{min.y = ((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.min.y;}
					if (((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.min.z < ownBounds.min.z)
					{min.z = ((TurnOffAtDistance)childTurnOffScripts[i]).ownBounds.min.z;}
				}
				boundsAreImportant = true;
				
				//Debug.DrawLine(max, min);
				//Debug.Break();
				
				ownBounds.SetMinMax(min, max);
			}
			
			
		}
		
		public bool CheckBounds(Bounds _bounds)
		{
			if (!isInitialized)
			{
				return false;
			}
			
			if (boundsAreImportant)
			{
				return CheckBoundsIntersect(_bounds);
			}else
			{
				return CheckInsideBounds(_bounds);
			}
		}
		
		
		public bool CheckInsideBounds (Bounds _bounds) {
			
			if (TurnOffAtDistance.PointInsideBoundsXZ(transform.position, _bounds))
			{
				//Debug.DrawLine(transform.position, distanceObject.transform.position,Color.red);
				/*
	            if (!gameObject.active)
				{
					gameObject.SetActiveRecursively(true);
				}*/
	            return true;
					
				//gameObject.active = false;
				
				
			}else
			{
				//Debug.DrawLine(transform.position, distanceObject.transform.position,Color.green);
				
	            /*
				if (gameObject.active)
				{
					gameObject.SetActiveRecursively(false);
				}*/
	            return false;
				
				//active = true;
			}
		}
	
	    public bool CheckBoundsIntersect(Bounds _bounds)
	    {
			
			if (TurnOffAtDistance.BoundsIntersectXZ(ownBounds, _bounds))
			{
				
	            /*if (!gameObject.active)
				{
					gameObject.SetActiveRecursively(true);
				}*/
	            return true;
				
			}else
			{
				
	            /*
				if (gameObject.active)
				{
					gameObject.SetActiveRecursively(false);
				}*/
	            return false;
				
				//active = true;
			}
		}
	
	    public void SetActiveState(bool state)
	    {          
	        if (state == true)
	        {
	            gameObject.SetActiveRecursively(state);
	            if (notifyGameObjectOnStatusChange)
	            {     
	                gameObject.SendMessage("OnTurnOnAtDistance", SendMessageOptions.DontRequireReceiver);
	            }
	        }
	        else
	        {
	            if(notifyGameObjectOnStatusChange){
	                gameObject.SendMessage("OnTurnOffAtDistance", SendMessageOptions.DontRequireReceiver);
	            }
	            gameObject.SetActiveRecursively(state);
	        }
	    }
		
		// Update is called once per frame
		public void CheckDistance (float distanceSqrt, Vector3 _pos) {
			
			if (!isInitialized)
			{
				return;
			}
			
			distanceVector.x = _pos.x - transform.position.x;
			distanceVector.y = _pos.z - transform.position.z;
			
			if (distanceVector.sqrMagnitude > distanceSqrt + distanceMod)
			{
				Debug.DrawLine(transform.position, _pos,Color.red);
				if (gameObject.active)
				{
					gameObject.SetActiveRecursively(false);
				}			
			}
			else
			{
				Debug.DrawLine(transform.position, _pos,Color.green);
				
				if (!gameObject.active)
				{
					gameObject.SetActiveRecursively(true);
				}
			}
		}
		
		
		public static bool BoundsIntersectXZ(Bounds b1, Bounds b2)
		{
			lLeft = b1.center.x - b1.extents.x;
			
			rLeft = b2.center.x - b2.extents.x;
			
			lRight = b1.center.x + b1.extents.x;
			
			rRight = b2.center.x + b2.extents.x;
	
			lTop = b1.center.z - b1.extents.z;
			
			rTop = b2.center.z - b2.extents.z;
	
			lBottom = b1.center.z + b1.extents.z;
			
			rBottom = b2.center.z + b2.extents.z;
			
			return !((lLeft > rRight) || (lRight < rLeft) || (lTop > rBottom) || (lBottom < rTop));
		}
		
		public static bool BoundsIntersectXY(Bounds b1, Bounds b2)
		{
			lLeft = b1.center.x - b1.extents.x;
			
			rLeft = b2.center.x - b2.extents.x;
			
			lRight = b1.center.x + b1.extents.x;
			
			rRight = b2.center.x + b2.extents.x;
	
			lTop = b1.center.y - b1.extents.y;
			
			rTop = b2.center.y - b2.extents.y;
	
			lBottom = b1.center.y + b1.extents.y;
			
			rBottom = b2.center.y + b2.extents.y;
			
			return !((lLeft > rRight) || (lRight < rLeft) || (lTop > rBottom) || (lBottom < rTop));
		}
	
		public static bool PointInsideBoundsXY(Vector3 p, Bounds bounds)
		{
			return (p.x >= (bounds.center.x - bounds.extents.x) && p.x <= (bounds.center.x + bounds.extents.x) && p.y >= (bounds.center.y - bounds.extents.y) && p.y <= (bounds.center.y + bounds.extents.y));
		}
		
		public static bool PointInsideBoundsXZ(Vector3 p, Bounds bounds)
		{
			return (p.x >= (bounds.center.x - bounds.extents.x) && p.x <= (bounds.center.x + bounds.extents.x) && p.z >= (bounds.center.z - bounds.extents.z) && p.z <= (bounds.center.z + bounds.extents.z));
		}
	}
}