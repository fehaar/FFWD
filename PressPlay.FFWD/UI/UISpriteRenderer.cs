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
                /*
                if (value != null)
                {
                    _bounds = value.Bounds;
                }
                */
            }
        }

        private Vector2 sourceOffset = Vector2.zero;
        private Rectangle _sourceRect = Rectangle.Empty;
        public Rectangle sourceRect
        {
            get
            {
                if (_sourceRect == Rectangle.Empty)
                {
                    return new Rectangle(0, 0, texture.Bounds.Width, texture.Bounds.Height);
                }
                else
                {
                    return _sourceRect;
                }
            }
            set
            {
                _sourceRect = value;
            }
        }
        /*
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
        */
        public Vector2 origin = Vector2.zero;
        public float scale = 1f;
        public SpriteEffects effects = SpriteEffects.None;
        public float layerDepth = 0;

        private Vector2 drawPosition = Vector2.zero;

        public UISpriteRenderer() : base()
        {

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
            /*
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
            }*/
        }

        #region IRenderable Members

        public override int Draw(GraphicsDevice device, Camera cam)
        {
            if (texture == null || texture.IsDisposed)
            {
                return 0;
            }

            Rectangle bounds;
            if (control != null)
            {
                bounds = control.bounds;
            }
            else
            {
                Vector2 pos = transform.position;
                Vector2 sz = transform.lossyScale;
                bounds = new Rectangle((int)pos.x, (int)pos.y, (int)sz.x, (int)sz.y);
            }

            float depth = 1 - ((float)transform.position / 10000f);

            UIRenderer.batch.Draw(texture, bounds, sourceRect, material.color, transform.eulerAngles.y, origin, effects, depth);
            return 0;
        }
        #endregion
    }
}
