using System;
using System.Collections.Generic;
#if UNITY_5
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;
#endif

namespace Facepunch.Unity
{
    public static class RenderInfo
    {
        public struct RendererInstance
        {
            public bool IsVisible;
            public bool CastShadows;
            public bool Enabled;
            public bool RecieveShadows;
            public float Size;
            public float Distance;
            public int BoneCount;
            public int MaterialCount;
            public int VertexCount;
            public int TriangleCount;
            public int SubMeshCount;
            public int BlendShapeCount;
            public string RenderType;
            public string MeshName;
            public string ObjectName;
            public string EntityName;
            public uint EntityId;
            public bool UpdateWhenOffscreen;
            public int ParticleCount;

#if UNITY_5

            public static RendererInstance From( Renderer renderer )
            {
                var r = new RendererInstance();

                r.IsVisible = renderer.isVisible;
                r.CastShadows = renderer.shadowCastingMode != ShadowCastingMode.Off;
                r.RecieveShadows = renderer.receiveShadows;
                r.Enabled = renderer.enabled && renderer.gameObject.activeInHierarchy;
                r.Size = renderer.bounds.size.magnitude;
                r.Distance = Vector3.Distance( renderer.bounds.center, Camera.main.transform.position );
                r.MaterialCount = renderer.materials.Length;
                r.RenderType = renderer.GetType().Name;

                var ent = renderer.gameObject.ToBaseEntity();
                if ( ent )
                {
                    r.EntityName = ent.PrefabName;

                    if ( ent.net != null )
                        r.EntityId = ent.net.ID;
                }
                else
                {
                    r.ObjectName = renderer.transform.GetRecursiveName();
                }

                if ( renderer is MeshRenderer )
                {
                    r.BoneCount = 0;
                    var filter = renderer.GetComponent<MeshFilter>();
                    if ( filter )
                    {
                        r.ReadMesh( filter.sharedMesh );
                    }
                }

                if ( renderer is SkinnedMeshRenderer )
                {
                    var smr = renderer as SkinnedMeshRenderer;
                    r.ReadMesh( smr.sharedMesh );
                    r.UpdateWhenOffscreen = smr.updateWhenOffscreen;
                }

                if ( renderer is ParticleSystemRenderer )
                {
                    var ps = renderer.GetComponent<ParticleSystem>();
                    if ( ps )
                    {
                        r.MeshName = ps.name;
                        r.ParticleCount = ps.particleCount;
                    }
                }

                return r;
            }

            public void ReadMesh( Mesh mesh )
            {
                if ( mesh == null )
                {
                    MeshName = "<NULL>";
                    return;
                }

                VertexCount = mesh.vertexCount;
                SubMeshCount = mesh.subMeshCount;
                BlendShapeCount = mesh.blendShapeCount;
                MeshName = mesh.name;
            }
#endif
        }

#if UNITY_5
        public static void GenerateReport()
        {
            var renderers = GameObject.FindObjectsOfType<Renderer>();
            var list = new List<RendererInstance>();

            foreach ( var r in renderers )
            {
                var ri = RendererInstance.From( r );
                list.Add( ri );
            }

            var filename = string.Format( Application.dataPath + "/../RenderInfo-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now );
            var json = Newtonsoft.Json.JsonConvert.SerializeObject( list, Formatting.Indented );
            System.IO.File.WriteAllText( filename, json );

#if UNITY_STANDALONE_WIN
            System.Diagnostics.Process.Start( Application.streamingAssetsPath + "/RenderInfo.exe", filename );
#endif

        }
#endif
    }
}