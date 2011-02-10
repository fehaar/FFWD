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

        // NOTE: These values are not accessible in Unity, therefore we have to deal with them ourselves
        [ContentSerializer(Optional=true)]
        internal Vector3 ellipsoid;
        [ContentSerializer(Optional = true)]
        internal bool oneShot = false;
        [ContentSerializer(Optional = true)]
        internal Vector3 tangentVelocity;
        [ContentSerializer(Optional = true)]
        internal float minEmitterRange;

        [ContentSerializerIgnore]
        public int particleCount
        {
            get;
            private set;
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
                particleCount = 0;
                for (int i = 0; i < _particles.Length; i++)
                {
                    if (_particles[i].Energy > 0)
                    {
                        particleCount++;
                    }
                }
            }
        }

        private float timeToNextEmit = 0.0f;

        // introduced in order to fade water splash below water line.
        internal bool yLimit = false;
        internal float fadeBelow = 0;
        
        public override void Awake()
        {
            if (oneShot)
            {
                int parts = Mathf.FloorToInt(maxEmission);
                particles = new Particle[parts];
                particleCount = 0;
            }
            else
            {
                int parts = Mathf.CeilToInt(maxEmission) * (int)Math.Max(1, maxEnergy * 2);
                particles = new Particle[parts];
                particleCount = 0;
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

#if DEBUG
            Application.particleAnimTimer.Start();
#endif


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

            if (numToEmit == 0 && particleCount == 0)
            {
                return;
            }

            int particlesToCheck = particleCount;
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].Energy > 0)
                {
                    if ((particles[i].Energy -= Time.deltaTime) <= 0 || (particles[i].Size < 0))
                    {
                        particles[i].Energy = 0;
                        particleCount--;
                    }
                    particlesToCheck--;
                }
                else
                {
                    if (numToEmit > 0)
                    {
                        numToEmit--;
                        particleCount++;
                        EmitNewParticleAt(i);
                    }
                }
                if (particlesToCheck == 0 && numToEmit == 0)
                {
                    break;
                }
            }

#if DEBUG
            Application.particleAnimTimer.Stop();
#endif
        }

        private void EmitNewParticleAt(int index)
        {
            particles[index].Energy = particles[index].StartingEnergy = Random.Range(minEnergy, maxEnergy);

            Vector3 pointInUnitSphere = Random.insideUnitSphere;
            particles[index].Position = Microsoft.Xna.Framework.Vector3.Transform(ellipsoid, transform.rotation) * pointInUnitSphere.x;

            Vector3 emitterVelocity = Vector3.zero;
            if (gameObject.rigidbody != null && emitterVelocityScale > 0 && useWorldSpace)
            {
                emitterVelocity = gameObject.rigidbody.velocity * emitterVelocityScale;
            }
            Vector3 randomVel = new Vector3(Random.Range(-rndVelocity.x, rndVelocity.x),
                                            Random.Range(-rndVelocity.y, rndVelocity.y),
                                            Random.Range(-rndVelocity.z, rndVelocity.z)) / 2;

            Vector3 emitterTangentVelocity = Random.onUnitSphere * tangentVelocity;

            Vector3 localVel = -Microsoft.Xna.Framework.Vector3.Transform(localVelocity, transform.rotation);
            particles[index].Velocity = emitterVelocity + worldVelocity + localVel + randomVel + emitterTangentVelocity;

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
                particleCount = 0;
            }
        }

        public void SetEllipsoid(Vector3 _ellipsoid)
        {
            ellipsoid = _ellipsoid;
        }
    }
}
