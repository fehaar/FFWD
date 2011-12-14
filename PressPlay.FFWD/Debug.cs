using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;
using System.Text;
using System.Diagnostics;

namespace PressPlay.FFWD
{
    public class Debug
    {
        internal struct Line
        {
            internal Line(Vector3 start, Vector3 end, Color color)
            {
                this.start = start;
                this.end = end;
                this.color = color;
            }

            internal Microsoft.Xna.Framework.Vector3 start;
            internal Microsoft.Xna.Framework.Vector3 end;
            internal Microsoft.Xna.Framework.Color color;
        }

        private static List<UnityObject> gosToDisplay = new List<UnityObject>();
        private static Dictionary<string, string> _debugDisplay = new Dictionary<string, string>();

        private static List<Line> lines;

        public static bool DisplayLog = false;

#if DEBUG
        private static StringBuilder logBuilder = new StringBuilder();
#endif


        public static void Log(params object[] message)
        {
#if DEBUG
#if WINDOWS
            logBuilder.Clear();
#else
            logBuilder = new StringBuilder();
#endif
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] == null)
                {
                    logBuilder.Append("[null]");
                }
                else
                {
                    logBuilder.Append(message[i].ToString());
                    logBuilder.Append(" ");
                }
            }
//#if WINDOWS
            System.Diagnostics.Debug.WriteLine(logBuilder.ToString());
//#endif
            if (DisplayLog)
            {
                Display("Log", logBuilder.ToString());
            }
#endif
        }

        public static void LogFormat(string format, params object[] args)
        {
#if DEBUG
#if WINDOWS
            logBuilder.Clear();
#else
            logBuilder = new StringBuilder();
#endif
            logBuilder.AppendFormat(format, args);
#if WINDOWS
            System.Diagnostics.Debug.WriteLine(logBuilder.ToString());
#endif
            if (DisplayLog)
            {
                Display("Log", logBuilder.ToString());
            }
#endif
        }

        public static void LogError(string message)
        {
            System.Diagnostics.Debug.WriteLine("ERROR: " + message.ToString());
        }

        public static void LogWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine("Warning: " + message.ToString());
        }

        public static void Display(string key, object value)
        {
            _debugDisplay[key] = value.ToString();
        }

        public static IEnumerable<KeyValuePair<string, string>> DisplayStrings
        {
            get
            {
                return _debugDisplay.Concat(ObjectDisplays);
            }
        }

        public static void DisplayHierarchy(UnityObject obj)
        {
            gosToDisplay.Add(obj);
        }

        public static IEnumerable<KeyValuePair<string, string>> ObjectDisplays
        {
            get
            {
                foreach (UnityObject obj in gosToDisplay)
                {
                    Transform trans = (obj is GameObject) ? (obj as GameObject).transform : (obj as Component).transform;
                    while (trans != null)
                    {
                        yield return new KeyValuePair<string, string>(trans.name, trans.transform.position.ToString());
                        trans = trans.transform.parent;
                    }
                }
            }
        }

        [Conditional("DEBUG")]
        public static void Break()
        {
            //Debugger.Break();
        }

        [Conditional("DEBUG")]
        public static void DrawLine(Vector3 start, Vector3 end)
        {
            DrawLine(start, end, Color.white);
        }

        [Conditional("DEBUG")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            if (lines == null)
            {
                lines = new List<Line>();
            }
            lines.Add(new Line(start, end, color));
        }

        [Conditional("DEBUG")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            /// TODO: Make duration do something
            if (lines == null)
            {
                lines = new List<Line>();
            }
            lines.Add(new Line(start, end, color));
        }

        [Conditional("DEBUG")]
        public static void DrawRay(Vector3 start, Vector3 direction, Color color)
        {
            if (lines == null)
            {
                lines = new List<Line>();
            }
            lines.Add(new Line(start, start + direction, color));
        }

        [Conditional("DEBUG")]
        public static void DrawFilledBox(Vector3 center, Vector3 size, Color color)
        {
            Vector3 width = new Vector3(size.x, 0, 0) * 0.5f;
            Vector3 height = new Vector3(0, 0, size.z) * 0.5f;

            for (int i = 0; i < 20; i++)
            {
                Vector3 tmpWidth = Vector3.Lerp(width, -width, (float)i / 20f);

                Debug.DrawLine(center + tmpWidth + height, center + tmpWidth - height, color);
            }

            for (int i = 0; i < 20; i++)
            {
                Vector3 tmpHeight = Vector3.Lerp(height, -height, (float)i / 20f);

                Debug.DrawLine(center + width + tmpHeight, center -width + tmpHeight, color);
            }
        }

        [Conditional("DEBUG")]
        public static void DrawBox(Vector3 upperLeft, Vector3 upperRight, Vector3 lowerLeft, Vector3 lowerRight, Color color)
        {
            Debug.DrawLine(upperLeft, upperRight, color);
            Debug.DrawLine(upperRight, lowerRight, color);
            Debug.DrawLine(lowerRight, lowerLeft, color);
            Debug.DrawLine(lowerLeft, upperLeft, color);
        }

        [Conditional("DEBUG")]
        public static void DrawTransform(Transform t)
        {
            // TODO: Scale so it can be seen in the viewport no matter the distance
            Debug.DrawRay(t.position + t.up * 0.1f, t.forward * 5, Color.blue);
            Debug.DrawRay(t.position + t.up * 0.1f, t.right * 5, Color.red);
            Debug.DrawRay(t.position + t.up * 0.1f, t.up * 5, Color.green);
        }

        private static BasicEffect effect;
        [Conditional("DEBUG")]
        internal static void DrawLines(GraphicsDevice device, Camera cam)
        {
            if (lines == null || lines.Count == 0)
            {
                return;
            }

            if (cam == null)
            {
                lines.Clear();
                return;
            }

            if (effect == null)
            {
                effect = new BasicEffect(device);
            }

            device.RasterizerState = RasterizerState.CullNone;
            device.DepthStencilState = DepthStencilState.Default;
            device.BlendState = BlendState.AlphaBlend;

            effect.World = Matrix.Identity;
            effect.View = cam.view;
            effect.Projection = cam.projectionMatrix;
            effect.VertexColorEnabled = true;
            effect.Alpha = 1.0f;
            effect.LightingEnabled = false;

            // TODO: This can be optimized by not recreating data every time
            VertexPositionColor[] data = new VertexPositionColor[lines.Count * 2];
            int dataPos = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                data[dataPos++] = new VertexPositionColor()
                {
                    Position = lines[i].start,
                    Color = lines[i].color
                };
                data[dataPos++] = new VertexPositionColor()
                {
                    Position = lines[i].end,
                    Color = lines[i].color
                };
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    data,
                    0,
                    data.Length / 2
                );
            }

            lines.Clear();
        }

        [Conditional("DEBUG")]
        internal static void ClearLines()
        {
            if (lines != null)
            {
                lines.Clear();
            }
        }

        public static bool isDebugBuild 
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
    }
}
