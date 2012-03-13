using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class MeshRenderer : Renderer, IUpdateable
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
            createRenderItems = false;
        }

        public override void Draw(GraphicsDevice device, Camera cam)
        {
            int i = 0;
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
