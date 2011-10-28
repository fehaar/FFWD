using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Interfaces;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;
using FarseerPhysics;

namespace PressPlay.FFWD
{
    public static class Physics
    {
        private static World _world;
        public static World world
        {
            get
            {
                if (_world == null)
                {
                    Initialize();
                }
                return _world;
            }
            internal set
            {
                // TODO: If we have an old world we must dispose it properly
                _world = value;
            }
        }

        public const int kDefaultRaycastLayers = -5;

        private static bool isPaused = false;
        private static GameObjectContactProcessor contactProcessor;
        public static int velocityIterations = 2;
        public static int positionIterations = 2;

        private static readonly List<Body> movingBodies = new List<Body>(50);
        private static readonly List<Body> rigidBodies = new List<Body>(50);

        #region FFWD specific methods
        public static void Initialize()
        {
            Initialize(Vector2.zero);
        }

        public static void Initialize(Vector2 gravity)
        {
#if DEBUG
            Settings.EnableDiagnostics = true;
#else
            Settings.EnableDiagnostics = false;
#endif
            Settings.VelocityIterations = velocityIterations;
            Settings.PositionIterations = positionIterations;
            Settings.ContinuousPhysics = false;

            world = new World(gravity);
            
            Physics.contactProcessor = new GameObjectContactProcessor();

            world.ContactManager.BeginContact = Physics.contactProcessor.BeginContact;
            world.ContactManager.EndContact = Physics.contactProcessor.EndContact;
        }

        public static void TogglePause()
        {
            isPaused = !isPaused;
        }

        public static void Update(float elapsedTime)
        {
            for (int i = movingBodies.Count - 1; i >= 0; i--)
            {
                Body body = movingBodies[i];
                Component comp = (Component)body.UserData;
                BodyType bodyType = body.BodyType;
                if (bodyType == BodyType.Static)
                {
                    movingBodies.RemoveAt(i);
                    continue;
                }
                if (comp.gameObject.active && bodyType == BodyType.Kinematic)
                {
                    float rad = -MathHelper.ToRadians(comp.transform.eulerAngles.y);
                    if (body.Position != (Microsoft.Xna.Framework.Vector2)comp.transform.position || body.Rotation != rad)
                    {
                        Microsoft.Xna.Framework.Vector2 pos = comp.transform.position;
                        body.SetTransformIgnoreContacts(ref pos, rad);
                    }
                }
                if (comp.collider.allowTurnOff)
                {
                    body.Enabled = comp.gameObject.active;
                }
            }
#if DEBUG
            if (ApplicationSettings.ShowBodyCounter)
	        {
                Debug.Display("Body count", world.BodyList.Count);
	        }
#endif

            world.Step(elapsedTime);

            for (int i = rigidBodies.Count - 1; i >= 0; i--)
            {
                Body body = rigidBodies[i];
                Component comp = (Component)body.UserData;
                FarseerPhysics.Common.Transform t;
                body.GetTransform(out t);
                comp.transform.SetPositionFromPhysics(t.Position, t.Angle);
            }

            contactProcessor.Update();
        }
        #endregion

        #region Helper methods to create physics objects

        public static Bounds BoundsFromAABB(AABB aabb, float width)
        {
            Vector2 center = aabb.Center;
            Vector2 size = aabb.Extents;
            return new Bounds(new Vector3(center.x, 0, center.y),new Vector3(size.x*2, width, size.y*2));
        }

        public static Body AddBody()
        {
            return new Body(world);
        }

        public static void AddBox(Body body, bool isTrigger, float width, float height, Vector2 position, float density)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            Fixture fix = FixtureFactory.AttachRectangle(width, height, density, position, body);
            fix.IsSensor = isTrigger;            
        }

        public static void AddCircle(Body body, bool isTrigger, float radius, Vector2 position, float density)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            CircleShape shp = new CircleShape(radius, density);
            shp.Position = position;
            Fixture fix = body.CreateFixture(shp);
            fix.IsSensor = isTrigger;
        }

        public static void AddTriangle(Body body, bool isTrigger, Vertices vertices, float density)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            PolygonShape shp = new PolygonShape(vertices, density);
            Fixture fix = body.CreateFixture(shp);
            fix.IsSensor = isTrigger;
        }

        public static void AddMesh(Body body, bool isTrigger, List<Microsoft.Xna.Framework.Vector2[]> tris, float density)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            for (int i = 0; i < tris.Count(); i++)
            {
                Vertices verts = new Vertices(tris.ElementAt(i));
                try
                {
                    PolygonShape shp = new PolygonShape(verts, density);
                    Fixture fix = body.CreateFixture(shp);
                    fix.IsSensor = isTrigger;
                }
                catch (Exception ex)
                {
                    Debug.Log(body.UserData + ". Collider triangle is broken: " + verts[0] + "; " + verts[1] + "; " + verts[2] + ": " + ex.Message);
                }
            }
        }

        public static void AddMesh(Body body, bool isTrigger, List<Vertices> tris, float density)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            List<Fixture> fixes = FixtureFactory.AttachCompoundPolygon(tris, density, body);
            for (int i = 0; i < fixes.Count; i++)
            {
                fixes[i].IsSensor = isTrigger;
            }
        }
        #endregion

        #region Raycast methods
        public static bool Raycast(Vector2 origin, Vector2 direction)
        {
            return Raycast(origin, direction, Mathf.Infinity, kDefaultRaycastLayers);
        }

        public static bool Raycast(Vector2 origin, Vector2 direction, float distance)
        {
            return Raycast(origin, direction, distance, kDefaultRaycastLayers);
        }

        public static bool Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
        {
#if DEBUG
            Application.raycastTimer.Start();
#endif
            RaycastHelper.SetValues(distance, true, layerMask);

            Vector2 pt2 = origin + (direction * distance);
            if (pt2 == origin)
            {
                return false;
            }
            try
            {
                world.RayCast(null, origin, pt2);
            }
            catch (InvalidOperationException)
            {
                Debug.Log("RAYCAST THREW InvalidOperationException");
                return false;
            }
            finally
            {
#if DEBUG
                Application.raycastTimer.Stop();
#endif
            }
            return (RaycastHelper.HitCount > 0);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction)
        {
            return Raycast(origin, direction, Mathf.Infinity, kDefaultRaycastLayers);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, float distance)
        {
            return Raycast(origin, direction, distance, kDefaultRaycastLayers);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask)
        {
            return Raycast((Vector2)origin, (Vector2)direction, distance, layerMask);
        }

        public static bool Raycast(Vector2 origin, Vector2 direction, out RaycastHit hitInfo, float distance, int layerMask)
        {
#if DEBUG
            Application.raycastTimer.Start();
#endif
            RaycastHelper.SetValues(distance, true, layerMask);

            Vector2 pt2 = origin + (direction * distance);
            if (pt2 == origin)
            {
                hitInfo = new RaycastHit();
                return false;
            }
            try
            {
                world.RayCast(null, origin, pt2);
                hitInfo = RaycastHelper.ClosestHit();
            }
            catch (InvalidOperationException)
            {
                hitInfo = new RaycastHit();
                Debug.Log("RAYCAST THREW InvalidOperationException");
                return false;
            }
            finally
            {
#if DEBUG
                Application.raycastTimer.Stop();
#endif
            }
            return (RaycastHelper.HitCount > 0);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
        {
            return Raycast(origin, direction, out hitInfo, Mathf.Infinity, kDefaultRaycastLayers);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance)
        {
            return Raycast(origin, direction, out hitInfo, distance, kDefaultRaycastLayers);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask)
        {
            return Raycast((Vector2)origin, (Vector2)direction, out hitInfo, distance, layerMask);
        }

        public static bool Raycast(Ray ray)
        {
            return Raycast(ray.origin, ray.direction, Mathf.Infinity, kDefaultRaycastLayers);
        }

        public static bool Raycast(Ray ray, float distance)
        {
            return Raycast(ray.origin, ray.direction, distance, kDefaultRaycastLayers);
        }

        public static bool Raycast(Ray ray, float distance, int layerMask)
        {
            return Raycast(ray.origin, ray.direction, distance, layerMask);
        }

        public static bool Raycast(Ray ray, out RaycastHit hitInfo)
        {
            return Raycast(ray.origin, ray.direction, out hitInfo, Mathf.Infinity, kDefaultRaycastLayers);
        }

        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float distance)
        {
            return Raycast(ray.origin, ray.direction, out hitInfo, distance, kDefaultRaycastLayers);
        }

        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float distance, int layerMask)
        {
            return Raycast(ray.origin, ray.direction, out hitInfo, distance, layerMask);
        }

        public static RaycastHit[] RaycastAll(Vector2 origin, Vector2 direction)
        {
            return RaycastAll(origin, direction, Mathf.Infinity, kDefaultRaycastLayers);
        }

        public static RaycastHit[] RaycastAll(Vector2 origin, Vector2 direction, float distance)
        {
            return RaycastAll(origin, direction, distance, kDefaultRaycastLayers);
        }

        public static RaycastHit[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask)
        {
#if DEBUG
            Application.raycastTimer.Start();
#endif

            RaycastHelper.SetValues(distance, false, layerMask);

            Vector2 pt2 = origin + (direction * distance);
            if (pt2 == origin)
            {
                return new RaycastHit[0];
            }
            world.RayCast(null, origin, pt2);
#if DEBUG
            Application.raycastTimer.Stop();
#endif
            return RaycastHelper.Hits;
        }

        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction)
        {
            return RaycastAll(origin, direction, Mathf.Infinity, kDefaultRaycastLayers);
        }

        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float distance)
        {
            return RaycastAll(origin, direction, distance, kDefaultRaycastLayers);
        }

        public static RaycastHit[] RaycastAll(Ray ray, float distance, int layerMask)
        {
            return RaycastAll(ray.origin, ray.direction, distance, layerMask);
        }

        internal static bool Raycast(Body body, Ray ray, out RaycastHit hitInfo, float distance)
        {
            RayCastOutput output;
            RayCastInput input = new RayCastInput() { Point1 = ray.origin, Point2 = ray.origin + ray.direction, MaxFraction = distance };
            hitInfo = new RaycastHit() { body = body };
            for (int i = 0; i < body.FixtureList.Count; i++)
            {
                if (body.FixtureList[i].RayCast(out output, ref input, 0))
                {
                    hitInfo.normal = output.Normal;
                    hitInfo.distance = output.Fraction;
                    hitInfo.point = ray.GetPoint(output.Fraction);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Pointcast methods
        public static bool Pointcast(Vector2 point)
        {
            return Pointcast(point, kDefaultRaycastLayers);
        }

        public static bool Pointcast(Vector2 point, int layerMask)
        {
#if DEBUG
            Application.raycastTimer.Start();
#endif

            RaycastHelper.SetValues(float.MaxValue, true, layerMask);

            AABB aabb = new AABB(new Vector2(point.x - float.Epsilon, point.y - float.Epsilon), new Vector2(point.x + float.Epsilon, point.y + float.Epsilon));
            world.QueryAABB(RaycastHelper.pointCastCallback, ref aabb);
            return RaycastHelper.HitCount > 0;
        }

        public static bool Pointcast(Vector2 point, out RaycastHit hitInfo)
        {
            return Pointcast(point, out hitInfo, kDefaultRaycastLayers);
        }

        public static bool Pointcast(Vector2 point, out RaycastHit hitInfo, int layerMask)
        {
#if DEBUG
            Application.raycastTimer.Start();
#endif
            RaycastHelper.SetValues(float.MaxValue, true, layerMask);

            AABB aabb = new AABB(new Vector2(point.x - float.Epsilon, point.y - float.Epsilon), new Vector2(point.x + float.Epsilon, point.y + float.Epsilon));
            world.QueryAABB(RaycastHelper.pointCastCallback, ref aabb);
            if (RaycastHelper.HitCount > 0)
            {
                hitInfo = RaycastHelper.ClosestHit();
#if DEBUG
                Application.raycastTimer.Stop();
#endif
                return true;
            }
            else
            {
                hitInfo = new RaycastHit();
#if DEBUG
                Application.raycastTimer.Stop();
#endif
                return false;
            }
        }
        #endregion

        #region Linecast methods
        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, int layerMask)
        {

#if DEBUG
            Application.raycastTimer.Start();
#endif
            Vector2 pt2 = end;
            Vector2 origin = start;
            RaycastHelper.SetValues((origin - pt2).magnitude, true, layerMask);
            if (pt2 == origin)
            {
                hitInfo = new RaycastHit();
                return false;
            }
            world.RayCast(null, origin, pt2);
            hitInfo = RaycastHelper.ClosestHit();
#if DEBUG
            Application.raycastTimer.Stop();
#endif
            return (RaycastHelper.HitCount > 0);
        }
        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo)
        {
            return Linecast(start, end, out hitInfo, kDefaultRaycastLayers);
        }
        #endregion

        #region CheckCapsule methods
        public static bool CheckCapsule(Vector3 start, Vector3 end, float radius)
        {
            return CheckCapsule(start, end, radius, kDefaultRaycastLayers);
        }

        public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, LayerMask layermask)
        {
            //TODO an actual capsule check.. not just a raycasts            
            Vector3 forward = (end - start).normalized;
            Vector3 right = new Vector3(forward.z,0,-forward.x);

            Ray middleRay = new Ray(start, forward);
            Ray rightRay = new Ray(start + right * radius + forward * radius, forward);
            Ray leftRay = new Ray(start - right * radius + forward * radius, forward);

            float middleLength = (end - start).magnitude;
            float sidesLength = middleLength - radius * 2;

            return (Raycast(middleRay, middleLength, layermask) || Raycast(rightRay, sidesLength, layermask) || Raycast(leftRay, sidesLength, layermask));
        }
        #endregion

        #region OverlapSpehere methods
        public static Collider[] OverlapSphere(Vector3 position, float radius)
        {
            return OverlapSphere(position, radius, kDefaultRaycastLayers);
        }

        public static Collider[] OverlapSphere(Vector3 position, float radius, LayerMask layermask)
        {
            // TODO: Implement this
            return new Collider[0];
        }
        #endregion

        public static void IgnoreCollision(Collider collider1, Collider collider2)
        {
            IgnoreCollision(collider1, collider2, true);
        }

        public static void IgnoreCollision(Collider collider1, Collider collider2, bool ignore)
        {
            if (ignore)
            {
                collider1.connectedBody.IgnoreCollisionWith(collider2.connectedBody);
                RemoveStays(collider1, collider2);
            }
            else
            {
                collider1.connectedBody.RestoreCollisionWith(collider2.connectedBody);
            }
        }

        private static void RemoveStays(Collider collider1, Collider collider2)
        {
            contactProcessor.RemoveStay(collider1, collider2);
        }

        internal static void RemoveStays(Collider collider)
        {
            contactProcessor.ResetStays(collider);
        }

        internal static void AddMovingBody(Body body)
        {
            if (!movingBodies.Contains(body) && body.UserData != null)
            {
                movingBodies.Add(body);
            }
        }

        internal static void AddRigidBody(Body body)
        {
            if (!rigidBodies.Contains(body) && body.UserData != null)
            {
                rigidBodies.Add(body);
            }
        }

        internal static void RemoveBody(Body body)
        {
            if (body.UserData == null)
            {
                return;
            }
            world.RemoveBody(body);
            body.UserData = null;
            if (movingBodies.Contains(body))
            {
                movingBodies.Remove(body);
            }
            if (rigidBodies.Contains(body))
            {
                rigidBodies.Remove(body);
            }
        }
    }
}
