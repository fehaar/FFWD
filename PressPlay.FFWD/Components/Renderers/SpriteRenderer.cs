using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class SpriteRenderer : Renderer, Interfaces.IUpdateable
    {
        #region Content properties
        [ContentSerializer(Optional=true)]
        public string Texture;
        #endregion

        [ContentSerializerIgnore]
        private Texture2D _texture;

        public Texture2D texture
        {
            get
            {
                return _texture;
            }
            set
            {
                _texture = value;

                if (_texture != null)
                {
                    bounds = _texture.Bounds;
                }
            }
        }

        public Vector2 Position = Vector2.zero;
        public Rectangle bounds = Rectangle.Empty;
        public Vector2 Origin = Vector2.zero;
        public float Scale = 1f;
        public SpriteEffects Effects = SpriteEffects.None;
        public float LayerDepth = 0;

        public SpriteRenderer()
        {

        }

        public SpriteRenderer(string texture)
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
                material.texture = texture;
            }

            if(texture != null){
                bounds = texture.Bounds;
            }

            if (material == null)
            {
                material = new Material();
            }
        }

        #region IUpdateable Members

        public void Update()
        {
            Position.x = transform.localPosition.x;
            Position.y = transform.localPosition.y;
        }

        #endregion

        #region IRenderable Members
        protected SpriteBatch batch;
        public override void Draw(GraphicsDevice device, Camera cam)
        {
            if (texture == null)
            {
                return;
            }

            if (batch == null)
            {
                batch = new SpriteBatch(device);
            }

            batch.Begin();
            batch.Draw(texture, Position, bounds, material.color, transform.eulerAngles.y, Origin, Scale, Effects, LayerDepth);
            batch.End();
        }
        #endregion
    }
}
