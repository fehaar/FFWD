using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.UI
{
    public class UISpriteRenderer : UIRenderer
    {
        public string Texture;

        public Texture2D texture
        {
            get
            {                
                return material.texture;
            }
            set
            {
                material.texture = value;

                if (material.texture != null)
                {
                    bounds = material.texture.Bounds;
                }
            }
        }

        public Rectangle bounds = Rectangle.Empty;
        public Vector2 origin = Vector2.zero;
        public float scale = 1f;
        public SpriteEffects effects = SpriteEffects.None;
        public float layerDepth = 0;

        public UISpriteRenderer() : base()
        {

        }

        public UISpriteRenderer(string texture) : this()
        {
            this.Texture = texture;

            if (material == null)
            {
                material = new Material();
                material.renderQueue = 1000;
            }
        }

        public override void Awake()
        {
            base.Awake();
            ContentHelper.LoadTexture(Texture);
        }

        public override void Start()
        {
            base.Start();
            if (texture == null)
            {
                texture = ContentHelper.GetTexture(Texture);
            }

            if (material == null)
            {
                material = new Material();
            }
        }

        #region IRenderable Members
        protected SpriteBatch batch;
        public override void Draw(GraphicsDevice device, Camera cam)
        {
            if (batch == null)
            {
                batch = new SpriteBatch(device);
            }            
            
            if (texture == null)
            {
                return;
            }

            batch.Begin();
            batch.Draw(texture, transform.localPosition, bounds, material.color, transform.eulerAngles.y, origin, scale, effects, layerDepth);
            batch.End();
        }
        #endregion
    }
}
