using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using PressPlay.U2X.Xna.Interfaces;

namespace PressPlay.U2X.Xna
{
    public static class Physics
    {
        internal static World world;
        private static bool isPaused = false;
        private static IContactProcessor contactProcessor;
        public static int velocityIterations = 6;
        public static int positionIterations = 4;

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

        public static void AddDebugDraw(DebugDraw debugDraw)
        {
            world.DebugDraw = debugDraw;
        }

        public static DebugDraw DebugDraw
        {
            get
            {
                return world.DebugDraw;
            }
        }

    }
}
