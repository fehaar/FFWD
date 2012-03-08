using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    /// <summary>
    /// A shader encapsulates Effects in XNA and gives us a way of rendering meshes.
    /// </summary>
    public abstract class Shader
    {
        public string name;
        public int renderQueue;

        /// <summary>
        /// Finds the appropriate shader for the given material. Typically by name. 
        /// 
        /// It can also load shaders from file if it is supported.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        internal static Shader GetShader(Material mat)
        {
            Shader s = null;
            switch (mat.name)
            {
                default:
                    s = new BasicEffectShader();
                    break;
            }
            s.Configure(mat);
            return s;
        }

        internal Effect effect { get; set; }

        public abstract void Configure(Material mat);
        internal abstract void ApplyPreRenderSettings(bool useVertexColor);

        public bool supportsTextures { get; protected set; }
        public bool supportsLights { get; protected set; }
        public bool supportsVertexColor { get; protected set; }
    }
}
