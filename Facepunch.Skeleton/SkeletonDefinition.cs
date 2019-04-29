using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	[CreateAssetMenu(fileName="Skeleton", menuName="Facepunch/Skeleton Definition")]
	public class SkeletonDefinition : ScriptableObject
	{
		public GameObject SourceObject;

		public SkeletonDefinition.Bone[] Bones;

		public SkeletonDefinition()
		{
		}

		public SkeletonDefinition.Bone FindBone(string name)
		{
			return (
				from x in this.Bones
				where string.Compare(name, x.Name, true) == 0
				select x).FirstOrDefault<SkeletonDefinition.Bone>();
		}

		[Serializable]
		public struct Bone
		{
			public int Id;

			public int Depth;

			public int Parent;

			public string Name;

			public GameObject Target;

			public BoneFlag Flags;
		}
	}
}