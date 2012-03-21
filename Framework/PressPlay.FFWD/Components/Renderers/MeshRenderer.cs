using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public class MeshRenderer : Renderer, PressPlay.FFWD.Interfaces.IUpdateable
    {
        private MeshFilter filter;

        public override void Initialize(AssetHelper assets)
        {
            if (isPartOfStaticBatch)
            {
                return;
            }

            filter = (MeshFilter)GetComponent(typeof(MeshFilter));
            CreateRenderItems();
        }

        protected virtual void CreateRenderItems()
        {
            base.CreateRenderItems();
            Material[] mats = materials;
            if (mats.HasElements())
            {
                mats = mats.Where(m => m != null).ToArray();
            }
            if (filter.meshToRender != null && !filter.isDynamicMesh && mats.HasElements())
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
            createRenderItems = false;
        }

        public override void Draw(GraphicsDevice device, Camera cam)
        {
            if (filter == null || renderItems != null)
            {
                return;
            }

            Mesh mesh = filter.meshToRender;
            BoundingSphere sphere = new BoundingSphere(transform.TransformPoint(filter.boundingSphere.Center), filter.boundingSphere.Radius * Math.Max(transform.lossyScale.x, Math.Max(transform.lossyScale.y, transform.lossyScale.z)));
            bool cull = cam.DoFrustumCulling(ref sphere);
            if (cull)
            {
#if DEBUG
                if (Camera.logRenderCalls)
                {
                    Debug.LogFormat("VP cull mesh {0} with center {1} radius {2} cam {3} at {4}", gameObject, sphere.Center, sphere.Radius, cam.gameObject, cam.transform.position);
                }
#endif
                return;
            }

            if (filter.CanBatch())
            {
                cam.BatchRender(filter.meshToRender, sharedMaterials, transform);
            }
        }

        public void Update()
        {
            if (filter != null && createRenderItems)
            {
                CreateRenderItems();
                ReconsiderForCulling();
            }
        }

        public void LateUpdate()
        {
        }
    }
}
