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
            Dictionary<int, UnityObject> idMap = new Dictionary<int, UnityObject>();
            input.AfterLoad(idMap);
            scene = input;

            // Create static batch renderers
            Dictionary<int, StaticBatchRenderer> staticRenderers = new Dictionary<int, StaticBatchRenderer>();
            foreach (Renderer r in Application.newComponents.Where(c => (c is Renderer)).ToArray())
            {
                if (r.gameObject == null)
                {
                    Debug.LogError("The gameObject of " + r + " has vanished?!");
                    continue;
                }
                if (!r.gameObject.isStatic)
                {
                    continue;
                }
                Material m = r.material;
                if (!staticRenderers.ContainsKey(m.GetInstanceID()))
                {
                    GameObject sbGo = new GameObject("Static - " + m.name);
                    while (idMap.ContainsKey(sbGo.GetInstanceID()))
                    {
                        sbGo.SetNewId(null);
                    }
                    StaticBatchRenderer sbr = sbGo.AddComponent<StaticBatchRenderer>();
                    while (idMap.ContainsKey(sbr.GetInstanceID()))
                    {
                        sbr.SetNewId(null);
                    }
                    staticRenderers[m.GetInstanceID()] = sbr;
                    staticRenderers[m.GetInstanceID()].materials = (Material[])r.materials.Clone();
                }

                MeshFilter mf = r.gameObject.GetComponent<MeshFilter>();
                int id = mf.meshToRender.GetInstanceID();
                Mesh mesh = (Mesh)scene.assets.Where(a => a.GetInstanceID() == id).FirstOrDefault();
                if (mesh == null)
                {
                    throw new Exception("The mesh with Id " + id + " was not found in assets. Gotten from " + mf);
                }
                if (staticRenderers[m.GetInstanceID()].AddMesh(mesh, r.transform.world))
                {
                    r.isPartOfStaticBatch = true;
                }
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

            TypeSet tc = new TypeSet();
            foreach (Assembly ass in assembliesUsed)
            {
                foreach (Type type in ass.GetTypes())
                {
                    tc.Add(type);
                }
            }
            scene.typeCaps = tc.ToList();
            Application.Reset();
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
