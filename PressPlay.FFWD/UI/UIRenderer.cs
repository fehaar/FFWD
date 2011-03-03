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

        private static List<UIRenderer> uiRenderQueue = new List<UIRenderer>();
        internal static int doRender(GraphicsDevice device)
        {
            int estDrawCalls = 0;
            Camera.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            for (int i = 0; i < uiRenderQueue.Count; i++)
            {
                if (uiRenderQueue[i].gameObject == null)
                {
                    // This will happen if the game object has been destroyed in update.
                    // It is acceptable behaviour.
                    continue;
                }
                if (!uiRenderQueue[i].enabled || !uiRenderQueue[i].gameObject.active)
                {
                    continue;
                }
                estDrawCalls++;
                uiRenderQueue[i].Draw(device, null);
            }
            Camera.spriteBatch.End();
            return estDrawCalls;
        }

        internal static void AddRenderer(UIRenderer renderer)
        {
            if (!uiRenderQueue.Contains(renderer))
            {
                uiRenderQueue.Add(renderer);
            }
        }

        internal static void RemoveRenderer(UIRenderer renderer)
        {
            if (uiRenderQueue.Contains(renderer))
            {
                uiRenderQueue.Remove(renderer);
            }
        }
    }
}
