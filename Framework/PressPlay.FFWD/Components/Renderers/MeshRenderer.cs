using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Extensions;
using System;

namespace PressPlay.FFWD.Components
{
    public class MeshRenderer : Renderer
    {
        private MeshFilter filter;
        
        private VertexBuffer vBuffer;
        
        public override void Start()
        {
            base.Start();
            filter = (MeshFilter)GetComponent(typeof(MeshFilter));
            if (filter.meshToRender != null)
            {
                bounds = filter.meshToRender.bounds;

                if (false)
                {
                    // Determine what vertex buffer to create
                    Mesh m = filter.meshToRender;
                    bool hasTexture = (material.mainTexture != null && material.shader.supportsTextures);
                    bool hasVertexColor = (m.colors.HasElements() && material.shader.supportsVertexColor);
                    bool hasLights = (Light.HasLights && m.normals.HasElements() && material.shader.supportsLights);

                    if (hasTexture)
                    {
                        if (hasVertexColor)
                        {
                            // NOTE: Here, lights are not supported
                            VertexPositionColorTexture[] data = new VertexPositionColorTexture[m.vertexCount];
                            for (int i = 0; i < m.vertexCount; i++)
                            {
                                data[i] = new VertexPositionColorTexture(
                                    m.vertices[i],
                                    m.colors[i],
                                    m.uv[i]
                                );
                            }
                            vBuffer = new VertexBuffer(Camera.Device, data.GetType().GetElementType(), data.Length, BufferUsage.WriteOnly);
                            vBuffer.SetData(data);
                        }
                        else
                        {
                            if (hasLights)
                            {
                                VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[m.vertexCount];
                                for (int i = 0; i < m.vertexCount; i++)
                                {
                                    data[i] = new VertexPositionNormalTexture(
                                        m.vertices[i],
                                        m.normals[i],
                                        m.uv[i]
                                    );
                                }
                                vBuffer = new VertexBuffer(Camera.Device, data.GetType().GetElementType(), data.Length, BufferUsage.WriteOnly);
                                vBuffer.SetData(data);
                            }
                            else
                            {
                                VertexPositionTexture[] data = new VertexPositionTexture[m.vertexCount];
                                for (int i = 0; i < m.vertexCount; i++)
                                {
                                    data[i] = new VertexPositionTexture(
                                        m.vertices[i],
                                        m.uv[i]
                                    );
                                }
                                vBuffer = new VertexBuffer(Camera.Device, data.GetType().GetElementType(), data.Length, BufferUsage.WriteOnly);
                                vBuffer.SetData(data);
                            }
                        }
                    }
                    else
                    {
                        if (hasVertexColor)
                        {
                            VertexPositionColor[] data = new VertexPositionColor[m.vertexCount];
                            for (int i = 0; i < m.vertexCount; i++)
                            {
                                data[i] = new VertexPositionColor(
                                    m.vertices[i],
                                    m.colors[i]
                                );
                            }
                            vBuffer = new VertexBuffer(Camera.Device, data.GetType().GetElementType(), data.Length, BufferUsage.WriteOnly);
                            vBuffer.SetData(data);
                        }
                        else
                        {
                            // Not supported yet
                        }
                    }
                    
                }

            }
        }

        #region IRenderable Members
        public override int Draw(GraphicsDevice device, Camera cam)
        {
            if (filter == null)
            {                
                return 0;
            }

            BoundingSphere sphere = new BoundingSphere((Microsoft.Xna.Framework.Vector3)transform.position + filter.boundingSphere.Center, filter.boundingSphere.Radius * transform.lossyScale.sqrMagnitude);
            if (cam.DoFrustumCulling(ref sphere))
            {
#if DEBUG
                if (Camera.logRenderCalls)
                {
                    Debug.LogFormat("VP cull {0} with radius {1} pos {2} cam {3} at {4}", gameObject, filter.boundingSphere.Radius, transform.position, cam.gameObject, cam.transform.position);
                }
#endif
                return 0;
            }

            // NOTE: I don't want to look at this now... Use the old rendering method.
            if (false /*vBuffer != null */)
	        {
                device.SetVertexBuffer(vBuffer);
                IndexBuffer iBuffer = filter.mesh.GetIndexBuffer();
                device.Indices = iBuffer;
                if (material != null)
                {
                    Render(device, cam, material, vBuffer.VertexCount, iBuffer.IndexCount / 3);
                    //for (int i = 1; i < sharedMaterials.Length; i++)
                    //{
                    //    sharedMaterials[i].Render(vBuffer, iBuffer);
                    //}
                }
                else
                {
                    Debug.LogFormat("We have no material for {0}, so it is not rendered", this);
                }
	        }
            else
        	{
                if (filter.CanBatch())
                {
                    return cam.BatchRender(filter.meshToRender, sharedMaterials, transform);
                }
        	}

            return 0;
        }

        private void Render(GraphicsDevice device, Camera cam, Material material, int vertexCount, int primCount)
        {
            if (material == null | material.shader == null)
            {
                return;
            }
            Effect e = material.shader.effect;
            material.shader.ApplyPreRenderSettings(filter.mesh.colors != null && filter.mesh.colors.Length > 0);
            material.SetBlendState(device);

            IEffectMatrices ems = e as IEffectMatrices;
            if (ems != null)
	        {
                ems.World = transform.world;
                ems.View = cam.view;
                ems.Projection = cam.projectionMatrix;
	        }
            foreach (EffectPass pass in e.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    vertexCount,
                    0,
                    primCount
                );
            }
        }
        #endregion
    }
}
