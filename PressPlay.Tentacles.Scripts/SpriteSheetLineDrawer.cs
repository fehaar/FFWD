using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.Tentacles.Scripts {
	public class SpriteSheetLineDrawer : UVSpriteSheetAnimator 
	{
	    // Material - Must be a particle material that has the "Tint Color" property
	    public Material material;
	
	    // Lifetime of each segment
	    public float startWidth;
		public float endWidth;
	    
		private float widthDif;
		private float curWidth = 1;
		private float nextWidth = 1;

		private Vector3[] vertices;// = new Vector3[pointCnt * 5];
	    //private Vector2[] uvs;// = new Vector2[pointCnt * 5];
	    private short[] triangles;// = new int[((pointCnt-1)*9)+6];
		private bool[] flipped;// = new bool[pointCnt];
		private float lineIncrementDeltaFraction;// = 1f/((float)pointCnt-1f);
		
		private Vector3 p1;// = points[i];
	    private Vector3 p2;// = points[i+1];
		private Vector3 p3;// = points[i+2];
		
		private Vector3 tmpVerticePosUpper;
		private Vector3 tmpVerticePosLower;
		
		Vector3 orthogonalVector =  new Vector3();
		
	    // Points
	    Vector3[] points;// = new Vector3[1000];
	    int pointCnt = 0;
		int oldPointCnt = -1;
	    
		
		public override void Start ()
		{
			
		}
		
		public override void Initialize()
	    {
			InitializeLine(100);



            tileSize = new Vector2(1f / (float)xCount, 1f / (float)yCount);
			
			DrawLine(new Vector3[]{Vector3.zero, Vector3.forward});
			
			if (automaticPlay && automaticPlayAnim != null)
			{
				Play(automaticPlayAnim);
			}
		}
		
	    public void InitializeLine(int _maxPoints)
	    {
	        MeshFilter meshFilter = (MeshFilter) gameObject.AddComponent(typeof(MeshFilter));
			Mesh tmpMesh = new Mesh();
			
			//Debug.Log("meshFilter  "+meshFilter.name + " tmpMesh " + tmpMesh);
			
			meshFilter.sharedMesh = tmpMesh;
	        mesh = meshFilter.sharedMesh;
			
	        if (gameObject.renderer == null || gameObject.renderer.GetType() != typeof(MeshRenderer))
			{
				gameObject.AddComponent(typeof(MeshRenderer));
			}
	        gameObject.renderer.sharedMaterial = material;
			
			points = new Vector3[_maxPoints+1];
			
			
			
			Rebuild();
	    }
	    // Do we add any new points?
	    public void AddPoint(Vector3 newPos)
	    {
	        //if (pointCnt == points.Length)
	          //  System.Array.Resize(ref points, points.Length + 50);
	        insertPoint(newPos);
	        //Rebuild();
	    }
		
		public void DrawLine(Transform[] _transforms)
		{
			Vector3[] tmpPositions = new Vector3[_transforms.Length];
			
			int i = 0;
			foreach(Transform t in _transforms)
			{
				tmpPositions[i] = t.position;
				i++;
			}
			
			DrawLine(tmpPositions);
		}
		
	    public void DrawLine(Vector3[] _newPositions)
	    {
			/*if (pointCnt % 2 == 1 )
			{
				if(oldPointCnt != _newPositions.Length+1)
				{
					points = new Vector3[_newPositions.Length+1];
				}
			}
			else */
			
			/*if (oldPointCnt != _newPositions.Length+1)
			{
				points = new Vector3[_newPositions.Length+1];
			}*/
			
	        pointCnt = 0;
	        for (int i = 0; i < _newPositions.Length; i++)
	        {
	            AddPoint(transform.InverseTransformPoint(_newPositions[i]));
	        }
	        Rebuild();
			
			/*if (isPlaying)
			{
				UpdateAnim();
				
				Debug.Log("SpriteSheetLineDrawer anim currentFrameIndex : "+currentFrameIndex);
				ShowFrameWithIndex(currentFrameIndex);
			}*/
	    }
	    void OnDrawGizmos() {
            //for (int i=0; i< pointCnt-1; i++){
            //    Gizmos.color = Color.Green;
            //        Gizmos.DrawLine(points[i],points[i+1]);
            //}
	    
	    }
		
		public override void Update ()
		{
			//Rebuild();
			//base.Update ();
		}
		
	    public void Rebuild() {
			
			widthDif = endWidth - startWidth;
			
	        // Rebuild it
	       	if (pointCnt < 2) return;
	        if (pointCnt % 2 == 1) AddPoint(points[pointCnt-1]);
	 
			bool rebuildArraysAndUVs = (oldPointCnt != pointCnt);
			oldPointCnt = pointCnt;
			//reset arrays if the new point count is different from the old
			if (rebuildArraysAndUVs)
			{
				//Debug.Log("LineDrawerXZ on "+name+" rebuilding arrays and UVs");
				
		        vertices = new Vector3[pointCnt * 5];
		        baseUVs = new Vector2[pointCnt * 5];
                tmpUVs = new Vector2[pointCnt * 5];
		        triangles = new short[((pointCnt-1)*9)+6];
				flipped = new bool[pointCnt];
				
				lineIncrementDeltaFraction = 1f/((float)pointCnt-1f);
			}
			
	        for (int i = 0; i < pointCnt-1; i++)
	        {
				float lineIncrementFraction = i*lineIncrementDeltaFraction;
				//Debug.Log("lineIncrementFraction "+lineIncrementFraction+" i "+i);
	            //float width;
	            curWidth = (startWidth + (widthDif * lineIncrementFraction)) * 0.5f;
				nextWidth = (startWidth + (widthDif * (lineIncrementFraction + lineIncrementDeltaFraction))) * 0.5f;
	            			
				p1 = points[i];
	            p2 = points[i+1];
				p3 = points[i+2];
				
				// todo fix with if..
	            float rad = Mathf.Atan2(p2.z - p1.z, p1.x - p2.x);
				float tmpRad = rad;
				if (tmpRad < 0) tmpRad+= 2*Mathf.PI;
				float tmpRad2 = Mathf.Atan2(p3.z - p2.z, p2.x - p3.x);
				if (tmpRad2 < 0) tmpRad2+= 2*Mathf.PI;
				tmpRad2 = tmpRad-tmpRad2;
				if (tmpRad2 < 0) tmpRad2+= 2*Mathf.PI;
				tmpRad2 *= Mathf.Rad2Deg;
				//Debug.Log(p1 + " rad2= "+ tmpRad2);
				//Debug.Log("rad3= "+ rad3);
	
				if (tmpRad2 >180) {
					flipped[i] = true;
				}
				else{
					flipped[i] = false;
				}
							
				orthogonalVector.x = Mathf.Sin(rad);
				orthogonalVector.z = Mathf.Cos(rad);
				
				//vertices[i * 5] = p1 + orthogonalVector * curWidth;
	            //vertices[(i * 5) + 1] = p1 - orthogonalVector * curWidth;
				
				if (i == 0)
				{
					//If this is the first vertices, they have not been set in previous iteration. Do so now.
					tmpVerticePosUpper = p1 + orthogonalVector * curWidth;
					tmpVerticePosLower = p1 - orthogonalVector * curWidth;
	
				}
				
				vertices[i * 5] = tmpVerticePosUpper;
	            vertices[(i * 5) + 1] = tmpVerticePosLower;
	           
				vertices[(i * 5) + 2] = p2 + orthogonalVector * nextWidth;
	            vertices[(i * 5) + 3] = p2 - orthogonalVector * nextWidth;
				vertices[(i * 5) + 4] = p2;
				
				//set start vertices for next iteration
				tmpVerticePosUpper = vertices[(i * 5) + 2];
				tmpVerticePosLower = vertices[(i * 5) + 3];
	        }
			
			
			//float uvX = 0;
			
			if (rebuildArraysAndUVs)
			{
                tmpUVs = new Vector2[baseUVs.Length];
			
				for (int i = 0; i < baseUVs.Length; i++) 
				{
                    baseUVs[i] = new Vector2(0, 0);
                    tmpUVs[i] = new Vector2(0, 0);
				}
				
		        for (int i = 0; i < pointCnt - 1; i++)
		        {
					float lineIncrementFraction = i*lineIncrementDeltaFraction;
					
		            // UVs
		            baseUVs[i * 5].x = lineIncrementFraction / (float)xCount;
					baseUVs[i * 5].y = 0;
					
		            baseUVs[i * 5 + 1].x = lineIncrementFraction / (float)xCount;
					baseUVs[i * 5 + 1].y = 1 / (float)yCount;
					
		            baseUVs[i * 5 + 2].x = (lineIncrementFraction+lineIncrementDeltaFraction) / (float)xCount;
					baseUVs[i * 5 + 2].y = 0;
					
		            baseUVs[i * 5 + 3].x = (lineIncrementFraction+lineIncrementDeltaFraction) / (float)xCount;
					baseUVs[i * 5 + 3].y = 1 / (float)yCount;
		            
					//ASK MIKKEL WHAT THIS DOES
                    baseUVs[i * 5 + 4] = new Vector2(.5f, .5f);
					
		        }
			}
			if (rebuildArraysAndUVs)
			{
		        for (int i = 0; i < triangles.Length; i++)
		        {
		            int adder = (i / 9) * 5;
		            int tmpComp = (i) % 9;
		            if (tmpComp == 0)
		                triangles[i] = (short)(0 + adder);
		            else if (tmpComp == 1)
                        triangles[i] = (short)(1 + adder);
		            else if (tmpComp == 2)
		                triangles[i] = (short)(2 + adder);
		            else if (tmpComp == 3)
		                triangles[i] = (short)(3 + adder);
		            else if (tmpComp == 4)
		                triangles[i] = (short)(2 + adder);
		            else if (tmpComp == 5)
		                triangles[i] = (short)(1 + adder);
					if (i < triangles.Length-9)
					{
				//	Debug.Log((adder/5) + " flipped= "+ flipped[adder/5]);
						if (flipped[adder/5]){
							if (tmpComp == 6)
				                triangles[i] = (short)(3 + adder);
				            else if (tmpComp == 7)
				                triangles[i] = (short)(6 + adder);
				            else if (tmpComp == 8)
				                triangles[i] = (short)(4 + adder);
					
						}
						else{
							if (tmpComp == 6)
				                triangles[i] = (short)(5 + adder);
				            else if (tmpComp == 7)
				                triangles[i] = (short)(2 + adder);
				            else if (tmpComp == 8)
				                triangles[i] = (short)(4 + adder);
						}
					}
		        }
			}
			
	/*      for (int i = 0; i < vertices.Length; i++)
	        {
	            Debug.Log(i + " vertices= " + vertices[i]);
	        }
	        
	        
	        for (int i = 0; i < triangles.Length; i++)
	        {
	            Debug.Log(i + " triangles= " + triangles[i]);
	        }
	*/ 
	        mesh.Clear();
	        mesh.vertices = vertices;
			
			mesh.uv = CreateUVs(XPosFromIndex(currentFrameIndex), YPosFromIndex(currentFrameIndex));
			
			mesh.normals = vertices;
	        mesh.triangles = triangles;
			
			if (isPlaying)
			{
				UpdateAnim();
			}
	    }
	
	
	    void insertPoint(Vector3 newpos)
	    {
	        points[pointCnt] = newpos;
	        pointCnt++;
	    }
	
	}
}