using System;
using UnityEngine;

public class MeshReplacement : MonoBehaviour
{
	public SkinnedMeshRenderer Female;

	public MeshReplacement()
	{
	}

	internal static void Process(GameObject go, bool IsFemale)
	{
		if (!IsFemale)
		{
			return;
		}
		MeshReplacement[] componentsInChildren = go.GetComponentsInChildren<MeshReplacement>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			MeshReplacement meshReplacement = componentsInChildren[i];
			SkinnedMeshRenderer component = meshReplacement.GetComponent<SkinnedMeshRenderer>();
			component.sharedMesh = meshReplacement.Female.sharedMesh;
			component.rootBone = meshReplacement.Female.rootBone;
			component.bones = meshReplacement.Female.bones;
		}
	}
}