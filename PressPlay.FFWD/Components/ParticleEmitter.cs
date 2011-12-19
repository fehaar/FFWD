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
        public delegate void OnParticleCountChanged();
        [ContentSerializerIgnore]
        public OnParticleCountChanged onParticleCountChanged;

        float _minEnergy;
        public float minEnergy
        {
            get { return _minEnergy; }
            set
            {
                _minEnergy = value;
            }
        }
        float _maxEnergy;
        public float maxEnergy
        {
            get { return _maxEnergy; }
            set
            {
                if (value > _maxEnergy)
                {
                    _maxEnergy = value;
                    particles = new Particle[particlesToAllocate()];

                    if (onParticleCountChanged != null)
                    { onParticleCountChanged(); }
                }
                _maxEnergy = value;
            }
        }
        
        
        float timeSinceEmit;
        float _minEmission;
        public float minEmission
        {
            get { return _minEmission; }
            set
            {
                _minEmission = value;
                timeToNextEmit = GetNewEmissionTime();
            }
        }
        float _maxEmissionEver;
        float _maxEmission;
        public float maxEmission
        {
            get { return _maxEmission; }
            set
            {
                //this is sort of hacky...
                //float timeSinceEmit = timeToNextEmit - (1/maxEmission);

                if (value > _maxEmissionEver)
                {
                    _maxEmission = value;
                    _maxEmissionEver = value;

                    Particle[] newParticles = new Particle[particlesToAllocate()];
                    for (int i = 0; i < particles.Length; i++)
                    {
                        if (i >= newParticles.Length) { break; }

                        newParticles[i] = particles[i];
                    }
                    particles = newParticles;

                    if (onParticleCountChanged != null)
                    { onParticleCountChanged(); }
                }
                _maxEmission = value;



                timeToNextEmit = GetNewEmissionTime() - timeSinceEmit;
            }
        }

        public bool emit;
        public float minSize;
        public float maxSize;
        //public float minEnergy;

        //public float minEmission;
        
        

        public float emitterVelocityScale;
        public Vector3 worldVelocity;
        public Vector3 localVelocity;
        public Vector3 rndVelocity;
        public bool useWorldSpace;
        public bool enabled;

        [ContentSerializerIgnore]
        public float minRotationSpeed = 0;
        [ContentSerializerIgnore]
        public float maxRotationSpeed = 0;
        [ContentSerializerIgnore]
        public bool randomRotation = false;
        [ContentSerializerIgnore]
        public int textureTileCountX = 1;
        [ContentSerializerIgnore]
        public int textureTileCountY = 1;

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
            particles = new Particle[particlesToAllocate()];
            particleCount = 0;
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

        internal int particlesToAllocate()
        {
            if (oneShot)
            {
                return Mathf.FloorToInt(maxEmission);
            }
            else
            {
                return Mathf.CeilToInt(maxEmission) * (int)Math.Max(1, maxEnergy * 2);
            }
        }

        public void FixedUpdate()
        {
            if (!enabled)
            {
                return;
            }

#if DEBUG
            Application.particleEmitTimer.Start();
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
                    timeSinceEmit += Time.deltaTime;
                    timeToNextEmit -= Time.deltaTime;
                    while (timeToNextEmit < 0)
                    {
                        numToEmit++;
                        timeToNextEmit += GetNewEmissionTime();
                    }
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
                        EmitNewParticle(ref particles[i]);
                    }
                }
                if (particlesToCheck == 0 && numToEmit == 0)
                {
                    break;
                }
            }

#if DEBUG
            Application.particleEmitTimer.Stop();
#endif
        }

        private void EmitNewParticle(ref Particle particle)
        {
            timeSinceEmit = 0;

            particle.Energy = particle.StartingEnergy = Random.Range(minEnergy, maxEnergy);

            Vector3 pointInUnitSphere = Random.insideUnitSphere;
            particle.Position = Microsoft.Xna.Framework.Vector3.Transform(ellipsoid, transform.rotation) * pointInUnitSphere.x;

            Vector3 velocity = Vector3.zero;
            
            if (useWorldSpace && gameObject.rigidbody != null && emitterVelocityScale > 0)
            {
                velocity += gameObject.rigidbody.velocity * emitterVelocityScale;
            }
            
            if (rndVelocity.x != 0 || rndVelocity.y != 0 || rndVelocity.z != 0)
            {
                velocity += Random.onUnitSphere * rndVelocity;
            }

            if (tangentVelocity.x != 0 || tangentVelocity.y != 0 || tangentVelocity.z != 0)
            {
                velocity += Random.onUnitSphere * tangentVelocity;
            }
            if (localVelocity.x != 0 || localVelocity.y != 0 || localVelocity.z != 0)
            {
                //velocity += (Vector3)(-Microsoft.Xna.Framework.Vector3.Transform(localVelocity, transform.rotation));
                velocity += (Vector3)(Microsoft.Xna.Framework.Vector3.Transform(localVelocity, transform.rotation));
            }

            if (worldVelocity.x != 0 || worldVelocity.y != 0 || worldVelocity.z != 0)
            {
                //velocity += (Vector3)(-Microsoft.Xna.Framework.Vector3.Transform(localVelocity, transform.rotation));
                velocity += worldVelocity;
            }

            particle.Velocity = velocity;

            if (useWorldSpace)
            {
                particle.Position += gameObject.transform.position;
            }

            particle.Size = Random.Range(minSize, maxSize);
            particle.Color = renderer.material.color;

            particle.RotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
            if (randomRotation)
	        {
                particle.Rotation = MathHelper.TwoPi * Random.value;
	        }

            int x = Random.Range(0, textureTileCountX);
            int y = Random.Range(0, textureTileCountY);
            particle.TextureScale = new Vector2(1.0f / textureTileCountX, 1.0f / textureTileCountY);
            particle.TextureOffset = new Vector2(x * particle.TextureScale.x, y * particle.TextureScale.y);
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
