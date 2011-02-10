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

        public RenderMethod renderMethod = RenderMethod.billboard;

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

        private BasicEffect basicEffect;
        private Vector3 cameraFront = new Vector3(0, 0, -1);

        public TextRenderer3D(SpriteFont font)
            : this("", font)
        {
        }

        public TextRenderer3D(string text, SpriteFont font)
        {
            this.font = font;
            this.text = text;
        }

        protected SpriteBatch batch;
        public override int Draw(GraphicsDevice device, Camera cam)
        {
            if (batch == null)
            {
                batch = new SpriteBatch(device);
            }

            if (basicEffect == null)
            {
                basicEffect = new BasicEffect(device)
                {
                    TextureEnabled = true,
                    VertexColorEnabled = true,
                };
            }

            //Debug.Log("DRAWING TextRenderer3D: "+text+" On position: "+transform.position);

            //Vector3 textPosition = new Vector3(0, 45, 0);

            if (renderMethod == RenderMethod.billboard)
            {
                basicEffect.World = Matrix.CreateConstrainedBillboard(transform.position, transform.position - cam.transform.forward.normalized, cam.transform.up * -1, null, null);
            }
            else
            {
                throw new NotImplementedException("This function is not yet implemented");
                basicEffect.World = Matrix.CreateScale(1, -1, 1) * Matrix.CreateTranslation(transform.position);
            }
            basicEffect.View = cam.view;
            basicEffect.Projection = cam.projectionMatrix;

            Vector2 textOrigin = font.MeasureString(text) / 2;
            const float textSize = 0.005f;

            batch.Begin(0, null, null, DepthStencilState.DepthRead, RasterizerState.CullNone, basicEffect);
            batch.DrawString(font, text, Vector2.zero, Color.white, 0, textOrigin, textSize, 0, 0);
            batch.End();

            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.LinearClamp;

            return 0;
        }        
    }
}
