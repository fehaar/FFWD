using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.UI.Controls
{
    public class ImageControl : Control
    {



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

        public ImageControl(Texture2D texture, Rectangle sourceRect)
            : this(texture)
        {
            ((UISpriteRenderer)gameObject.renderer).sourceRect = sourceRect;
        }

        public override Vector2 ComputeSize()
        {
            UISpriteRenderer r = (UISpriteRenderer)gameObject.renderer;
            Vector2 scale = transform.lossyScale;
            if (r.texture != null)
            {
                if (r.sourceRect != Rectangle.Empty)
                {
                    return new Vector2(r.sourceRect.Width, r.sourceRect.Height) * scale;
                }
                else
                {
                    return new Vector2(texture.Width, texture.Height) * scale;
                }
            }
            return Vector2.zero;
        }
    }
}
