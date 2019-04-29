using ConVar;
using Rust;
using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.Murderer
{
	public class MurdererSpawner : MonoBehaviour, IServerComponent, ISpawnGroup
	{
		public GameObjectRef MurdererPrefab;

		[NonSerialized]
		public List<MurdererDomain> Spawned = new List<MurdererDomain>();

		[NonSerialized]
		public BaseSpawnPoint[] SpawnPoints;

		public int MaxPopulation = 1;

		public bool InitialSpawn;

		public float MinRespawnTimeMinutes = 20f;

		public float MaxRespawnTimeMinutes = 20f;

		public HTNDomain.MovementRule Movement = HTNDomain.MovementRule.FreeMove;

		public float MovementRadius = -1f;

		public bool ReducedLongRangeAccuracy;

		private float nextRespawnTime;

		private bool pendingRespawn;

		public int currentPopulation
		{
			get
			{
				return this.Spawned.Count;
			}
		}

		public MurdererSpawner()
		{
		}

		private void Awake()
		{
			this.SpawnPoints = base.GetComponentsInChildren<BaseSpawnPoint>();
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
			}
		}

		public void CheckIfRespawnNeeded()
		{
			if (!this.pendingRespawn)
			{
				if (this.Spawned == null || this.Spawned.Count == 0 || this.IsAllSpawnedDead())
				{
					this.ScheduleRespawn();
					return;
				}
			}
			else if ((this.Spawned == null || this.Spawned.Count == 0 || this.IsAllSpawnedDead()) && UnityEngine.Time.time >= this.nextRespawnTime)
			{
				this.DoRespawn();
			}
		}

		public void Clear()
		{
			foreach (MurdererDomain spawned in this.Spawned)
			{
				BaseEntity baseEntity = spawned.gameObject.ToBaseEntity();
				if (!baseEntity)
				{
					continue;
				}
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
			this.Spawned.Clear();
		}

		public void DoRespawn()
		{
			if (!Rust.Application.isLoading && !Rust.Application.isLoadingSave)
			{
				this.SpawnScientist();
			}
			this.pendingRespawn = false;
		}

		public void Fill()
		{
			this.DoRespawn();
		}

		private BaseSpawnPoint GetSpawnPoint(out Vector3 pos, out Quaternion rot)
		{
			BaseSpawnPoint spawnPoints = null;
			pos = Vector3.zero;
			rot = Quaternion.identity;
			int num = UnityEngine.Random.Range(0, (int)this.SpawnPoints.Length);
			for (int i = 0; i < (int)this.SpawnPoints.Length; i++)
			{
				spawnPoints = this.SpawnPoints[(num + i) % (int)this.SpawnPoints.Length];
				if (spawnPoints && spawnPoints.gameObject.activeSelf)
				{
					break;
				}
			}
			if (spawnPoints)
			{
				spawnPoints.GetLocation(out pos, out rot);
			}
			return spawnPoints;
		}

		private bool IsAllSpawnedDead()
		{
			for (int i = 0; i < this.Spawned.Count; i++)
			{
				MurdererDomain item = this.Spawned[i];
				if (!(item == null) && !(item.transform == null) && item.MurdererContext != null && !(item.MurdererContext.Body == null) && !item.MurdererContext.Body.IsDestroyed && !item.MurdererContext.Body.IsDead())
				{
					return false;
				}
				this.Spawned.RemoveAt(i);
				i--;
			}
			return true;
		}

		public void ScheduleRespawn()
		{
			this.nextRespawnTime = UnityEngine.Time.time + UnityEngine.Random.Range(this.MinRespawnTimeMinutes, this.MaxRespawnTimeMinutes) * 60f;
			this.pendingRespawn = true;
		}

		public void SpawnInitial()
		{
			this.nextRespawnTime = UnityEngine.Time.time + UnityEngine.Random.Range(3f, 4f);
			this.pendingRespawn = true;
		}

		public void SpawnRepeating()
		{
			this.CheckIfRespawnNeeded();
		}

		public void SpawnScientist()
		{
			Vector3 vector3;
			Quaternion quaternion;
			if (this.Spawned == null || this.Spawned.Count >= this.MaxPopulation)
			{
				Debug.LogWarning("Attempted to spawn a Scientist, but the spawner was full!");
				return;
			}
			if (!ConVar.AI.npc_enable)
			{
				return;
			}
			int maxPopulation = this.MaxPopulation - this.Spawned.Count;
			for (int i = 0; i < maxPopulation; i++)
			{
				if (this.GetSpawnPoint(out vector3, out quaternion) != null)
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(this.MurdererPrefab.resourcePath, vector3, quaternion, false);
					MurdererDomain component = baseEntity.GetComponent<MurdererDomain>();
					if (!component)
					{
						baseEntity.Kill(BaseNetworkable.DestroyMode.None);
						return;
					}
					baseEntity.enableSaving = false;
					baseEntity.gameObject.AwakeFromInstantiate();
					baseEntity.Spawn();
					component.Movement = this.Movement;
					component.MovementRadius = this.MovementRadius;
					component.ReducedLongRangeAccuracy = this.ReducedLongRangeAccuracy;
					this.Spawned.Add(component);
				}
			}
		}
	}
}