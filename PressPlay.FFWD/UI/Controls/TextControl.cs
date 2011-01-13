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
        private SpriteFont _font;
        private string _text;

        public Color Color;

        // Actual text to draw
        public string text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
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
            this.text = text;
            this.font = font;
            this.position = position;
            this.Color = color;

            gameObject.AddComponent(new UITextRenderer(text, font));
            gameObject.name = "TextControl";
        }

        /*
        public override void Draw(DrawContext context)
        {
            base.Draw(context);

            context.SpriteBatch.DrawString(font, Text, context.DrawOffset, Color);
        }
        */

        override public Vector2 ComputeSize()
        {
            return font.MeasureString(text);
        }
    }
}