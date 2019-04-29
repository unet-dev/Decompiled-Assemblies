using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public static class GameObjectExtensions
	{
		public static void SetIgnoreCollisions(this GameObject self, GameObject other, bool ignore)
		{
			Collider[] componentsInChildren = self.GetComponentsInChildren<Collider>(true);
			Collider[] colliderArray = other.GetComponentsInChildren<Collider>(true);
			Collider[] colliderArray1 = componentsInChildren;
			for (int i = 0; i < (int)colliderArray1.Length; i++)
			{
				Collider collider = colliderArray1[i];
				Collider[] colliderArray2 = colliderArray;
				for (int j = 0; j < (int)colliderArray2.Length; j++)
				{
					Physics.IgnoreCollision(collider, colliderArray2[j], ignore);
				}
			}
		}
	}
}