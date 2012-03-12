using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Components
{
    internal static class RenderStats
    {
        public static int DrawCalls;
        public static int BatchedDrawCalls;
        public static int VerticesDrawn;
        public static int TrianglesDrawn;
        public static int RenderTextures;

        public static void Clear()
        {
            DrawCalls = BatchedDrawCalls = VerticesDrawn = TrianglesDrawn = RenderTextures = 0;
        }

        internal static void AddDrawCall(int batches, int vertices, int triangles)
        {
            if (batches > 1)
            {
                BatchedDrawCalls++;
            }
            else
            {
                DrawCalls++;
            }
            VerticesDrawn += vertices;
            TrianglesDrawn += triangles;
        }
    }
}
