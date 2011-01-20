using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Import.Animation
{
    [ContentProcessor(DisplayName = "Animated Non Skinned Model")]
    public class AnimatedModelProcessor : ModelProcessor
    {
        /// <summary>
        /// The main Process method converts an intermediate format content pipeline
        /// NodeContent tree to a ModelContent object with embedded animation data.
        /// </summary>
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            Microsoft.Xna.Framework.Quaternion rotation = Microsoft.Xna.Framework.Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(RotationY), MathHelper.ToRadians(RotationX), MathHelper.ToRadians(RotationZ));
            Matrix m = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(rotation);

            // Reset scane and rotation values so the model processor does not take them into account
            Scale = 1;
            RotationX = 0;
            RotationY = 0;
            RotationZ = 0;

            ModelContent model = base.Process(input, context);

            GameObjectAnimationData skinningData = SkinningHelpers.GetGameObjectAnimationData(input, context);
            skinningData.BakedTransform = m;

            model.Tag = skinningData;
            return model;
        }
    }
}
