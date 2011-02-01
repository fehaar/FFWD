using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.UI.Controls;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PressPlay.FFWD.UI
{
    public abstract class UIRenderer : Renderer
    {
        public UIRenderer()
            : base()
        {
            if (material == null)
            {
                material = new Material();
                material.renderQueue = 1000;
                material.SetColor("", Color.white);
            }
        }

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

        internal static SpriteBatch batch;
        private static List<UIRenderer> uiRenderQueue = new List<UIRenderer>();
        internal static void doRender(GraphicsDevice device)
        {
            if (batch == null)
            {
                batch = new SpriteBatch(device);
            }
            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            for (int i = 0; i < uiRenderQueue.Count; i++)
            {
                if (uiRenderQueue[i].gameObject == null)
                {
                    // This will happen if the game object has been destroyed in update.
                    // It is acceptable behaviour.
                    continue;
                }
                uiRenderQueue[i].Draw(device, null);
            }
            batch.End();
        }

        internal static void AddRenderer(UIRenderer renderer)
        {
            uiRenderQueue.Add(renderer);
        }

        internal static void RemoveRenderer(UIRenderer renderer)
        {
            uiRenderQueue.Remove(renderer);
        }
    }
}
