using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public enum Space { World, Self }

    public class Transform
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
                _hasDirtyWorld = true;
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
                _hasDirtyWorld = true;
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
                _hasDirtyWorld = true;
            }
        }


        [ContentSerializer(Optional = true, CollectionItemName = "child")]
        internal List<GameObject> children { get; set; }

        internal GameObject gameObject;

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
                _parent = value;
                if (_parent == null)
                {
                    return;
                }
                if (_parent.children == null)
                {
                    _parent.children = new List<GameObject>();
                }
                if (gameObject == null)
                {
                    gameObject = new GameObject() { transform = this };
                }
                _parent.children.Add(gameObject);
                _hasDirtyWorld = true;
            }
        }

        private Matrix _world = Matrix.Identity;

        private bool _hasDirtyWorld = true;
        internal bool hasDirtyWorld
        {
            get
            {
                if (_parent == null)
                {
                    return _hasDirtyWorld;
                }
                else
                {
                    return _hasDirtyWorld || _parent.hasDirtyWorld;
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

        [ContentSerializerIgnore]
        public Vector3 position
        {
            get
            {
                return world.Translation;
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
        }

        public void Rotate(Vector3 axis, float angle, Space relativeTo)
        {
            angle = MathHelper.ToRadians(angle);
            if (relativeTo == Space.World)
            {
                Matrix rot;
                Matrix.CreateFromAxisAngle(ref axis, angle, out rot);
                Matrix.Multiply(ref _world, ref rot, out _world);
            }
            else
            {
                Quaternion q;
                Quaternion.CreateFromAxisAngle(ref axis, angle, out q);
                Quaternion.Multiply(ref _localRotation, ref q, out _localRotation);
                _hasDirtyWorld = true;
            }
            WorldChanged();
        }

        // TODO: Calculate new local values
        private void WorldChanged()
        {            
        }
    }
}
