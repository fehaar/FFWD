using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using PressPlay.FFWD.Components;
using System.Reflection;
using PressPlay.FFWD.Attributes;
using Microsoft.Xna.Framework;
using System.Xml;

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
            CreateStaticBatchRenderers(context, idMap);
            PurgeColliders();

            foreach (Component cmp in scene.components)
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
            scene.hasBeenProcessed = true;
            return input;
        }

        private void CreateStaticBatchRenderers(ContentProcessorContext context, Dictionary<int, UnityObject> idMap)
        {
            Dictionary<string, StaticBatchRenderer> staticRenderers = new Dictionary<string, StaticBatchRenderer>();
            foreach (Renderer r in scene.components.Where(c => (c is Renderer)).ToArray())
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
                string key = GetMaterialKey(r.sharedMaterials);
                if (!staticRenderers.ContainsKey(key))
                {
                    GameObject sbGo = new GameObject("Static - " + key);
                    while (idMap.ContainsKey(sbGo.GetInstanceID()))
                    {
                        sbGo.SetNewId(null);
                    }
                    StaticBatchRenderer sbr = sbGo.AddComponent<StaticBatchRenderer>();
                    while (idMap.ContainsKey(sbr.GetInstanceID()))
                    {
                        sbr.SetNewId(null);
                    }
                    staticRenderers[key] = sbr;
                    staticRenderers[key].sharedMaterials = (Material[])r.sharedMaterials.Clone();
                }

                MeshFilter mf = r.gameObject.GetComponent<MeshFilter>();
                if (mf == null)
                {
                    throw new Exception("The statically marked renderer at " + r.gameObject + " does not have a meshfilter.");
                }
                if (mf.meshToRender == null)
                {
                    throw new Exception("The static meshfilter at " + r.gameObject + " does not have a mesh.");
                }
                int id = mf.meshToRender.GetInstanceID();
                Mesh mesh = context.BuildAndLoadAsset<Mesh, Mesh>(new ExternalReference<Mesh>(mf.meshToRender.name + ".xml"), null, null, "XmlImporter");
                if (mesh == null)
                {
                    throw new Exception("The mesh with Id " + id + " was not found in assets. Gotten from " + mf);
                }
                if (staticRenderers[key].AddMesh(mesh, r.transform.world))
                {
                    r.isPartOfStaticBatch = true;
                    mf.mesh = null;
                }
            }

            foreach (KeyValuePair<string, StaticBatchRenderer> sbr in staticRenderers)
            {
              sbr.Value.PrepareQuadTree();
            }

            scene.gameObjects.AddRange(staticRenderers.Values.Select(sbr => sbr.gameObject));
        }

        /// <summary>
        /// This will search for object that have PolygonColliders and purge other colliders on that GameObject
        /// </summary>
        private void PurgeColliders()
        {
            foreach (PolygonCollider pc in scene.components.Where(c => (c is PolygonCollider)).ToArray())
            {
                foreach (Collider item in pc.GetComponents<Collider>())
                {
                    if (item != pc)
                    {
                        pc.gameObject.RemoveComponent(item);
                    }
                }
            }            
        }

        private string GetMaterialKey(Material[] material)
        {
            if (material.Length == 1)
            {
                return material[0].GetInstanceID().ToString();
            }
            StringBuilder sb = new StringBuilder(material[0].GetInstanceID().ToString());
            for (int i = 1; i < material.Length; i++)
            {
                sb.AppendFormat("-{0}", material[i].GetInstanceID());
            }
            return sb.ToString();
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
