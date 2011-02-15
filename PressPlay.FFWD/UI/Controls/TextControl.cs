//-----------------------------------------------------------------------------
// TextControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace PressPlay.FFWD.UI.Controls
{
    /// <summary>
    /// TextControl is a control that displays a single string of text. By default, the
    /// size is computed from the given text and spritefont.
    /// </summary>
    public class TextControl : Control
    {

        private UITextRenderer textRenderer;

        private SpriteFont _font;
        private string _text;

        //public Color Color;

        // Actual text to draw
        public string text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    _text = _text.Replace("”", "");
                    textRenderer.text = _text;
                    InvalidateAutoSize();
                }
            }
        }

        // Font to use
        public SpriteFont font
        {
            get { return _font; }
            set
            {
                if (_font != value)
                {
                    _font = value;
                    textRenderer.font = value;
                    InvalidateAutoSize();
                }
            }
        }

        public TextControl()
            : this(string.Empty, null, Color.white, Vector2.zero)
        {
        }

        public TextControl(string text, SpriteFont font)
            : this(text, font, Color.white, Vector2.zero)
        {
        }

        public TextControl(string text, SpriteFont font, Color color)
            : this(text, font, color, Vector2.zero)
        {
        }

        public TextControl(string text, SpriteFont font, Color color, Vector2 position) : base()
        {
            textRenderer = (UITextRenderer)gameObject.AddComponent(new UITextRenderer(text, font));
            gameObject.name = "TextControl";


            this.text = text;
            this.font = font;
            this.position = position;
            SetColor(color);

            //this.Color = color;
        }

        public void ScaleTextToFit(Rectangle rect, float margin)
        {
            if (bounds.Width < rect.Width - margin)
            {
                return;
            }
            transform.localScale = new Vector3((rect.Width - margin) / (bounds.Width));
            InvalidateAutoSize();
        }

        public void CenterTextWithinBounds(Rectangle rect)
        {
            Vector2 pos = Vector2.zero;

            pos.x = rect.X + rect.Width / 2 - size.x / 2;
            pos.y = rect.Y + rect.Height / 2 - size.y / 2;

            transform.position = new Vector3(pos.x, transform.position.y, pos.y);
        }

        public void SetColor(Color color)
        {
            renderer.material.color = color;
        }

        override public Vector2 ComputeSize()
        {
            if (font == null)
            {
                return Vector2.zero;
            }

            return font.MeasureString(text);
        }
    }
}