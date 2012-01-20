using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Extensions;
using System.IO;

namespace PressPlay.FFWD
{
    public class Mesh : Asset
    {
        [ContentSerializer(ElementName="vertices", Optional=true)]
        internal Microsoft.Xna.Framework.Vector3[] _vertices;
        [ContentSerializerIgnore]
        public Vector3[] vertices
        {
            get
            {
                if (_vertices == null)
                {
                    return null;
                }
                // NOTE: We could cache this if we use it too much
                Vector3[] v = new Vector3[_vertices.Length];
                for (int i = 0; i < _vertices.Length; i++)
                {
                    v[i] = _vertices[i];
                }
                return v;
            }
            set
            {
                if (value == null)
                {
                    _vertices = null;
                    return;
                }
                _vertices = new Microsoft.Xna.Framework.Vector3[value.Length];
                for (int i = 0; i < _vertices.Length; i++)
                {
                    _vertices[i] = value[i];
                }
            }
        }

        [ContentSerializer(ElementName = "normals", Optional = true)]
        internal Microsoft.Xna.Framework.Vector3[] _normals;
        [ContentSerializerIgnore]
        public Vector3[] normals
        {
            get
            {
                if (_normals == null)
                {
                    return null;
                }
                // NOTE: We could cache this if we use it too much
                Vector3[] n = new Vector3[_normals.Length];
                for (int i = 0; i < _normals.Length; i++)
                {
                    n[i] = _normals[i];
                }
                return n;
            }
            set
            {
                if (value == null)
                {
                    _normals = null;
                    return;
                }
                _normals = new Microsoft.Xna.Framework.Vector3[value.Length];
                for (int i = 0; i < _normals.Length; i++)
                {
                    _normals[i] = value[i];
                }
            }
        }

        [ContentSerializer(Optional = true)]
        public Vector4[] tangents;

        [ContentSerializer(ElementName = "uv", Optional = true)]
        internal Microsoft.Xna.Framework.Vector2[] _uv;
        [ContentSerializerIgnore]
        public Vector2[] uv
        {
            get
            {
                if (_uv == null)
                {
                    return null;
                }
                // NOTE: We could cache this if we use it too much
                Vector2[] u = new Vector2[_uv.Length];
                for (int i = 0; i < _uv.Length; i++)
                {
                    u[i] = _uv[i];
                }
                return u;
            }
            set
            {
                if (value == null)
                {
                    _uv = null;
                    return;
                }
                _uv = new Microsoft.Xna.Framework.Vector2[value.Length];
                for (int i = 0; i < _uv.Length; i++)
                {
                    _uv[i] = value[i];
                }
            }
        }

        [ContentSerializer(ElementName = "uv2", Optional = true)]
        internal Microsoft.Xna.Framework.Vector2[] _uv2;
        [ContentSerializerIgnore]
        public Vector2[] uv2
        {
            get
            {
                if (_uv2 == null)
                {
                    return null;
                }
                // NOTE: We could cache this if we use it too much
                Vector2[] u = new Vector2[_uv2.Length];
                for (int i = 0; i < _uv2.Length; i++)
                {
                    u[i] = _uv2[i];
                }
                return u;
            }
            set
            {
                if (value == null)
                {
                    _uv2 = null;
                    return;
                }
                _uv2 = new Microsoft.Xna.Framework.Vector2[value.Length];
                for (int i = 0; i < _uv2.Length; i++)
                {
                    _uv2[i] = value[i];
                }
            }
        }

        [ContentSerializerIgnore]
        public short[] triangles;
        [ContentSerializer(Optional = true)]
        private short[][] triangleSets;
        [ContentSerializer(Optional = true)]
        public Color[] colors;
        [ContentSerializer(Optional = true)]
        internal BoneWeight[] boneWeights;
        [ContentSerializer(Optional = true)]
        internal Matrix[] bindPoses;

        [ContentSerializer(ElementName="bounds", Optional=true)]
        public Bounds bounds;

        protected override void DoLoadAsset(AssetHelper assetHelper)
        {
            if (!String.IsNullOrEmpty(name))
            {
                Mesh mesh = assetHelper.LoadAsset<Mesh>(name);
                if (mesh != null)
                {
                    mesh.FlattenTriangleSets();
                    CloneTo(mesh, this);
                }
            }
        }

        private void FlattenTriangleSets()
        {
            if (triangleSets == null)
            {
                return;
            }
            int triCount = 0;
            int triIndex = 0;
            for (int i = 0; i < triangleSets.Length; i++)
            {
                triCount += triangleSets[i].Length;
            }
            triangles = new short[triCount];
            for (int i = 0; i < triangleSets.Length; i++)
            {
                triangleSets[i].CopyTo(triangles, triIndex);
                triIndex += triangleSets[i].Length;
            }
        } 

        public void Clear()
        {
            _vertices = null;
            _normals = null;
            _uv = null;
            triangles = null;
            triangleSets = null;
        }

        public int subMeshCount
        {
            get
            {
                if (triangleSets == null)
                {
                    if (triangles == null)
                    {
                        return 0;
                    }
                    return 1;
                }
                return triangleSets.Length;
            }
        }

        public short[] GetTriangles(int subMeshIndex)
        {
            if (triangleSets == null && subMeshIndex == 0)
            {
                return triangles;
            }
            return triangleSets[subMeshIndex];
        }

        #region ICloneable Members
        internal override UnityObject Clone()
        {
            Mesh clone = new Mesh();
            CloneTo(this, clone);
            return clone;
        }

        private static void CloneTo(Mesh from, Mesh to)
        {
            if (from._vertices.HasElements())
            {
                to._vertices = (Microsoft.Xna.Framework.Vector3[])from._vertices.Clone();
            }
            if (from._normals.HasElements())
            {
                to._normals = (Microsoft.Xna.Framework.Vector3[])from._normals.Clone();
            }
            if (from._uv.HasElements())
            {
                to._uv = (Microsoft.Xna.Framework.Vector2[])from._uv.Clone();
            }
            if (from._uv2.HasElements())
            {
                to._uv2 = (Microsoft.Xna.Framework.Vector2[])from._uv2.Clone();
            }
            if (from.tangents.HasElements())
            {
                to.tangents = (Vector4[])from.tangents.Clone();
            }
            if (from.triangles.HasElements())
            {
                to.triangles = (short[])from.triangles.Clone();
            }
            if (from.triangleSets.HasElements())
            {
                to.triangleSets = (short[][])from.triangleSets.Clone();
            }
            to.bounds = from.bounds;
            // Note that these are not actually cloned as they will not be changed
            to.boneWeights = from.boneWeights;
            to.bindPoses = from.bindPoses;
        }
        #endregion

        public override string ToString()
        {
            return String.Format("{0} - {1} ({3})", GetType().Name, name, GetInstanceID());
        }

        public void RecalculateBounds()
        {
            bounds = new Bounds();
            bounds.Encapsulate(_vertices);
        }

        public int vertexCount
        {
            get { return _vertices.Length; }
        }
    }
}
