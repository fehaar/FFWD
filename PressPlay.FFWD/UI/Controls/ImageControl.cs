using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.UI.Controls
{
    public class ImageControl : Control
    {
        // Position within the source texture, in texels. Default is (0,0) for the upper-left corner.
        public Vector2 origin;

        // Size in texels of source rectangle. If null (the default), size will be the same as the size of the control.
        // You only need to set this property if you want texels scaled at some other size than 1-to-1; normally
        // you can just set the size of both the source and destination rectangles with the Size property.
        public Vector2? sourceSize;

        // Color to modulate the texture with. The default is white, which displays the original unmodified texture.
        public Color color;

        // Texture to draw
        public Texture2D texture
        {
            get { return renderer.material.texture; }
            set
            {
                if (renderer.material.texture != value)
                {
                    renderer.material.texture = value;
                    InvalidateAutoSize();
                }
            }
        }

        public ImageControl(Texture2D texture) : base()
        {
            gameObject.AddComponent(new UISpriteRenderer());
            gameObject.name = "ImageControl";
            this.texture = texture;
        }

        public override Vector2 ComputeSize()
        {
            if (texture != null)
            {
                return new Vector2(texture.Width, texture.Height);
            }
            return Vector2.zero;
        }
    }
}
