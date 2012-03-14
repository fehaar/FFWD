using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;

public class DisableTimer : MonoBehaviour {
	public float interval;
	private float timer;
	
	// Update is called once per frame
	public override void Update () {
		if ((timer += Time.deltaTime) > interval)
		{
			timer -= interval;
			renderer.enabled = !renderer.enabled;
		}
	}
}
