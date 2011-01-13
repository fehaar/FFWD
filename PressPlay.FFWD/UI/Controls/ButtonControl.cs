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

    public class ButtonControlEventArgs : EventArgs
    {
        string _link;

        public ButtonControlEventArgs(string link)
        {
            this._link = link;
        }

        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public string link
        {
            get { return _link; }
        }


    }
    
    public class ButtonControl : Control
    {

        public ButtonControl(Texture2D texture, string link)
            : this(texture, link, "", null, Vector2.zero)
        {

        }

        public ButtonControl(Texture2D texture, string link, string text, SpriteFont font, Vector2 textPosition)
        {
            gameObject.name = "ButtonControl";

            this.link = link;

            AddChild(new ImageControl(texture));

            if (text != "" && font != null)
            {
                AddChild(new TextControl(text, font, Color.white, textPosition));
            }
        }

        private string link;
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
        public event EventHandler<ButtonControlEventArgs> OnClickEvent;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnClickMethod()
        {
            if (OnClickEvent != null)
            {
                OnClickEvent(this, new ButtonControlEventArgs(link));
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
