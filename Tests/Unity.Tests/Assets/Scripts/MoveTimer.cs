using UnityEngine;
using System.Collections;

public class MoveTimer : MonoBehaviour {

	public float interval;
	public Vector3 moveTarget;
	private Vector3 original;
	
	void Start() {
		original = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float t = Mathf.PingPong(Time.realtimeSinceStartup, interval);
		transform.position = Vector3.Lerp(original, moveTarget, t);
	}
}
