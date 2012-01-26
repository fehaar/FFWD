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
                inPos.y = inPos.z;
                inPos.z = cam.nearClipPlane;
                PressPlay.FFWD.Vector2 castPos = cam.ScreenToWorldPoint(inPos).Convert(ApplicationSettings.To2dMode.DropZ);
                Debug.Display("Mouse / Physics", inPos + " / " + castPos);
                RaycastHit hit;
                if (Physics.Pointcast(castPos, out hit, cam.cullingMask))
                {
                    Debug.Display("Over", hit.collider);
                }
                else
                {
                    Debug.Display("Over", "");
                }
            }
            return 0;
        }
    }
#endif
}
