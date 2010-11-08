using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.U2X.Xna
{
    public class Transform
    {
        public Transform()
        {
            localRotation = Quaternion.Identity;
        }

        public Vector3 localPosition { get; set; }
        public Vector3 localScale { get; set; }
        public Quaternion localRotation { get; set; }
        [ContentSerializer(Optional = true, CollectionItemName = "child")]
        public List<GameObject> children { get; set; }

        [ContentSerializerIgnore]
        public Transform parent;

        [ContentSerializerIgnore]
        internal Matrix world
        {
            get
            {                
                return Matrix.CreateScale(localScale * 2.5f) *
                       // TODO: Find out why we need to rotate around X
                       Matrix.CreateRotationX(MathHelper.PiOver2) * 
                       Matrix.CreateFromQuaternion(localRotation) *
                       Matrix.CreateTranslation(localPosition);
            }
        }
    }
}
