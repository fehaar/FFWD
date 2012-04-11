using System;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD.Extensions;

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
        [ContentSerializer(Optional = true)]
        public bool useLightMap = false;

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
                createRenderItems = true;
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
                createRenderItems = true;
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
                createRenderItems = true;
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
                createRenderItems = true;
            }
        }

        public Bounds bounds
        {
            get;
            internal set;
        }

        internal float renderQueue = 0f;
        internal RenderItem[] renderItems;
        protected bool createRenderItems = true;

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
            if (_sharedMaterials.HasElements())
            {
                renderQueue = _sharedMaterials.Min(m => (m == null) ? 0 : m.finalRenderQueue);
            }
            else
            {
                renderQueue = 0;
            }
        }

        protected virtual void CreateRenderItems()
        {
            if (renderItems.HasElements())
            {
                for (int i = 0; i < renderItems.Length; i++)
                {
                    renderItems[i].RemoveReference(transform);
                }
                renderItems = null;
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
            RenderQueue.ReconsiderForCulling(this);
        }

        internal void UpdateCullingInfo(Camera cam)
        {
            if (renderItems.HasElements())
            {
                for (int i = 0; i < renderItems.Length; i++)
                {
                    if (renderItems[i].UpdateCullingInfo(cam, transform))
                    {
                        cam.CulledRenderQueue.Add(renderItems[i]);
                    }
                }
            }
        }

        internal void AddRenderItems(RenderQueue rq)
        {
            if (renderItems.HasElements())
            {
                for (int i = 0; i < renderItems.Length; i++)
                {
                    rq.Add(renderItems[i]);
                }
            }
        }

        internal void RemoveRenderItems(RenderQueue rq)
        {
            if (renderItems.HasElements() && rq.Count > 0)
            {
                for (int i = 0; i < renderItems.Length; i++)
                {
                    rq.Remove(renderItems[i]);
                }
            }
        }
    }
}
