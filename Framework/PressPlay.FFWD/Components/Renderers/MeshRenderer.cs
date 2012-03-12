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
            if (isPartOfStaticBatch)
            {
                return;
            }

            filter = (MeshFilter)GetComponent(typeof(MeshFilter));
            Material[] mats = materials;
            if (filter.meshToRender != null && mats.HasElements())
            {
                bounds = filter.meshToRender.bounds;

                // TODO: The thought is that this can actually be done at compile time so the initialization will occur at runtime
                renderItems = new RenderItem[mats.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    RenderItem item = RenderItem.Create(mats[i], filter.meshToRender, i, transform);
                    item.Initialize(Camera.Device);
                    renderItems[i] = item;
                }
            }
        }

        public override void Draw(GraphicsDevice device, Camera cam)
        {
            if (filter == null)
            {                
                return;
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
                return;
            }

            if (filter.CanBatch())
            {
                cam.BatchRender(filter.meshToRender, sharedMaterials, transform);
            }
        }

        internal override void RendererMoved()
        {
            base.RendererMoved();
            for (int i = 0; i < renderItems.Length; i++)
			{
                RenderQueue.RenderItemMoved(renderItems[i]);
			}
        }
    }
}
