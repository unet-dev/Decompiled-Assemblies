using Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace Facepunch.Unity
{
	public static class RenderInfo
	{
		public static void GenerateReport()
		{
			Renderer[] rendererArray = UnityEngine.Object.FindObjectsOfType<Renderer>();
			List<RenderInfo.RendererInstance> rendererInstances = new List<RenderInfo.RendererInstance>();
			Renderer[] rendererArray1 = rendererArray;
			for (int i = 0; i < (int)rendererArray1.Length; i++)
			{
				rendererInstances.Add(RenderInfo.RendererInstance.From(rendererArray1[i]));
			}
			string str = string.Format(string.Concat(UnityEngine.Application.dataPath, "/../RenderInfo-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt"), DateTime.Now);
			File.WriteAllText(str, JsonConvert.SerializeObject(rendererInstances, Formatting.Indented));
			string str1 = string.Concat(UnityEngine.Application.streamingAssetsPath, "/RenderInfo.exe");
			string str2 = string.Concat("\"", str, "\"");
			UnityEngine.Debug.Log(string.Concat("Launching ", str1, " ", str2));
			Process.Start(str1, str2);
		}

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

			public static RenderInfo.RendererInstance From(Renderer renderer)
			{
				RenderInfo.RendererInstance length = new RenderInfo.RendererInstance()
				{
					IsVisible = renderer.isVisible,
					CastShadows = renderer.shadowCastingMode != ShadowCastingMode.Off,
					RecieveShadows = renderer.receiveShadows,
					Enabled = (!renderer.enabled ? false : renderer.gameObject.activeInHierarchy),
					Size = renderer.bounds.size.magnitude
				};
				Bounds bound = renderer.bounds;
				length.Distance = Vector3.Distance(bound.center, Camera.main.transform.position);
				length.MaterialCount = (int)renderer.sharedMaterials.Length;
				length.RenderType = renderer.GetType().Name;
				BaseEntity baseEntity = renderer.gameObject.ToBaseEntity();
				if (!baseEntity)
				{
					length.ObjectName = renderer.transform.GetRecursiveName("");
				}
				else
				{
					length.EntityName = baseEntity.PrefabName;
					if (baseEntity.net != null)
					{
						length.EntityId = baseEntity.net.ID;
					}
				}
				if (renderer is MeshRenderer)
				{
					length.BoneCount = 0;
					MeshFilter component = renderer.GetComponent<MeshFilter>();
					if (component)
					{
						length.ReadMesh(component.sharedMesh);
					}
				}
				if (renderer is SkinnedMeshRenderer)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
					length.ReadMesh(skinnedMeshRenderer.sharedMesh);
					length.UpdateWhenOffscreen = skinnedMeshRenderer.updateWhenOffscreen;
				}
				if (renderer is ParticleSystemRenderer)
				{
					ParticleSystem particleSystem = renderer.GetComponent<ParticleSystem>();
					if (particleSystem)
					{
						length.MeshName = particleSystem.name;
						length.ParticleCount = particleSystem.particleCount;
					}
				}
				return length;
			}

			public void ReadMesh(Mesh mesh)
			{
				if (mesh == null)
				{
					this.MeshName = "<NULL>";
					return;
				}
				this.VertexCount = mesh.vertexCount;
				this.SubMeshCount = mesh.subMeshCount;
				this.BlendShapeCount = mesh.blendShapeCount;
				this.MeshName = mesh.name;
			}
		}
	}
}