using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.UI
{
    public class UIButtonRenderer : UISpriteRenderer
    {

        private string _text = "";
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                textHasChanged = true;
            }
        }

        public bool upperCase = false;
        public SpriteFont font;
        public Vector2 textOffset = Vector2.zero;
        public Color textColor = Color.white;

        private Vector2 textSize = Vector2.zero;
        private bool textHasChanged = false;
        
        public UIButtonRenderer() : base()
        {

        }

        public UIButtonRenderer(string texture) : base(texture)
        {

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
        public override void Draw(GraphicsDevice device, Camera cam)
        {
            base.Draw(device, cam);

            if (font == null || text == "") return;

            if (textHasChanged)
            {
                textSize = font.MeasureString(_text);
                textHasChanged = false;
            }

            batch.Begin();
            batch.DrawString(font, ((upperCase) ? _text.ToUpper() : _text), new Vector2(transform.localPosition.x + bounds.Width / 2 - textSize.x / 2 + textOffset.x, transform.localPosition.y + bounds.Height / 2 - textSize.y / 2 + textOffset.y), textColor);
            batch.End();
        }
        #endregion
    }
}
