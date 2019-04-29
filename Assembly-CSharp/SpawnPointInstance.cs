using Rust;
using System;
using UnityEngine;

public class SpawnPointInstance : MonoBehaviour
{
	public SpawnGroup parentSpawnGroup;

	public BaseSpawnPoint parentSpawnPoint;

	public SpawnPointInstance()
	{
	}

	public void Notify()
	{
		if (this.parentSpawnGroup)
		{
			this.parentSpawnGroup.ObjectSpawned(this);
		}
		if (this.parentSpawnPoint)
		{
			this.parentSpawnPoint.ObjectSpawned(this);
		}
	}

	protected void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (this.parentSpawnGroup)
		{
			this.parentSpawnGroup.ObjectRetired(this);
		}
		if (this.parentSpawnPoint)
		{
			this.parentSpawnPoint.ObjectRetired(this);
		}
	}
}