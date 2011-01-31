using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD.Import
{
    [ContentProcessor(DisplayName = "FFWD - Scene processor")]
    public class SceneProcessor : ContentProcessor<Scene, Scene>
    {
        private Scene scene;

        public override Scene Process(Scene input, ContentProcessorContext context)
        {
            input.AfterLoad(null);
            scene = input;

            foreach (GameObject go in input.gameObjects)
            {
                PurgeStaticallyBatchedRenderers(go);
            }

            return input;
        }

        private void PurgeStaticallyBatchedRenderers(GameObject go)
        {
            StaticBatchRenderer renderer = go.GetComponent<StaticBatchRenderer>();
            if (renderer != null)
            {
                MeshRenderer[] renderers = renderer.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    GameObject rendererGo = renderers[i].gameObject;
                    rendererGo.RemoveComponent(renderers[i]);
                    MeshFilter filter = rendererGo.GetComponent<MeshFilter>();
                    if (filter != null)
                    {
                        rendererGo.RemoveComponent(filter);
                    }
                    if (rendererGo.ComponentCount == 1)
                    {
                        if (rendererGo.transform.parent == null)
                        {
                            scene.gameObjects.Remove(rendererGo);
                        }
                        else
                        {
                            rendererGo.transform.parent = null;
                        }
                    }
                }
            }
        }
    }
}
