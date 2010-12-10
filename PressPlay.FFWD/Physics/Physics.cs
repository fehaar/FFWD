using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD.Extensions;

namespace PressPlay.FFWD
{
    public static class Physics
    {
        private static World _world;
        internal static World world
        {
            get
            {
                if (_world == null)
                {
                    Initialize();
                }
                return _world;
            }
            set
            {
                // TODO: If we have an old world we must dispose it properly
                _world = value;
            }
        }

        private static bool isPaused = false;
        private static IContactProcessor contactProcessor;
        public static int velocityIterations = 6;
        public static int positionIterations = 4;

        #region FFWD specific methods
        public static void Initialize()
        {
            Initialize(Vector2.Zero, null);
        }

        public static void Initialize(Vector2 gravity, IContactProcessor contactProcessor)
        {
            world = new World(gravity, true);
            Physics.contactProcessor = contactProcessor ?? new GameObjectContactProcessor();
            world.ContactListener = Physics.contactProcessor;
            world.ContinuousPhysics = true;
        }

        public static void TogglePause()
        {
            isPaused = !isPaused;
        }

        public static void Update(float elapsedTime)
        {
            world.Step(elapsedTime, velocityIterations, positionIterations);

            // Sync positions of game objects
            Body body = world.GetBodyList();
            do
            {
                Component comp = (Component)body.GetUserData();
                if (comp != null)
                {
                    if (body.GetType() == BodyType.Static)
                    {
                        body.SetTransform(comp.transform.position.To2d(), comp.transform.angleY);
                    }
                    else
                    {
                        Box2D.XNA.Transform t;
                        body.GetTransform(out t);
                        comp.transform.position = t.Position.To3d();
                        comp.transform.angleY = t.GetAngle();
                    }
                }
                body = body.GetNext();
            } while (body != null);

            contactProcessor.Update();
        }

        public static DebugDraw DebugDraw
        {
            get
            {
                return world.DebugDraw;
            }
            set
            {
                world.DebugDraw = value;
            }
        }

        public static void DoDebugDraw()
        {
            if (world.DebugDraw != null)
            {
                DebugDraw.Reset();
                world.DrawDebugData();
            }
        }
        #endregion

        #region Helper methods to create physics objects
        public static Body AddBody()
        {
            return world.CreateBody(new BodyDef());
        }

        public static Body AddBody(BodyDef definition)
        {
            return world.CreateBody(definition);
        }

        public static Body AddBox(Body body, bool isTrigger, float width, float height, Vector2 position, float angle, float density)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            PolygonShape shp = new PolygonShape();
            shp.SetAsBox(width / 2, height / 2, position, angle);
            Fixture fix = body.CreateFixture(shp, density);
            fix.SetSensor(isTrigger);
            return body;
        }

        public static Body AddCircle(Body body, bool isTrigger, float radius, Vector2 position, float angle, float density)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            CircleShape shp = new CircleShape() { _radius = radius };
            Fixture fix = body.CreateFixture(shp, density);
            fix.SetSensor(isTrigger);
            return body;
        }

        public static Body AddTriangle(Body body, bool isTrigger, Vector2[] vertices, float angle, float density)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            PolygonShape shp = new PolygonShape();
            shp.Set(vertices, vertices.Length);
            Fixture fix = body.CreateFixture(shp, density);
            fix.SetSensor(isTrigger);
            return body;
        }

        public static Body AddMesh(Body body, bool isTrigger, IEnumerable<Vector2[]> tris, float density)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            for (int i = 0; i < tris.Count(); i++)
            {
                Vector2[] tri = tris.ElementAt(i);
                try
                {
                    PolygonShape shp = new PolygonShape();
                    shp.Set(tri, tri.Length);
                    Fixture fix = body.CreateFixture(shp, density);
                    fix.SetSensor(isTrigger);
                }
                catch (Exception ex)
                {
                    Debug.Log(body._userData + ". Collider triangle is broken: " + tri[0] + "; " + tri[1] + "; " + tri[2] + ": " + ex.Message);
                }
            }
            return body;
        }
        #endregion

        #region Unity methods
        public static bool Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
        {
            RaycastHelper helper = new RaycastHelper(distance, true);
            Vector2 pt2 = origin + (direction * distance);
            if (pt2 == origin)
            {
                return false;
            }
            world.RayCast(helper.rayCastCallback, origin, pt2);
            return (helper.HitCount > 0);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask)
        {
            return Raycast(origin.To2d(), direction.To2d(), distance, layerMask);
        }

        public static bool Raycast(Vector2 origin, Vector2 direction, out RaycastHit hitInfo, float distance, int layerMask)
        {
            RaycastHelper helper = new RaycastHelper(distance, true);
            Vector2 pt2 = origin + (direction * distance);
            if (pt2 == origin)
            {
                hitInfo = new RaycastHit();
                return false;
            }
            world.RayCast(helper.rayCastCallback, origin, pt2);
            hitInfo = helper.ClosestHit();
            return (helper.HitCount > 0);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask)
        {
            return Raycast(origin.To2d(), direction.To2d(), out hitInfo, distance, layerMask);
        }

        public static bool Raycast(Ray ray, float distance, int layerMask)
        {
            return Raycast(ray.Position, ray.Direction, distance, layerMask);
        }

        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float distance, int layerMask)
        {
            return Raycast(ray.Position, ray.Direction, out hitInfo, distance, layerMask);
        }

        public static RaycastHit[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask)
        {
            RaycastHelper helper = new RaycastHelper(distance, false);
            Vector2 pt2 = origin + (direction * distance);
            if (pt2 == origin)
            {
                return new RaycastHit[0];
            }
            world.RayCast(helper.rayCastCallback, origin, pt2);
            return helper.Hits;
        }

        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float distance, int layerMask)
        {
            return RaycastAll(origin.To2d(), direction.To2d(), distance, layerMask);
        }

        public static RaycastHit[] RaycastAll(Ray ray, float distance, int layerMask)
        {
            return RaycastAll(ray.Position.To2d(), ray.Direction.To2d(), distance, layerMask);
        }
        #endregion
    }
}
