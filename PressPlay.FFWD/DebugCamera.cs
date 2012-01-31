using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
#if DEBUG
    public class DebugCamera : Renderer
    {
        private DebugViewXNA physicsDebugView;

        public static bool activated = false;

        private List<FarseerPhysics.Dynamics.Fixture> fixtures = new List<FarseerPhysics.Dynamics.Fixture>(10);

        public override void Awake()
        {
            activated = true;
            physicsDebugView = new DebugViewXNA(Physics.world);
            physicsDebugView.LoadContent(Application.screenManager.GraphicsDevice);
            material = new Material() { name = "Debug material", color = Color.white, renderQueue = 100000, shader = "Transparent" };
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
                //Matrix view = Matrix.CreateRotationX(MathHelper.ToRadians(90)) * cam.view;
                Matrix view = cam.view;
                physicsDebugView.RenderDebugData(ref proj, ref view);

                PressPlay.FFWD.Vector3 inPos = Input.mousePosition;
                inPos.z = cam.nearClipPlane;
                PressPlay.FFWD.Vector2 castPos = cam.ScreenToWorldPoint(inPos);
                PressPlay.FFWD.Vector2 fromPos = cam.ScreenToWorldPoint(Vector2.zero);
                Debug.Display("Mouse / Physics", inPos + " / " + castPos);
                physicsDebugView.BeginCustomDraw(ref proj, ref view);
                physicsDebugView.DrawArrow(fromPos, castPos, 0.1f, 0.1f, false, Color.red);
                physicsDebugView.EndCustomDraw();

                RaycastHit hit;
                if (Physics.Pointcast(castPos, out hit, cam.cullingMask))
                {
                    Debug.Display("Over", hit.collider);
                }
                else
                {
                    Debug.Display("Over", "");
                }
                //RaycastHit[] hits = Physics.RaycastFromTo(fromPos, castPos, cam.cullingMask);
                //RaycastHit? hit = null;
                //if (hits.Length > 0)
                //{
                //    FarseerPhysics.Dynamics.Body b = hits[0].body;
                //    for (int i = 0; i < b.FixtureList.Count; i++)
                //    {
                //        FarseerPhysics.Common.Transform xf = b.Xf;
                //        Microsoft.Xna.Framework.Vector2 v = castPos;
                //        if (b.FixtureList[0].Shape.TestPoint(ref xf, ref v))
                //        {
                //            hit = hits[0];
                //        }
                //    }
                //}
                //if (hit.HasValue)
                //{
                //    Debug.Display("Over", hit.Value.collider);
                //}
                //else
                //{
                //    Debug.Display("Over", "");
                //}
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
