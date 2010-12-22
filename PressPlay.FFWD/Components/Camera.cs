using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
                Microsoft.Xna.Framework.Vector3.Transform(up, transform.localRotation));
            return _view;            
        }

        public Viewport viewPort { get; set; }
        public Matrix projectionMatrix { get; set; }

        public Ray ScreenPointToRay(Vector2 screen)
        {
            Vector3 near = viewPort.Unproject(new Vector3(screen.x, screen.y, 0), projectionMatrix, View(), Matrix.Identity);
            Vector3 far = viewPort.Unproject(new Vector3(screen.x, screen.y, 1), projectionMatrix, View(), Matrix.Identity);
            return new Ray(near, (far - near).normalized);
        }
    }
}
