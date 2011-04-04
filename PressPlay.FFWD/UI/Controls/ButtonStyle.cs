using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.UI.Controls
{
    public class ButtonStyle
    {
        public Texture2D texture;

        private Dictionary<int, Rectangle> sources;

        public Rectangle this[int state]
        {
            get
            {
                return sources[state];
            }
            set
            {
                sources[state] = value;
            }
        }

        public ButtonStyle(Texture2D texture)
            : this(texture, texture.Bounds, texture.Bounds, texture.Bounds, texture.Bounds)
        {

        }

        public ButtonStyle(Texture2D texture, Rectangle normal)
            : this(texture, normal, normal, normal, normal)
        {

        }

        public ButtonStyle(Texture2D texture, Rectangle normal, Rectangle pressed)
            : this(texture, normal, pressed, normal, normal)
        {

        }

        public ButtonStyle(Texture2D texture, Rectangle normal, Rectangle pressed, Rectangle hover)
            : this(texture, normal, pressed, hover, normal)
        {

        }

        public ButtonStyle(Texture2D texture, Rectangle normal, Rectangle pressed, Rectangle hover, Rectangle disabled)
        {
            this.texture = texture;
            
            sources = new Dictionary<int, Rectangle>();
            sources.Add(1, normal);
            sources.Add(2, pressed);
            sources.Add(0, hover);
            sources.Add(3, disabled);
        }

        public Rectangle GetSourceRect(int state)
        {
            if (sources[state] != Rectangle.Empty)
            {
                return sources[state];
            }

            return sources[1];
        }
    }

    public struct ButtonTexture{
        public Texture2D texture;
        public Rectangle sourceRect;
    }
}
