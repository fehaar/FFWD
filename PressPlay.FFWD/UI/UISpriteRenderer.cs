using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.UI.Controls;

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

                if (value != null)
                {
                    _bounds = value.Bounds;
                }
            }
        }

        private Vector2 sourceOffset = Vector2.zero;
        private Rectangle sourceRect
        {
            get
            {
                return new Rectangle((int)sourceOffset.x, (int)sourceOffset.y, texture.Bounds.Width, texture.Bounds.Height);
            }
        }
        private Rectangle _bounds = Rectangle.Empty;
        public Rectangle bounds
        {
            get
            {
                //_bounds = new Rectangle(0, 0, (int)control.size.x, (int)control.size.y);
                _bounds = new Rectangle((int)(control.drawOffset.x + transform.position.x), (int)(control.drawOffset.y + transform.position.z), texture.Bounds.Width, texture.Bounds.Height);
                
                return _bounds;
            }
        }

        public Vector2 origin = Vector2.zero;
        public float scale = 1f;
        public SpriteEffects effects = SpriteEffects.None;
        public float layerDepth = 0;

        private Vector2 drawPosition = Vector2.zero;

        private Control _control;
        private Control control{
            get
            {
                if (_control == null)
                {
                    _control = gameObject.GetComponent<Control>();
                }

                return _control;
            }
        }

        public UISpriteRenderer() : base()
        {
            if (material == null)
            {
                material = new Material();
                material.renderQueue = 1000;
                material.SetColor("", Color.white);
            }
        }

        public UISpriteRenderer(string texture) : this()
        {
            this.Texture = texture;
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

        private void CalculateDrawOffset(){
            if (drawPosition.y < 0)
            {
                if(drawPosition.y + bounds.Height < 0){
                    sourceOffset.y = -(drawPosition.y + bounds.Height);
                }else{
                    sourceOffset.y = -drawPosition.y;
                }
            }
            else if (drawPosition.y + bounds.Height > clipRect.Height)
            {
                if (drawPosition.y < clipRect.Height)
                {
                    sourceOffset.y = (drawPosition.y + bounds.Height);
                }
                else
                {
                    sourceOffset.y = drawPosition.y;
                }
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

            
            if (!clipRect.Contains(bounds))
            {
                //return;
            }

            drawPosition = new Vector2(control.drawOffset.x + transform.position.x,  control.drawOffset.y + transform.position.z);

            //CalculateDrawOffset();

            batch.Begin();
            batch.Draw(texture, drawPosition, sourceRect, material.color, transform.eulerAngles.y, origin, scale, effects, layerDepth);
            batch.End();
        }
        #endregion
    }
}
