using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD.Components
{
    public class ParticleEmitter : Component, IFixedUpdateable
    {
        public bool emit;
        public float minSize;
        public float maxSize;
        public float minEnergy;
        public float maxEnergy;
        public float minEmission;
        public float maxEmission;
        public float emitterVelocityScale;
        public Vector3 worldVelocity;
        public Vector3 localVelocity;
        public Vector3 rndVelocity;
        public bool useWorldSpace;
        public bool enabled;

        [ContentSerializerIgnore]
        public int particleCount
        {
            get
            {
                return particles.Length;
            }
        }

        internal Vector3 ellipsoid = Vector2.one;
        internal bool oneShot = false;

        [ContentSerializerIgnore]
        public Vector3 minEmitterRange { get; set; }

        private Particle[] _particles;
        [ContentSerializerIgnore]
        public Particle[] particles       
        {
            get
            {
                return _particles;
            }
            set
            {
                _particles = value;
                particlesInUse = 0;
                for (int i = 0; i < _particles.Length; i++)
                {
                    if (_particles[i].Energy > 0)
                    {
                        particlesInUse++;
                    }
                }
            }
        }

        internal int particlesAllocated;
        internal int particlesInUse;

        private float timeToNextEmit = 0.0f;

        // introduced in order to fade water splash below water line.
        internal bool yLimit = false;
        internal float fadeBelow = 0;
        
        private Vector3 localVelocityInGlobalCoords;
        private Vector3 RandomVelocityInGlobalCoords;

        public override void Start()
        {
            base.Start();

            //NonExportedValuesHack();

            if (oneShot)
            {
                //particles.AllocateMemory(MaxEmission);
                int parts = Mathf.FloorToInt(maxEmission);
                particles = new Particle[parts];
                particlesInUse = 0;
                particlesAllocated = parts;
            }
            else
            {
                //particles.AllocateMemory(MaxEmission * (int)Math.Max(1, MaxEnergy * 2));
                particlesAllocated = Mathf.CeilToInt(maxEmission) * (int)Math.Max(1, maxEnergy * 2);
                particles = new Particle[particlesAllocated];
                particlesInUse = 0;
            }
            if (minEmission <= 0)
            {
                minEmission = 1;
            }
            if (maxEmission <= minEmission)
            {
                maxEmission = minEmission;
            }
            if (!oneShot)
            {
                timeToNextEmit = GetNewEmissionTime();
            }
            //localVelocityInGlobalCoords = gameObject.LocalDirection2GlobalDirection(localVelocity);
            //RandomVelocityInGlobalCoords = gameObject.LocalDirection2GlobalDirection(rndVelocity);
            localVelocityInGlobalCoords = localVelocity;
            RandomVelocityInGlobalCoords = rndVelocity;
        }

        //private void NonExportedValuesHack()
        //{
        //    if (gameObject.Name == "particles")
        //    {
        //        ellipsoid.x = 60;
        //        ellipsoid.y = 10;
        //      // gameObject.Position += Vector2.UnitY *5;
                
        //    }
        //    if (gameObject.Name == "lavaParticles")
        //    {

        //        ellipsoid.x = gameObject.GetParent().FindChild("lavaFade").Scale.x/0.16f;
        //        gameObject.Renderer.Layer = 17 ;
        //        minSize *= 2;
        //        maxSize *= 2;
        //    }

        //    if (gameObject.Name == "Part_erruptLava")
        //    {

        //        ellipsoid.x = 14;
        //    }

        //    if (gameObject.Name == "puffWallBreakBig")
        //    {

        //        ellipsoid.x = 2;
        //        ellipsoid.y = 70;
        //    }


        //    if (gameObject.Name == "std_thruster")
        //    {
        //        ellipsoid.x = 0.01f;
        //        minSize = 2;
        //        maxSize = 4;
        //        minEnergy = 0.2f;
        //        maxEnergy = 0.3f;
        //    }

        //    if (gameObject.Name == "prtcle_rockShatter")
        //    {
        //        rndVelocity = new Vector2(50 , 50);
             
        //    }

        //    if (gameObject.Name == "obj_rainCloud_RAYCAST_TEST")
        //    {
        //        minSize = 15;
        //        maxSize = 20;

        //    }

        //    if (gameObject.Name == "toxicParticles")
        //    {
        //        GameObject toxicfade = gameObject.GetParent().FindChild("toxicFade");
        //        if (toxicfade != null && gameObject.GetParent().GetParent().Name != "obj_Moving_toxic")
        //        {
        //            ellipsoid.x = toxicfade.Scale.x / 0.16f;
        //        }
        //        else if (gameObject.GetParent().GetParent().Name == "obj_Moving_toxic")
        //        {
        //            ellipsoid.x = 80;
        //        }
        //        else
        //            ellipsoid.x = 10;

        //        ellipsoid = Tools.RotateVector(ellipsoid, -gameObject.GetParent().Angle);

        //        gameObject.Renderer.Layer = 17;
        //    }

        //    if (gameObject.Name == "pickup_secret_dust" || 
        //        gameObject.Name == "spawn_dust" || 
        //        gameObject.Name == "prtcle_pickupSecret_stars" ||
        //        gameObject.Name == "particle_collapsingBridge" ||
        //        gameObject.Name == "prtcle_rockShatter" 
        //        || gameObject.Name == "Particle System"
        //        || gameObject.Name == "exp1"
        //        || gameObject.Name == "exp2"
        //        || gameObject.Name == "expDrops"
        //        /*
        //        /*gameObject.Name == "particle_red_exp" || 
        //        gameObject.Name == "particle_blue_exp" || 
        //        gameObject.Name == "particle_green_exp"*/)
        //    {
        //        oneShot = true;
        //    }
        //    if (gameObject.Name == "puffWater" || 
        //        gameObject.Name == "puffFountain" ||  
        //        gameObject.Name == "monsterBurb2")
        //    {
        //        oneShot = false;
        //    }
        //    // Hack to make bar on top of fountain in level 2 to behave better.
        //    if (gameObject.Name == "puffWater" && gameObject.GetParent().GetParent().GetParent().Name == "Section 2 (fountain 1)")
        //    {
        //        localVelocity = new Vector2(localVelocity.x, localVelocity.y - 20);
        //    }


        //}

        public void FixedUpdate()
        {
            if (!enabled)
            {
                return;
            }

            int numToEmit = 0;
            if (emit)
            {
                if (oneShot)
                {
                    numToEmit = Mathf.FloorToInt(Random.Range(minEmission, maxEmission));
                    emit = false;
                }
                else
                {
                    while (timeToNextEmit < 0)
                    {
                        numToEmit++;
                        timeToNextEmit += GetNewEmissionTime();
                    }
                    timeToNextEmit -= Time.deltaTime;
                }
            }

            if (numToEmit == 0 && particlesInUse == 0)
            {
                return;
            }

            int particlesToCheck = particlesInUse;
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].Energy > 0)
                {
                    if ((particles[i].Energy -= Time.deltaTime) <= 0)
                    {
                        particlesInUse--;
                    }
                    if (--particlesToCheck <= 0 && numToEmit == 0)
                    {
                        break;
                    }
                }
                else
                {
                    if (numToEmit > 0)
                    {
                        numToEmit--;
                        particlesInUse++;
                        SetNewParticleAt(i);
                    }
                }
            }
        }

        //public override void Update()
        //{
        //    base.Update();
        //    if (!Scene.CurrentScene.drawDebug)
        //        return;
        //    DebugDrawController.Instance.AddLine2Draw(gameObject.Position + Vector2.UnitY * ellipsoid.y / 2, gameObject.Position - Vector2.UnitY * ellipsoid.y / 2);
        //    DebugDrawController.Instance.AddLine2Draw(gameObject.Position + Vector2.UnitX * ellipsoid.x / 2, gameObject.Position - Vector2.UnitX * ellipsoid.x / 2);
        //    DebugDrawController.Instance.AddLine2Draw(gameObject.Position, gameObject.Position + worldVelocity + localVelocityInGlobalCoords);
        //    DebugDrawController.Instance.AddLine2Draw(gameObject.Position, gameObject.Position + RandomVelocityInGlobalCoords);
        //}

        private void SetNewParticleAt(int index)
        {
            particles[index].Energy = particles[index].StartingEnergy = Random.Range(minEnergy, maxEnergy);

            Vector3 pointInUnitSphere = Random.insideUnitSphere;
            particles[index].Position.x = (ellipsoid.x - minEmitterRange.x) * pointInUnitSphere.x;
            particles[index].Position.y = (ellipsoid.y - minEmitterRange.y) * pointInUnitSphere.y;
            particles[index].Position.z = (ellipsoid.z - minEmitterRange.z) * pointInUnitSphere.z;

            Vector3 emitterVelocity = Vector3.zero;
            //if (gameObject.body != null && emitterVelocityScale > 0 && useWorldSpace)
            //{
            //    emitterVelocity = gameObject.body.GetLinearVelocity() * emitterVelocityScale;
            //}
            Vector3 randomVel = new Vector3(Random.Range(-RandomVelocityInGlobalCoords.x, RandomVelocityInGlobalCoords.x),
                                            Random.Range(-RandomVelocityInGlobalCoords.y, RandomVelocityInGlobalCoords.y),
                                            Random.Range(-RandomVelocityInGlobalCoords.z, RandomVelocityInGlobalCoords.z)) / 2;
            particles[index].Velocity = emitterVelocity + worldVelocity + localVelocityInGlobalCoords + randomVel;

            if (useWorldSpace)
            {
                particles[index].Position += gameObject.transform.position;
            }

            particles[index].Size = Random.Range(minSize, maxSize) / 10;
        }

        private float GetNewEmissionTime()
        {
            return Random.Range(1.0f / minEmission, 1.0f / maxEmission);
        }


        internal void ClearParticles()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Energy = 0;
            }
        }

        //public override void ResetSpawned()
        //{
        //    base.ResetSpawned();
        //    if (oneShot)
        //        emit = true;
        //}
    }
}
