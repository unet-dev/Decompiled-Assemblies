using System;
using UnityEngine;

public class Model : MonoBehaviour, IPrefabPreProcess
{
	public SphereCollider collision;

	public Transform rootBone;

	public Transform headBone;

	public Transform eyeBone;

	public Animator animator;

	[HideInInspector]
	public Transform[] boneTransforms;

	[HideInInspector]
	public string[] boneNames;

	internal BoneDictionary boneDict;

	internal int skin;

	public Model()
	{
	}

	public void BuildBoneDictionary()
	{
		if (this.boneDict != null)
		{
			return;
		}
		this.boneDict = new BoneDictionary(base.transform, this.boneTransforms, this.boneNames);
	}

	public Transform FindBone(string name)
	{
		this.BuildBoneDictionary();
		Transform transforms = this.rootBone;
		if (string.IsNullOrEmpty(name))
		{
			return transforms;
		}
		transforms = this.boneDict.FindBone(name, true);
		return transforms;
	}

	public Transform FindBone(int hash)
	{
		this.BuildBoneDictionary();
		Transform transforms = this.rootBone;
		if (hash == 0)
		{
			return transforms;
		}
		transforms = this.boneDict.FindBone(hash, true);
		return transforms;
	}

	private Transform FindBoneInternal(string name)
	{
		this.BuildBoneDictionary();
		return this.boneDict.FindBone(name, false);
	}

	public Transform FindClosestBone(Vector3 worldPos)
	{
		Transform transforms = this.rootBone;
		float single = Single.MaxValue;
		for (int i = 0; i < (int)this.boneTransforms.Length; i++)
		{
			Transform transforms1 = this.boneTransforms[i];
			if (transforms1 != null)
			{
				float single1 = Vector3.Distance(transforms1.position, worldPos);
				if (single1 < single)
				{
					transforms = transforms1;
					single = single1;
				}
			}
		}
		return transforms;
	}

	public int GetSkin()
	{
		return this.skin;
	}

	protected void OnEnable()
	{
		this.skin = -1;
	}

	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this == null)
		{
			return;
		}
		if (this.animator == null)
		{
			this.animator = base.GetComponent<Animator>();
		}
		if (this.rootBone == null)
		{
			this.rootBone = base.transform;
		}
		this.boneTransforms = this.rootBone.GetComponentsInChildren<Transform>(true);
		this.boneNames = new string[(int)this.boneTransforms.Length];
		for (int i = 0; i < (int)this.boneTransforms.Length; i++)
		{
			this.boneNames[i] = this.boneTransforms[i].name;
		}
	}
}