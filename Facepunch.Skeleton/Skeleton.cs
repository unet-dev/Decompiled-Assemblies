using System;
using UnityEngine;

namespace Facepunch
{
	[AddComponentMenu("Facepunch/Skeleton")]
	public class Skeleton : MonoBehaviour
	{
		public SkeletonDefinition Source;

		public GameObject[] Bones;

		public Skeleton()
		{
		}

		public void CopyPositions(Skeleton to)
		{
			if (to.Source != this.Source)
			{
				throw new ArgumentException("Trying to copy transforms between different skeletons");
			}
			if ((int)to.Bones.Length != (int)this.Bones.Length)
			{
				throw new ArgumentException("Bone arrays are different sizes, skeleton might need rebuilding");
			}
			for (int i = 0; i < (int)this.Bones.Length; i++)
			{
				if (!(this.Bones[i] == null) && !(to.Bones[i] == null))
				{
					to.Bones[i].transform.SetPositionAndRotation(this.Bones[i].transform.position, this.Bones[i].transform.rotation);
				}
			}
		}

		public GameObject GetGameObject(int id)
		{
			if (id < 0)
			{
				return null;
			}
			return this.Bones[id];
		}

		public Transform GetTransform(int id)
		{
			if (id < 0)
			{
				return null;
			}
			return this.Bones[id].transform;
		}
	}
}