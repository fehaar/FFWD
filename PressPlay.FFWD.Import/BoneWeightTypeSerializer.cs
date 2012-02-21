using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class BoneWeightTypeSerializer : ContentTypeSerializer<BoneWeight>
    {
        protected override BoneWeight Deserialize(IntermediateReader input, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format, BoneWeight existingInstance)
        {
            string[] s = input.Xml.ReadContentAsString().Trim().Split(' ');
            if (s.Length != 8)
            {
                throw new Exception("Too few data items for the BoneWeight");
            }
            BoneWeight bw = new BoneWeight();
            bw.weights.x = Single.Parse(s[0]);
            bw.weights.y = Single.Parse(s[1]);
            bw.weights.z = Single.Parse(s[2]);
            bw.weights.w = Single.Parse(s[3]);
            bw.boneIndex0 = Int32.Parse(s[4]);
            bw.boneIndex1 = Int32.Parse(s[5]);
            bw.boneIndex2 = Int32.Parse(s[6]);
            bw.boneIndex3 = Int32.Parse(s[7]);
            return bw;
        }

        protected override void Serialize(IntermediateWriter output, BoneWeight w, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format)
        {
            output.Xml.WriteString(String.Format("{0} {1} {2} {3} {4} {5} {6} {7}", w.weights.x, w.weights.y, w.weights.z, w.weights.w, w.boneIndex0, w.boneIndex1, w.boneIndex2, w.boneIndex3));
        }
    }
}
