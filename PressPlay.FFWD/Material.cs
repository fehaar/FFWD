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

        public void SetColor(string name, Color color)
        {
            if (blendState == BlendState.Additive)
            {
                this.color = new Color(color.r, color.g, color.b, 1.0f);
            }
            else
            {
                this.color = color;
            }
        }

        internal override void LoadAsset(AssetHelper assetHelper)
        {
            if (mainTexture != null)
            {
                texture = assetHelper.Load<Texture2D>("Textures/" + mainTexture);
            }

            // NOTE: We have hardcoded shader values here that should be configurable in some other way
            blendState = BlendState.Opaque;
            if (shader == "iPhone/Particles/Additive Culled")
            {
                // HACK : Fix color for Additive shader to disallow alpha
                color = new Color(color.r, color.g, color.b, Mathf.Clamp01(color.a * 3));
                blendState = BlendState.Additive;
            } 
            else if (renderQueue == 3000 || shader == "TransperantNoLight")
            {
                blendState = BlendState.AlphaBlend;
            }
        }

        [ContentSerializerIgnore]
        public BlendState blendState { get; private set; }

        internal void SetBlendState(GraphicsDevice device)
        {
            if (device.BlendState != blendState)
            {
                device.BlendState = blendState;
            }
        }

        private float finalRenderQueue = float.MinValue;
        internal float CalculateRenderQueue()
        {
            if (finalRenderQueue < renderQueue)
            {
                finalRenderQueue = renderQueue;
                if (blendState == BlendState.AlphaBlend)
                {
                    finalRenderQueue += 0.1f;
                }
                if (blendState == BlendState.Additive)
                {
                    finalRenderQueue += 0.2f;
                }
            }
            return finalRenderQueue;
        }

        public static readonly Material Default = new Material();
    }
}
