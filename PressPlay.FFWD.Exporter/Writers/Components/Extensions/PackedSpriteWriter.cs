using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Reflection;

namespace PressPlay.FFWD.Exporter.Writers.Components.Extensions
{
    public class PackedSpriteWriter : MonoBehaviourWriter
    {
        public PackedSpriteWriter()
        {
            filterIsExclude = false;
            memberFilter.AddRange(new string[] {
                "drawLayer",
                "width",
                "height",
                "offset",
                "color",
                "pixelsPerUV"
            });
            exportScript = false;
        }

        public override void Write(SceneWriter scene, object component)
        {
            base.Write(scene, component);
            MeshRenderer renderer = (component as Component).renderer as MeshRenderer;
            if (renderer == null)
            {
                throw new Exception("PackedSpriteWriter needs a MeshRenderer on the same object");
            }
            scene.WriteTexture(renderer.sharedMaterial.mainTexture);
        }
    }
}
