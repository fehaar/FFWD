using UnityEngine;
using System.Collections;

[FFWD_DontExport]
public class XNAEllipsoidParticleEmitter : MonoBehaviour {
	public Vector3 ellipsoid;
	public bool oneShot = false;
	public Vector3 tangentVelocity;
	public float minEmitterRange;
	
	public StretchParticles stretchParticles = StretchParticles.Billboard;
	
	public enum StretchParticles{
		Billboard,
		Stretched,
		HorizontalBillboard
	}
	
	// Use this for initialization
	void Awake () {
		Destroy(this);
	}
}
