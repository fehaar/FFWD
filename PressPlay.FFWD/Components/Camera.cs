using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class Camera : Component
    {
        public Camera()
        {
            fieldOfView = MathHelper.ToRadians(60);
            nearClipPlane = 0.3f;
            farClipPlane = 1000;
        }

        public enum ClearFlags
        {
            Skybox,
            Color,
            Depth,
            Nothing
        }

        public float fieldOfView { get; set; }
        public float nearClipPlane { get; set; }
        public float farClipPlane { get; set; }
        public float orthographicSize { get; set; }
        public bool orthographic { get; set; }
        public int depth { get; set; }
        public float aspect { get; set; }
        public int cullingMask { get; set; }
        public Color backgroundColor { get; set; }
        public Rectangle rect { get; set; }
        public ClearFlags clearFlags { get; set; }

        private static Camera _main = null;
        public static Camera main
        {
            get
            {
                if (_main == null)
                {
                    _main = (Camera)GameObject.FindObjectOfType(typeof(Camera));
                }
                return _main;
            }
            set
            {
                _main = value;
            }
        }

        public static Viewport FullScreen;

        private Matrix _view;
        public Matrix View()
        {
            _view = Matrix.CreateLookAt(
                transform.position,
                transform.position + (Vector3)Microsoft.Xna.Framework.Vector3.UnitY,
                Microsoft.Xna.Framework.Vector3.Transform(Microsoft.Xna.Framework.Vector3.Backward, transform.rotation));
            return _view;            
        }

        [ContentSerializerIgnore]
        public Viewport viewPort 
        { 
            get
            {
                return FullScreen;
            }
        }

        private Matrix _projectionMatrix = Matrix.Identity;
        [ContentSerializerIgnore]
        public Matrix projectionMatrix
        {
            get
            {
                if (_projectionMatrix == Matrix.Identity)
                {
                    Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView), FullScreen.AspectRatio, nearClipPlane, farClipPlane, out _projectionMatrix);
                }
                return _projectionMatrix;
            }
        }

        public Ray ScreenPointToRay(Vector2 screen)
        {
            Vector3 near = viewPort.Unproject(new Vector3(screen.x, screen.y, 0), projectionMatrix, View(), Matrix.Identity);
            Vector3 far = viewPort.Unproject(new Vector3(screen.x, screen.y, 1), projectionMatrix, View(), Matrix.Identity);
            return new Ray(near, (far - near).normalized);
        }

    }
}
