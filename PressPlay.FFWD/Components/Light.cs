using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public enum LightType { Spot, Directional, Point }

    public class Light : Component, IInitializable
    {
        public LightType type;
        public Color color;
        public float intensity;
        public float range;
        public float spotAngle;
        public LayerMask cullingMask;

        internal static List<Light> Lights = new List<Light>(ApplicationSettings.DefaultCapacities.Lights);

        public void Initialize()
        {
            Lights.Add(this);
        }

        protected override void Destroy()
        {
            base.Destroy();
            Lights.Remove(this);
        }

        internal static bool HasLights
        {
            get
            {
                return Lights.Count > 0;
            }
        }

        internal static void EnableLighting(IEffectLights effect)
        {
            int directionalLightIndex = 0;
            for (int i = 0; i < Lights.Count; i++)
            {
                // TODO: We have only made directional lighting!
                Light light = Lights[i];
                switch (light.type)
                {
                    case LightType.Spot:
                        break;
                    case LightType.Directional:
                        switch (directionalLightIndex++)
	                    {
                            case 0:
                                EnableDirectionalLight(effect.DirectionalLight0, light);
                                break;
                            case 1:
                                EnableDirectionalLight(effect.DirectionalLight1, light);
                                break;
                            case 2:
                                EnableDirectionalLight(effect.DirectionalLight2, light);
                                break;
                            default:
                                Debug.LogWarning("We only support up to three directional lights");
                                break;
	                    }
                        break;
                    case LightType.Point:
                        break;
                }
            }
            if (directionalLightIndex == 0)
            {
                effect.DirectionalLight0.Enabled = false;
            }
            if (directionalLightIndex <= 1)
            {
                effect.DirectionalLight1.Enabled = false;
            }
            if (directionalLightIndex <= 2)
            {
                effect.DirectionalLight2.Enabled = false;
            }
        }

        private static void EnableDirectionalLight(DirectionalLight directionalLight, Light light)
        {
            directionalLight.Enabled = true;
            directionalLight.DiffuseColor = light.color * (light.intensity * 2);
            directionalLight.Direction = light.transform.forward;
        }
    }
}
