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

        public static void DrawTexture(Rect rect, Texture texture)
        {
            if (isRendering)
            {
                Rectangle r = rect;
                if (Camera.main.viewPort.Bounds.Contains(r))
                {
                    spriteBatch.Draw((Texture2D)texture, r, color);
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
                if (Camera.main.viewPort.Bounds.Contains(r))
                {
                    Microsoft.Xna.Framework.Vector2 pos = new Microsoft.Xna.Framework.Vector2(r.Location.X, r.Location.Y);
                    spriteBatch.DrawString(ApplicationSettings.DebugFont, text, pos, color);
                }
            }
        }

        public static bool Button(Rect rect, string text)
        {
            throw new NotImplementedException();
        }

        public static bool Button(Rect rect, Texture texture)
        {
            return Button(rect, texture, GUIStyle.none);
        }

        public static bool Button(Rect rect, Texture texture, GUIStyle style)
        {
            if (isRendering)
            {
                Rectangle r = rect;
                if (Camera.main.viewPort.Bounds.Contains(r))
                {
                    spriteBatch.Draw((Texture2D)texture, r, color);
                }
            }
            // TODO: We need to check input
            return false;
        }

        public static string TextField(Rect rect, string m_strLevelName)
        {
            throw new NotImplementedException();
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
                list[i].OnGUI();
            }
        }
    }
}
