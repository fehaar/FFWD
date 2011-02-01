using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.UI;

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

        public static bool wireframeRender = false;

        private Color _backgroundColor = Color.black;
        public Color backgroundColor
        { 
            get 
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = new Color(value.r, value.g, value.b, 1);
            }
        }

        public Rectangle rect { get; set; }
        public ClearFlags clearFlags { get; set; }

        public override void Awake()
        {
            _allCameras.Add(this);
            _allCameras.Sort(this);

            for (int i = nonAssignedRenderers.Count - 1; i >= 0; i--)
            {
                if (addRenderer(nonAssignedRenderers[i]))
                {
                    nonAssignedRenderers.RemoveAt(i);
                }
            }

            if (gameObject.CompareTag("MainCamera") && (main == null))
            {
                main = this;
            }
        }

        public static void RemoveCamera(Camera cam)
        {
            _allCameras.Remove(cam);
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
                transform.position + transform.forward,
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

        public Vector3 WorldToViewportPoint(Vector3 position)
        {
            return viewPort.Project(position, projectionMatrix, View(), Matrix.Identity);
        }

        internal BoundingFrustum GetBoundingFrustum()
        {
            return new BoundingFrustum(_view * projectionMatrix);
        }

        internal static List<Renderer> nonAssignedRenderers = new List<Renderer>();
        internal static void AddRenderer(Renderer renderer)
        {
            // Tag UI renderes og gem dem i en liste. Returner så kameraerne ikke f'r den
            if (renderer is UIRenderer)
            {
                UIRenderer.AddRenderer(renderer as UIRenderer);
                return;
            }

            bool isAdded = false;
            for (int i = 0; i < _allCameras.Count; i++)
            {
                isAdded |= _allCameras[i].addRenderer(renderer);
            }
            if (!isAdded)
            {
                nonAssignedRenderers.Add(renderer);
            }
        }

        List<Renderer> renderQueue = new List<Renderer>();
        bool isRenderQueueSorted = true;
        private bool addRenderer(Renderer renderer)
        {
            if ((cullingMask & (1 << renderer.gameObject.layer)) > 0)
            {
                renderQueue.Add(renderer);
                isRenderQueueSorted = false;
                return true;
            }
            return false;
        }

        internal static void RemoveRenderer(Renderer renderer)
        {
            if (renderer is UIRenderer)
            {
                UIRenderer.RemoveRenderer(renderer as UIRenderer);
                return;
            }

            for (int i = 0; i < _allCameras.Count; i++)
            {
                _allCameras[i].removeRenderer(renderer);
            }
        }

        private void removeRenderer(Renderer renderer)
        {
            renderQueue.Remove(renderer);
        }

        internal static void DoRender(GraphicsDevice device)
        {
            if (device == null)
            {
                return;
            }

            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.LinearClamp;
            if (wireframeRender)
            {
                RasterizerState state = new RasterizerState();
                state.FillMode = FillMode.WireFrame;
                device.RasterizerState = state;
            }

            for (int i = 0; i < _allCameras.Count; i++)
            {
                _allCameras[i].doRender(device);
            }

            if (wireframeRender)
            {
                device.RasterizerState = RasterizerState.CullCounterClockwise;
            }

            UIRenderer.doRender(device);
        }

        internal void doRender(GraphicsDevice device)
        {
            Clear(device);

            if (!isRenderQueueSorted)
            {
                renderQueue.Sort(this);
                isRenderQueueSorted = true;
            }
            for (int i = 0; i < renderQueue.Count; i++)
            {
                if (renderQueue[i].gameObject == null)
                {
                    // This will happen if the game object has been destroyed in update.
                    // It is acceptable behaviour.
                    continue;
                }
                if (renderQueue[i].gameObject.active && renderQueue[i].enabled)
                {
                    renderQueue[i].Draw(device, this);
                }
            }
        }

        private void Clear(GraphicsDevice device)
        {
            switch (clearFlags)
            {
                case ClearFlags.Skybox:
                    device.Clear(backgroundColor);
                    break;
                case ClearFlags.Color:
                    device.Clear(backgroundColor);
                    break;
                case ClearFlags.Depth:
                    device.Clear(ClearOptions.DepthBuffer, backgroundColor, 1.0f, 0);
                    break;
            }
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
            return renderer.material.CalculateRenderQueue();
        }
        #endregion

        internal static Camera FindByName(string name)
        {
            for (int i = 0; i < _allCameras.Count; i++)
            {
                if (_allCameras[i].name == name)
                {
                    return _allCameras[i];
                }
            }
            return null;
        }
    }
}
