using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class MeshRenderer : Renderer
    {
        private MeshFilter filter;

        public override void Start()
        {
            base.Start();
            filter = (MeshFilter)GetComponent(typeof(MeshFilter));
        }

        #region IRenderable Members
        public override int Draw(GraphicsDevice device, Camera cam)
        {
            if (filter == null)
            {                
                return 0;
            }

            BoundingSphere sphere = new BoundingSphere(transform.position, filter.boundingSphere.Radius * transform.lossyScale.sqrMagnitude);
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

            if (filter.CanBatch())
            {
                return cam.BatchRender(filter.meshToRender, materials, transform);
            }
            return 0;
            //return DrawModelDirectly(device, cam);
        }

//        private int DrawModelDirectly(GraphicsDevice device, Camera cam)
//        {
//            // Draw the model.
//            ModelMesh mesh = filter.GetModelMesh();
//            if (mesh != null)
//            {
//#if DEBUG
//                if (Camera.logRenderCalls)
//                {
//                    Debug.LogFormat("Mesh model: {0} on {1}", gameObject, cam.gameObject);
//                }
//#endif

//                Matrix world = transform.world;
//                cam.BasicEffect.World = world;
//                cam.BasicEffect.VertexColorEnabled = false;
//                material.SetBlendState(device);
//                material.SetTextureState(cam.BasicEffect);
//                foreach (EffectPass pass in cam.BasicEffect.CurrentTechnique.Passes)
//                {
//                    pass.Apply();
//                    for (int i = 0; i < mesh.MeshParts.Count; i++)
//                    {
//                        ModelMeshPart part = mesh.MeshParts[i];
//                        device.SetVertexBuffer(part.VertexBuffer);
//                        device.Indices = part.IndexBuffer;
//                        device.DrawIndexedPrimitives(
//                            PrimitiveType.TriangleList,
//                            part.VertexOffset,
//                            0,
//                            part.NumVertices,
//                            part.StartIndex,
//                            part.PrimitiveCount
//                        );
//                    }
//                }
//                return 1;
//            }
//            return 0;
//        }
        #endregion
    }
}
