using System;
using System.Collections.Generic;
using UnityEngine;

public class BoneDictionary
{
	public Transform transform;

	public Transform[] transforms;

	public string[] names;

	private Dictionary<string, Transform> nameDict = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);

	private Dictionary<int, Transform> hashDict = new Dictionary<int, Transform>();

	public int Count
	{
		get
		{
			return (int)this.transforms.Length;
		}
	}

	public BoneDictionary(Transform rootBone)
	{
		this.transform = rootBone;
		this.transforms = rootBone.GetComponentsInChildren<Transform>(true);
		this.names = new string[(int)this.transforms.Length];
		for (int i = 0; i < (int)this.transforms.Length; i++)
		{
			Transform transforms = this.transforms[i];
			if (transforms != null)
			{
				this.names[i] = transforms.name;
			}
		}
		this.BuildBoneDictionary();
	}

	public BoneDictionary(Transform rootBone, Transform[] boneTransforms, string[] boneNames)
	{
		this.transform = rootBone;
		this.transforms = boneTransforms;
		this.names = boneNames;
		this.BuildBoneDictionary();
	}

	private void BuildBoneDictionary()
	{
		for (int i = 0; i < (int)this.transforms.Length; i++)
		{
			Transform transforms = this.transforms[i];
			string str = this.names[i];
			int hashCode = str.GetHashCode();
			if (!this.nameDict.ContainsKey(str))
			{
				this.nameDict.Add(str, transforms);
			}
			if (!this.hashDict.ContainsKey(hashCode))
			{
				this.hashDict.Add(hashCode, transforms);
			}
		}
	}

	public Transform FindBone(string name, bool defaultToRoot = true)
	{
		Transform transforms = null;
		if (this.nameDict.TryGetValue(name, out transforms))
		{
			return transforms;
		}
		if (!defaultToRoot)
		{
			return null;
		}
		return this.transform;
	}

	public Transform FindBone(int hash, bool defaultToRoot = true)
	{
		Transform transforms = null;
		if (this.hashDict.TryGetValue(hash, out transforms))
		{
			return transforms;
		}
		if (!defaultToRoot)
		{
			return null;
		}
		return this.transform;
	}
}