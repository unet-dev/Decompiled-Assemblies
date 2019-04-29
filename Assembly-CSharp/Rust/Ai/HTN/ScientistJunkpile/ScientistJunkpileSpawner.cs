using ConVar;
using Rust;
using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile
{
	public class ScientistJunkpileSpawner : MonoBehaviour, IServerComponent, ISpawnGroup
	{
		public GameObjectRef ScientistPrefab;

		[NonSerialized]
		public List<ScientistJunkpileDomain> Spawned = new List<ScientistJunkpileDomain>();

		[NonSerialized]
		public BaseSpawnPoint[] SpawnPoints;

		public int MaxPopulation = 1;

		public bool InitialSpawn;

		public float MinRespawnTimeMinutes = 120f;

		public float MaxRespawnTimeMinutes = 120f;

		public HTNDomain.MovementRule Movement = HTNDomain.MovementRule.FreeMove;

		public float MovementRadius = -1f;

		public bool ReducedLongRangeAccuracy;

		public ScientistJunkpileSpawner.JunkpileType SpawnType;

		[Range(0f, 1f)]
		public float SpawnBaseChance = 1f;

		private float nextRespawnTime;

		private bool pendingRespawn;

		public int currentPopulation
		{
			get
			{
				return this.Spawned.Count;
			}
		}

		public ScientistJunkpileSpawner()
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
			if (!this.IsUnderGlobalSpawnThreshold())
			{
				return;
			}
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
			if (this.Spawned == null)
			{
				return;
			}
			foreach (ScientistJunkpileDomain spawned in this.Spawned)
			{
				if (spawned == null || spawned.gameObject == null || spawned.transform == null)
				{
					continue;
				}
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
				ScientistJunkpileDomain item = this.Spawned[i];
				if (!(item == null) && !(item.transform == null) && item.ScientistContext != null && !(item.ScientistContext.Body == null) && !item.ScientistContext.Body.IsDestroyed && !item.ScientistContext.Body.IsDead())
				{
					return false;
				}
				this.Spawned.RemoveAt(i);
				i--;
			}
			return true;
		}

		private bool IsUnderGlobalSpawnThreshold()
		{
			if (ScientistJunkpileDomain.AllJunkpileNPCs != null && ScientistJunkpileDomain.AllJunkpileNPCs.Count >= ConVar.AI.npc_max_junkpile_count)
			{
				return false;
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
			if (!ConVar.AI.npc_enable)
			{
				return;
			}
			if (this.Spawned == null || this.Spawned.Count >= this.MaxPopulation || !this.IsUnderGlobalSpawnThreshold())
			{
				return;
			}
			float spawnBaseChance = this.SpawnBaseChance;
			ScientistJunkpileSpawner.JunkpileType spawnType = this.SpawnType;
			if (spawnType == ScientistJunkpileSpawner.JunkpileType.A)
			{
				spawnBaseChance = ConVar.AI.npc_junkpile_a_spawn_chance;
			}
			else if (spawnType == ScientistJunkpileSpawner.JunkpileType.G)
			{
				spawnBaseChance = ConVar.AI.npc_junkpile_g_spawn_chance;
			}
			if (UnityEngine.Random.@value > spawnBaseChance)
			{
				return;
			}
			int maxPopulation = this.MaxPopulation - this.Spawned.Count;
			for (int i = 0; i < maxPopulation; i++)
			{
				if (this.GetSpawnPoint(out vector3, out quaternion) != null)
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(this.ScientistPrefab.resourcePath, vector3, quaternion, false);
					ScientistJunkpileDomain component = baseEntity.GetComponent<ScientistJunkpileDomain>();
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

		public enum JunkpileType
		{
			A,
			B,
			C,
			D,
			E,
			F,
			G
		}
	}
}