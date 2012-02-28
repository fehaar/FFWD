using UnityEngine;
using System.Collections;

[FFWD_DontExport]
[ExecuteInEditMode]
public class XNALineRendererExport : MonoBehaviour {
	public float startWidth;
	public float endWidth;
	public Color startColor = Color.white;
	public Color endColor = Color.white;
	
	public Vector3[] positions;
	
	LineRenderer l;
	
	void Start()
	{
		Destroy(this);
	}
	
	void Update()
	{
		//if (!Application.isEditor){return;}
		
		if (l == null)
		{
			l = GetComponent<LineRenderer>();
		}
		
		if (l != null)
		{
			l.SetWidth(startWidth, endWidth);
			l.SetColors(startColor, endColor);
			
			l.SetVertexCount(positions.Length);
			for (int i = 0; i < positions.Length; i++) {
				l.SetPosition(i,positions[i]);
			}
		}
			
	}
}
