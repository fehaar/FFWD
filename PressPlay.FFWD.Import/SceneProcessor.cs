using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using PressPlay.FFWD.Components;
using System.Reflection;
using PressPlay.FFWD.Attributes;

namespace PressPlay.FFWD.Import
{
    [ContentProcessor(DisplayName = "FFWD - Scene processor")]
    public class SceneProcessor : ContentProcessor<Scene, Scene>
    {
        private Scene scene;

        private List<Assembly> assembliesUsed = new List<Assembly>();

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
                Type tp = cmp.GetType();
                if (!assembliesUsed.Contains(tp.Assembly))
                {
                    assembliesUsed.Add(tp.Assembly);
                }
            }

            foreach (Assembly ass in assembliesUsed)
            {
                foreach (Type type in ass.GetTypes())
                {
                    AddBehaviourTypeProperties(type);
                    if (type.GetCustomAttributes(typeof(FixReferencesAttribute), true).Length > 0)
                    {
                        scene.fixReferences.Add(type.Name);
                    }
                }
            }

            return input;
        }

        private void AddBehaviourTypeProperties(Type tp)
        {
            MethodInfo info = tp.GetMethod("Update");
            if (info != null && info.DeclaringType != typeof(MonoBehaviour))
            {
                scene.isUpdateable.Add(tp.Name);
            }

            info = tp.GetMethod("LateUpdate");
            if (info != null && info.DeclaringType != typeof(MonoBehaviour))
            {
                scene.isLateUpdateable.Add(tp.Name);
            }

            info = tp.GetMethod("FixedUpdate");
            if (info != null && info.DeclaringType != typeof(MonoBehaviour))
            {
                scene.isFixedUpdateable.Add(tp.Name);
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
