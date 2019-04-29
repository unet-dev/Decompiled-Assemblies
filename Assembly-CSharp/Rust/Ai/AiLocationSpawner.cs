using ConVar;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class AiLocationSpawner : SpawnGroup
	{
		public AiLocationSpawner.SquadSpawnerLocation Location;

		public AiLocationManager Manager;

		public JunkPile Junkpile;

		public bool IsMainSpawner = true;

		public float chance = 1f;

		private int defaultMaxPopulation;

		private int defaultNumToSpawnPerTickMax;

		private int defaultNumToSpawnPerTickMin;

		public AiLocationSpawner()
		{
		}

		protected override BaseSpawnPoint GetSpawnPoint(out Vector3 pos, out Quaternion rot)
		{
			return base.GetSpawnPoint(out pos, out rot);
		}

		protected override void Spawn(int numToSpawn)
		{
			AiLocationSpawner.SquadSpawnerLocation location;
			Vector3 vector3;
			Quaternion quaternion;
			if (!ConVar.AI.npc_enable)
			{
				this.maxPopulation = 0;
				this.numToSpawnPerTickMax = 0;
				this.numToSpawnPerTickMin = 0;
				return;
			}
			if (numToSpawn == 0)
			{
				if (!this.IsMainSpawner)
				{
					this.maxPopulation = this.defaultMaxPopulation;
					this.numToSpawnPerTickMax = this.defaultNumToSpawnPerTickMax;
					this.numToSpawnPerTickMin = this.defaultNumToSpawnPerTickMin;
					numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
				}
				else
				{
					location = this.Location;
					if (location != AiLocationSpawner.SquadSpawnerLocation.MilitaryTunnels)
					{
						this.maxPopulation = this.defaultMaxPopulation;
						this.numToSpawnPerTickMax = this.defaultNumToSpawnPerTickMax;
						this.numToSpawnPerTickMin = this.defaultNumToSpawnPerTickMin;
						numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
					}
					else
					{
						this.maxPopulation = ConVar.AI.npc_max_population_military_tunnels;
						this.numToSpawnPerTickMax = ConVar.AI.npc_spawn_per_tick_max_military_tunnels;
						this.numToSpawnPerTickMin = ConVar.AI.npc_spawn_per_tick_min_military_tunnels;
						numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
					}
				}
			}
			float npcJunkpileASpawnChance = this.chance;
			location = this.Location;
			if (location == AiLocationSpawner.SquadSpawnerLocation.JunkpileA)
			{
				npcJunkpileASpawnChance = ConVar.AI.npc_junkpile_a_spawn_chance;
			}
			else if (location == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
			{
				npcJunkpileASpawnChance = ConVar.AI.npc_junkpile_g_spawn_chance;
			}
			if (numToSpawn == 0 || UnityEngine.Random.@value > npcJunkpileASpawnChance || (this.Location == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.Location == AiLocationSpawner.SquadSpawnerLocation.JunkpileG) && NPCPlayerApex.AllJunkpileNPCs.Count >= ConVar.AI.npc_max_junkpile_count)
			{
				return;
			}
			numToSpawn = Mathf.Min(numToSpawn, this.maxPopulation - base.currentPopulation);
			for (int i = 0; i < numToSpawn; i++)
			{
				BaseSpawnPoint spawnPoint = this.GetSpawnPoint(out vector3, out quaternion);
				if (spawnPoint)
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(base.GetPrefab(), vector3, quaternion, true);
					if (baseEntity)
					{
						if (this.Manager != null)
						{
							NPCPlayerApex manager = baseEntity as NPCPlayerApex;
							if (manager != null)
							{
								manager.AiContext.AiLocationManager = this.Manager;
								if (this.Junkpile != null)
								{
									this.Junkpile.AddNpc(manager);
								}
							}
						}
						baseEntity.Spawn();
						SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
						spawnPointInstance.parentSpawnGroup = this;
						spawnPointInstance.parentSpawnPoint = spawnPoint;
						spawnPointInstance.Notify();
					}
				}
			}
		}

		public override void SpawnInitial()
		{
			if (!this.IsMainSpawner)
			{
				this.defaultMaxPopulation = this.maxPopulation;
				this.defaultNumToSpawnPerTickMax = this.numToSpawnPerTickMax;
				this.defaultNumToSpawnPerTickMin = this.numToSpawnPerTickMin;
			}
			else if (this.Location != AiLocationSpawner.SquadSpawnerLocation.MilitaryTunnels)
			{
				this.defaultMaxPopulation = this.maxPopulation;
				this.defaultNumToSpawnPerTickMax = this.numToSpawnPerTickMax;
				this.defaultNumToSpawnPerTickMin = this.numToSpawnPerTickMin;
			}
			else
			{
				this.maxPopulation = ConVar.AI.npc_max_population_military_tunnels;
				this.numToSpawnPerTickMax = ConVar.AI.npc_spawn_per_tick_max_military_tunnels;
				this.numToSpawnPerTickMin = ConVar.AI.npc_spawn_per_tick_min_military_tunnels;
				this.respawnDelayMax = ConVar.AI.npc_respawn_delay_max_military_tunnels;
				this.respawnDelayMin = ConVar.AI.npc_respawn_delay_min_military_tunnels;
			}
			base.SpawnInitial();
		}

		public enum SquadSpawnerLocation
		{
			MilitaryTunnels,
			JunkpileA,
			JunkpileG,
			CH47,
			None,
			Compound,
			BanditTown,
			CargoShip
		}
	}
}