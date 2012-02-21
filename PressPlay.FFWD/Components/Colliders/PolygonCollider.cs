using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;

namespace PressPlay.FFWD.Components
{
    public class PolygonCollider : Collider
    {
        public enum Plane2d { XY, XZ, YZ }

        public Plane2d plane2d;
        public Vector2[] relativePoints;

        protected override void DoAddCollider(FarseerPhysics.Dynamics.Body body, float mass)
        {
            switch (plane2d)
            {
                case Plane2d.XY:
                    to2dMode = Physics.To2dMode.DropZ;
                    break;
                case Plane2d.XZ:
                    to2dMode = Physics.To2dMode.DropY;
                    break;
                case Plane2d.YZ:
                    to2dMode = Physics.To2dMode.DropX;
                    break;
            }
            connectedBody = body;
            Vector2 scale = VectorConverter.Convert(transform.lossyScale, to2dMode);
            Vertices v = new Vertices();
            for (int i = 0; i < relativePoints.Length; i++)
            {
                v.Add(relativePoints[i] * scale);
            }
            Physics.AddMesh(body, isTrigger, EarclipDecomposer.ConvexPartition(v), mass);
        }
    }
}
