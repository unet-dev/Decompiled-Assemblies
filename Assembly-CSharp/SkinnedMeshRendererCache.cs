using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class SkinnedMeshRendererCache
{
	public static Dictionary<Mesh, SkinnedMeshRendererCache.RigInfo> dictionary;

	static SkinnedMeshRendererCache()
	{
		SkinnedMeshRendererCache.dictionary = new Dictionary<Mesh, SkinnedMeshRendererCache.RigInfo>();
	}

	public static void Add(Mesh mesh, SkinnedMeshRendererCache.RigInfo info)
	{
		if (!SkinnedMeshRendererCache.dictionary.ContainsKey(mesh))
		{
			SkinnedMeshRendererCache.dictionary.Add(mesh, info);
		}
	}

	public static SkinnedMeshRendererCache.RigInfo Get(SkinnedMeshRenderer renderer)
	{
		SkinnedMeshRendererCache.RigInfo rigInfo;
		if (!SkinnedMeshRendererCache.dictionary.TryGetValue(renderer.sharedMesh, out rigInfo))
		{
			rigInfo = new SkinnedMeshRendererCache.RigInfo();
			Transform transforms = renderer.rootBone;
			Transform[] transformArrays = renderer.bones;
			if (transforms == null)
			{
				Debug.LogWarning(string.Concat("Renderer without a valid root bone: ", renderer.name, " ", renderer.sharedMesh.name), renderer.gameObject);
				return rigInfo;
			}
			renderer.transform.position = Vector3.zero;
			renderer.transform.rotation = Quaternion.identity;
			renderer.transform.localScale = Vector3.one;
			rigInfo.root = transforms.name;
			rigInfo.rootHash = rigInfo.root.GetHashCode();
			rigInfo.bones = (
				from x in (IEnumerable<Transform>)transformArrays
				select x.name).ToArray<string>();
			rigInfo.boneHashes = (
				from x in rigInfo.bones
				select x.GetHashCode()).ToArray<int>();
			rigInfo.transforms = (
				from x in (IEnumerable<Transform>)transformArrays
				select x.transform.localToWorldMatrix).ToArray<Matrix4x4>();
			rigInfo.rootTransform = renderer.rootBone.transform.localToWorldMatrix;
			SkinnedMeshRendererCache.dictionary.Add(renderer.sharedMesh, rigInfo);
		}
		return rigInfo;
	}

	[Serializable]
	public class RigInfo
	{
		public string root;

		public int rootHash;

		public string[] bones;

		public int[] boneHashes;

		public Matrix4x4[] transforms;

		public Matrix4x4 rootTransform;

		public RigInfo()
		{
		}
	}
}