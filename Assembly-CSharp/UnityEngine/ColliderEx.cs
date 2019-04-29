using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class ColliderEx
	{
		public static PhysicMaterial GetMaterialAt(this Collider obj, Vector3 pos)
		{
			if (!(obj is TerrainCollider))
			{
				return obj.sharedMaterial;
			}
			return TerrainMeta.Physics.GetMaterial(pos);
		}
	}
}