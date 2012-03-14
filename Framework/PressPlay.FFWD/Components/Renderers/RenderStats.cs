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

        internal static void AddDrawCall(int batched, int vertices, int triangles)
        {
            if (batched > 1)
            {
                BatchedDrawCalls += batched;
            }
            DrawCalls++;
            VerticesDrawn += vertices;
            TrianglesDrawn += triangles;
        }
    }
}
