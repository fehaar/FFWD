using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class SimpleRotate : MonoBehaviour {
		
		public float rotationSpeed = 0.3f;
		public bool switchDirection = false;
	
		// Use this for initialization
		public override void Start () {
		
		}
		
		// Update is called once per frame
		public override void Update () {
			if(!switchDirection){
			transform.Rotate(Vector3.Up, rotationSpeed* -Time.deltaTime, Space.Self);
			}else{
				transform.Rotate(Vector3.Up, rotationSpeed*Time.deltaTime, Space.Self);
			}
		}
	}
}