using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public abstract class Renderer : Component
    {
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

        public override void Awake()
        {
 	        base.Awake();
            if (sharedMaterial != null)
            {
                sharedMaterial.PrepareLoadContent();
            }
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].PrepareLoadContent();
                }
            }
        }

        public override void Start()
        {
            base.Start();
            if (sharedMaterial != null)
            {
                sharedMaterial.EndLoadContent();
            }
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].EndLoadContent();
                }
            }
        }

        #region IRenderable Members
        public abstract void Draw(GraphicsDevice device, Camera cam);
        #endregion

    }
}
