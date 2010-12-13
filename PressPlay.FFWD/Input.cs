using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;

namespace PressPlay.FFWD
{
    public static class Input
    {
        private static MouseState lastMouseState;
        private static MouseState currentMouseState;

        internal static void Update()
        {
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public static Vector2 mousePosition
        {
            get
            {
                return new Vector2(currentMouseState.X, currentMouseState.Y);
            }
        }

        public static float GetAxis(string axisName)
        {
            return 0.0f;
        }

        public static bool GetButton(string buttonName)
        {
            return false;
        }

        public static bool GetButtonUp(string buttonName)
        {
            return false;
        }

        public static bool GetButtonDown(string buttonName)
        {
            return false;
        }

        public static bool GetMouseButton(int button)
        {
            switch (button)
            {
                case 0:
                    return currentMouseState.LeftButton == ButtonState.Pressed;
                case 1:
                    return currentMouseState.MiddleButton == ButtonState.Pressed;
                case 2:
                    return currentMouseState.RightButton == ButtonState.Pressed;
            }
            return false;
        }

        public static bool GetMouseButtonDown(int button)
        {
            switch (button)
            {
                case 0:
                    return (currentMouseState.LeftButton == ButtonState.Pressed) && (lastMouseState.LeftButton == ButtonState.Released);
                case 1:
                    return (currentMouseState.MiddleButton == ButtonState.Pressed) && (lastMouseState.MiddleButton == ButtonState.Released);
                case 2:
                    return (currentMouseState.RightButton == ButtonState.Pressed) && (lastMouseState.RightButton == ButtonState.Released);
            }
            return false;
        }

        public static bool GetMouseButtonUp(int button)
        {
            switch (button)
            {
                case 0:
                    return (currentMouseState.LeftButton == ButtonState.Released) && (lastMouseState.LeftButton == ButtonState.Pressed);
                case 1:
                    return (currentMouseState.MiddleButton == ButtonState.Released) && (lastMouseState.MiddleButton == ButtonState.Pressed);
                case 2:
                    return (currentMouseState.RightButton == ButtonState.Released) && (lastMouseState.RightButton == ButtonState.Pressed);
            }
            return false;
        }
    }
}
