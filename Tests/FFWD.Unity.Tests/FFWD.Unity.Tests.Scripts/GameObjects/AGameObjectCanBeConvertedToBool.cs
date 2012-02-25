using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;

public class AGameObjectCanBeConvertedToBool : MonoBehaviour {
	
	bool? tested;
	
	// Update is called once per frame
	public override void Update () {
		if (!tested.HasValue) {
			
			GameObject go = new GameObject();
			bool exists = (bool)go;
			tested = exists;
			Destroy(go);
			Debug.Log("Test: " + tested);
			
		}
	}
}
