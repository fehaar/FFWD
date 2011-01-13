using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.ScreenManager;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.UI.Controls
{
    public class LevelSelectButton : Control
    {

        private int levelId = -1;

        private Rectangle openSourceRect;
        private Rectangle lockedSourceRect;
        private UISpriteRenderer background;
        private TextControl levelText;
        private TextControl scoreText;

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
        public event EventHandler<LevelSelectEventArgs> OnClickEvent;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnClickMethod()
        {
            if (OnClickEvent != null)
            {
                OnClickEvent(this, new LevelSelectEventArgs(levelId));
                background.sourceRect = lockedSourceRect;
            }
        }

        public LevelSelectButton(int levelId, Texture2D texture, Rectangle openSourceRect, Rectangle lockedSourceRect, EventHandler<LevelSelectEventArgs> clickHandler)
            : base()
        {
            gameObject.name = "LevelSelectButton";


            this.levelId = levelId;
            this.openSourceRect = openSourceRect;
            this.lockedSourceRect = lockedSourceRect;

            OnClickEvent += clickHandler;

            ImageControl backgroundImage = new ImageControl(texture);
            background = (UISpriteRenderer)backgroundImage.renderer;
            background.sourceRect = openSourceRect;

            AddChild(backgroundImage);

            levelText = new TextControl("99", ContentHelper.Content.Load<SpriteFont>("Textures/Fonts/BerlinSansXNA_60"));
            AddChild(levelText);

            //levelText.transform.position = new Vector2(bounds.Width / 2 - 15, 40);

            scoreText = new TextControl("9999", ContentHelper.Content.Load<SpriteFont>("Textures/Fonts/BerlinSansXNA_30"));
            AddChild(scoreText);

            //scoreText.transform.position = new Vector2(bounds.Width / 2 - 15, 160);
        }

        public void SetPositions(Vector2 levelTextPos, Vector2 scoreTextPos, Vector2 starPos)
        {
            levelText.transform.position = levelTextPos;
            scoreText.transform.position = scoreTextPos;
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            if (isInputWithinBounds(input))
            {
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
