using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    internal struct VertexPositionNormalDualTexture : IVertexType
    {
        public Microsoft.Xna.Framework.Vector3 Position;
        public Microsoft.Xna.Framework.Vector3 Normal;
        public Microsoft.Xna.Framework.Vector2 TextureCoordinate;
        public Microsoft.Xna.Framework.Vector2 TextureCoordinate2;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
