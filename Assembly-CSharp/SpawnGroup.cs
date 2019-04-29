using ConVar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnGroup : BaseMonoBehaviour, IServerComponent, ISpawnGroup
{
	public List<SpawnGroup.SpawnEntry> prefabs;

	public int maxPopulation = 5;

	public int numToSpawnPerTickMin = 1;

	public int numToSpawnPerTickMax = 2;

	public float respawnDelayMin = 10f;

	public float respawnDelayMax = 20f;

	public bool wantsInitialSpawn = true;

	public bool temporary;

	protected bool fillOnSpawn;

	public BaseSpawnPoint[] spawnPoints;

	private List<SpawnPointInstance> spawnInstances = new List<SpawnPointInstance>();

	private LocalClock spawnClock = new LocalClock();

	public int currentPopulation
	{
		get
		{
			return this.spawnInstances.Count;
		}
	}

	public SpawnGroup()
	{
	}

	protected void Awake()
	{
		this.spawnPoints = base.GetComponentsInChildren<BaseSpawnPoint>();
		if (this.WantsTimedSpawn())
		{
			this.spawnClock.Add(this.GetSpawnDelta(), this.GetSpawnVariance(), new Action(this.Spawn));
		}
		if (!this.temporary && SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
		}
	}

	public void Clear()
	{
		foreach (SpawnPointInstance spawnInstance in this.spawnInstances)
		{
			BaseEntity baseEntity = spawnInstance.gameObject.ToBaseEntity();
			if (!baseEntity)
			{
				continue;
			}
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}
		this.spawnInstances.Clear();
	}

	public void Fill()
	{
		this.Spawn(this.maxPopulation);
	}

	protected string GetPrefab()
	{
		string str;
		float single = (float)this.prefabs.Sum<SpawnGroup.SpawnEntry>((SpawnGroup.SpawnEntry x) => x.weight);
		if (single == 0f)
		{
			return null;
		}
		float single1 = UnityEngine.Random.Range(0f, single);
		List<SpawnGroup.SpawnEntry>.Enumerator enumerator = this.prefabs.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SpawnGroup.SpawnEntry current = enumerator.Current;
				float single2 = single1 - (float)current.weight;
				single1 = single2;
				if (single2 > 0f)
				{
					continue;
				}
				str = current.prefab.resourcePath;
				return str;
			}
			return this.prefabs[this.prefabs.Count - 1].prefab.resourcePath;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return str;
	}

	public float GetSpawnDelta()
	{
		return (this.respawnDelayMax + this.respawnDelayMin) * 0.5f / SpawnHandler.PlayerScale(Spawn.player_scale);
	}

	protected virtual BaseSpawnPoint GetSpawnPoint(out Vector3 pos, out Quaternion rot)
	{
		BaseSpawnPoint baseSpawnPoint = null;
		pos = Vector3.zero;
		rot = Quaternion.identity;
		int num = UnityEngine.Random.Range(0, (int)this.spawnPoints.Length);
		for (int i = 0; i < (int)this.spawnPoints.Length; i++)
		{
			baseSpawnPoint = this.spawnPoints[(num + i) % (int)this.spawnPoints.Length];
			if (baseSpawnPoint && baseSpawnPoint.gameObject.activeSelf)
			{
				break;
			}
		}
		if (baseSpawnPoint)
		{
			baseSpawnPoint.GetLocation(out pos, out rot);
		}
		return baseSpawnPoint;
	}

	public float GetSpawnVariance()
	{
		return (this.respawnDelayMax - this.respawnDelayMin) * 0.5f / SpawnHandler.PlayerScale(Spawn.player_scale);
	}

	public void ObjectRetired(SpawnPointInstance instance)
	{
		this.spawnInstances.Remove(instance);
	}

	public void ObjectSpawned(SpawnPointInstance instance)
	{
		this.spawnInstances.Add(instance);
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 0f, 1f);
		Gizmos.DrawSphere(base.transform.position, 0.25f);
	}

	protected virtual void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
	}

	public void Spawn()
	{
		this.Spawn(UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1));
	}

	protected virtual void Spawn(int numToSpawn)
	{
		Vector3 vector3;
		Quaternion quaternion;
		numToSpawn = Mathf.Min(numToSpawn, this.maxPopulation - this.currentPopulation);
		for (int i = 0; i < numToSpawn; i++)
		{
			BaseSpawnPoint spawnPoint = this.GetSpawnPoint(out vector3, out quaternion);
			if (spawnPoint)
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(this.GetPrefab(), vector3, quaternion, false);
				if (baseEntity)
				{
					baseEntity.enableSaving = false;
					baseEntity.gameObject.AwakeFromInstantiate();
					baseEntity.Spawn();
					this.PostSpawnProcess(baseEntity, spawnPoint);
					SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
					spawnPointInstance.parentSpawnGroup = this;
					spawnPointInstance.parentSpawnPoint = spawnPoint;
					spawnPointInstance.Notify();
				}
			}
		}
	}

	public virtual void SpawnInitial()
	{
		if (!this.wantsInitialSpawn)
		{
			return;
		}
		if (!this.fillOnSpawn)
		{
			this.Spawn();
			return;
		}
		this.Spawn(this.maxPopulation);
	}

	public void SpawnRepeating()
	{
		for (int i = 0; i < this.spawnClock.events.Count; i++)
		{
			LocalClock.TimedEvent item = this.spawnClock.events[i];
			if (UnityEngine.Time.time > item.time)
			{
				item.delta = this.GetSpawnDelta();
				item.variance = this.GetSpawnVariance();
				this.spawnClock.events[i] = item;
			}
		}
		this.spawnClock.Tick();
	}

	public virtual bool WantsInitialSpawn()
	{
		return this.wantsInitialSpawn;
	}

	public virtual bool WantsTimedSpawn()
	{
		return this.respawnDelayMax != Single.PositiveInfinity;
	}

	[Serializable]
	public class SpawnEntry
	{
		public GameObjectRef prefab;

		public int weight;

		public bool mobile;

		public SpawnEntry()
		{
		}
	}
}