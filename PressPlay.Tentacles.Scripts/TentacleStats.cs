using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class TentacleStats : MonoBehaviour {
		
		public float dragDistMin = 2; //a lower tentacle tip distance will not result in a pull
		
		public float connectionMaxLength = 12;
		public float tentacleLength = 9;
		public float overMaxLengthElasticity = 0.04f;
		//public float topSpeed = 2;
		public float dragBodyForce = 0.5f;
		public float dragCurvePow = 0.3f;
		public float tentacleTipMoveSpeed = 30;
		public float optimalConnectionDistance = 2.6f;
		public float connectionTimeout = 1.4f;
		public float searchForConnectionTimeout = 1.4f;
		
		public float wallSeekHelpDistance = 1.5f;
		public float wallSeekHelpPower = 5f;
		
		public float controlFlickCurvePow = 0.25f;
		public float controlFlickStrength = 1500;
		
		public float minShootSpeed = 20;
		public float maxShootSpeed = 100;
		
		//public int tentacleJoints = 9; //bones per unit of tentacle length, rounded up
		
		public float controlForce = 40;
		public float controlForceCurvePow = 0.3f;
		
		// Use this for initialization
		public override void Start () {
		
		}
	}
}