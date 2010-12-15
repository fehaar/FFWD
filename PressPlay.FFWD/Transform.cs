using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public enum Space { World, Self }

    public class Transform : Component
    {
        public Transform()
        {
            localRotation = Quaternion.Identity;
            localScale = Vector3.One;
        }

        private Vector3 _localPosition;
        public Vector3 localPosition
        { 
            get
            {
                return _localPosition;
            }
            set
            {
                _localPosition = value;
                hasDirtyWorld = true;
            }
        }

        private Vector3 _localScale;
        public Vector3 localScale 
        {
            get
            {
                return _localScale;
            }
            set
            {
                _localScale = value;
                hasDirtyWorld = true;
            }
        }

        private Quaternion _localRotation;
        public Quaternion localRotation 
        {
            get
            {
                return _localRotation;
            }
            set
            {
                _localRotation = value;
                hasDirtyWorld = true;
            }
        }


        [ContentSerializer(Optional = true, CollectionItemName = "child")]
        internal List<GameObject> children { get; set; }

        internal Transform _parent;
        [ContentSerializerIgnore]
        public Transform parent {
            get
            {
                return _parent;
            }
            set 
            {
                if (_parent == value)
                {
                    return;
                }
                if (_parent != null)
                {
                    _parent.children.Remove(gameObject);
                }
                Vector3 pos = position;
                _parent = value;
                if (_parent == null)
                {
                    return;
                }
                if (_parent.children == null)
                {
                    _parent.children = new List<GameObject>();
                }
                _parent.children.Add(gameObject);
                position = pos;
                hasDirtyWorld = true;
            }
        }

        private Matrix _world = Matrix.Identity;

        private bool _hasDirtyWorld = true;
        private bool hasDirtyWorld
        {
            get
            {
                return _hasDirtyWorld;
            }
            set
            {
                _hasDirtyWorld = value;
                if (children != null)
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        children[i].transform.hasDirtyWorld = true;
                    }
                }
            }
        }

        [ContentSerializerIgnore]
        internal Matrix world
        {
            get
            {
                if (hasDirtyWorld)
                {
                    calculateWorld();
                }
                return _world;
            }
        }

        private void calculateWorld()
        {
            _hasDirtyWorld = false;
            _world = Matrix.CreateScale(localScale) *
                   Matrix.CreateFromQuaternion(localRotation) *
                   Matrix.CreateTranslation(localPosition);
            if (_parent != null)
            {
                _world = _world * _parent.world;
            }
        }

        internal void SetPositionFromPhysics(Vector3 pos, float ang)
        {
            if (parent == null)
            {
                localPosition = pos;
            }
            else
            {
                localPosition = pos - parent.position;
            }
            localRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, ang);
        }

        [ContentSerializerIgnore]
        public Vector3 position
        {
            get
            {
                return world.Translation;
            }
            set
            {
                if (parent == null)
                {
                    localPosition = value;
                }
                else
                {
                    Vector3 trans = Vector3.Transform(value, Matrix.Invert(parent.world));
                    localPosition = trans;
                }
                if (rigidbody != null)
                {
                    rigidbody.MovePosition(position);
                }
            }
        }

        [ContentSerializerIgnore]
        public Vector3 lossyScale 
        { 
            get
            {
                if (parent == null)
                {
                    return localScale;
                }
                else
                {
                    Vector3 scale;
                    Quaternion rot;
                    Vector3 pos;
                    world.Decompose(out scale, out rot, out pos);
                    return scale;
                }
            }
        }

        [ContentSerializerIgnore]
        public Quaternion rotation 
        { 
            get
            {
                if (parent == null)
                {
                    return localRotation;
                }
                else
                {
                    Vector3 scale;
                    Quaternion rot;
                    Vector3 pos;
                    world.Decompose(out scale, out rot, out pos);
                    return rot;
                }
            }
            set
            {
                // TODO: This does not work yet
            }
        }

        [ContentSerializerIgnore]
        public float angleY
        {
            get
            {
                Vector3 normFwd = Vector3.Normalize(world.Forward);
                float dot = Vector3.Dot(normFwd, Vector3.Forward);
                if (world.Forward.X > 0)
                {
                    return (float)Math.Acos(dot);
                }
                else
                {
                    return (float)((Math.PI * 2) - Math.Acos(dot));
                }
            }
            set
            {
                localRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, value);
                if (rigidbody != null)
                {
                    rigidbody.MoveRotation(localRotation);
                }
            }
        }

        public void Rotate(Vector3 axis, float angle, Space relativeTo)
        {
            angle = MathHelper.ToRadians(angle);
            if (relativeTo == Space.World)
            {
                // TODO: This will have issues with parent rotations
                Matrix rot;
                Matrix.CreateFromAxisAngle(ref axis, angle, out rot);
                Matrix.Multiply(ref _world, ref rot, out _world);
                WorldChanged();
            }
            else
            {
                Quaternion q;
                Quaternion.CreateFromAxisAngle(ref axis, angle, out q);
                Quaternion.Multiply(ref _localRotation, ref q, out _localRotation);
                hasDirtyWorld = true;
            }
        }

        public void LookAt(Vector3 worldPosition, Vector3 worldUp)
        {
            Matrix m = Matrix.CreateWorld(position, worldPosition - position, worldUp);
            Vector3 scale;
            Quaternion rot;
            Vector3 pos;
            if (m.Decompose(out scale, out rot, out pos))
            {
                localRotation = rot;
                if (rigidbody != null)
                {
                    rigidbody.MoveRotation(localRotation);
                }
            }
        }

        public void LookAt(Vector3 worldPosition)
        {
            LookAt(worldPosition, Vector3.UnitY);
        }

        private void WorldChanged()
        {
            Vector3 scale;
            Quaternion rot;
            Vector3 pos;
            if (_world.Decompose(out scale, out rot, out pos))
            {
                _localScale = scale;
                _localRotation = rot;
                _localPosition = pos;
                hasDirtyWorld = false;
            }
        }

        public Vector3 right 
        {
            get
            {
                return world.Right;
            }
        }

        public Vector3 forward
        {
            get
            {
                return world.Forward;
            }
        }

        public Vector3 up 
        { 
            get
            {
                return world.Up;
            }
        }

        [ContentSerializerIgnore]
        public Transform root 
        { 
            get
            {
                if (parent != null)
                {
                    return parent.root;
                }
                return this;
            }
        }

        public Vector3 TransformPoint(Vector3 position)
        {
            return Vector3.Transform(position, world);
        }

        public Vector3 InverseTransformPoint(Vector3 position)
        {
            return Vector3.Transform(position, Matrix.Invert(world));
        }
    }
}
