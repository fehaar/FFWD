using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD.Components
{
    public class ParticleAnimator : Component, IFixedUpdateable
    {
        public bool doesAnimateColor;
        public Vector3 worldRotationAxis;
        public Vector3 localRotationAxis;
        public float sizeGrow;
        public Vector3 rndForce;
        public Vector3 force;
        public float damping;
        public bool autodestruct;

        [ContentSerializer(ElementName="colorAnimation")]
        private Microsoft.Xna.Framework.Color[] _colorAnimation;
        [ContentSerializerIgnore]
        public Color[] colorAnimation
        {
            get
            {
                if (_colorAnimation == null)
                {
                    return null;
                }
                Color[] anim = new Color[_colorAnimation.Length];
                for (int i = 0; i < _colorAnimation.Length; i++)
                {
                    anim[i] = _colorAnimation[i];
                }
                return anim;
            }
            set
            {
                if (value == null)
                {
                    _colorAnimation = null;
                    return;
                }
                _colorAnimation = new Microsoft.Xna.Framework.Color[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    _colorAnimation[i] = value[i];
                }
            }
        }

        private ParticleEmitter emitter;
        private bool hasHadParticles = false;


        public override void Awake()
        {
            base.Awake();

            emitter = gameObject.GetComponent<ParticleEmitter>();
        }

        public void FixedUpdate()
        {
#if DEBUG     
            Application.particleAnimTimer.Start();
#endif
            bool hasDamping = (damping != 1);
            bool hasForces = (force != Vector3.zero || rndForce != Vector3.zero);
            bool hasSizing = (sizeGrow != 0);
            bool hasTangentForces = (emitter.tangentVelocity != Vector3.zero);

            bool destroy = hasHadParticles;
            int particlesToCheck = emitter.particleCount;
            for (int i = 0; i < emitter.particles.Length; i++)
            {
                if (emitter.particles[i].Energy > 0)
                {
                    hasHadParticles = true;
                    destroy = false;
                    // Move particle
                    emitter.particles[i].Position += emitter.particles[i].Velocity * Time.deltaTime;

                    if (hasDamping)
                    {
                        // Apply damping to velocity
                        //emitter.particles[i].Velocity *= (1 - (1 - damping) * 0.015f); // (1 – c ⋅ dt) ⋅ vold hacked to fixed update
                        //emitter.particles[i].Velocity *= (1 - (1 - damping) * Time.deltaTime); // (1 – c ⋅ dt) ⋅ vold
                        emitter.particles[i].Velocity *= Mathf.Pow(damping, Time.deltaTime); // (1 – c ⋅ dt) ⋅ vold
                    }

                    if (hasForces)
                    {
                        // TODO: The forces clearly affect particles too much! Maybe that the order in which we do damping/forces could have significance.
                        Vector3 RandomForce = Random.insideUnitSphere * rndForce / 2;
                        emitter.particles[i].Velocity += (force + RandomForce) * Time.deltaTime;
                    }

                    //if (hasTangentForces)
                    //{
                    //    // Apply tangent forces
                    //    Vector3 v = Vector3.Cross(transform.up, emitter.particles[i].Velocity);
                    //    emitter.particles[i].Velocity += (v * emitter.tangentVelocity) * Time.deltaTime;
                    //}

                    emitter.particles[i].Rotation += Time.deltaTime * emitter.particles[i].RotationSpeed;

                    if (hasSizing)
                    {
                        // Update size of particle
                        emitter.particles[i].Size += sizeGrow * Time.deltaTime;
                    }

                    UpdateParticleColor(ref emitter.particles[i]);

                    if (--particlesToCheck == 0)
                    {
                        break;
                    }
                }
            }

            if (destroy && autodestruct)
            {
                Destroy(gameObject);
            }

#if DEBUG
            Application.particleAnimTimer.Stop();
#endif
        }

        public void UpdateParticleColor(ref Particle particle)
        {
            if (doesAnimateColor)
            {
                float colorScale = 1 - (particle.Energy / particle.StartingEnergy);
                float startIndex  = colorScale * 4;
                int iStartIndex = (int)Mathf.Floor(startIndex);

                if (iStartIndex == 4)
                {
                  iStartIndex = 3;
                }
                colorScale = startIndex - iStartIndex;

                Microsoft.Xna.Framework.Color nonPremul = Microsoft.Xna.Framework.Color.Lerp(_colorAnimation[iStartIndex], _colorAnimation[iStartIndex + 1], colorScale);
                particle.Color = Microsoft.Xna.Framework.Color.FromNonPremultiplied(nonPremul.R, nonPremul.G, nonPremul.B, nonPremul.A);
            }
            else
            {
                particle.Color = renderer.material.color;
            }
        }

    }
}
