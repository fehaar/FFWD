using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.ScreenManager;
using Microsoft.Xna.Framework.Input.Touch;

namespace PressPlay.FFWD.UI.Controls
{
    public class ScrollingPanelControl : PanelControl
    {
        private ScrollTracker scrollTracker = new ScrollTracker();
        private Vector3 startPosition;
        private bool hasScrolled = false;

        public ScrollingPanelControl(int width, int height)
            : base()
        {
            scrollTracker.ViewRect.Width = width;
            scrollTracker.ViewRect.Width = height;
            //scrollTracker.CanvasRect = scrollTracker.ViewRect;

            startPosition = transform.position;
            gameObject.name = "ScrollingPanelControl";
        }

        public override void Update()
        {

            /*
            scrollTracker.CanvasRect.X = bounds.X;
            scrollTracker.CanvasRect.Y = bounds.Y;
            scrollTracker.CanvasRect.Width = bounds.Width;
            scrollTracker.CanvasRect.Height = bounds.Height;
            */

            scrollTracker.CanvasRect = bounds;
            scrollTracker.Update();
            
            base.Update();
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            bool doScrollInput = false;
            for (int i = 0; i < input.TouchState.Count; i++)
            {
                //Debug.Log(i + ". TouchPosition: " + input.TouchState[i].Position + " bounds: " + scrollTracker.CanvasRect);
                
                if (scrollTracker.CanvasRect.Contains((int)input.TouchState[i].Position.X, (int)input.TouchState[i].Position.Y))
                {
                    doScrollInput = true;
                }
            }            

            if(doScrollInput){
                //Debug.Log("Updating scrollbar: " + scrollTracker.ViewRect.Y);
                scrollTracker.HandleInput(input);
            }

            if (hasScrolled && children != null)
            {
                foreach (Control c in children)
                {
                    //c.drawOffset.y = -scrollTracker.ViewRect.Y;
                    PositionChildControls(new Vector2(0, -scrollTracker.ViewRect.Y));
                    //((UISpriteRenderer)c.renderer).origin.y = scrollTracker.ViewRect.Y;
                }
            }

            hasScrolled = scrollTracker.IsMoving;
        }

        private void PositionChildControls(Vector2 topPosition)
        {
            if (children != null || children.Count > 0)
            {
                float y = yMargin + topPosition.y;

                //Debug.Log("TopPosition: "+topPosition);

                for (int i = 0; i < childCount; i++)
                {
                    Control child = this[i];
                    child.transform.localPosition = new Vector2 { x = xMargin, y = y };
                    y += child.bounds.Height + ySpacing;
                }
            }

            InvalidateAutoSize();
        }
    }
}
