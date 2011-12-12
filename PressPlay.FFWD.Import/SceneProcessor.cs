using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using PressPlay.FFWD.Components;
using System.Reflection;
using PressPlay.FFWD.Attributes;
using Microsoft.Xna.Framework;

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

            // Create static batch renderers
            Dictionary<int, StaticBatchRenderer> staticRenderers = new Dictionary<int, StaticBatchRenderer>();
            foreach (Renderer r in Application.newComponents.Where(c => (c is Renderer) && (c as Renderer).gameObject.isStatic).ToArray())
            {
                Material m = r.material;
                if (!staticRenderers.ContainsKey(m.GetInstanceID()))
                {
                    GameObject sbGo = new GameObject("Static - " + m.name);
                    staticRenderers[m.GetInstanceID()] = sbGo.AddComponent<StaticBatchRenderer>();
                    staticRenderers[m.GetInstanceID()].materials = (Material[])r.materials.Clone();
                }

                MeshFilter mf = r.gameObject.GetComponent<MeshFilter>();
                int id = mf.meshToRender.GetInstanceID();
                staticRenderers[m.GetInstanceID()].AddMesh((Mesh)scene.assets.Where(a => a.GetInstanceID() == id).FirstOrDefault(), r.transform.world);
                r.isPartOfStaticBatch = true;
            }
            input.gameObjects.AddRange(staticRenderers.Values.Select(sbr => sbr.gameObject));

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

            info = tp.GetMethod("Awake");
            if (info != null && info.DeclaringType != typeof(Component))
            {
                scene.hasAwake.Add(tp.Name);
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
                    PurgeGo(rendererGo);
                }
            }
        }

        private void PurgeGo(GameObject go)
        {
            if (go.ComponentCount == 1)
            {
                if (go.transform.childCount > 0)
                {
                    foreach (Transform trans in go.GetComponentsInChildren<Transform>())
                    {
                        if (go.transform != trans)
                        {
                            PurgeGo(trans.gameObject);
                        }
                    }
                    if (go.transform.childCount > 0)
                    {
                        return;
                    }
                }

                if (go.transform.parent == null)
                {
                    scene.gameObjects.Remove(go);
                }
                else
                {
                    go.transform.parent = null;
                }
            }
        }
    }
}
