using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.UI.Controls;
using PressPlay.FFWD;

namespace PressPlay.FFWD.Components.Renderers
{
    public class TextRenderer : Renderer
    {
        private Vector2 textSize = Vector2.zero;
        public Color color = Color.white;

        public Vector2 Position;

        private string _text = "";
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value != _text)
                {
                    _text = value;
                    textSize = font.MeasureString(_text);
                }
            }
        }

        private SpriteFont _font;
        public SpriteFont font{
            get { return _font; }
            set { _font = value; }
        }

        public TextRenderer(SpriteFont font)
            : this("", font)
        {
        }

        public TextRenderer(string text, SpriteFont font)
        {
            this.font = font;
            this.text = text;
        }

        public void Update()
        {
            Position.x = transform.localPosition.x;
            Position.y = transform.localPosition.y;
        }

        protected SpriteBatch batch;
        public override void Draw(GraphicsDevice device, Camera cam)
        {
            if (batch == null)
            {
                batch = new SpriteBatch(device);
            }

            batch.DrawString(font, text, Position, material.color);
        }
    }
}
