using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;

public class MoveTimer : MonoBehaviour {

	public float interval;
	public Vector3 moveTarget;
	private Vector3 original;
	
	public override void Start() {
		original = transform.position;
	}
	
	// Update is called once per frame
	public override void Update () {
		float t = Mathf.PingPong(Time.realtimeSinceStartup, interval);
		transform.position = Vector3.Lerp(original, moveTarget, t);
	}
}
