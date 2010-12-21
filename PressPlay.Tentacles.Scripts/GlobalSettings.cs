using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class GlobalSettings : MonoBehaviour {
	
		
		public LayerMask allUILayers;
		
		public LayerMask squishLemmyLayers;
		
		
		public LayerMask allWallsLayers_ClawSpecific;
	    public LayerMask allWallsAndShields_ClawSpecific;
		
		
		public LayerMask allWallsLayers;
	    public LayerMask allWallsAndShields;
	    public int shieldLayer;
		
		public LayerMask tentacleBounceColliderLayerIndex = 14;
		public LayerMask tentacleBounceColliderLayers;
		
	    public LayerMask tentacleColliderLayerInt = 8;
		public LayerMask tentacleColliderLayer;
		public int tentacleColliderLayerIndex;
		
		public LayerMask enemyLayer;
	    public int enemyLayerInt = 12;
	
		public LayerMask inputLayer;
		
		public LayerMask enemyInputLayer;
	    public int enemyInputLayerInt = 16;
	
		public LayerMask lemmyLayer;
	
	    public LayerMask guiMask;
	
		public string lemmyTag;
		public string clawTag;
		public string tentacleTipTag = "TentacleTip";
		
		public string enemyHitLumpTag;
		
		//public string killLemmyTag;
		//public string damageLemmyTag;
		//public string tickleLemmyTag;
		public string pickupTag;
		public string triggeredByLemmyTag;
		
		//public float tickleForce;
		//public float onHitDamage;
		//public float inAreaDamagePerSecond;
		
		public static bool isLoaded = false;
		private static GlobalSettings instance;
		
		public static GlobalSettings Instance
	    {
	        get
	        {
	            if (instance == null)
	            {
	                Debug.LogError("Attempt to access instance of GlobalSettings singleton earlier than Start or without it being attached to a GameObject.");
	            }
	            return instance;
	        }
	    }
		
		public override void Awake()
	    {
	        if (instance != null)
	        {
	            Debug.LogError("Cannot have two instances of GlobalSettings. Self destruction in 3...");
	            Destroy(this);
	            return;
	        }
			isLoaded = true;
	        instance = this;
			
			//DontDestroyOnLoad(gameObject);
	    }
	}
}