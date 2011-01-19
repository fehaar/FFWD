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
        private SpriteFont font;
        private Vector2 renderPosition = Vector2.zero;
        private Vector2 textSize = Vector2.zero;

        public Vector2 textOffset = Vector2.zero;
        public Color color = Color.white;

        public SpriteEffects effects = new SpriteEffects();

        private string _text = "";
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value != _text)
                {
                    _text = value.Replace("”", "");
                    textSize = font.MeasureString(_text);
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

        public override void Draw(GraphicsDevice device, Camera cam)
        {
            //UIRenderer.batch.DrawString(font, text, transform.position, material.color);
            float depth = 1 - ((float)transform.position / 10000f);
            UIRenderer.batch.DrawString(font, text, transform.position, material.color, 0, Microsoft.Xna.Framework.Vector2.Zero, transform.localScale, effects, depth);
        }

        protected static char[] splitTokens = { ' ', '-' };
        protected static string spaceString = " ";
        /// <summary>
        /// A simple word-wrap algorithm that formats based on word-breaks.
        /// it's not completely accurate with respect to kerning & spaces and
        /// doesn't handle localized text, but is easy to read for sample use.
        /// </summary>
        protected static string WordWrap(string input, int width, SpriteFont font)
        {
            StringBuilder output = new StringBuilder();
            output.Length = 0;

            string[] wordArray = input.Split(splitTokens, StringSplitOptions.None);

            int space = (int)font.MeasureString(spaceString).X;

            int lineLength = 0;
            int wordLength = 0;
            int wordCount = 0;

            for (int i = 0; i < wordArray.Length; i++)
            {
                wordLength = (int)font.MeasureString(wordArray[i]).X;

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
