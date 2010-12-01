using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public class Camera
    {
        public Camera()
        {
            transform = new Transform();
        }
        
        public static Camera main = new Camera();

        public Transform transform { get; set; }
        public Vector3 forward { get; set; }
        public Vector3 up { get; set; }
        private Matrix _view;

        public Matrix View()
        {
            _view = Matrix.CreateLookAt(
                transform.localPosition,
                transform.localPosition + forward,
                Vector3.Transform(up, transform.localRotation));
            return _view;            
        }

        public Matrix projectionMatrix { get; set; }
    }
}
