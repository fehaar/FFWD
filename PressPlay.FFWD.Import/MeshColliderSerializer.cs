using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class MeshColliderSerializer : ContentTypeSerializer<MeshCollider>
    {
        protected override MeshCollider Deserialize(IntermediateReader input, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format, MeshCollider existingInstance)
        {
            MeshCollider collider = new MeshCollider();

            ContentSerializerAttribute attr = new ContentSerializerAttribute();
            attr.ElementName = "id";

            // SET ID
            int id = input.ReadObject<int>(attr);
            Type type = typeof(UnityObject);
            FieldInfo fld = type.GetField("_id", BindingFlags.Instance | BindingFlags.NonPublic);
            fld.SetValue(collider, id);

            attr.ElementName = "isTrigger";
            collider.isTrigger = input.ReadObject<bool>(attr);

            attr.ElementName = "triangles";
            short[] triangles = input.ReadObject<short[]>(attr);

            attr.ElementName = "vertices";
            Microsoft.Xna.Framework.Vector3[] vertices = input.ReadObject<Microsoft.Xna.Framework.Vector3[]>(attr);

            List<Triangle> tris = new List<Triangle>();
            for (int i = 0; i < triangles.Length; i += 3)
            {
                if (vertices[triangles[i]].Y + vertices[triangles[i + 1]].Y + vertices[triangles[i + 2]].Y > 1)
                {
                    Debug.Log(" Warning: " + ToString() + " has non zero Y in collider");
                }
                Triangle tri = new Triangle(
                    vertices[triangles[i]].X, vertices[triangles[i]].Z,
                    vertices[triangles[i + 2]].X, vertices[triangles[i + 2]].Z,
                    vertices[triangles[i + 1]].X, vertices[triangles[i + 1]].Z
                );
                tris.Add(tri);
            }

            collider.vertices = EarclipDecomposer.PolygonizeTriangles(tris, int.MaxValue, 0);

            return collider;
        }

        protected override void Serialize(IntermediateWriter output, MeshCollider value, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
