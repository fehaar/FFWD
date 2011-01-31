using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.UI.Controls
{
    public class ButtonStyle
    {
        public Texture2D texture;

        private Dictionary<ButtonControlStates, Rectangle> sources;

        public Rectangle this[ButtonControlStates state]
        {
            get
            {
                return sources[state];
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

        public ButtonStyle(Texture2D texture, Rectangle normal, Rectangle pressed, Rectangle hover, Rectangle disabled)
        {
            this.texture = texture;
            
            sources = new Dictionary<ButtonControlStates, Rectangle>();
            sources.Add(ButtonControlStates.normal, normal);
            sources.Add(ButtonControlStates.pressed, pressed);
            sources.Add(ButtonControlStates.hover, hover);
            sources.Add(ButtonControlStates.disabled, disabled);
        }

        public ButtonTexture GetButtonTexture(ButtonControlStates state)
        {
            if (sources[state] != Rectangle.Empty)
            {
                return new ButtonTexture(texture, sources[state]);
            }

            return new ButtonTexture(texture, sources[ButtonControlStates.normal]);
        }
    }

    public class ButtonTexture{
        public Texture2D texture;
        public Rectangle sourceRect;

        public ButtonTexture(Texture2D texture, Rectangle sourceRect)
        {
            this.texture = texture;
            this.sourceRect = sourceRect;
        }
    }
}
