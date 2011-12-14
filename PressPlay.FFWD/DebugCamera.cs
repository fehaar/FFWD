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
        public Camera activeCamera;

        public static bool activated = false;

        public override void Awake()
        {
            activated = true;
            activeCamera = Camera.main;
            physicsDebugView = new DebugViewXNA(Physics.world);
            physicsDebugView.LoadContent(Application.screenManager.GraphicsDevice);
            material = new Material() { name = "Debug material", color = Color.white, renderQueue = 100000, shader = "Transparent" };
            base.Awake();
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
                Matrix view = Matrix.CreateRotationX(MathHelper.ToRadians(90)) * cam.view;
                physicsDebugView.RenderDebugData(ref proj, ref view);
            }
            return 0;
        }
    }
#endif
}
