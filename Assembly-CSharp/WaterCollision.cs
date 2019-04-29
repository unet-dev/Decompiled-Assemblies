using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	private HashSet<Collider> waterColliders;

	public WaterCollision()
	{
	}

	private void Awake()
	{
		this.ignoredColliders = new ListDictionary<Collider, List<Collider>>(8);
		this.waterColliders = new HashSet<Collider>();
	}

	public void Clear()
	{
		if (this.waterColliders.Count == 0)
		{
			return;
		}
		HashSet<Collider>.Enumerator enumerator = this.waterColliders.GetEnumerator();
		while (enumerator.MoveNext())
		{
			foreach (Collider key in this.ignoredColliders.Keys)
			{
				Physics.IgnoreCollision(key, enumerator.Current, false);
			}
		}
		this.ignoredColliders.Clear();
	}

	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		return GamePhysics.CheckSphere<WaterVisibilityTrigger>(pos, radius, 262144, QueryTriggerInteraction.Collide);
	}

	public bool GetIgnore(Bounds bounds)
	{
		return GamePhysics.CheckBounds<WaterVisibilityTrigger>(bounds, 262144, QueryTriggerInteraction.Collide);
	}

	public bool GetIgnore(RaycastHit hit)
	{
		if (!this.waterColliders.Contains(hit.collider))
		{
			return false;
		}
		return this.GetIgnore(hit.point, 0.01f);
	}

	public bool GetIgnore(Collider collider)
	{
		if (this.waterColliders.Count == 0 || !collider)
		{
			return false;
		}
		return this.ignoredColliders.Contains(collider);
	}

	protected void LateUpdate()
	{
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
				HashSet<Collider>.Enumerator enumerator = this.waterColliders.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Physics.IgnoreCollision(key, enumerator.Current, false);
				}
				int num1 = i;
				i = num1 - 1;
				this.ignoredColliders.RemoveAt(num1);
			}
		}
	}

	public void Reset(Collider collider)
	{
		if (this.waterColliders.Count == 0 || !collider)
		{
			return;
		}
		HashSet<Collider>.Enumerator enumerator = this.waterColliders.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Physics.IgnoreCollision(collider, enumerator.Current, false);
		}
		this.ignoredColliders.Remove(collider);
	}

	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (this.waterColliders.Count == 0 || !collider)
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
			HashSet<Collider>.Enumerator enumerator = this.waterColliders.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Physics.IgnoreCollision(collider, enumerator.Current, true);
			}
			this.ignoredColliders.Add(collider, colliders);
			return;
		}
	}
}