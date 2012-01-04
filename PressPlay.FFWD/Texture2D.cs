using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PressPlay.FFWD
{
    public class Texture2D : Texture
    {
        public Texture2D()
        {
        }

        public Texture2D(int width, int height)
            : base(width, height)
        {
        }

        internal Texture2D(Microsoft.Xna.Framework.Graphics.Texture2D t)
            : base(t)
        {
        }

        internal static Texture2D LoadFromResource(string name)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var t = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(Application.Instance.GraphicsDevice, asm.GetManifestResourceStream(String.Format("PressPlay.FFWD.{0}", name))); 
            return new Texture2D(t);
        }
    }
}
