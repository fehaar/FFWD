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
        private VertexPositionColorTexture[] vertexPositionColorTextures;
        private short[] triangleIndexData;
        private float lineIncrementDeltaFraction;
        private float widthDif;
        private float curWidth = 1;
        Vector3 orthogonalVector = new Vector3();
        int pointCnt = 0;
        Vector3[] transformedPositions = new Vector3[0];

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
        
        public override void Awake()
        {
            base.Awake();

            widthDif = endWidth - startWidth;
            Vector3[] oldPositions = (Vector3[])positions.Clone();
            SetVertexCount(positions.Length);
            for (int i = 0; i < oldPositions.Length; i++)
            {
                positions[i] = oldPositions[i];
            }
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
            widthDif = endWidth - startWidth;
        }

        public void SetVertexCount(int count)
        {
            if (count < 2)
            { return; }

            //creqate new arrays if the current ones are too short
            if (transformedPositions == null || transformedPositions.Length < count)
            {
                positions = new Vector3[count];
                transformedPositions = new Vector3[count];
                vertexPositionColorTextures = new VertexPositionColorTexture[count * 2];
                triangleIndexData = new short[(count - 1) * 6];
                //create triangles
                for (int i = pointCnt; i < count - 1; i++)
                {
                    int triangleOffset = i * 6;
                    int verticeOffset = i * 2;
                    triangleIndexData[triangleOffset] = (short)verticeOffset;
                    triangleIndexData[triangleOffset + 1] = (short)(verticeOffset + 1);
                    triangleIndexData[triangleOffset + 2] = (short)(verticeOffset + 2);
                    triangleIndexData[triangleOffset + 3] = (short)(verticeOffset + 2);
                    triangleIndexData[triangleOffset + 4] = (short)(verticeOffset + 1);
                    triangleIndexData[triangleOffset + 5] = (short)(verticeOffset + 3);
                }
            }

            //create other drawing information
            lineIncrementDeltaFraction = 1f / ((float)count - 1f);
            

            Vector2 texScale = material.mainTextureScale;
            for (int i = 0; i < count - 1; i++)
            {
                float lineIncrementFraction = i * lineIncrementDeltaFraction;
                vertexPositionColorTextures[i * 2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(lineIncrementFraction * texScale.x, 0);
                vertexPositionColorTextures[i * 2 + 1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(lineIncrementFraction * texScale.x, 1);

                vertexPositionColorTextures[i * 2].Color = Color.Lerp(startColor, endColor, lineIncrementFraction);
                vertexPositionColorTextures[i * 2 + 1].Color = vertexPositionColorTextures[i * 2].Color;
            }

            vertexPositionColorTextures[count * 2 - 2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0);
            vertexPositionColorTextures[count * 2 - 1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1);

            vertexPositionColorTextures[count * 2 - 2].Color = endColor;
            vertexPositionColorTextures[count * 2 - 1].Color = endColor;

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

        public override void Draw(Microsoft.Xna.Framework.Graphics.GraphicsDevice device, Camera cam)
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

            material.SetTextureState(cam.BasicEffect);
            material.SetBlendState(device);

            cam.BasicEffect.VertexColorEnabled = true;
            cam.BasicEffect.LightingEnabled = false;

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
        }

        void Rebuild(Vector3[] positions, int pointCnt, Vector3 cameraPosition)
        {
            if (pointCnt < 2) return;

            for (int i = 0; i < pointCnt - 1; i++)
            {
                Vector3 lineDir = positions[i + 1] - positions[i];
                Vector3 cameraDir = cameraPosition - positions[i];
                orthogonalVector = Vector3.Cross(lineDir, cameraDir);
                orthogonalVector.Normalize();
//#if DEBUG
//                Debug.DrawRay(positions[i], lineDir, Color.green);
//                Debug.DrawRay(positions[i], orthogonalVector * 10, Color.red);
//                Debug.DrawRay(positions[i], cameraDir, Color.red);
//#endif
                float lineIncrementFraction = i * lineIncrementDeltaFraction;
                curWidth = (startWidth + (widthDif * lineIncrementFraction)) * 0.5f;
                
                vertexPositionColorTextures[i * 2].Position = positions[i] - orthogonalVector * curWidth;
                vertexPositionColorTextures[i * 2 + 1].Position = positions[i] + orthogonalVector * curWidth;
            }
            vertexPositionColorTextures[(pointCnt - 2) * 2 + 2].Position = positions[(pointCnt - 2) + 1] - orthogonalVector * endWidth;
            vertexPositionColorTextures[(pointCnt - 2) * 2 + 3].Position = positions[(pointCnt - 2) + 1] + orthogonalVector * endWidth;

//#if DEBUG
//            for (int i = 0; i < vertexPositionColorTextures.Length-1; i++)
//            {
//                Debug.DrawLine(vertexPositionColorTextures[i].Position, vertexPositionColorTextures[i + 1].Position, new Color(0.8f, 0.8f, 0.8f, 0.5f));
//                if (i < vertexPositionColorTextures.Length - 2)
//                { Debug.DrawLine(vertexPositionColorTextures[i].Position, vertexPositionColorTextures[i + 2].Position, new Color(0.8f, 0.8f, 0.8f, 0.5f)); }
//            }
//#endif
        }
    }
}
