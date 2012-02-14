using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Components;
using FarseerPhysics.Dynamics;

namespace PressPlay.FFWD
{
    public enum Space { World, Self }
    [Flags]
    internal enum TransformChanges { None = 0x0, Position = 0x1, Rotation = 0x2, Scale = 0x4, Everything = 0x7 };

    public class Transform : Component, IEnumerable
    {
        #region Constructors
        internal Transform()
        {
            localRotation = Quaternion.identity;
            localScale = Vector3.one;
        }
        #endregion

        #region Static members
        private static Queue<Transform> transformsChanged = new Queue<Transform>(ApplicationSettings.DefaultCapacities.TransformChanges);
        #endregion

        #region Properties
        internal TransformChanges changes = TransformChanges.None;

        [ContentSerializer(ElementName = "p", Optional = true)]
        internal Vector3 _localPosition;
        [ContentSerializerIgnore]
        public Vector3 localPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                if (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z))
                {
                    throw new InvalidOperationException();
                }
                if (_localPosition != value)
                {
                    _localPosition = value;
                    RecordChanges(TransformChanges.Position);
                }
            }
        }

        [ContentSerializer(ElementName = "s", Optional = true)]
        internal Vector3 _localScale = Vector3.one;
        [ContentSerializerIgnore]
        public Vector3 localScale
        {
            get
            {
                return _localScale;
            }
            set
            {
                if (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z))
                {
                    throw new InvalidOperationException();
                }
                if (_localScale != value)
                {
                    _localScale = value;
                    RecordChanges(TransformChanges.Scale);
                }
            }
        }

        [ContentSerializer(ElementName = "r", Optional = true)]
        internal Quaternion _localRotation = Quaternion.identity;
        [ContentSerializerIgnore]
        public Quaternion localRotation
        {
            get
            {
                return _localRotation;
            }
            set
            {
                if (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z) || float.IsNaN(value.w))
                {
                    throw new InvalidOperationException();
                }
                if (_localRotation != value)
                {
                    _localRotation = value;
                    RecordChanges(TransformChanges.Rotation);
                }
            }
        }

        [ContentSerializer(Optional = true, CollectionItemName = "go", FlattenContent = true)]
        private List<GameObject> children { get; set; }

        internal Transform _parent;
        [ContentSerializerIgnore]
        public Transform parent
        {
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
                Quaternion rot = rotation;
                Vector3 scale = lossyScale;

                _parent = value;
                if (_parent == null)
                {
                    localPosition = pos;
                    localRotation = rot;
                    localScale = scale;
                    return;
                }
                position = pos;
                rotation = rot;
                localScale = scale / _parent.lossyScale;
                if (_parent.children == null)
                {
                    _parent.children = new List<GameObject>();
                }
                _parent.children.Add(gameObject);
            }
        }

        private bool hasDirtyWorld = true;

        private Matrix _world = Matrix.Identity;
        [ContentSerializerIgnore]
        public Matrix world
        {
            get
            {
                if (hasDirtyWorld)
                {
                    hasDirtyWorld = false;
                    _world = Matrix.CreateScale(localScale) *
                           Matrix.CreateFromQuaternion(localRotation) *
                           Matrix.CreateTranslation(localPosition);
                    if (_parent != null)
                    {
                        _world = _world * _parent.world;
                    }
                }
                return _world;
            }
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
                if (float.IsNaN(value.x) || float.IsNegativeInfinity(value.x))
                {
                    value.x = 0;
                }

                if (float.IsNaN(value.y) || float.IsNegativeInfinity(value.y))
                {
                    value.y = 0;
                }

                if (float.IsNaN(value.z) || float.IsNegativeInfinity(value.z))
                {
                    value.z = 0;
                }
                if (parent == null)
                {
                    localPosition = value;
                }
                else
                {
                    Vector3 trans = Microsoft.Xna.Framework.Vector3.Transform(value, Matrix.Invert(parent.world));
                    localPosition = trans;
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
                    return localScale * parent.lossyScale;
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
                    return localRotation * parent.rotation;
                }
            }
            set
            {
                if (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z) || float.IsNaN(value.w))
                {
                    throw new InvalidOperationException();
                }
                if (parent == null)
                {
                    localRotation = value;
                }
                else
                {
                    localRotation = Quaternion.Inverse(parent.rotation) * value;
                }
            }
        }

        [ContentSerializerIgnore]
        public Vector3 eulerAngles
        {
            get
            {
                return rotation.eulerAngles;
            }
            set
            {
                if (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z))
                {
                    throw new InvalidOperationException();
                }
                rotation = Quaternion.Euler(value);
            }
        }

        [ContentSerializerIgnore]
        public Vector3 localEulerAngles
        {
            get
            {
                return localRotation.eulerAngles;
            }
            set
            {
                if (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z))
                {
                    throw new InvalidOperationException();
                }
                localRotation = Quaternion.Euler(value);
            }
        }

        [ContentSerializerIgnore]
        public Vector3 right
        {
            get
            {
                return ((Vector3)world.Right).normalized;
            }
        }

        [ContentSerializerIgnore]
        public Vector3 forward
        {
            get
            {
                return ((Vector3)world.Backward).normalized;
            }
        }

        [ContentSerializerIgnore]
        public Vector3 up
        {
            get
            {
                return ((Vector3)world.Up).normalized;
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

        public int childCount
        {
            get
            {
                if (children == null)
                {
                    return 0;
                }
                return children.Count;
            }
        }

        [ContentSerializerIgnore]
        public Matrix4x4 worldToLocalMatrix
        {
            get
            {
                return (Matrix4x4)world;
            }
        }

        [ContentSerializerIgnore]
        public Matrix4x4 localToWorldMatrix
        {
            get
            {
                return (Matrix4x4)Matrix.Invert(world);
            }
        }
        #endregion

        #region Private and internal methods
        internal void RecordChanges(TransformChanges transformChanges)
        {
            if (transformChanges == TransformChanges.None)
            {
                return;
            }
            if (changes == TransformChanges.None)
            {
                Transform.transformsChanged.Enqueue(this);
            }
            changes |= transformChanges;
            if (childCount > 0)
            {
                TransformChanges childChanges = changes;
                // If we rotate, we can change the child position as well
                if ((changes & TransformChanges.Rotation) == TransformChanges.Rotation)
                {
                    childChanges |= TransformChanges.Position;
                }
                for (int i = 0; i < children.Count; i++)
                {
                    children[i].transform.RecordChanges(childChanges);
                }
            }
            hasDirtyWorld = true;
        }

        internal void SetPositionFromPhysics(Vector3 pos, float ang, Vector3 up)
        {
            position = pos;
            rotation = Quaternion.AngleAxis(ang, up);
        }

        internal override void AfterLoad(Dictionary<int, UnityObject> idMap)
        {
            base.AfterLoad(idMap);
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    children[i].isPrefab = isPrefab;
                    children[i].transform._parent = this;
                    children[i].AfterLoad(idMap);
                }
            }
        }

        internal override UnityObject Clone()
        {
            Transform obj = base.Clone() as Transform;
            if (children != null)
            {
                obj.children = new List<GameObject>();
                for (int i = 0; i < children.Count; i++)
                {
                    GameObject child = children[i].Clone() as GameObject;
                    child.transform._parent = obj;
                    obj.children.Add(child);
                }
            }
            return obj;
        }

        internal override void SetNewId(Dictionary<int, UnityObject> idMap)
        {
            base.SetNewId(idMap);
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    children[i].SetNewId(idMap);
                }
            }
        }

        internal override void FixReferences(Dictionary<int, UnityObject> idMap)
        {
            // NOTE: We should not call base as transform has no references to hide
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    children[i].FixReferences(idMap);
                }
            }
        }

        internal void SetActiveRecursively(bool state)
        {
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    children[i].SetActiveRecursively(state);
                }
            }
        }

        protected override void Destroy()
        {
            base.Destroy();
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    Destroy(children[i]);
                }
            }
        }

        internal void DontDestroyOnLoadOnChildren()
        {
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    Application.DontDestroyOnLoad(children[i]);
                }
            }
        }
        #endregion

        #region Public methods
        public void Translate(Vector3 translation)
        {
            Translate(translation, Space.Self);
        }

        public void Translate(Vector3 translation, Space space)
        {
            if (space == Space.Self)
            {
                // TODO: Test this properly
                Vector3 trans = Microsoft.Xna.Framework.Vector3.TransformNormal(translation, world);
                localPosition += trans;
            }
            else
            {
                localPosition += translation;
            }
        }

        public void Translate(float x, float y, float z)
        {
            Translate(new Vector3(x, y, z), Space.Self);
        }

        public void Translate(float x, float y, float z, Space space)
        {
            Translate(new Vector3(x, y, z), space);
        }

        public void Translate(Vector3 translation, Transform relativeTo)
        {
            Vector3 trans = Microsoft.Xna.Framework.Vector3.TransformNormal(translation, relativeTo.world);
            Translate(translation, Space.World);
        }

        public void Translate(float x, float y, float z, Transform relativeTo)
        {
            Translate(new Vector3(x, y, z), relativeTo);
        }

        public void Rotate(Vector3 axis, float angle, Space relativeTo)
        {
            // TODO: Test this as this probably does not work
            if (relativeTo == Space.World)
            {
                Quaternion q = Quaternion.AngleAxis(angle, axis);
                rotation *= q;
            }
            else
            {
                Quaternion q = Quaternion.AngleAxis(angle, axis);
                localRotation *= q;
            }
        }

        public void Rotate(Vector3 axis, float angle)
        {
            Rotate(axis, angle, Space.Self);
        }

        public void Rotate(Vector3 eulerAngles, Space space)
        {
            Rotate(eulerAngles.x, eulerAngles.y, eulerAngles.z, space);
        }

        public void Rotate(Vector3 eulerAngles)
        {
            Rotate(eulerAngles.x, eulerAngles.y, eulerAngles.z, Space.Self);
        }

        public void Rotate(float x, float y, float z)
        {
            Rotate(x, y, z, Space.Self);
        }

        public void Rotate(float x, float y, float z, Space relativeTo)
        {
            if (relativeTo == Space.World)
            {
                Quaternion q = Quaternion.Euler(x, y, z);
                rotation *= q;
            }
            else
            {
                Quaternion q = Quaternion.Euler(x, y, z);
                localRotation *= q;
            }
        }

        public void LookAt(Vector3 worldPosition, Vector3 worldUp)
        {
            // We cannot look at the point where we are - it will have no effect.
            if (worldPosition == position) { return; }

            Matrix m = Matrix.CreateWorld(position, position - worldPosition, worldUp);
            Microsoft.Xna.Framework.Vector3 scale;
            Microsoft.Xna.Framework.Quaternion rot;
            Microsoft.Xna.Framework.Vector3 pos;

            if (m.Decompose(out scale, out rot, out pos))
            {
                rotation = rot;
            }
        }

        public void LookAt(Transform target, Vector3 worldUp)
        {
            LookAt(target.position, worldUp);
        }

        public void LookAt(Vector3 worldPosition)
        {
            LookAt(worldPosition, Vector3.up);
        }

        public void LookAt(Transform t)
        {
            LookAt(t.position, Vector3.up);
        }

        public IEnumerator GetEnumerator()
        {
            if (children == null)
            {
                return (new List<Transform>()).GetEnumerator();
            }
            
            return children.GetEnumerator();
        }

        public Vector3 TransformDirection(Vector3 position)
        {            
            return Microsoft.Xna.Framework.Vector3.TransformNormal(position, world);
        }

        public Vector3 TransformDirection(float x, float y, float z)
        {
            return Microsoft.Xna.Framework.Vector3.TransformNormal(new Microsoft.Xna.Framework.Vector3(x, y, z), world);
        }

        public Vector3 InverseTransformDirection(Vector3 position)
        {
            return Microsoft.Xna.Framework.Vector3.TransformNormal(position, Microsoft.Xna.Framework.Matrix.Invert(world));
        }

        public Vector3 InverseTransformDirection(float x, float y, float z)
        {
            return Microsoft.Xna.Framework.Vector3.TransformNormal(new Microsoft.Xna.Framework.Vector3(x, y, z), Microsoft.Xna.Framework.Matrix.Invert(world));
        }

        public Vector3 TransformPoint(Vector3 position)
        {
            return Microsoft.Xna.Framework.Vector3.Transform(position, world);
        }

        public Vector3 TransformPoint(float x, float y, float z)
        {
            return Microsoft.Xna.Framework.Vector3.Transform(new Microsoft.Xna.Framework.Vector3(x, y, z), world);
        }

        public Vector3 InverseTransformPoint(Vector3 position)
        {
            return Microsoft.Xna.Framework.Vector3.Transform(position, Matrix.Invert(world));
        }

        public Vector3 InverseTransformPoint(float x, float y, float z)
        {
            return Microsoft.Xna.Framework.Vector3.Transform(new Microsoft.Xna.Framework.Vector3(x, y, z), Matrix.Invert(world));
        }

        public void RotateAround(Vector3 vector3, float rotateThisFrame)
        {
            // TODO: Implement this
            throw new NotImplementedException("Not implemented yet");
        }

        public void DetachChildren()
        {
            while (childCount > 0)
            {
                children[0].transform.parent = null;
            }
        }

        public Transform Find(string name)
        {
            bool depthSearch = false;
            if (name.StartsWith("//"))
            {
                depthSearch = true;
                name = name.Substring(2);
            }
            for (int i = 0; i < childCount; i++)
            {
                int pathIndex = name.IndexOf('/');
                if (pathIndex > -1)
                {
                    if (name.StartsWith(children[i].name + '/'))
                    {
                        return children[i].transform.Find(name.Substring(pathIndex + 1));
                    }
                }
                else
                {
                    if (name == children[i].name || name + "(Clone)" == children[i].name)
                    {
                        return children[i].transform;
                    }
                    if (depthSearch)
                    {
                        Transform t = children[i].transform.Find("//" + name);
                        if (t != null)
                        {
                            return t;
                        }
                    }
                }
            }
            return null;
        }

        public bool IsChildOf(Transform trans)
        {
            if (parent == null)
            {
                return false;
            }
            if (parent == trans)
            {
                return true;
            }
            return parent.IsChildOf(trans);
        }

        public Transform FindChild(string name)
        {
            return Find(name);
        }

        public Transform GetChild(int i)
        {
            return children[i].transform;
        }
        #endregion

        #region Component locator methods
        internal void GetComponentsInChildrenInt(Type type, List<Component> list)
        {
            if (children != null)
            {
                for (int childIndex = 0; childIndex < children.Count; childIndex++)
                {
                    children[childIndex].GetComponentsInChildren(type, list);
                }
            }
        }

        internal void GetComponentsInChildrenInt<T>(List<Component> list) where T : Component
        {
            if (children != null)
            {
                for (int childIndex = 0; childIndex < children.Count; childIndex++)
                {
                    children[childIndex].GetComponentsInChildren<T>(list);
                }
            }
        }

        internal Component GetComponentInChildrenInt(Type type)
        {
            if (transform.children != null)
            {
                for (int childIndex = 0; childIndex < transform.children.Count; childIndex++)
                {
                    Component cmp = transform.children[childIndex].GetComponentInChildren(type);
                    if (cmp != null)
                    {
                        return cmp;
                    }
                }
            }
            return null;
        }
        #endregion

        internal void BroadcastMessage(string methodName, object value, SendMessageOptions sendMessageOptions)
        {
            if (transform.children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    children[i].BroadcastMessage(methodName, value, sendMessageOptions);
                }
            }
        }

        internal static void ApplyPositionChanges()
        {
            while (transformsChanged.Count > 0)
            {
                Transform t = transformsChanged.Dequeue();
                if (t.camera != null)
                {
                    t.camera.RecalculateView();
                }
                Collider coll = t.collider;
                if (coll != null)
                {
                    Physics.MoveCollider(coll);
                }
                t.changes = TransformChanges.None;
            }
        }
    }
}
