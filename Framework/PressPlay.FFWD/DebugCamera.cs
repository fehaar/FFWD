using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD
{
#if DEBUG
    public class DebugCamera : Renderer
    {
        private DebugViewXNA physicsDebugView;

        public static bool activated = false;
        public static Plane? rayCastPlane = null;
        public static List<Collider> dontDrawColliders = new List<Collider>();

        private List<FarseerPhysics.Dynamics.Fixture> fixtures = new List<FarseerPhysics.Dynamics.Fixture>(10);

        public override void Awake()
        {
            activated = true;
            physicsDebugView = new DebugViewXNA(Physics.world);
            //physicsDebugView.DrawSolidShapes = false;
            physicsDebugView.LoadContent(Application.Instance.GraphicsDevice);
            material = new Material() { name = "Debug material", color = Color.white, renderQueue = 100000, shaderName = "Transparent" };
            base.Awake();
        }

        protected override void Destroy()
        {
            activated = false;
            base.Destroy();
        }

        public override int Draw(Microsoft.Xna.Framework.Graphics.GraphicsDevice device, Camera cam)
        {
            if (ApplicationSettings.ShowDebugLines)
            {
                Debug.DrawLines(device, cam);
            }
            if (ApplicationSettings.ShowDebugPhysics)
            {
                Matrix proj = cam.projectionMatrix;
                Matrix view = cam.view;
                if (rayCastPlane.HasValue)
                {
                    if (rayCastPlane.Value.normal == Vector3.up)
                    {
                        view = Matrix.CreateRotationX(MathHelper.ToRadians(90)) * view;
                    }
                }

                physicsDebugView.RenderDebugData(ref proj, ref view, b => dontDrawColliders.Contains(b.UserData));

                // NOTE: This only works for XY plane stuff
                PressPlay.FFWD.Vector3 inPos = Input.mousePosition;
                inPos.z = cam.nearClipPlane;
                PressPlay.FFWD.Vector2 castPos = cam.ScreenToWorldPoint(inPos);
                if (rayCastPlane.HasValue)
                {
                    float dist;
                    Ray ray = Camera.main.ScreenPointToRay(inPos);
                    if (rayCastPlane.Value.Raycast(ray, out dist))
                    {
                        Vector3 pt = ray.GetPoint(dist);
                        castPos = new Vector2(pt.x, pt.z);
                    }
                    else
                    {
                        castPos = Vector2.zero;
                    }
                }
                Debug.Display("Mouse / Physics", inPos + " / " + castPos);

                RaycastHit[] hits = Physics.PointcastAll(castPos, cam.cullingMask);
                if (hits.Length > 0)
                {
                    Debug.Display("Over", String.Join("\n", hits.Select(h => h.collider.ToString()).OrderBy(s => s).ToArray()));
                }
                else
                {
                    Debug.Display("Over", "");
                }
            }

            if(ApplicationSettings.ShowDebugPhysicsCustom)
            {
                Physics.DrawDebug();
            }
            return 0;
        }
    }
#endif
}
