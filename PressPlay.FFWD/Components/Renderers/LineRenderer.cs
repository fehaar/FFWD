using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public class LineRenderer : Renderer
    {
        //for drawing
        //private int vertexCount = 0;
        private VertexPositionColorTexture[] vertexPositionColorTextures;
        private short[] triangleIndexData;

        private Vector3[] vertices;
        private Microsoft.Xna.Framework.Vector2[] uvs;
        private short[] triangles;
        private bool[] flipped;
        private float lineIncrementDeltaFraction;
        private float widthDif;
        private float curWidth = 1;
        private float nextWidth = 1;
        

        private Vector3 p1;
        private Vector3 p2;
        private Vector3 p3;

        private Vector3 tmpVerticePosUpper;
        private Vector3 tmpVerticePosLower;
        Vector3 orthogonalVector = new Vector3();
        Vector3[] tmpPositions;
        int pointCnt = -1;
        int oldPointCnt = -1;

        //LineRenderer members
        public bool useWorldSpace;
        [ContentSerializer(Optional = true)]
        float startWidth;
        [ContentSerializer(Optional = true)]
        float endWidth;
        [ContentSerializer(Optional = true)]
        Microsoft.Xna.Framework.Color startColor;
        [ContentSerializer(Optional = true)]
        Microsoft.Xna.Framework.Color endColor;
        [ContentSerializer(Optional = true)]
        Vector3[] positions = new Vector3[0];
        
        Vector3[] transformedPositions = new Vector3[0];


        public override void Awake()
        {
            base.Awake();
            transformedPositions = new Vector3[positions.Length];
            vertexPositionColorTextures = new VertexPositionColorTexture[positions.Length * 2];
            triangleIndexData = new short[positions.Length * 6];
            pointCnt = positions.Length;
        }

        public void SetColors(Color start, Color end)
        {
            startColor = start;
            endColor = end;
        }

        public void SetWidth(float start, float end)
        {
            startWidth = start;
            endWidth = end;
        }

        public void SetVertexCount(int count)
        {
            if (vertices == null || vertices.Length < count)
            {
                positions = new Vector3[count];
                transformedPositions = new Vector3[count];
                vertexPositionColorTextures = new VertexPositionColorTexture[count * 2];
                triangleIndexData = new short[count * 6];
            }
            pointCnt = count;
        }

        public void SetPosition(int index, Vector3 position)
        {
            if (index < 0 || index >= pointCnt)
            {
                throw new IndexOutOfRangeException("Trying to set a vertex that does not exist!");
            }
            positions[index] = position;
        }

        public override int Draw(Microsoft.Xna.Framework.Graphics.GraphicsDevice device, Camera cam)
        {
            if (!useWorldSpace)
            {
                //transform local positions to world space
                for (int i = 0; i < pointCnt; i++)
                {
                    transformedPositions[i] = transform.TransformPoint(positions[i]);
                }
                Rebuild(transformedPositions, pointCnt, cam.transform.position);
            }
            else
            {
                Rebuild(positions, pointCnt, cam.transform.position);
            }

            

            foreach (EffectPass pass in cam.BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
                    PrimitiveType.TriangleList,
                    vertexPositionColorTextures,
                    0,
                    pointCnt * 2,
                    triangleIndexData,
                    0,
                    (pointCnt-1) * 2
                );
            }

            return 1;
        }

        void Rebuild(Vector3[] positions, int pointCnt, Vector3 cameraPosition)
        {
            if (pointCnt < 2) return;

            for (int i = 0; i < pointCnt-1; i++)
            {
                Debug.DrawLine(positions[i], positions[i + 1], Color.white);
            }
            widthDif = endWidth - startWidth;
            bool rebuildArraysAndUVs = (oldPointCnt != pointCnt);
            oldPointCnt = pointCnt;
            if (rebuildArraysAndUVs)
            {
                //Debug.Log("LineDrawerXZ on "+name+" rebuilding arrays and UVs");

                vertices = new Vector3[pointCnt * 5];
                uvs = new Microsoft.Xna.Framework.Vector2[pointCnt * 5];
                triangles = new short[((pointCnt - 1) * 9)];
                flipped = new bool[pointCnt];

                lineIncrementDeltaFraction = 1f / ((float)pointCnt - 1f);


                triangleIndexData = new short[(pointCnt - 1) * 6];
            }

            

            for (int i = 0; i < pointCnt - 1; i++)
            {
                Vector3 lineDir = positions[i + 1] - positions[i];
                Vector3 cameraDir = cameraPosition - positions[i];
                orthogonalVector = Vector3.Cross(lineDir, cameraDir);
                orthogonalVector.Normalize();
                Debug.DrawRay(positions[i], lineDir, Color.green);
                Debug.DrawRay(positions[i], orthogonalVector * 10, Color.red);
                Debug.DrawRay(positions[i], cameraDir, Color.red);

                float lineIncrementFraction = i * lineIncrementDeltaFraction;
                //Debug.Log("lineIncrementFraction "+lineIncrementFraction+" i "+i);
                //float width;
                curWidth = (startWidth + (widthDif * lineIncrementFraction)) * 0.5f;
                nextWidth = (startWidth + (widthDif * (lineIncrementFraction + lineIncrementDeltaFraction))) * 0.5f;

                p1 = positions[i];
                p2 = positions[i + 1];
                p3 = positions[Mathf.Clamp(i + 2, 0, pointCnt - 1)];

                // todo fix with if..
                float rad = Mathf.Atan2(p2.z - p1.z, p1.x - p2.x);
                float tmpRad = rad;
                if (tmpRad < 0) tmpRad += 2 * Mathf.PI;
                float tmpRad2 = Mathf.Atan2(p3.z - p2.z, p2.x - p3.x);
                if (tmpRad2 < 0) tmpRad2 += 2 * Mathf.PI;
                tmpRad2 = tmpRad - tmpRad2;
                if (tmpRad2 < 0) tmpRad2 += 2 * Mathf.PI;
                tmpRad2 *= Mathf.Rad2Deg;
                //Debug.Log(p1 + " rad2= "+ tmpRad2);
                //Debug.Log("rad3= "+ rad3);

                if (tmpRad2 > 180)
                {
                    flipped[i] = true;
                }
                else
                {
                    flipped[i] = false;
                }

                orthogonalVector.x = Mathf.Sin(rad);
                orthogonalVector.z = Mathf.Cos(rad);

                //vertices[i * 5] = p1 + orthogonalVector * curWidth;
                //vertices[(i * 5) + 1] = p1 - orthogonalVector * curWidth;

                //if (i == 0)
                //{
                    //If this is the first vertices, they have not been set in previous iteration. Do so now.
                    //tmpVerticePosUpper = p1 + orthogonalVector * curWidth;
                    //tmpVerticePosLower = p1 - orthogonalVector * curWidth;

                //}

                //vertices[i * 5] = tmpVerticePosUpper;
                //vertices[(i * 5) + 1] = tmpVerticePosLower;

                //vertices[(i * 5) + 2] = p2 + orthogonalVector * nextWidth;
                //vertices[(i * 5) + 3] = p2 - orthogonalVector * nextWidth;
                //vertices[(i * 5) + 4] = p2;


                vertexPositionColorTextures[i * 2].Position = p1 + orthogonalVector * curWidth;
                vertexPositionColorTextures[i * 2 + 1].Position = p1 - orthogonalVector * curWidth;

                if (i == pointCnt-2)
                {
                    vertexPositionColorTextures[i * 2 + 2].Position = p2 + orthogonalVector * nextWidth;
                    vertexPositionColorTextures[i * 2 + 3].Position = p2 - orthogonalVector * nextWidth;
                }

                //Debug.DrawLine(vertices[i * 5], vertices[(i * 5) + 1], Color.black);
                //Debug.DrawLine(vertices[(i * 5) + 1], vertices[(i * 5) + 2], Color.black);


                //set start vertices for next iteration
                //tmpVerticePosUpper = vertices[(i * 5) + 2];
                //tmpVerticePosLower = vertices[(i * 5) + 3];
            }

            for (int i = 0; i < vertexPositionColorTextures.Length-1; i++)
            {
                Debug.DrawLine(vertexPositionColorTextures[i].Position, vertexPositionColorTextures[i+1].Position, Color.black);
            }

            if (rebuildArraysAndUVs)
            {
                Vector2 texScale = material.mainTextureScale;
                for (int i = 0; i < pointCnt - 1; i++)
                {
                    float lineIncrementFraction = i * lineIncrementDeltaFraction;

                    //// UVs
                    //uvs[i * 5].X = lineIncrementFraction * texScale.x;
                    //uvs[i * 5].Y = 0;

                    //uvs[i * 5 + 1].X = lineIncrementFraction * texScale.x;
                    //uvs[i * 5 + 1].Y = 1;

                    //uvs[i * 5 + 2].X = (lineIncrementFraction + lineIncrementDeltaFraction) * texScale.x;
                    //uvs[i * 5 + 2].Y = 0;

                    //uvs[i * 5 + 3].X = (lineIncrementFraction + lineIncrementDeltaFraction) * texScale.x;
                    //uvs[i * 5 + 3].Y = 1 * texScale.y;

                    ////ASK MIKKEL WHAT THIS DOES
                    //uvs[i * 5 + 4] = new Vector2(.5f, .5f);

                    vertexPositionColorTextures[i * 2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(lineIncrementFraction * texScale.x,0);
                    vertexPositionColorTextures[i * 2 + 1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(lineIncrementFraction * texScale.x, 1);

                    vertexPositionColorTextures[i * 2].Color = Color.Lerp(startColor, endColor, lineIncrementFraction);
                    vertexPositionColorTextures[i * 2 + 1].Color = vertexPositionColorTextures[i * 2].Color;
                }

                vertexPositionColorTextures[pointCnt*2 - 2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0);
                vertexPositionColorTextures[pointCnt*2 - 1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1);

                vertexPositionColorTextures[pointCnt*2 - 2].Color = endColor;
                vertexPositionColorTextures[pointCnt*2 - 1].Color = endColor;

            }
            if (rebuildArraysAndUVs)
            {
                for (int i = 0; i < pointCnt-1; i++)
                {
                    int triangleOffset = i * 6;
                    int verticeOffset = i * 2;
                    triangleIndexData[triangleOffset] = (short)verticeOffset;
                    triangleIndexData[triangleOffset + 1] = (short)(verticeOffset + 1);
                    triangleIndexData[triangleOffset + 2] = (short)(verticeOffset + 2);
                    triangleIndexData[triangleOffset + 3] = (short)(verticeOffset + 1);
                    triangleIndexData[triangleOffset + 4] = (short)(verticeOffset + 2);
                    triangleIndexData[triangleOffset + 5] = (short)(verticeOffset + 3);
                }


                //for (int i = 0; i < triangles.Length; i++)
                //{
                //    int adder = (i / 9) * 5;
                //    int tmpComp = (i) % 9;
                //    if (tmpComp == 0)
                //        triangles[i] = (short)(0 + adder);
                //    else if (tmpComp == 1)
                //        triangles[i] = (short)(1 + adder);
                //    else if (tmpComp == 2)
                //        triangles[i] = (short)(2 + adder);
                //    else if (tmpComp == 3)
                //        triangles[i] = (short)(3 + adder);
                //    else if (tmpComp == 4)
                //        triangles[i] = (short)(2 + adder);
                //    else if (tmpComp == 5)
                //        triangles[i] = (short)(1 + adder);
                //    if (i < triangles.Length - 9)
                //    {
                //        if (flipped[adder / 5])
                //        {
                //            if (tmpComp == 6)
                //                triangles[i] = (short)(3 + adder);
                //            else if (tmpComp == 7)
                //                triangles[i] = (short)(6 + adder);
                //            else if (tmpComp == 8)
                //                triangles[i] = (short)(4 + adder);

                //        }
                //        else
                //        {
                //            if (tmpComp == 6)
                //                triangles[i] = (short)(5 + adder);
                //            else if (tmpComp == 7)
                //                triangles[i] = (short)(2 + adder);
                //            else if (tmpComp == 8)
                //                triangles[i] = (short)(4 + adder);
                //        }
                //    }
                //}
            }
        }
    }
}
