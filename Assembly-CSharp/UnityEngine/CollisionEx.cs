using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class CollisionEx
	{
		public static BaseEntity GetEntity(this Collision col)
		{
			RaycastHit raycastHit;
			if (col.transform.CompareTag("MeshColliderBatch") && col.gameObject.GetComponent<MeshColliderBatch>())
			{
				for (int i = 0; i < (int)col.contacts.Length; i++)
				{
					ContactPoint contactPoint = col.contacts[i];
					Ray ray = new Ray(contactPoint.point + (contactPoint.normal * 0.01f), -contactPoint.normal);
					if (col.collider.Raycast(ray, out raycastHit, 1f))
					{
						return raycastHit.GetEntity();
					}
				}
			}
			return col.gameObject.ToBaseEntity();
		}
	}
}