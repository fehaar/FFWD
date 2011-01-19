using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.UI.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.UI
{
    public abstract class UIRenderer : Renderer
    {
        public Rectangle clipRect = Rectangle.Empty;

        protected Control _control;
        protected Control control
        {
            get
            {
                if (_control == null)
                {
                    _control = gameObject.GetComponent<Control>();
                }

                return _control;
            }
        }

        // This is set within the camera class for centralized batching
        public static SpriteBatch batch;

        public static void SetSpriteBatch(GraphicsDevice device)
        {
            UIRenderer.batch = new SpriteBatch(device);
        }

        public UIRenderer() : base(){
            if (material == null)
            {
                material = new Material();
                material.renderQueue = 1000;
                material.SetColor("", Color.white);
            }
        }
    }
}
