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

        [ContentSerializer(CollectionItemName = "material")]
        public Material[] materials{ get; set; }

        [ContentSerializerIgnore]
        public bool enabled { get; set; }

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
                if (materials == null)
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

        #region IRenderable Members
        public abstract void Draw(GraphicsDevice device, Camera cam);
        #endregion

    }
}
