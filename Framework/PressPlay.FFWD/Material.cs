using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Extensions;

namespace PressPlay.FFWD
{
    public class Material : Asset
    {
        [ContentSerializer(ElementName="shader")]
        internal string shaderName;
        [ContentSerializer]
        public int renderQueue;
        [ContentSerializer(Optional = true)]
        public Color color = Color.white;
        [ContentSerializer(Optional = true)]
        public float cutOff = 0.5f;
        [ContentSerializer(Optional = true)]
        public Texture2D mainTexture;
        [ContentSerializer(Optional = true)]
        public Vector2 mainTextureOffset = Vector2.zero;
        [ContentSerializer(Optional = true)]
        public Vector2 mainTextureScale = Vector2.one;
        [ContentSerializer(Optional = true)]
        internal bool wrapRepeat;

        private static readonly Dictionary<string, int> textureRenderIndexes = new Dictionary<string, int>();
        private Dictionary<string, Texture> textures;
        internal Shader shader;

        public void SetColor(string name, Color color)
        {
            this.color = color;
        }

        public Texture GetTexture(string propertyName)
        {
            if (propertyName == "_MainTex")
            {
                return mainTexture;
            }
            if (textures.HasElements() && textures.ContainsKey(propertyName))
            {
                return textures[propertyName];
            }
            return null;
        }

        public void SetTexture(string propertyName, Texture texture)
        {
            if (propertyName == "_MainTex")
            {
                mainTexture = (Texture2D)texture;
            }
            if (textures == null)
            {
                textures = new Dictionary<string, Texture>(1);
            }
            textures[propertyName] = texture;
        }

        public Vector2 GetTextureOffset(string propertyName)
        {
            if (propertyName == "_MainTex")
            {
                return mainTextureOffset;
            }
            // TODO: We should do this properly
            return Vector2.zero;
        }

        public void SetTextureOffset(string propertyName, Vector2 offset)
        {
            if (propertyName == "_MainTex")
            {
                mainTextureOffset = offset;
            }
            // TODO: We should set this properly
        }

        protected override void DoLoadAsset(AssetHelper assetHelper)
        {
            shader = Shader.GetShader(this);

            // NOTE: We have hardcoded shader values here that should be configurable in some other way
            blendState = BlendState.Opaque;
            if (shaderName == "iPhone/Particles/Additive Culled" || shaderName == "Particles/Additive")
            {
                blendState = BlendState.Additive;
            }
            else if (renderQueue == 3000 || (shaderName ?? "").StartsWith("Trans") || shaderName == "Particles/VertexLit Blended" || shaderName == "Particles/Alpha Blended")
            {
                blendState = BlendState.AlphaBlend;
            }
            if (shaderName == "Particles/Multiply (Double)")
            {
                color = new Color(color.r, color.g, color.b, 0.5f);
            }
            //if (name == "Default-Particle")
            //{
            //    color = Color.white;
            //}

            CalculateRenderQueue();
        }

        [ContentSerializerIgnore]
        public BlendState blendState { get; private set; }

        internal void SetBlendState(GraphicsDevice device)
        {
            if (device.BlendState != blendState)
            {
                device.BlendState = blendState;
            }
            if (IsTransparent())
            {
                device.DepthStencilState = DepthStencilState.DepthRead;
            }
            else
            {
                device.DepthStencilState = DepthStencilState.Default;
            }

            if (wrapRepeat)
            {
                device.SamplerStates[0] = SamplerState.LinearWrap;
            }
            else
            {
                device.SamplerStates[0] = SamplerState.LinearClamp;
            }
        }

        internal bool IsTransparent()
        {
            string sh = (shaderName ?? "");
            // HACK: This is a very bad way of making sure the right things are transparent. The need for this will go away with the new rendering system.
            // It is needed because a single Renderer can draw multiple materials but not in the right order.
            if (sh.Contains("Cutout"))
	        {
                return !name.Contains("Bonsai");
	        }
            return (renderQueue == 3000 || sh.StartsWith("Trans"));
        }

        internal float finalRenderQueue = float.MinValue;

        internal void CalculateRenderQueue()
        {
            finalRenderQueue = renderQueue * 10;
            if (blendState == BlendState.AlphaBlend)
            {
                finalRenderQueue += 1000f;
            }
            if (blendState == BlendState.Additive)
            {
                finalRenderQueue += 2000f;
            }
            string texName = (mainTexture == null) ? string.Empty : mainTexture.name;
            if (!textureRenderIndexes.ContainsKey(texName))
            {
                textureRenderIndexes.Add(texName, textureRenderIndexes.Count);
            }
            finalRenderQueue += textureRenderIndexes[texName];
        }

        public static readonly Material Default = new Material() { name = "DefaultDiffuse", shaderName = "Diffuse" };

        internal void SetTextureState(BasicEffect basicEffect)
        {
            if (mainTexture != null)
            {
                basicEffect.TextureEnabled = true;
                basicEffect.Texture = mainTexture;
                basicEffect.Alpha = color.a;
            }
            else
            {
                basicEffect.TextureEnabled = false;
                basicEffect.Alpha = color.a;
            }
            basicEffect.DiffuseColor = color;
            basicEffect.Alpha = color.a;
        }

        public static bool operator ==(Material value1, Material value2)
        {
            if ((object)value1 == null)
            {
                return ((object)value2 == null);
            }
            if ((object)value2 == null)
            {
                return ((object)value1 == null);
            }
            return value1.GetInstanceID() == value2.GetInstanceID();
        }

        public static bool operator !=(Material value1, Material value2)
        {
            if ((object)value1 == null)
            {
                return ((object)value2 != null);
            }
            if ((object)value2 == null)
            {
                return ((object)value1 != null);
            }
            return value1.GetInstanceID() != value2.GetInstanceID();
        }

        public override bool Equals(object obj)
        {
            if (obj is Material)
            {
                return this == (Material)obj;
            }
            return base.Equals(obj);
        }

        internal static void LoadRenderIndices(AssetHelper helper)
        {
            textureRenderIndexes.Clear();
            helper.AddStaticAsset("TextureRenderIndexes");
            string[] names = helper.Load<String[]>("TextureRenderIndexes");
            if (names != null)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    textureRenderIndexes.Add(names[i], i);
                }
            }
        }
    }
}
