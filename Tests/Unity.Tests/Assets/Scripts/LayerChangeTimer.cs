using UnityEngine;
using System.Collections;

public class LayerChangeTimer : MonoBehaviour {
	
	public float interval;
	private float timer;
	public int layerToChangeTo;
	private int nextLayer;

	// Use this for initialization
	void Start () {
		nextLayer = layerToChangeTo;
	}
	
	// Update is called once per frame
	void Update () {
		if ((timer += Time.deltaTime) > interval)
		{
			timer -= interval;
			int current = gameObject.layer;
			gameObject.layer = nextLayer;
			nextLayer = current;
		}		
	}
}
