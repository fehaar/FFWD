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

        public Color GetParticleColor(int index)
        {
            if (doesAnimateColor)
            {
                // PERF: We can optimize this further by pregenerating a number of color samples and just find the correct index according to time
                float colorScale = 1 - (emitter.particles[index].Energy / emitter.particles[index].StartingEnergy);
                float startIndex = colorScale * 4;
                if (startIndex == 4)
                {
                    startIndex = 3;
                }
                colorScale = startIndex - (int)startIndex;
                Color c1 = colorAnimation[(int)startIndex];
                Color c2 = colorAnimation[(int)startIndex + 1];
                return Microsoft.Xna.Framework.Color.FromNonPremultiplied(
                        (int)MathHelper.Lerp(c1.R, c2.R, colorScale),
                        (int)MathHelper.Lerp(c1.G, c2.G, colorScale),
                        (int)MathHelper.Lerp(c1.B, c2.B, colorScale),
                        (int)MathHelper.Lerp(c1.A, c2.A, colorScale)
                    );
            }
            else
            {
                return FixedColor;
            }
        }

        //public override void Update()
        //{
        //    base.Update();
        //    if (!Scene.CurrentScene.drawDebug) return;
        //    Vector2 arrowDir1 = new Vector2(Force.y, -Force..x) - Force;
        //    Vector2 arrowDir2 = new Vector2(-Force.y, Force..x) - Force;
        //    arrowDir1.Normalize();
        //    arrowDir2.Normalize();
        //    DebugDrawController.Instance.AddLine2Draw(gameObject.Position, gameObject.Position + Force);
        //    DebugDrawController.Instance.AddLine2Draw(gameObject.Position + Force, gameObject.Position + Force + arrowDir1 * 5);
        //    DebugDrawController.Instance.AddLine2Draw(gameObject.Position + Force, gameObject.Position + Force + arrowDir2 * 5);
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
    }
}
