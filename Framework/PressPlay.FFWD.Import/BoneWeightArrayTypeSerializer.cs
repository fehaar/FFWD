using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class BoneWeightArrayTypeSerializer : ContentTypeSerializer<BoneWeight[]>
    {
        protected override BoneWeight[] Deserialize(IntermediateReader input, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format, BoneWeight[] existingInstance)
        {
            string[] s = input.Xml.ReadContentAsString().Trim().Split(' ');
            if (s.Length > 0 && s.Length % 8 != 0)
            {
                throw new Exception("Too few data items for the BoneWeight");
            }
            BoneWeight[] boneWeights = new BoneWeight[(int)(s.Length / 8)];
            int index = 0;
            for (int i = 0; i < s.Length; i+=8, index++)
            {
                BoneWeight bw = new BoneWeight();
                bw.weights.x = Single.Parse(s[i]);
                bw.weights.y = Single.Parse(s[i + 1]);
                bw.weights.z = Single.Parse(s[i + 2]);
                bw.weights.w = Single.Parse(s[i + 3]);
                bw.boneIndex0 = Int32.Parse(s[i + 4]);
                bw.boneIndex1 = Int32.Parse(s[i + 5]);
                bw.boneIndex2 = Int32.Parse(s[i + 6]);
                bw.boneIndex3 = Int32.Parse(s[i + 7]);
                boneWeights[index] = bw;
            }
            return boneWeights;
        }

        protected override void Serialize(IntermediateWriter output, BoneWeight[] w, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format)
        {
            //output.Xml.WriteString(String.Format("{0} {1} {2} {3} {4} {5} {6} {7}", w.weights.X, w.weights.Y, w.weights.Z, w.weights.W, w.boneIndex0, w.boneIndex1, w.boneIndex2, w.boneIndex3));
        }
    }
}
