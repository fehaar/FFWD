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
            enabled = true;
        }

        [ContentSerializer(Optional=true)]
        public bool isPartOfStaticBatch = false;

        public bool enabled { get; set; }

        [ContentSerializer(CollectionItemName = "material")]
        public Material[] materials{ get; set; }

        [ContentSerializerIgnore]
        public Material sharedMaterial
        {
            get 
            {
                return material;
            }
            set
            {
                material = value;
            }
        }

        [ContentSerializerIgnore]
        public Material material
        {
            get
            {
                if (materials == null || materials.Length == 0)
                {
                    return null;
                }
                return materials[0];
            }
            set
            {
                if (materials == null)
                {
                    materials = new Material[1]; 
                }
                materials[0] = value;
            }
        }

        public Bounds bounds
        {
            get
            {
                //throw new NotImplementedException("Not implemented");
                return new Bounds();
            }
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
