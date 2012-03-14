using UnityEngine;
using System.Collections;

public class DisableTimer : MonoBehaviour {
	public float interval;
	private float timer;
	
	// Update is called once per frame
	void Update () {
		if ((timer += Time.deltaTime) > interval)
		{
			timer -= interval;
			renderer.enabled = !renderer.enabled;
		}
	}
}
