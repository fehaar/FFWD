using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public abstract class Renderer : Component
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
                if (sharedMaterials == null || sharedMaterials.Length == 0)
                {
                    return null;
                }
                return sharedMaterials[0];
            }
            set
            {
                if (sharedMaterials == null || sharedMaterials.Length == 0)
                {
                    sharedMaterials = new Material[1];
                }
                sharedMaterials[0] = value;
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

        #region IRenderable Members
        /// <summary>
        /// Draw the actual thing that we want to render
        /// </summary>
        /// <param name="device"></param>
        /// <param name="cam"></param>
        /// <returns>Returns an estimated number of draw calls that we make in the draw routine.</returns>
        public abstract int Draw(GraphicsDevice device, Camera cam);
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
    }
}
