using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public static class ApplicationSettings
    {
        public static bool ShowComponentProfile = false;
        public static bool ShowTurnOffTime = false;
        public static bool ShowTimeBetweenUpdates = false;
        public static bool ShowRaycastTime = false;
        public static bool ShowParticleAnimTime = false;
        public static bool ShowBodyCounter = false;
        public static bool ShowDebugPhysics = false;
        public static bool ShowDebugPhysicsCustom = false;
#if DEBUG
        public static bool ShowFPSCounter = true;
        public static bool ShowDebugDisplays = true;
        public static bool ShowDebugLines = true;
        public static bool ShowPerformanceBreakdown = true;
        public static bool ShowPeakMemory = true;
#else
        public static bool ShowFPSCounter = false;
        public static bool ShowDebugDisplays = false;
        public static bool ShowDebugLines = false;
        public static bool ShowPerformanceBreakdown = false;
        public static bool ShowPeakMemory = false;
#endif

#if WINDOWS
        public static int AssetLoadInterval = 0; // In Milliseconds
#else
        public static int AssetLoadInterval = 50; // In Milliseconds
#endif
        public static bool UseFallbackRendering = false;

        /// <summary>
        /// This can be disabled to avoid moving static colliders, which is pretty expensive.
        /// If you do it a lot, consider flipping this and making the colliders kinematic.
        /// </summary>
        public static bool Physics_MoveStaticColliders = true;
        public static Physics.To2dMode Physics_Default2DMode = Physics.To2dMode.DropY;

        public static class DefaultCapacities
        {
            /// <summary>
            /// All of these settings can be tuned in order to adjust memory usage of the engine.
            /// You should do this in your game contructor.
            /// </summary>
            #region Default list sizes
            public static int Lights = 4;
            public static int QueryHelper = 20;
            public static int RaycastHits = 20;
            public static int GestureSamples = 4;
            public static int Touches = 4;
            public static int AnimationComponents = 30;
            public static int TransformChanges = 500;
            public static int ComponentLists = 1500;
            public static int RenderQueues = 100;
            public static int RenderCullingQueue = 20;
            public static int ColliderContacts = 50;
            public static int PriorityQueuePoolSize = 100; 
            #endregion
        }

        public static class DefaultValues
        {
            public static float MinimumNearClipPlane = 0.1f;
            /// <summary>
            ///  This is the vertex limit for single size static batches. If we have more vertices than that, the batch will be cut up into tiled pieces with the world size set below.
            /// </summary>
            public static int StaticBatchVertexLimit = 2000;
            public static float StaticBatchTileSize = 200.0f;
        }

        public static class LogSettings
        {
            public static bool LogCulling = false;
        }
    }
}
