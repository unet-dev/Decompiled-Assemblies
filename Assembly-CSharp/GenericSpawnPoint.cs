using System;
using UnityEngine;
using UnityEngine.Events;

public class GenericSpawnPoint : BaseSpawnPoint
{
	public bool dropToGround = true;

	public bool randomRot;

	public GameObjectRef spawnEffect;

	public UnityEvent OnObjectSpawnedEvent = new UnityEvent();

	public UnityEvent OnObjectRetiredEvent = new UnityEvent();

	public GenericSpawnPoint()
	{
	}

	public override void GetLocation(out Vector3 pos, out Quaternion rot)
	{
		pos = base.transform.position;
		if (!this.randomRot)
		{
			rot = base.transform.rotation;
		}
		else
		{
			rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		}
		if (this.dropToGround)
		{
			base.DropToGround(ref pos, ref rot);
		}
	}

	public override void ObjectRetired(SpawnPointInstance instance)
	{
		this.OnObjectRetiredEvent.Invoke();
		base.gameObject.SetActive(true);
	}

	public override void ObjectSpawned(SpawnPointInstance instance)
	{
		if (this.spawnEffect.isValid)
		{
			Effect.server.Run(this.spawnEffect.resourcePath, instance.GetComponent<BaseEntity>(), 0, Vector3.zero, Vector3.up, null, false);
		}
		this.OnObjectSpawnedEvent.Invoke();
		base.gameObject.SetActive(false);
	}
}