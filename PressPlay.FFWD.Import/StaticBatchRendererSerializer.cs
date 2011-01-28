using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class StaticBatchRendererSerializer : ContentTypeSerializer<StaticBatchRenderer>
    {
        protected override StaticBatchRenderer Deserialize(IntermediateReader input, ContentSerializerAttribute format, StaticBatchRenderer existingInstance)
        {
            StaticBatchRenderer renderer = new StaticBatchRenderer();

            // TOOD: We should import this
            input.ReadObject<int>(new ContentSerializerAttribute() { ElementName = "id" });


            renderer.materials = input.ReadObject<Material[]>(new ContentSerializerAttribute() { ElementName = "materials" });
            renderer.triangles = input.ReadObject<short[]>(new ContentSerializerAttribute() { ElementName = "triangles" });
            float[] vertices = input.ReadObject<float[]>(new ContentSerializerAttribute() { ElementName = "vertices" });
            float[] normals = input.ReadObject<float[]>(new ContentSerializerAttribute() { ElementName = "normals" });
            float[] uv = input.ReadObject<float[]>(new ContentSerializerAttribute() { ElementName = "uv" });

            VertexPositionNormalTexture[] buf = new VertexPositionNormalTexture[vertices.Length / 3];
            for (int i = 0; i < vertices.Length / 3; i++)
            {
                int vertexIndex = i * 3;
                int texCoordIndex = i * 2;
                buf[i] = new VertexPositionNormalTexture(
                    new Microsoft.Xna.Framework.Vector3(vertices[vertexIndex], vertices[vertexIndex + 1], vertices[vertexIndex + 2]),
                    new Microsoft.Xna.Framework.Vector3(normals[vertexIndex], normals[vertexIndex + 1], normals[vertexIndex + 2]),
                    new Microsoft.Xna.Framework.Vector2(uv[texCoordIndex], uv[texCoordIndex + 1])
                    );
            }
            renderer.buffer = buf;
            return renderer;
        }

        protected override void Serialize(IntermediateWriter output, StaticBatchRenderer value, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
