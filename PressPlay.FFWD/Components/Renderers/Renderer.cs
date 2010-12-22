using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public abstract class Renderer : Component, IRenderable
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
                ContentHelper.LoadTexture(sharedMaterial.mainTexture);
            }
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    ContentHelper.LoadTexture(materials[i].mainTexture);
                }
            }
        }

        public override void Start()
        {
            base.Start();
            if (sharedMaterial != null)
            {
                sharedMaterial.texture = ContentHelper.GetTexture(sharedMaterial.mainTexture);
            }
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].texture = ContentHelper.GetTexture(materials[i].mainTexture);
                }
            }
        }

        #region IRenderable Members
        public abstract void Draw(SpriteBatch batch);
        #endregion

    }
}
