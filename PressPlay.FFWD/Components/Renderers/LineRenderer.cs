using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Components
{
    public class LineRenderer : Renderer
    {
        public bool useWorldSpace;

        private int vertexCount = 0;
        private Vector3[] vertices = null;

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
            if (vertices == null || vertices.Length < count)
            {
                vertices = new Vector3[count];
            }
            vertexCount = count;
        }

        public void SetPosition(int index, Vector3 position)
        {
            if (index < 0 || index > vertexCount)
            {
                throw new IndexOutOfRangeException("Trying to set a vertex that does not exist!");
            }
            vertices[index] = position;
        }
    }
}
