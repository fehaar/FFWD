using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;

public class LayerChangeTimer : MonoBehaviour {
	
	public float interval;
	private float timer;
	public int layerToChangeTo;
	private int nextLayer;

	// Use this for initialization
	public override void Start () {
		nextLayer = layerToChangeTo;
	}
	
	// Update is called once per frame
	public override void Update () {
		if ((timer += Time.deltaTime) > interval)
		{
			timer -= interval;
			int current = gameObject.layer;
			gameObject.layer = nextLayer;
			nextLayer = current;
		}		
	}
}
