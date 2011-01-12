using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.ScreenManager;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace PressPlay.FFWD.UI.Controls
{
    public class ButtonControl : Control
    {

        public ButtonControl(Texture2D texture)
            : base()
        {
            gameObject.name = "ButtonControl";

            AddChild(new ImageControl(texture));
        }

        private bool useCustomClickRect = false;
        private Rectangle _clickRect;
        public Rectangle clickRect
        {
            get
            {
                return _clickRect;
            }
            set
            {
                useCustomClickRect = (clickRect != null) ? true : false;
                _clickRect = value;
            }
        }

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler OnClickEvent;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnClickMethod()
        {
            if (OnClickEvent != null)
            {
                OnClickEvent(this, null);
            }
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            if(isInputWithinBounds(input)){
                foreach (GestureSample sample in input.Gestures)
                {
                    switch (sample.GestureType)
                    {
                        case GestureType.Tap:
                            OnClickMethod();

                            break;
                    }
                }
            }
        }

        protected override bool isInputWithinBounds(PressPlay.FFWD.ScreenManager.InputState input)
        {

            //return base.isInputWithinBounds(input);

            if (useCustomClickRect)
            {
                return isInputWithinBounds(input, clickRect);
            }
            else
            {
                return base.isInputWithinBounds(input);
            }
        }
    }
}
