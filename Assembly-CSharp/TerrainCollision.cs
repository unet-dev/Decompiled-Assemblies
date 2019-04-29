using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCollision : TerrainExtension
{
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	private TerrainCollider terrainCollider;

	public TerrainCollision()
	{
	}

	public void Clear()
	{
		if (!this.terrainCollider)
		{
			return;
		}
		foreach (Collider key in this.ignoredColliders.Keys)
		{
			Physics.IgnoreCollision(key, this.terrainCollider, false);
		}
		this.ignoredColliders.Clear();
	}

	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		return GamePhysics.CheckSphere<TerrainCollisionTrigger>(pos, radius, 262144, QueryTriggerInteraction.Collide);
	}

	public bool GetIgnore(RaycastHit hit)
	{
		if (!(hit.collider is TerrainCollider))
		{
			return false;
		}
		return this.GetIgnore(hit.point, 0.01f);
	}

	public bool GetIgnore(Collider collider)
	{
		if (!this.terrainCollider || !collider)
		{
			return false;
		}
		return this.ignoredColliders.Contains(collider);
	}

	protected void LateUpdate()
	{
		if (this.ignoredColliders == null)
		{
			return;
		}
		for (int i = 0; i < this.ignoredColliders.Count; i++)
		{
			KeyValuePair<Collider, List<Collider>> byIndex = this.ignoredColliders.GetByIndex(i);
			Collider key = byIndex.Key;
			List<Collider> value = byIndex.Value;
			if (key == null)
			{
				int num = i;
				i = num - 1;
				this.ignoredColliders.RemoveAt(num);
			}
			else if (value.Count == 0)
			{
				Physics.IgnoreCollision(key, this.terrainCollider, false);
				int num1 = i;
				i = num1 - 1;
				this.ignoredColliders.RemoveAt(num1);
			}
		}
	}

	public void Reset(Collider collider)
	{
		if (!this.terrainCollider || !collider)
		{
			return;
		}
		Physics.IgnoreCollision(collider, this.terrainCollider, false);
		this.ignoredColliders.Remove(collider);
	}

	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (!this.terrainCollider || !collider)
		{
			return;
		}
		if (this.GetIgnore(collider))
		{
			List<Collider> item = this.ignoredColliders[collider];
			if (ignore)
			{
				if (!item.Contains(trigger))
				{
					item.Add(trigger);
					return;
				}
			}
			else if (item.Contains(trigger))
			{
				item.Remove(trigger);
			}
		}
		else if (ignore)
		{
			List<Collider> colliders = new List<Collider>()
			{
				trigger
			};
			Physics.IgnoreCollision(collider, this.terrainCollider, true);
			this.ignoredColliders.Add(collider, colliders);
			return;
		}
	}

	public override void Setup()
	{
		this.ignoredColliders = new ListDictionary<Collider, List<Collider>>(8);
		this.terrainCollider = this.terrain.GetComponent<TerrainCollider>();
	}
}