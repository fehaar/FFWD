using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using PressPlay.U2X.Xna.Interfaces;
using PressPlay.U2X.Xna.Extensions;

namespace PressPlay.U2X.Xna
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
            contactProcessor.Update();
            if (world.DebugDraw != null)
            {
                world.DrawDebugData();
            }
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
        #endregion

        #region Helper methods to create physics objects
        public static Body AddBody(BodyDef definition)
        {
            return world.CreateBody(definition);
        }

        public static Body AddBox(float width, float height, Vector2 position, float angle, float density)
        {
            return AddBox(width, height, position, angle, density, new BodyDef());
        }

        public static Body AddBox(float width, float height, Vector2 position, float angle, float density, BodyDef definition)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            Body body = world.CreateBody(definition);
            PolygonShape shp = new PolygonShape();
            shp.SetAsBox(width / 2, height / 2, position, angle);
            body.CreateFixture(shp, density);
            return body;
        }

        public static Body AddCircle(float radius, Vector2 position, float angle, float density)
        {
            return AddCircle(radius, angle, density, new BodyDef() { position = position });
        }

        public static Body AddCircle(float radius, float angle, float density, BodyDef definition)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            Body body = world.CreateBody(definition);
            CircleShape shp = new CircleShape() { _radius = radius };
            body.CreateFixture(shp, density);
            return body;
        }

        public static Body AddTriangle(Vector2[] vertices, float angle, float density)
        {
            return AddTriangle(vertices, density, new BodyDef() { angle = angle });
        }

        public static Body AddTriangle(Vector2[] vertices, float density, BodyDef definition)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            Body body = world.CreateBody(definition);
            PolygonShape shp = new PolygonShape();
            shp.Set(vertices, vertices.Length);
            body.CreateFixture(shp, density);
            return body;
        }

        public static Body AddMesh(IEnumerable<Vector2[]> tris, int density)
        {
            return AddMesh(tris, density, new BodyDef());
        }

        public static Body AddMesh(IEnumerable<Vector2[]> tris, int density, BodyDef definition)
        {
            if (world == null)
            {
                throw new InvalidOperationException("You have to Initialize the Physics system before adding bodies");
            }
            Body body = world.CreateBody(definition);
            for (int i = 0; i < tris.Count(); i++)
            {
                Vector2[] tri = tris.ElementAt(i);
                try
                {
                    PolygonShape shp = new PolygonShape();
                    shp.Set(tri, tri.Length);
                    body.CreateFixture(shp, density);
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
