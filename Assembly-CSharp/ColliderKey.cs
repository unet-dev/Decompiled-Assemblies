using System;
using UnityEngine;

public struct ColliderKey : IEquatable<ColliderKey>
{
	public PhysicMaterial material;

	public int layer;

	public ColliderKey(PhysicMaterial material, int layer)
	{
		this.material = material;
		this.layer = layer;
	}

	public ColliderKey(Collider collider)
	{
		this.material = collider.sharedMaterial;
		this.layer = collider.gameObject.layer;
	}

	public ColliderKey(ColliderBatch batch)
	{
		this.material = batch.BatchCollider.sharedMaterial;
		this.layer = batch.BatchCollider.gameObject.layer;
	}

	public override bool Equals(object other)
	{
		if (!(other is ColliderKey))
		{
			return false;
		}
		return this.Equals((ColliderKey)other);
	}

	public bool Equals(ColliderKey other)
	{
		if (this.material != other.material)
		{
			return false;
		}
		return this.layer == other.layer;
	}

	public override int GetHashCode()
	{
		return this.material.GetHashCode() ^ this.layer.GetHashCode();
	}
}