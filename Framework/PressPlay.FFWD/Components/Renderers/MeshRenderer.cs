using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Extensions;
using System;

namespace PressPlay.FFWD.Components
{
    public class MeshRenderer : Renderer
    {
        private MeshFilter filter;

        private RenderItem[] renderItems;

        public override void Initialize(AssetHelper assets)
        {
            filter = (MeshFilter)GetComponent(typeof(MeshFilter));
            Material[] mats = materials;
            if (filter.meshToRender != null && mats.HasElements())
            {
                bounds = filter.meshToRender.bounds;

                renderItems = new RenderItem[mats.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    renderItems[i] = RenderItem.Create(mats[i], filter.meshToRender, transform);
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

            if (filter.CanBatch())
            {
                return cam.BatchRender(filter.meshToRender, sharedMaterials, transform);
            }

            return 0;
        }
        #endregion
    }
}
