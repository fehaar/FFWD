using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD;
using PressPlay.FFWD.ScreenManager;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

#if WINDOWS_PHONE
using Microsoft.Xna.Framework.Input.Touch;
#endif
 
#if WINDOWS
using Microsoft.Xna.Framework.Input;
#endif

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

    public enum ButtonControlStates
    {
        hover,
        normal,
        pressed,
        disabled,
        selected
    }

    public class ButtonControl : Control
    {
        public TextControl textControl;
        public AudioClip buttonSound;
        protected ImageControl background;
        private ButtonStyle buttonStyle;

        private ButtonControlStates previousState = ButtonControlStates.normal;
        private ButtonControlStates _state = ButtonControlStates.normal;
        public ButtonControlStates state
        {
            get
            {
                return _state;
            }
            set
            {
                ChangeState(value);
            }
        }

        public bool selectable = false;

        public string link;
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
        private Vector3 lastPressPosition;

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<ButtonControlEventArgs> OnClickEvent;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected virtual void OnClickMethod()
        {
            if (OnClickEvent != null)
            {
                OnClickEvent(this, new ButtonControlEventArgs(link));
            }
        }

        public ButtonControl(ButtonStyle buttonStyle, string link)
        {
            gameObject.name = "ButtonControl";

            this.buttonStyle = buttonStyle;
            this.link = link;

            background = new ImageControl(buttonStyle.texture, buttonStyle[(int)ButtonControlStates.normal]);
            AddChild(background);

            textControl = new TextControl();
            textControl.ignoreSize = true;
            AddChild(textControl);
        }

        private void ChangeState(ButtonControlStates newState)
        {
            ((UISpriteRenderer)background.renderer).texture = buttonStyle.texture;
            ((UISpriteRenderer)background.renderer).sourceRect = buttonStyle.GetSourceRect((int)newState);

            previousState = _state;
            _state = newState;

            if (newState == ButtonControlStates.disabled)
            {
                textControl.SetColor(FFWD.Color.gray);
            }
            if (previousState == ButtonControlStates.disabled)
            {
                textControl.SetColor(FFWD.Color.white);
            }
        }

        public void ScaleTextToFit()
        {
            ScaleTextToFit(0);
        }

        public void ScaleText(float scale)
        {
            textControl.transform.localScale = new Vector3(scale);
            InvalidateAutoSize();
        }

        public Vector3 GetTextScale()
        {
            return textControl.transform.localScale;
        }

        public void ScaleTextToFit(float margin)
        {
            // TODO This needs to able to scale the text to fit
            // Something is broken with the size / bounds logic after scaling

            if (textControl.bounds.Width < background.bounds.Width - margin)
            {
                return;
            }
            textControl.transform.localScale = new Vector3((background.bounds.Width - margin) / (textControl.size.x * 1.15f));
            InvalidateAutoSize();
        }

        public override void HandleInput(InputState input)
        {
            if (state == ButtonControlStates.disabled)
            {
                return;
            }
            
            base.HandleInput(input);

            if (isMouseWithinBounds(input))
            {
                if (input.isMouseDown)
                {
                    if (state != ButtonControlStates.pressed)
                    {
                        ChangeState(ButtonControlStates.pressed);
                        lastPressPosition = transform.position;
                    }
                }
                else if(input.isMouseUp)
                {
                    if (state == ButtonControlStates.pressed)
                    {
                        ChangeState((selectable) ? ButtonControlStates.selected : ButtonControlStates.normal);
                        OnClickMethod();
                    }
                }
                else if (state != ButtonControlStates.pressed && state != ButtonControlStates.hover)
                {
                    ChangeState(ButtonControlStates.hover);
                }
            }
            else
            {
                if (state == ButtonControlStates.hover || state == ButtonControlStates.pressed)
                {
                    ChangeState((previousState == ButtonControlStates.selected) ? ButtonControlStates.selected : ButtonControlStates.normal);
                }
            }

            if (state == ButtonControlStates.pressed && lastPressPosition != transform.position)
            {
                ChangeState(ButtonControlStates.normal);
            }
        }

        protected override bool isMouseWithinBounds(InputState input)
        {
            if (useCustomClickRect)
            {
                return isMouseWithinBounds(input, clickRect);
            }
            else
            {
                return base.isMouseWithinBounds(input);
            }
        }

        public void Deselect()
        {
            if (state == ButtonControlStates.selected)
            {
                ChangeState(ButtonControlStates.normal);
            }
        }
    }
}
