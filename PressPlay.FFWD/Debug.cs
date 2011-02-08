using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;

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

        public static void Log(object message)
        {
            System.Diagnostics.Debug.WriteLine(message.ToString());
            if (DisplayLog)
            {
                Display("Log", message);
            }
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

        public static void DrawLine(Vector3 start, Vector3 end)
        {
            DrawLine(start, end, Color.white);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            if (lines == null)
            {
                lines = new List<Line>();
            }
            lines.Add(new Line(start, end, color));
        }

        public static void DrawRay(Vector3 start, Vector3 direction, Color color)
        {
            if (lines == null)
            {
                lines = new List<Line>();
            }
            lines.Add(new Line(start, start + direction, color));
        }

        public static void DrawFilledBox(Vector3 center, Vector3 size, Color color)
        {
            Vector3 width = new Vector3(size.x, 0, 0) * 0.5f;
            Vector3 height = new Vector3(0, 0, size.z) * 0.5f;

            //Debug.DrawLine(center + width + height, center + width - height, color);
            //Debug.DrawLine(center + width + height, center - width + height, color);

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

            //Debug.DrawLine(center - width + height, center - width - height, color);
            //Debug.DrawLine(center + width - height, center - width - height, color);
        }

        public static void DrawBox(Vector3 upperLeft, Vector3 upperRight, Vector3 lowerLeft, Vector3 lowerRight, Color color)
        {
            Debug.DrawLine(upperLeft, upperRight, color);
            Debug.DrawLine(upperRight, lowerRight, color);
            Debug.DrawLine(lowerRight, lowerLeft, color);
            Debug.DrawLine(lowerLeft, upperLeft, color);
        }

        private static BasicEffect effect;
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

            RasterizerState oldrasterizerState = device.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            effect.World = Matrix.Identity;
            effect.View = cam.view;
            effect.Projection = cam.projectionMatrix;
            effect.VertexColorEnabled = true;
            effect.Alpha = 1.0f;

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

            device.RasterizerState = oldrasterizerState;

            lines.Clear();
        }
    }
}
