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

        public ScrollingPanelControl(int width, int height)
            : base(width, height)
        {
            scrollTracker.ViewRect.Width = width;
            scrollTracker.ViewRect.Width = height;
            scrollTracker.CanvasRect = scrollTracker.ViewRect;


            gameObject.name = "ScrollingPanelControl";
        }

        public override void Update()
        {
            
            Vector2 size = ComputeSize();
            //scrollTracker.CanvasRect.Width = (int)size.x;
            //scrollTracker.CanvasRect.Height = (int)size.y;
            scrollTracker.Update();          
            
            base.Update();
        }

        public override void HandleInput(InputState input)
        {
            bool doScrollInput = false;
            for (int i = 0; i < input.TouchState.Count; i++)
            {
                Debug.Log(i + ". TouchPosition: " + input.TouchState[i].Position + " bounds: " + scrollTracker.CanvasRect);
                
                if (scrollTracker.CanvasRect.Contains((int)input.TouchState[i].Position.X, (int)input.TouchState[i].Position.Y))
                {
                    doScrollInput = true;
                }
            }            

            if(doScrollInput){
                scrollTracker.HandleInput(input);
                Debug.Log("I can ACCEPT input");
            }

            foreach (Control c in children)
            {
                c.drawOffset.y = -scrollTracker.ViewRect.Y;
                //((UISpriteRenderer)c.renderer).origin.y = scrollTracker.ViewRect.Y;
            }

            base.HandleInput(input);
        }
    }
}
