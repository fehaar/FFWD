using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using PressPlay.FFWD.Components;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class StaticBatchRendererSerializer : ContentTypeSerializer<StaticBatchRenderer>
    {
        protected override StaticBatchRenderer Deserialize(IntermediateReader input, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format, StaticBatchRenderer existingInstance)
        {
            StaticBatchRenderer renderer = new StaticBatchRenderer();

            /*ContentSerializerAttribute attr = new ContentSerializerAttribute();
            attr.ElementName = "id";

            // SET ID
            int id = input.ReadObject<int>(attr);
            Type type = typeof(UnityObject);
            FieldInfo fld = type.GetField("_id", BindingFlags.Instance | BindingFlags.NonPublic);
            fld.SetValue(renderer, id);

            attr.ElementName = "materials";
            attr.CollectionItemName = "material";
            renderer.materials = input.ReadObject<Material[]>(attr);

            attr.ElementName = "triangles";
            renderer.indices = input.ReadObject<short[][]>(attr);

            attr.ElementName = "vertices";
            Microsoft.Xna.Framework.Vector3[] vs = input.ReadObject<Microsoft.Xna.Framework.Vector3[]>(attr);

            attr.ElementName = "uv";
            Microsoft.Xna.Framework.Vector2[] uv = input.ReadObject<Microsoft.Xna.Framework.Vector2[]>(attr);
            renderer.boundingSphere = BoundingSphere.CreateFromPoints(vs);

            //VertexPositionNormalTexture[] buffer = new VertexPositionNormalTexture[vs.Length];
            //for (int i = 0; i < vs.Length; i++)
            //{
            //    Microsoft.Xna.Framework.Vector3 v = vs[i];
            //    v.Y = -v.Y;
            //    Microsoft.Xna.Framework.Vector2 u = uv[i];
            //    u.Y = 1 - u.Y;
            //    buffer[i] = new VertexPositionTexture(v, u);
            //}
            //renderer.vertices = buffer;*/

            return renderer;
        }

        protected override void Serialize(IntermediateWriter output, StaticBatchRenderer value, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
