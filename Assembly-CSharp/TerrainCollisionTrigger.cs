using System;
using UnityEngine;

public class TerrainCollisionTrigger : EnvironmentVolumeTrigger
{
	public TerrainCollisionTrigger()
	{
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (!TerrainMeta.Collision || other.isTrigger)
		{
			return;
		}
		this.UpdateCollider(other, true);
	}

	protected void OnTriggerExit(Collider other)
	{
		if (!TerrainMeta.Collision || other.isTrigger)
		{
			return;
		}
		this.UpdateCollider(other, false);
	}

	private void UpdateCollider(Collider other, bool state)
	{
		TerrainMeta.Collision.SetIgnore(other, base.volume.trigger, state);
		TerrainCollisionProxy component = other.GetComponent<TerrainCollisionProxy>();
		if (component)
		{
			for (int i = 0; i < (int)component.colliders.Length; i++)
			{
				TerrainMeta.Collision.SetIgnore(component.colliders[i], base.volume.trigger, state);
			}
		}
	}
}