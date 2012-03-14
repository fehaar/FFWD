using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD.Components
{
    public abstract class Renderer : Component, IInitializable
    {
        public Renderer()
            :base()
        {
        }

        [ContentSerializer(Optional=true)]
        public bool isPartOfStaticBatch = false;
        [ContentSerializer(Optional = true)]
        public bool enabled = true;
        [ContentSerializer(Optional = true)]
        public int lightmapIndex = -1;
        [ContentSerializer(Optional = true)]
        public Vector4 lightmapTilingOffset = Vector4.zero;

        [ContentSerializer(ElementName = "sharedMaterials", CollectionItemName = "material")]
        protected Material[] _sharedMaterials;
        [ContentSerializerIgnore]
        public Material[] sharedMaterials
        {
            get
            {
                return (_sharedMaterials == null) ? null : (Material[])_sharedMaterials.Clone();
            }
            set
            {
                _sharedMaterials = value;
            }
        }

        [ContentSerializerIgnore]
        public Material sharedMaterial
        {
            get 
            {
                if (_sharedMaterials == null || _sharedMaterials.Length == 0)
                {
                    return null;
                }
                return _sharedMaterials[0];
            }
            set
            {
                if (_sharedMaterials == null || _sharedMaterials.Length == 0)
                {
                    _sharedMaterials = new Material[1];
                }
                _sharedMaterials[0] = value;
                if (value != null)
                {
                    renderQueue = value.finalRenderQueue;
                    Camera.ChangeRenderQueue(this);
                }
            }
        }

        private Material _material;
        [ContentSerializerIgnore]
        public Material material
        {
            get
            {
                if (_material != null)
                {
                    return _material;
                }
                else
                {
                    return sharedMaterial;
                }
            }
            set
            {
                _material = value;
                renderQueue = material.finalRenderQueue;
                Camera.ChangeRenderQueue(this);
            }
        }

        protected Material[] _materials;
        [ContentSerializerIgnore]
        public Material[] materials
        {
            get
            {
                return (_materials == null) ? sharedMaterials : (Material[])_materials.Clone();
            }
            set
            {
                _materials = value;
            }
        }

        public Bounds bounds
        {
            get;
            internal set;
        }

        internal float renderQueue = 0f;
        internal RenderItem[] renderItems;

        #region IRenderable Members
        /// <summary>
        /// Draw the actual thing that we want to render
        /// </summary>
        /// <param name="device"></param>
        /// <param name="cam"></param>
        /// <returns>Returns an estimated number of draw calls that we make in the draw routine.</returns>
        public abstract void Draw(GraphicsDevice device, Camera cam);
        #endregion

        public override void Awake()
        {
            if (material == null)
            {
                renderQueue = 0;
            }
            else
            {
                renderQueue = material.finalRenderQueue;
            }
        }

        public virtual void Initialize(AssetHelper assets)
        {
        }

        public virtual bool ShouldPrefabsBeInitialized()
        {
            return false;
        }

        /// <summary>
        /// Flag that the renderer has changed in some way so we need to reconsider it for culling.
        /// </summary>
        internal void ReconsiderForCulling()
        {
            if (renderItems == null)
            {
                return;
            }
            for (int i = 0; i < renderItems.Length; i++)
            {
                RenderQueue.ReconsiderForCulling(renderItems[i]);
            }
        }

        internal void AddRenderItems(RenderQueue rq)
        {
            if (renderItems == null)
            {
                return;
            }
            for (int i = 0; i < renderItems.Length; i++)
            {
                rq.Add(renderItems[i]);
            }
        }

        internal void RemoveRenderItems(RenderQueue rq)
        {
            if (renderItems == null)
            {
                return;
            }
            for (int i = 0; i < renderItems.Length; i++)
            {
                rq.Remove(renderItems[i]);
            }            
        }
    }
}
