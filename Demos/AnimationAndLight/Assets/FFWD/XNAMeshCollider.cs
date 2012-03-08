using UnityEngine;
using System.Collections;

/// <summary>
/// This is a marker script to tell U2X to export this collider to XNA. Normal mesh colliders are not exported.
/// </summary>
public class XNAMeshCollider : MonoBehaviour {

	void Start () {
        //gameObject.active = false;
		Destroy(gameObject);
	}
}