using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;
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
        private static GamePadState _lastGamepadState;
        private static GamePadState _currentGamePadState;
        private static int _activeTouches;
        private static Touch[] _touches;
        private static readonly Touch[] _noTouch = new Touch[0];

        private static bool[] mouseHolds = new bool[3];
        private static bool[] mouseDowns = new bool[3];
        private static bool[] mouseUps = new bool[3];
        private static bool isInDraw = false;

        internal static void Initialize()
        {
            _touches = new Touch[ApplicationSettings.DefaultCapacities.Touches];
#if WINDOWS_PHONE
            TouchPanel.EnabledGestures = GestureType.None;
#endif
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

            TouchCollection tc = TouchPanel.GetState();
            _activeTouches = 0;
            mouseDowns[0] = false;
            mouseUps[0] = false;
            // Check old touches to see if they are gone
            for (int i = 0; i < _touches.Length; i++)
            {
                TouchLocation tl;
                Touch t = _touches[i];
                if (tc.FindById(t.fingerId, out tl))
	            {
                    Vector2 position = new Vector2(tl.Position.X, Camera.FullScreen.Height - tl.Position.Y);
                    t.deltaPosition = position - t.position;
                    t.deltaTime = Time.deltaTime;
                    t.position = position;
                    t.cleanPosition = tl.Position;
                    if ((t.phase = ToPhase(tl.State)) == TouchPhase.Moved && t.deltaPosition == Vector2.zero)
                    {
                        t.phase = TouchPhase.Stationary;
                    }
                    if (t.phase == TouchPhase.Ended)
                    {
                        mouseUps[0] = true;
                    }
                    if (_activeTouches != i)
                    {
                        _touches[i].fingerId = -1;
                    }
                    _touches[_activeTouches++] = t;
	            }
                else
                {
                    _touches[i].fingerId = -1;
                }
            }
            // Add new touches
            for (int i = 0; i < tc.Count; i++)
            {
                TouchLocation tl = tc[i];
                if (tl.State == TouchLocationState.Invalid)
                {
                    continue;
                }
                bool existing = false;
                for (int j = 0; j < _activeTouches; j++)
                {
                    if (_touches[j].fingerId == tl.Id)
                    {
                        existing = true;
                        break;
                    }
                }
                if (existing)
                {
                    continue;
                }
                Touch t = new Touch() { fingerId = tl.Id, position = new Vector2(tl.Position.X, Camera.FullScreen.Height - tl.Position.Y), cleanPosition = tl.Position, phase = ToPhase(tl.State) };
                if (t.phase == TouchPhase.Began)
	            {
                    mouseDowns[0] = true;
                }
                _touches[_activeTouches++] = t;
            }
#else
            UpdateMouseStates();
#endif
            UpdateGamepadStates();
        }

        private static void UpdateMouseStates()
        {
            _mousePosition = new Vector2(_currentMouseState.X, Camera.FullScreen.Height - _currentMouseState.Y);
            _mousePositionXna = new Vector2(_currentMouseState.X, _currentMouseState.Y);
            mouseHolds[0] &= (_currentMouseState.LeftButton == ButtonState.Pressed);
            mouseHolds[1] &= (_currentMouseState.MiddleButton == ButtonState.Pressed);
            mouseHolds[2] &= (_currentMouseState.RightButton == ButtonState.Pressed);
            if ((_currentMouseState.LeftButton == ButtonState.Pressed) && (_lastMouseState.LeftButton == ButtonState.Released))
            {
                mouseDowns[0] = true;
            }
            if ((_currentMouseState.MiddleButton == ButtonState.Pressed) && (_lastMouseState.MiddleButton == ButtonState.Released))
            {
                mouseDowns[1] = true;
            }
            if ((_currentMouseState.RightButton == ButtonState.Pressed) && (_lastMouseState.RightButton == ButtonState.Released))
            {
                mouseDowns[2] = true;
            }
            if ((_currentMouseState.LeftButton == ButtonState.Released) && (_lastMouseState.LeftButton == ButtonState.Pressed))
            {
                mouseDowns[0] = false;
                mouseUps[0] = true;
            }
            if ((_currentMouseState.MiddleButton == ButtonState.Released) && (_lastMouseState.MiddleButton == ButtonState.Pressed))
            {
                mouseDowns[1] = false;
                mouseUps[1] = true;
            }
            if ((_currentMouseState.RightButton == ButtonState.Released) && (_lastMouseState.RightButton == ButtonState.Pressed))
            {
                mouseDowns[2] = false;
                mouseUps[2] = true;
            }
        }

        private static void UpdateGamepadStates()
        {
            _lastGamepadState = _currentGamePadState;
            _currentGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        internal static void ClearStates()
        {
            // Clear all variables set since last fixed update to make sure that we get consistant values in draw
            for (int i = 0; i < mouseDowns.Length; i++)
            {
                mouseDowns[i] = false;
                mouseHolds[i] = true;
                mouseUps[i] = false;
            }
            isInDraw = false;
        }

        internal static void IsInDraw()
        {
            isInDraw = true;
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

        private static Vector2 _mousePosition;
        public static Vector2 mousePosition
        {
            get
            {
                return _mousePosition;
            }
        }

        private static Vector2 _mousePositionXna;
        /// <summary>
        /// This is the mouse position in XNA terms where Y is not inverted.
        /// </summary>
        internal static Vector2 mousePositionXna
        {
            get
            {
                return _mousePositionXna;
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
            return touchCount > 0;
#else
            switch (button)
            {
                case 0:
                    return (isInDraw) ? mouseHolds[0] : _currentMouseState.LeftButton == ButtonState.Pressed;
                case 1:
                    return (isInDraw) ? mouseHolds[1] : _currentMouseState.MiddleButton == ButtonState.Pressed;
                case 2:
                    return (isInDraw) ? mouseHolds[2] : _currentMouseState.RightButton == ButtonState.Pressed;
            }
            return false;
#endif
        }

        public static bool GetMouseButtonDown(int button)
        {
#if WINDOWS_PHONE
            return mouseDowns[button];
#else
            switch (button)
            {
                case 0:
                    return (isInDraw) ? mouseDowns[0] : (_currentMouseState.LeftButton == ButtonState.Pressed) && (_lastMouseState.LeftButton == ButtonState.Released);
                case 1:
                    return (isInDraw) ? mouseDowns[1] : (_currentMouseState.MiddleButton == ButtonState.Pressed) && (_lastMouseState.MiddleButton == ButtonState.Released);
                case 2:
                    return (isInDraw) ? mouseDowns[2] : (_currentMouseState.RightButton == ButtonState.Pressed) && (_lastMouseState.RightButton == ButtonState.Released);
            }
            return false;
#endif
        }

        public static bool GetMouseButtonUp(int button)
        {
#if WINDOWS_PHONE
            return mouseUps[button];
#else
            switch (button)
            {
                case 0:
                    return (isInDraw) ? mouseUps[0] : (_currentMouseState.LeftButton == ButtonState.Released) && (_lastMouseState.LeftButton == ButtonState.Pressed);
                case 1:
                    return (isInDraw) ? mouseUps[1] : (_currentMouseState.MiddleButton == ButtonState.Released) && (_lastMouseState.MiddleButton == ButtonState.Pressed);
                case 2:
                    return (isInDraw) ? mouseUps[2] : (_currentMouseState.RightButton == ButtonState.Released) && (_lastMouseState.RightButton == ButtonState.Pressed);
            }
            return false;
#endif
        }

        public static int touchCount
        {
            get { return _activeTouches; }
        }

        public static Touch[] touches
        {
            get
            {
                if (_activeTouches == 0)
                {
                    return _noTouch;
                }
                return _touches.Take(_activeTouches).ToArray();
            }
        }

        public static bool GetKeyUp(Keys key)
        {
#if WINDOWS_PHONE
            if (key == Keys.Back)
            {
                return (_lastGamepadState.Buttons.Back == ButtonState.Pressed) && (_currentGamePadState.Buttons.Back == ButtonState.Released);
            }
            return false;
#else
            return _currentKeyboardState.IsKeyUp(key) && _lastKeyboardState.IsKeyDown(key);
#endif
        }

        public static bool GetKeyDown(Keys key)
        {
#if WINDOWS_PHONE
            if (key == Keys.Back)
            {
                return (_lastGamepadState.Buttons.Back == ButtonState.Released) && (_currentGamePadState.Buttons.Back == ButtonState.Pressed);
            }
            return false;
#else
            return _currentKeyboardState.IsKeyDown(key) && _lastKeyboardState.IsKeyUp(key);
#endif
        }

        private static string _inputString = ""; // TODO
        public static string inputString
        {
            get
            {
                return _inputString;
            }
        }
    }
}
