using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public static class GUI
    {
        private static bool isRendering = false;
        internal static SpriteBatch spriteBatch = null;

        public static GUISkin skin = new GUISkin();
        public static Color backgroundColor = Color.white;
        public static Color color = Color.white;
        public static SpriteFont spriteFont;

        public static void DrawTexture(Rect rect, Texture texture)
        {
            if (isRendering)
            {
                Rectangle r = rect;
                if (Camera.FullScreen.Bounds.Contains(r))
                {
                    spriteBatch.Draw((Microsoft.Xna.Framework.Graphics.Texture2D)texture, r, color);
                }
            }
        }

        public static void Label(Rect rect, string text)
        {
            Label(rect, text, GUIStyle.none);
        }

        public static void Label(Rect rect, string text, GUIStyle style)
        {
            if (isRendering)
            {
                Rectangle r = rect;
                if (Camera.FullScreen.Bounds.Contains(r))
                {
                    Microsoft.Xna.Framework.Vector2 pos = new Microsoft.Xna.Framework.Vector2(r.Location.X, r.Location.Y);

                    if (style.alignment == TextAnchor.MiddleCenter)
                    {
                        Microsoft.Xna.Framework.Vector2 sz = GUI.spriteFont.MeasureString(text);
                        pos += new Microsoft.Xna.Framework.Vector2(Mathf.Floor((rect.width - sz.X) / 2), Mathf.Floor((rect.height - sz.Y) / 2));
                    }

                    spriteBatch.DrawString(GUI.spriteFont, text, pos, color);
                }
            }
        }

        public static bool Button(Rect rect, string text)
        {
            if (isRendering)
            {
                bool result = Button(rect, GUI.skin.button.normal.background, GUI.skin.button);
                Label(rect, text, GUI.skin.button);
                return result;
            }
            return false;
        }

        public static bool Button(Rect rect, Texture texture)
        {
            return Button(rect, texture, GUI.skin.button);
        }

        public static bool Button(Rect rect, Texture texture, GUIStyle style)
        {
            if (isRendering)
            {
                Rectangle r = rect;
                if (Camera.FullScreen.Bounds.Contains(r))
                {
                    spriteBatch.Draw((Texture2D)texture, r, color);
                }
                if (Input.GetMouseButtonDown(0) && rect.Contains(Input.mousePositionClean))
                {
                    return true;
                }
            }
            return false;
        }

        public static string TextField(Rect rect, string m_strLevelName)
        {
            //throw new NotImplementedException();
            return m_strLevelName;
        }

        internal static void StartRendering()
        {
            isRendering = true;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }

        internal static void EndRendering()
        {
            isRendering = false;
            spriteBatch.End();
        }

        internal static void RenderComponents(List<Components.MonoBehaviour> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if ((bool)list[i])
                {
                    list[i].OnGUI();
                }
            }
        }
    }
}
