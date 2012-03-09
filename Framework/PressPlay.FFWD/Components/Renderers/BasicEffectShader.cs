using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    internal class BasicEffectShader : Shader
    {
        public static int Count = 0;

        private static BasicEffect Effect;

        public BasicEffectShader()
        {
            Count++;
            if (Effect == null)
            {
                Effect = new BasicEffect(Camera.Device);
            }
            supportsLights = true;
            supportsTextures = true;
            supportsVertexColor = true;
        }

        public override void Configure(Material mat)
        {
            if (mat.mainTexture != null)
            {
                Effect.TextureEnabled = true;
                Effect.Texture = mat.mainTexture;
            }
            else
            {
                Effect.TextureEnabled = false;
            }
            Effect.DiffuseColor = mat.color;
            Effect.Alpha = mat.color.a;
        }

        internal override void ApplyPreRenderSettings(bool useVertexColor)
        {
            Effect.VertexColorEnabled = useVertexColor;
            Effect.LightingEnabled = Light.HasLights;
        }
    }
}
