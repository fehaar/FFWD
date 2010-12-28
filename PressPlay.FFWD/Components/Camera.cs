using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class Camera : Component
    {
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
        
        public static Camera main;

        private Matrix _view;

        public Matrix View()
        {
            _view = Matrix.CreateLookAt(
                transform.localPosition,
                transform.localPosition + (Vector3)Microsoft.Xna.Framework.Vector3.UnitY,
                Microsoft.Xna.Framework.Vector3.Transform(Microsoft.Xna.Framework.Vector3.Backward, transform.localRotation));
            return _view;            
        }

        [ContentSerializerIgnore]
        public Viewport viewPort { get; set; }
        [ContentSerializerIgnore]
        public Matrix projectionMatrix { get; set; }

        public Ray ScreenPointToRay(Vector2 screen)
        {
            Vector3 near = viewPort.Unproject(new Vector3(screen.x, screen.y, 0), projectionMatrix, View(), Matrix.Identity);
            Vector3 far = viewPort.Unproject(new Vector3(screen.x, screen.y, 1), projectionMatrix, View(), Matrix.Identity);
            return new Ray(near, (far - near).normalized);
        }

    }
}
