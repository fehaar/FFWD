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
        [ContentSerializer(Optional=true)]
        public Vector3 ellipsoid;
        [ContentSerializerIgnore]
        public bool oneShot = false;

        [ContentSerializerIgnore]
        public int particleCount
        {
            get
            {
                return particles.Length;
            }
        }


        //[ContentSerializerIgnore]
        //public Vector3 minEmitterRange { get; set; }

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
        
        public override void Start()
        {
            base.Start();

            if (oneShot)
            {
                int parts = Mathf.FloorToInt(maxEmission);
                particles = new Particle[parts];
                particlesInUse = 0;
                particlesAllocated = parts;
            }
            else
            {
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
        }

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
                    particlesToCheck--;
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
                if (particlesToCheck == 0 && numToEmit == 0)
                {
                    break;
                }
            }
        }

        private void SetNewParticleAt(int index)
        {
            particles[index].Energy = particles[index].StartingEnergy = Random.Range(minEnergy, maxEnergy);

            Vector3 pointInUnitSphere = Random.insideUnitSphere;
            particles[index].Position = ellipsoid * pointInUnitSphere.x;

            Vector3 emitterVelocity = Vector3.zero;
            if (gameObject.rigidbody != null && emitterVelocityScale > 0 && useWorldSpace)
            {
                emitterVelocity = gameObject.rigidbody.velocity * emitterVelocityScale;
            }
            Vector3 randomVel = new Vector3(Random.Range(-rndVelocity.x, rndVelocity.x),
                                            Random.Range(-rndVelocity.y, rndVelocity.y),
                                            Random.Range(-rndVelocity.z, rndVelocity.z)) / 2;
            particles[index].Velocity = emitterVelocity + worldVelocity + localVelocity + randomVel;

            if (useWorldSpace)
            {
                particles[index].Position += gameObject.transform.position;
            }

            particles[index].Size = Random.Range(minSize, maxSize);
            particles[index].Color = renderer.material.color;
        }

        private float GetNewEmissionTime()
        {
            return Random.Range(1.0f / minEmission, 1.0f / maxEmission);
        }

        public void ClearParticles()
        {
            if (particles != null)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].Energy = 0;
                }
                particlesInUse = 0;
            }
        }
    }
}
