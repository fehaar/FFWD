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
        public Color[] colorAnimation;

        private ParticleEmitter emitter;
        private bool hasHadParticles = false;
        private Color FixedColor;

        public override void Awake()
        {
            base.Awake();

            emitter = gameObject.GetComponent<ParticleEmitter>();
        }

        public override void Start()
        {
            base.Start();
            if (!emitter.useWorldSpace)
            {
                //Force = gameObject.LocalDirection2GlobalDirection(Force);
                //RndForce = gameObject.LocalDirection2GlobalDirection(RndForce);
            }
        }

        //public Color[] GetAnimationColors()
        //{
        //    int extraColors = 3;
        //    Color[] colors = new Color[AnimationColors.Length + (AnimationColors.Length - 1) * extraColors];
        //    for (int i = 0; i < AnimationColors.Length-1; i++)
        //    {
        //        int num = (extraColors + 1);
        //        Color c1 = AnimationColors[i];
        //        Color c2 = AnimationColors[i + 1];
        //        colors[num * i] = c1;
        //        for (int j = 1; j < extraColors + 1; j++)
        //        {
        //            colors[num * i + j] =
        //            Microsoft.Xna.Framework.Color.FromNonPremultiplied(
        //                    (int)MathHelper.Lerp(c1.R, c2.R, (float)j / num),
        //                    (int)MathHelper.Lerp(c1.G, c2.G, (float)j / num),
        //                    (int)MathHelper.Lerp(c1.B, c2.B, (float)j / num),
        //                    (int)MathHelper.Lerp(c1.A, c2.A, (float)j / num));
        //        }
        //    }
        //    colors[colors.Length-1] = AnimationColors[AnimationColors.Length-1];
        //    return colors;
        //}

        public void FixedUpdate()
        {
            bool destroy = hasHadParticles;
            int particlesToCheck = emitter.particlesInUse;
            for (int i = 0; i < emitter.particles.Length; i++)
            {
                if (emitter.particles[i].Energy > 0)
                {
                    hasHadParticles = true;
                    destroy = false;
                    emitter.particles[i].Position += emitter.particles[i].Velocity * Time.deltaTime;
                    emitter.particles[i].Velocity *= 1 - ((1 - damping) * Time.deltaTime);
                    Vector3 RandomForce = Random.insideUnitSphere * rndForce / 2;
                    emitter.particles[i].Velocity += (force + RandomForce) * Time.deltaTime;
                    emitter.particles[i].Size += sizeGrow * Time.deltaTime;
                    if (doesAnimateColor)
                    {
                        UpdateParticleColor(ref emitter.particles[i]);
                    }
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
        }

        public void UpdateParticleColor(ref Particle particle)
        {
            float colorScale = 1 - (particle.Energy / particle.StartingEnergy);
            float startIndex = colorScale * 4;
            if (startIndex == 4)
            {
                startIndex = 3;
            }
            colorScale = startIndex - (int)startIndex;
            particle.Color = Color.Lerp(colorAnimation[(int)startIndex], colorAnimation[(int)startIndex + 1], colorScale);
        }

    }
}
