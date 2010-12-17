using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class LemmyStats : MonoBehaviour {
		
		public float grabPickupTime = 2;
		
		public float rigidbodyDrag = 5;
		
		public int tentacles = 3;
		public float health = 100;
		public float regenerateDamagePerSecond = 20;
		
		// Use this for initialization
		public override void Start () {
		
		}
	}
}