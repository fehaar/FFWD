using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;

namespace PressPlay.U2X.Xna
{
    internal class RaycastHelper
    {
        public RaycastHelper(float distance)
        {
            this.distance = distance;
        }

        private float distance;
        private List<RaycastHit> _hits = new List<RaycastHit>();

        internal float rayCastCallback(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            float dist = distance * fraction;
            //int index = 0;
            //while (index < _hits.Count)
            //{
            //    if (_hits[index].distance > dist)
            //    {
            //        break;
            //    }
            //    index++;
            //}
            //_hits.Insert(index, new RaycastHit() { body = fixture._body, point = point, normal = normal, distance = dist });
            _hits.Add(new RaycastHit() { body = fixture._body, point = point, normal = normal, distance = dist });
            return fraction;
        }   

        internal int HitCount
        {
            get
            {
                return _hits.Count;
            }
        }

        internal RaycastHit[] Hits
        {
            get
            {
                return _hits.ToArray();
            }
        }

        internal RaycastHit ClosestHit()
        {
            return _hits.LastOrDefault();
        }
    }
}
