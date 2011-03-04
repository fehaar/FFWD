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

        public Rectangle this[ButtonControlStates state]
        {
            get
            {
                return sources[(int)state];
            }
            set
            {
                sources[(int)state] = value;
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
            sources.Add((int)ButtonControlStates.normal, normal);
            sources.Add((int)ButtonControlStates.pressed, pressed);
            sources.Add((int)ButtonControlStates.hover, hover);
            sources.Add((int)ButtonControlStates.disabled, disabled);
        }

        public Rectangle GetSourceRect(ButtonControlStates state)
        {
            if (sources[(int)state] != Rectangle.Empty)
            {
                return sources[(int)state];
            }

            return sources[(int)ButtonControlStates.normal];
        }
    }

    public struct ButtonTexture{
        public Texture2D texture;
        public Rectangle sourceRect;
    }
}
