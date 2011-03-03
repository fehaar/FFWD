using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.UI.Controls;
using PressPlay.FFWD;
using System.Text;
using System;

namespace PressPlay.FFWD.UI
{
    public class UITextRenderer : UIRenderer
    {
        public SpriteFont font;
        private Vector2 renderPosition = Vector2.zero;
        private Vector2 textSize = Vector2.zero;
        public TextControl.TextOrigin textOrigin = TextControl.TextOrigin.normal;

        public Vector2 textOffset = Vector2.zero;
        public Color color = Color.white;

        public SpriteEffects effects = new SpriteEffects();

        private bool hasDoneWordWrap = true;
        private string _text = "";
        public string text
        {
            get
            {
                if (!hasDoneWordWrap && control != null)
                {
                    _text = WordWrap(_text, control.bounds.Width, font);
                    hasDoneWordWrap = true;                    
                }
                return _text;
            }
            set
            {
                if (value != _text)
                {
                    value = value.Replace("”", "");

                    if (_text != value)
                    {
                        if (font != null)
                        {
                            textSize = font.MeasureString(_text);
                            if (control != null)
                            {
                                _text = WordWrap(value, control.bounds.Width, font);
                            }
                            else
                            {
                                _text = value;
                                hasDoneWordWrap = false;
                            }
                        }
                        else
                        {
                            _text = value;
                        }
                    }
                }
            }
        }

        public UITextRenderer(SpriteFont font) : this("", font)
        {
        }

        public UITextRenderer(string text, SpriteFont font)
        {
            this.font = font;
            this.text = text;
        }

        public override int Draw(GraphicsDevice device, Camera cam)
        {
            if (font == null)
            {
                return 0;
            }
            
            float depth = 1 - ((float)transform.position / 10000f);

            Camera.spriteBatch.DrawString(font, text, transform.position, material.color, transform.rotation.eulerAngles.y, GetOrigin(), transform.lossyScale, effects, depth);
            return 0;
        }

        protected Microsoft.Xna.Framework.Vector2 GetOrigin(){
            
            switch(textOrigin){
                case TextControl.TextOrigin.center:
                    return new Microsoft.Xna.Framework.Vector2(control.bounds.Width / 2, 0);
            }
            
            return Microsoft.Xna.Framework.Vector2.Zero;
        }

        protected static char[] splitTokens = { ' ', '-' };
        protected static string spaceString = " ";
        /// <summary>
        /// A simple word-wrap algorithm that formats based on word-breaks.
        /// it's not completely accurate with respect to kerning & spaces and
        /// doesn't handle localized text, but is easy to read for sample use.
        /// </summary>
        protected string WordWrap(string input, int width, SpriteFont font)
        {
            if (font == null)
            {
                return input;
            }
            
            StringBuilder output = new StringBuilder();
            output.Length = 0;

            string[] wordArray = input.Split(splitTokens, StringSplitOptions.None);

            int space = (int)(font.MeasureString(spaceString).X*transform.lossyScale.x);

            int lineLength = 0;
            int wordLength = 0;
            int wordCount = 0;

            for (int i = 0; i < wordArray.Length; i++)
            {
                wordLength = (int)(font.MeasureString(wordArray[i]).X*transform.lossyScale.x);

                // don't overflow the desired width unless there are no other words on the line
                if (wordCount > 0 && wordLength + lineLength > width)
                {
                    output.Append(System.Environment.NewLine);
                    lineLength = 0;
                    wordCount = 0;
                }

                output.Append(wordArray[i]);
                output.Append(spaceString);
                lineLength += wordLength + space;
                wordCount++;
            }

            return output.ToString();
        }
    }
}
