using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.UI.Controls;
using PressPlay.FFWD;

namespace PressPlay.FFWD.UI
{
    public class UITextRenderer : UIRenderer
    {
        private SpriteFont font;
        private Vector2 renderPosition = Vector2.zero;
        private Vector2 textSize = Vector2.zero;

        public Vector2 textOffset = Vector2.zero;
        public Color color = Color.white;

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

        public UITextRenderer(SpriteFont font) : this("", font)
        {
        }

        public UITextRenderer(string text, SpriteFont font)
        {
            this.font = font;
            this.text = text;
        }

        public override void Draw(GraphicsDevice device, Camera cam)
        {
            base.Draw(device, cam);
            /*
            renderPosition.x = control.bounds.Width/2 - textSize.x / 2 +  textOffset.x;
            renderPosition.y = control.bounds.Height/2 - textSize.y / 2 +  textOffset.y;
            */
            UIRenderer.batch.DrawString(font, text, transform.position, material.color);
        }
    }
}
