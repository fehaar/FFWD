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
            if (gameObject.CompareTag("MainCamera"))
            {
                main = this;
            }
        }
        public void Destroy()
        {
            _allCameras.Remove(this);
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

        public Vector3 WorldToViewportPoint(Vector3 position)
        {
            return viewPort.Project(position, projectionMatrix, View(), Matrix.Identity);
        }

        internal BoundingFrustum GetBoundingFrustum()
        {
            return new BoundingFrustum(_view * projectionMatrix);
        }

        private static List<UIRenderer> uiRenderQueue = new List<UIRenderer>();
        internal static void AddRenderer(Renderer renderer)
        {
            if (!renderer.enabled)
            {
                return;
            }

            // Tag UI renderes og gem dem i en liste. Returner så kameraerne ikke f'r den
            if (renderer is UIRenderer)
            {
                uiRenderQueue.Add((UIRenderer)renderer);
                return;
            }

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
            if (device != null)
            {
                device.BlendState = BlendState.Opaque;
                device.DepthStencilState = DepthStencilState.Default;
                device.SamplerStates[0] = SamplerState.LinearClamp;
                if (wireframeRender)
                {
                    RasterizerState state = new RasterizerState();
                    state.FillMode = FillMode.WireFrame;
                    device.RasterizerState = state;
                }
            }

            for (int i = 0; i < _allCameras.Count; i++)
            {
                _allCameras[i].doRender(device);
            }

            if (wireframeRender)
            {
                device.RasterizerState = RasterizerState.CullCounterClockwise;
            }
            if (UIRenderer.batch == null)
            {
                UIRenderer.SetSpriteBatch(device);
            }
            UIRenderer.batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            for (int i = 0; i < uiRenderQueue.Count; i++)
            {
                if (uiRenderQueue[i].gameObject == null)
                {
                    // This will happen if the game object has been destroyed in update.
                    // It is acceptable behaviour.
                    continue;
                }                
                uiRenderQueue[i].Draw(device, null);
            }
            UIRenderer.batch.End();
            uiRenderQueue.Clear();
        }

        internal void doRender(GraphicsDevice device)
        {
            Clear(device);

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
