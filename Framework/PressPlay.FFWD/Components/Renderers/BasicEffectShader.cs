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
        private static BasicEffect Effect;

        public BasicEffectShader()
        {
            if (Effect == null)
            {
                Effect = new BasicEffect(Camera.Device);
            }
            effect = Effect;
            supportsLights = true;
            supportsTextures = true;
            supportsVertexColor = true;
        }

        internal override void ApplyPreRenderSettings(Material mat, bool useVertexColor)
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
            Effect.VertexColorEnabled = useVertexColor;
            Effect.LightingEnabled = Light.HasLights;
        }
    }
}
