using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.ScreenManager;
#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif

namespace PressPlay.FFWD
{
    public static class Input
    {
        private static MouseState _lastMouseState;
        private static MouseState _currentMouseState;
        private static KeyboardState _lastKeyboardState;
        private static KeyboardState _currentKeyboardState;

        internal static void Initialize()
        {
            _touches = new Touch[ApplicationSettings.DefaultCapacities.Touches];
        }

        public static void Update()
        {
            _lastMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
            _lastKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

#if WINDOWS_PHONE
            gestureCount = 0;
            while (TouchPanel.IsGestureAvailable)
            {
                gestures[gestureCount++] = TouchPanel.ReadGesture();
            }

            _touchCount = 0;
            TouchCollection tc = TouchPanel.GetState();
            for (int i = 0; i < tc.Count; i++)
            {
                // TODO: Add support for deltas and stationary touches
                //_touches[i].phase = TouchPhase.Canceled;
                TouchLocation tl = tc[i];
                if (tl.State == TouchLocationState.Invalid)
                {
                    continue;
                }
                Touch t = new Touch() { fingerId = tl.Id, position = tl.Position, phase = ToPhase(tl.State) };
                Debug.Log("Touch " + _touchCount + " phase " + t.phase);
                _touches[_touchCount++] = t;
                //switch (tl.State)
                //{
                //    case TouchLocationState.Moved:
                //        _hasAFingerOnScreen = true;
                //        _lastTapPosition = tl.Position;
                //        break;
                //    case TouchLocationState.Pressed:
                //        _hasAFingerOnScreen = true;
                //        _newTap = true;
                //        _lastTapPosition = tl.Position;
                //        break;
                //}
            }
#endif
        }

#if WINDOWS_PHONE
        private static TouchPhase ToPhase(TouchLocationState touchLocationState)
        {
            switch (touchLocationState)
            {
                case TouchLocationState.Invalid:
                    return TouchPhase.Canceled;
                case TouchLocationState.Moved:
                    return TouchPhase.Moved;
                case TouchLocationState.Pressed:
                    return TouchPhase.Began;
                case TouchLocationState.Released:
                    return TouchPhase.Ended;
                default:
                    throw new InvalidOperationException("The touch state " + touchLocationState + " does not exist!");
            }
        }
#endif

#if WINDOWS_PHONE
        private static GestureSample[] gestures = new GestureSample[ApplicationSettings.DefaultCapacities.GestureSamples];
        private static int gestureCount = 0;
        public static IEnumerable<GestureSample> GetSample(GestureType type)
        {
            for (int i = 0; i < gestureCount; i++)
            {
                if ((gestures[i].GestureType & type) != 0)
                {
                    yield return gestures[i];
                }
            }
            yield break;
        }

        public static bool HasSample(GestureType type)
        {
            for (int i = 0; i < gestureCount; i++)
            {
                if ((gestures[i].GestureType & type) != 0)
                {
                    return true;
                }
            }
            return false;
        }
#endif

        public static Vector2 mousePosition
        {
            get
            {
#if WINDOWS_PHONE
                if (_touchCount > 0)
                {
                    for (int i = 0; i < _touchCount; i++)
                    {
                        return _touches[0].position;
                    }
                }
                return Vector2.zero;
#else
                return new Vector2(_currentMouseState.X, _currentMouseState.Y);
#endif
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
#if WINDOWS_PHONE
            return _touchCount > 0;
#else
            switch (button)
            {
                case 0:
                    return _currentMouseState.LeftButton == ButtonState.Pressed;
                case 1:
                    return _currentMouseState.MiddleButton == ButtonState.Pressed;
                case 2:
                    return _currentMouseState.RightButton == ButtonState.Pressed;
            }
            return false;
#endif
        }

        public static bool GetMouseButtonDown(int button)
        {
#if WINDOWS_PHONE
            for (int i = 0; i < _touchCount; i++)
            {
                if (_touches[i].phase == TouchPhase.Began)
                {
                    return true;
                }
            }
            return false;
#else
            switch (button)
            {
                case 0:
                    return (_currentMouseState.LeftButton == ButtonState.Pressed) && (_lastMouseState.LeftButton == ButtonState.Released);
                case 1:
                    return (_currentMouseState.MiddleButton == ButtonState.Pressed) && (_lastMouseState.MiddleButton == ButtonState.Released);
                case 2:
                    return (_currentMouseState.RightButton == ButtonState.Pressed) && (_lastMouseState.RightButton == ButtonState.Released);
            }
            return false;
#endif
        }

        public static bool GetMouseButtonUp(int button)
        {
#if WINDOWS_PHONE
            return !GetMouseButtonDown(button);
#else
            switch (button)
            {
                case 0:
                    return (_currentMouseState.LeftButton == ButtonState.Released) && (_lastMouseState.LeftButton == ButtonState.Pressed);
                case 1:
                    return (_currentMouseState.MiddleButton == ButtonState.Released) && (_lastMouseState.MiddleButton == ButtonState.Pressed);
                case 2:
                    return (_currentMouseState.RightButton == ButtonState.Released) && (_lastMouseState.RightButton == ButtonState.Pressed);
            }
            return false;
#endif
        }

        private static int _touchCount = 0;
        private static Touch[] _touches;
        private static Touch[] _noTouch = new Touch[0];
        public static Touch[] touches 
        { 
            get
            {
                if (_touchCount == 0)
                {
                    return _noTouch;
                }
                Debug.Log("Return " + _touchCount + " touches");
                return _touches.Take(_touchCount).ToArray();
            }
        }

        public static bool GetKeyUp(Keys key)
        {
#if WINDOWS_PHONE
            return false;
#else
            return _currentKeyboardState.IsKeyUp(key) && _lastKeyboardState.IsKeyDown(key);
#endif
        }
    }
}
