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
        public BasicEffectShader()
        {
            effect = new BasicEffect(Camera.Device);
            supportsLights = true;
            supportsTextures = true;
            supportsVertexColor = true;
        }

        public override void Configure(Material mat)
        {
            BasicEffect e = (effect as BasicEffect);
            if (mat.mainTexture != null)
            {
                e.TextureEnabled = true;
                e.Texture = mat.mainTexture;
            }
            else
            {
                e.TextureEnabled = false;
            }
            e.DiffuseColor = mat.color;
            e.Alpha = mat.color.a;
        }

        internal override void ApplyPreRenderSettings(bool useVertexColor)
        {
            BasicEffect e = (effect as BasicEffect);
            e.VertexColorEnabled = useVertexColor;
            e.LightingEnabled = Light.HasLights;
        }
    }
}
