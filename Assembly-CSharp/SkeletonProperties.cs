using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Skeleton Properties")]
public class SkeletonProperties : ScriptableObject
{
	public GameObject boneReference;

	[BoneProperty]
	public SkeletonProperties.BoneProperty[] bones;

	[NonSerialized]
	private Dictionary<uint, SkeletonProperties.BoneProperty> quickLookup;

	public SkeletonProperties()
	{
	}

	private void BuildDictionary()
	{
		this.quickLookup = new Dictionary<uint, SkeletonProperties.BoneProperty>();
		SkeletonProperties.BoneProperty[] bonePropertyArray = this.bones;
		for (int i = 0; i < (int)bonePropertyArray.Length; i++)
		{
			SkeletonProperties.BoneProperty boneProperty = bonePropertyArray[i];
			uint num = StringPool.Get(boneProperty.bone.name);
			if (this.quickLookup.ContainsKey(num))
			{
				string str = boneProperty.bone.name;
				string item = this.quickLookup[num].bone.name;
				Debug.LogWarning(string.Concat(new object[] { "Duplicate bone id ", num, " for ", str, " and ", item }));
			}
			else
			{
				this.quickLookup.Add(num, boneProperty);
			}
		}
	}

	public SkeletonProperties.BoneProperty FindBone(uint id)
	{
		if (this.quickLookup == null)
		{
			this.BuildDictionary();
		}
		SkeletonProperties.BoneProperty boneProperty = null;
		if (!this.quickLookup.TryGetValue(id, out boneProperty))
		{
			return null;
		}
		return boneProperty;
	}

	public void OnValidate()
	{
		if (this.boneReference == null)
		{
			Debug.LogWarning("boneReference is null", this);
			return;
		}
		List<SkeletonProperties.BoneProperty> list = this.bones.ToList<SkeletonProperties.BoneProperty>();
		foreach (Transform allChild in this.boneReference.transform.GetAllChildren())
		{
			if (!list.All<SkeletonProperties.BoneProperty>((SkeletonProperties.BoneProperty x) => x.bone != allChild.gameObject))
			{
				continue;
			}
			list.Add(new SkeletonProperties.BoneProperty()
			{
				bone = allChild.gameObject,
				name = new Translate.Phrase("", "")
				{
					token = allChild.name.ToLower(),
					english = allChild.name.ToLower()
				}
			});
		}
		this.bones = list.ToArray();
	}

	[Serializable]
	public class BoneProperty
	{
		public GameObject bone;

		public Translate.Phrase name;

		public HitArea area;

		public BoneProperty()
		{
		}
	}
}