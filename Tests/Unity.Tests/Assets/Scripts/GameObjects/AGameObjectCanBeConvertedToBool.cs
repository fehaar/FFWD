using UnityEngine;
using System.Collections;

public class AGameObjectCanBeConvertedToBool : MonoBehaviour {
	
	bool? tested;
	
	// Update is called once per frame
	void Update () {
		if (!tested.HasValue) {
			
			GameObject go = new GameObject();
			bool exists = (bool)go;
			tested = exists;
			Destroy(go);
			Debug.Log("Test: " + tested);
			
		}
	}
}
