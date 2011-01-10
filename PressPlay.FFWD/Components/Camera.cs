using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class Camera : Component, IComparer<Camera>, IComparer<Renderer>
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

        public override void Awake()
        {
            _allCameras.Add(this);
            _allCameras.Sort(this);
            if (gameObject.CompareTag("MainCamera"))
            {
                main = this;
            }
        }

        private static List<Camera> _allCameras = new List<Camera>();
        public static IEnumerable<Camera> allCameras
        {
            get
            {
                return _allCameras;
            }
        }

        public static Camera main { get; private set; }

        public static Viewport FullScreen;

        private Matrix _view;
        public Matrix View()
        {
            _view = Matrix.CreateLookAt(
                transform.position,
                transform.position - transform.forward,
                transform.up);
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

        internal static void AddRenderer(Renderer renderer)
        {
            for (int i = 0; i < _allCameras.Count; i++)
            {
                _allCameras[i].addRenderer(renderer);
            }
        }

        List<Renderer> renderQueue = new List<Renderer>();
        private void addRenderer(Renderer renderer)
        {
            if ((cullingMask & (1 << renderer.gameObject.layer)) > 0)
            {
                renderQueue.Add(renderer);
            }
        }

        internal static void DoRender(GraphicsDevice device)
        {
            for (int i = 0; i < _allCameras.Count; i++)
            {
                _allCameras[i].doRender(device);
            }
        }

        internal void doRender(GraphicsDevice device)
        {
            renderQueue.Sort(this);
            for (int i = 0; i < renderQueue.Count; i++)
            {
                if (renderQueue[i].gameObject == null)
                {
                    // This will happen if the game object has been destroyed in update.
                    // It is acceptable behaviour.
                    continue;
                }
                if (renderQueue[i].gameObject.active)
                {
                    renderQueue[i].Draw(device, this);
                }
            }
            renderQueue.Clear();
        }
            
        #region IComparer<Camera> Members
        public int Compare(Camera x, Camera y)
        {
            return x.depth.CompareTo(y.depth);
        }
        #endregion

        #region IComparer<IRenderable> Members
        public int Compare(Renderer x, Renderer y)
        {
            float xRq = GetRenderQueue(x);
            float yRq = GetRenderQueue(y);

            if (xRq == yRq)
            {
                if (xRq == 0)
                {
                    return 0;
                }
                string xTex = x.material.mainTexture ?? "";
                string yTex = y.material.mainTexture ?? "";
                return xTex.CompareTo(yTex);
            }
            return xRq.CompareTo(yRq);
        }

        private float GetRenderQueue(Renderer renderer)
        {
            if (renderer.material == null)
            {
                return 0;
            }
            float q = renderer.material.renderQueue;
            if (renderer.material.blendState == BlendState.AlphaBlend)
            {
                return q + 0.1f;
            }
            if (renderer.material.blendState == BlendState.Additive)
            {
                return q + 0.1f;
            }
            return q;
        }
        #endregion
    }
}
