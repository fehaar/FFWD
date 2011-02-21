using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public class TextRenderer3D : Renderer
    {
        public enum RenderMethod
        {
            normal,
            billboard
        }

        public RenderMethod renderMethod = RenderMethod.normal;

        private string _text;
        public string text
        {
            get { return _text; }
            set { _text = value; }
        }

        private SpriteFont _font;
        public SpriteFont font
        {
            get { return _font; }
            set { _font = value; }
        }

        public static SpriteBatch batch;
        public static BasicEffect basicEffect;
        public static Matrix invertY = Matrix.CreateScale(1, -1, 1);

        private Vector3 cameraFront = new Vector3(0, 0, -1);

        public TextRenderer3D(SpriteFont font)
            : this("", font)
        {
        }

        public TextRenderer3D(string text, SpriteFont font)
        {
            this.font = font;
            this.text = text;
            material = new Material();
            material.color = Color.white;
        }
        
        public override int Draw(GraphicsDevice device, Camera cam)
        {

            TextRenderer3D.basicEffect.Projection = cam.projectionMatrix;
            Vector3 viewSpaceTextPosition = Microsoft.Xna.Framework.Vector3.Transform(transform.position, cam.view * invertY);

            Vector2 textOrigin = font.MeasureString(text) / 2;
            const float textSize = 0.03f;

            batch.DrawString(font, text, new Vector2(viewSpaceTextPosition.x, viewSpaceTextPosition.y), material.color, 0, textOrigin, textSize, 0, viewSpaceTextPosition.z);
            return 0;
        }        
    }
}
