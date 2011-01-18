using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD
{
    public class Material : Asset
    {
        [ContentSerializer]
        private string shader { get; set; }
        [ContentSerializer]
        public int renderQueue { get; set; }
        [ContentSerializer(Optional = true)]
        public Color color { get; set; }
        [ContentSerializer(Optional = true)]
        public string mainTexture { get; set; }
        [ContentSerializer(Optional = true)]
        public Vector2 mainTextureOffset { get; set; }
        [ContentSerializer(Optional = true)]
        public Vector2 mainTextureScale { get; set; }

        [ContentSerializerIgnore]
        public Texture2D texture;

        //public void PrepareLoadContent()
        //{
        //    ContentHelper.LoadTexture(mainTexture);
        //}

        //public void EndLoadContent()
        //{
        //    if (texture == null)
        //    {
        //        texture = ContentHelper.GetTexture(mainTexture);
        //    }
        //}

        public void SetColor(string name, Color color)
        {
            this.color = color;
        }

        [ContentSerializerIgnore]
        public BlendState blendState
        {
            get
            {
                // NOTE: We have hardcoded shader values here that should be configurable in some other way
                if (shader == "iPhone/Particles/Additive Culled")
                {
                    return BlendState.Additive;
                }
                if (renderQueue == 3000 || shader == "TransperantNoLight")
                {
                    return BlendState.AlphaBlend;
                }
                return BlendState.Opaque;
            }
        }

        internal override void LoadAsset(AssetHelper assetHelper)
        {
            if (mainTexture != null)
            {
                texture = assetHelper.Load<Texture2D>("Textures/" + mainTexture);
            }
        }
    }
}
