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

        protected SpriteBatch batch;

        public UIRenderer() : base(){
            if (material == null)
            {
                material = new Material();
                material.renderQueue = 1000;
                material.SetColor("", Color.white);
            }
        }

        public override void Draw(GraphicsDevice device, Camera cam)
        {
            if (batch == null)
            {
                batch = new SpriteBatch(device);
            }
        }
    }
}
