using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class RaycastHitEx
{
	public static Collider GetCollider(this RaycastHit hit)
	{
		if (hit.triangleIndex < 0)
		{
			return hit.collider;
		}
		if (!hit.transform.CompareTag("MeshColliderBatch"))
		{
			return hit.collider;
		}
		MeshColliderBatch component = hit.transform.GetComponent<MeshColliderBatch>();
		if (!component)
		{
			return hit.collider;
		}
		Collider collider = component.LookupCollider(hit.triangleIndex);
		if (collider)
		{
			return collider;
		}
		return hit.collider;
	}

	public static BaseEntity GetEntity(this RaycastHit hit)
	{
		return hit.GetTransform().gameObject.ToBaseEntity();
	}

	public static Rigidbody GetRigidbody(this RaycastHit hit)
	{
		if (hit.triangleIndex < 0)
		{
			return hit.rigidbody;
		}
		if (!hit.transform.CompareTag("MeshColliderBatch"))
		{
			return hit.rigidbody;
		}
		MeshColliderBatch component = hit.transform.GetComponent<MeshColliderBatch>();
		if (!component)
		{
			return hit.rigidbody;
		}
		Rigidbody rigidbody = component.LookupRigidbody(hit.triangleIndex);
		if (rigidbody)
		{
			return rigidbody;
		}
		return hit.rigidbody;
	}

	public static Transform GetTransform(this RaycastHit hit)
	{
		if (hit.triangleIndex < 0)
		{
			return hit.transform;
		}
		if (!hit.transform.CompareTag("MeshColliderBatch"))
		{
			return hit.transform;
		}
		MeshColliderBatch component = hit.transform.GetComponent<MeshColliderBatch>();
		if (!component)
		{
			return hit.transform;
		}
		Transform transforms = component.LookupTransform(hit.triangleIndex);
		if (transforms)
		{
			return transforms;
		}
		return hit.transform;
	}
}