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

        public BasicEffectShader(string shaderName)
            : base(shaderName)
        {
            if (Effect == null)
            {
                Effect = new BasicEffect(Camera.Device);
            }
            effect = Effect;
            supportsLights = (!shaderName.Contains("Unlit"));
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
            if (supportsLights)
            {
                Effect.LightingEnabled = Light.HasLights;
                if (supportsLights && Light.HasLights)
                {
                    Light.EnableLighting(Effect, 0);
                }
            }
            else
            {
                Effect.LightingEnabled = false;
            }
        }
    }
}
