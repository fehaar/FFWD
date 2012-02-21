//-----------------------------------------------------------------------------
// ScrollTracker.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using PressPlay.FFWD.ScreenManager;


namespace PressPlay.FFWD.UI.Controls
{
    /// <remarks>
    /// ScrollTracker watches the touchpanel for drag and flick gestures, and computes the appropriate
    /// position and scale for a viewport within a larger canvas to emulate the behavior of the Silverlight
    /// scrolling controls.
    /// 
    /// This class only handles computation of the view rectangle; how that rectangle is used for rendering
    /// is up tot the client code.
    /// </remarks>
    public class ScrollTracker
    {
        
        /// Handling TouchPanel.EnabledGestures
        /// --------------------------
        /// This class watches for HorizontalDrag, DragComplete, and Flick gestures. However, it cannot just
        /// set TouchPanel.EnabledGestures, because that would most likely interfere with gestures needed
        /// elsewhere in the application. So it just exposes a const 'HandledGestures' field and relies on
        /// the client code to set TouchPanel.EnabledGestures appropriately.
        public const GestureType GesturesNeeded = GestureType.Flick | GestureType.VerticalDrag | GestureType.DragComplete;

        #region Tuning constants

        // How far the user is allowed to drag past the "real" border
        const float SpringMaxDrag = 400;

        // How far the display moves when dragged to 'SpringMaxDrag'
        const float SpringMaxOffset = SpringMaxDrag / 3;

        const float SpringReturnRate = 0.1f;
        const float SpringReturnMin = 2.0f;
        const float Deceleration = 500.0f;  // pixels/second^2
        const float MaxVelocity = 2000.0f;  // pixels/second
        #endregion

        /// <summary>
        /// A rectangle (set by the client code) giving the area of the canvas we want to scroll around in.
        /// Normally taller or wider than the viewport.
        /// </summary>
        public Rectangle CanvasRect;

        /// <summary>
        /// A rectangle describing the currently visible view area. Normally the caller will set this once
        /// to set the viewport size and initial position, and from then on let ScrollTracker move it around;
        /// however, you can set it at any time to change the position or size of the viewport.
        /// </summary>
        public Rectangle ViewRect;

        /// <summary>
        /// FullCanvasRect is the same as CanvasRect, except it's extended to be at least as large as ViewRect.
        /// The is the true canvas area that we scroll around in.
        /// </summary>
        public Rectangle FullCanvasRect
        {
            get
            {
                Rectangle value = CanvasRect;
                if (value.Width < ViewRect.Width)
                    value.Width = ViewRect.Width;
                if (value.Height < ViewRect.Height)
                    value.Height = ViewRect.Height;
                return value;
            }
        }

        // Current flick velocity.
        Vector2 Velocity;

        // What the view offset would be if we didn't do any clamping
        Vector2 ViewOrigin;

        // What the ViewOrigin would be if we didn't do any clamping
        Vector2 UnclampedViewOrigin;

        // True if we're currently tracking a drag gesture
        public bool IsTracking { get; private set; }

        public bool IsMoving
        {
            get { return IsTracking || Velocity.x != 0 || Velocity.y != 0 || !FullCanvasRect.Contains(ViewRect); }
        }

        public ScrollTracker()
        {
            ViewRect = new Rectangle { Width = TouchPanel.DisplayWidth, Height = TouchPanel.DisplayHeight };
            CanvasRect = ViewRect;
        }

        // This must be called manually each tick that the ScrollTracker is active.
        public void Update()
        {
#if WINDOWS

            return;
#endif
            // Apply velocity and clamping
            float dt = Time.deltaTime;

            Vector2 viewMin = new Vector2 { x = 0, y = 0 };
            Vector2 viewMax = new Vector2 { x = CanvasRect.Width - ViewRect.Width, y = CanvasRect.Height - ViewRect.Height };
            viewMax.x = Math.Max(viewMin.x, viewMax.x);
            viewMax.y = Math.Max(viewMin.y, viewMax.y);

            //Debug.Display("ViewMax", viewMax);

            if (IsTracking)
            {
                // ViewOrigin is a soft-clamped version of UnclampedOffset
                ViewOrigin.x = SoftClamp(UnclampedViewOrigin.x, viewMin.x, viewMax.x);
                ViewOrigin.y = SoftClamp(UnclampedViewOrigin.y, viewMin.y, viewMax.y);
            }
            else
            {
                // Apply velocity
                ApplyVelocity(dt, ref ViewOrigin.x, ref Velocity.x, viewMin.x, viewMax.x);
                ApplyVelocity(dt, ref ViewOrigin.y, ref Velocity.y, viewMin.y, viewMax.y);
            }

            ViewRect.X = (int)ViewOrigin.x;
            ViewRect.Y = (int)ViewOrigin.y;

            //Debug.Log("ViewRect: "+ViewRect+" CanvasRect: "+CanvasRect+" ViewMin: "+viewMin+" ViewMax: "+viewMax);
        }

        public void MoveScrollTracker(float value)
        {
            ViewRect.Y += (int)value;
        }

        // This must be called manually each tick that the ScrollTracker is active.
        public void HandleInput(InputState input)
        {

            // Turn on tracking as soon as we seen any kind of touch. We can't use gestures for this
            // because no gesture data is returned on the initial touch. We have to be careful to
            // pick out only 'Pressed' locations, because TouchState can return other events a frame
            // *after* we've seen GestureType.Flick or GestureType.DragComplete.
            if (!IsTracking)
            {
                
                for (int i = 0; i < input.TouchState.Count; i++)
                {                    
                    if (input.TouchState[i].State == TouchLocationState.Pressed)
                    {
                        Velocity = Vector2.zero;
                        UnclampedViewOrigin = ViewOrigin;
                        IsTracking = true;
                        break;
                    }
                }
            }

            foreach (GestureSample sample in input.Gestures)
            {
                
                switch (sample.GestureType)
                {
                    
                    case GestureType.VerticalDrag:
                        UnclampedViewOrigin.y -= sample.Delta.Y;

                        break;

                    case GestureType.Flick:
                        // Only respond to mostly-vertical flicks
                        if (Math.Abs(sample.Delta.X) < Math.Abs(sample.Delta.Y))
                        {
                            IsTracking = false;
                            Velocity = -sample.Delta;
                        }
                        break;

                    case GestureType.DragComplete:
                        IsTracking = false;
                        break;
                }
            }
        }

        // If x is within the range (min,max), just return x. Otherwise return a value outside of (min,max)
        // but only partway to where x is. This is used to get the partial-overdrag effect at the edges
        // of the list.
        static float SoftClamp(float x, float min, float max)
        {
            if (x < min)
            {
                return Math.Max(x - min, -SpringMaxDrag) * SpringMaxOffset / SpringMaxDrag + min;
            }
            if (x > max)
            {
                return Math.Min(x - max, SpringMaxDrag) * SpringMaxOffset / SpringMaxDrag + max;
            }
            return x;
        }

        // Integrate the given position and velocity over a timespan. Min and max give the
        // soft limits that the position is allowed to move to.
        void ApplyVelocity(float dt, ref float x, ref float v, float min, float max)
        {
            float x0 = x;
            x += v * dt;

            // Apply deceleration to gradually reduce velocity
            v = MathHelper.Clamp(v, -MaxVelocity, MaxVelocity);
            v = Math.Max(Math.Abs(v) - dt * Deceleration, 0.0f) * Math.Sign(v);

            // If we've scrolled past the edge, gradually reset to edge
            if (x < min)
            {
                x = Math.Min(x + (min - x) * SpringReturnRate + SpringReturnMin, min);
                v = 0;
            }
            if (x > max)
            {
                x = Math.Max(x - (x - max) * SpringReturnRate - SpringReturnMin, max);
                v = 0;
            }
        }
    }
}