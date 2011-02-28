using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using PressPlay.FFWD.Components;
using System.Reflection;

namespace PressPlay.FFWD.Import
{
    [ContentProcessor(DisplayName = "FFWD - Scene processor")]
    public class SceneProcessor : ContentProcessor<Scene, Scene>
    {
        private Scene scene;

        private HashSet<Type> hasProcessedType = new HashSet<Type>();

        public override Scene Process(Scene input, ContentProcessorContext context)
        {
            input.AfterLoad(null);
            scene = input;

            foreach (GameObject go in input.gameObjects)
            {
                PurgeStaticallyBatchedRenderers(go);
            }

            foreach (Component cmp in Application.newComponents)
            {
                AddBehaviourTypeProperties(cmp);
            }

            return input;
        }

        private void AddBehaviourTypeProperties(Component cmp)
        {
            Type tp = cmp.GetType();
            if (!hasProcessedType.Contains(tp))
            {
                hasProcessedType.Add(tp);
                MethodInfo info = tp.GetMethod("Update");
                if (info != null && info.DeclaringType != typeof(MonoBehaviour))
                {
                    scene.isUpdateable.Add(tp.AssemblyQualifiedName);
                }

                info = tp.GetMethod("LateUpdate");
                if (info != null && info.DeclaringType != typeof(MonoBehaviour))
                {
                    scene.isLateUpdateable.Add(tp.AssemblyQualifiedName);
                }

                info = tp.GetMethod("FixedUpdate");
                if (info != null && info.DeclaringType != typeof(MonoBehaviour))
                {
                    scene.isFixedUpdateable.Add(tp.AssemblyQualifiedName);
                }
            }
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
