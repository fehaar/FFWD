using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Components
{
    public class LineRenderer : Renderer
    {
        public override int Draw(Microsoft.Xna.Framework.Graphics.GraphicsDevice device, Camera cam)
        {
            return 0;
        }

        public void SetColors(Color start, Color end)
        {
            throw new NotImplementedException();
        }

        public void SetWidth(float start, float end)
        {
            throw new NotImplementedException();
        }

        public void SetVertexCount(int count)
        {
            throw new NotImplementedException();
        }

        public void SetPosition(int index, Vector3 position)
        {
            throw new NotImplementedException();
        }
    }
}
