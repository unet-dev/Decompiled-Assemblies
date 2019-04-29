using ConVar;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class ScientistSpawner : SpawnGroup
	{
		[Header("Scientist Spawner")]
		public bool Mobile = true;

		public bool NeverMove;

		public bool SpawnHostile;

		public bool OnlyAggroMarkedTargets = true;

		public bool IsPeacekeeper = true;

		public bool IsBandit;

		public bool IsMilitaryTunnelLab;

		public NPCPlayerApex.EnemyRangeEnum MaxRangeToSpawnLoc = NPCPlayerApex.EnemyRangeEnum.LongAttackRange;

		public WaypointSet Waypoints;

		public Transform[] LookAtInterestPointsStationary;

		public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

		public Model Model;

		[SerializeField]
		private AiLocationManager _mgr;

		private float _nextForcedRespawn = Single.PositiveInfinity;

		private bool _lastSpawnCallHadAliveMembers;

		private bool _lastSpawnCallHadMaxAliveMembers;

		public ScientistSpawner()
		{
		}

		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (this.LookAtInterestPointsStationary != null && this.LookAtInterestPointsStationary.Length != 0)
			{
				Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.5f);
				Transform[] lookAtInterestPointsStationary = this.LookAtInterestPointsStationary;
				for (int i = 0; i < (int)lookAtInterestPointsStationary.Length; i++)
				{
					Transform transforms = lookAtInterestPointsStationary[i];
					if (transforms != null)
					{
						Gizmos.DrawSphere(transforms.position, 0.1f);
						Gizmos.DrawLine(base.transform.position, transforms.position);
					}
				}
			}
		}

		protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
		{
			float single;
			object obj;
			object obj1;
			object obj2;
			Scientist component = entity.GetComponent<Scientist>();
			if (component)
			{
				component.Stats.Hostility = (this.SpawnHostile ? 1f : 0f);
				if (this.SpawnHostile)
				{
					single = 1f;
				}
				else
				{
					single = (this.IsBandit ? 1f : 0f);
				}
				component.Stats.Defensiveness = single;
				component.Stats.OnlyAggroMarkedTargets = this.OnlyAggroMarkedTargets;
				component.Stats.IsMobile = this.Mobile;
				component.NeverMove = this.NeverMove;
				component.WaypointSet = this.Waypoints;
				if (this.LookAtInterestPointsStationary != null && this.LookAtInterestPointsStationary.Length != 0)
				{
					component.LookAtInterestPointsStationary = this.LookAtInterestPointsStationary;
				}
				component.RadioEffectRepeatRange = this.RadioEffectRepeatRange;
				Scientist scientist = component;
				if (this.IsPeacekeeper)
				{
					obj = 1;
				}
				else
				{
					obj = null;
				}
				scientist.SetFact(NPCPlayerApex.Facts.IsPeacekeeper, (byte)obj, true, true);
				Scientist scientist1 = component;
				if (this.IsBandit)
				{
					obj1 = 1;
				}
				else
				{
					obj1 = null;
				}
				scientist1.SetFact(NPCPlayerApex.Facts.IsBandit, (byte)obj1, true, true);
				Scientist scientist2 = component;
				if (this.IsMilitaryTunnelLab)
				{
					obj2 = 1;
				}
				else
				{
					obj2 = null;
				}
				scientist2.SetFact(NPCPlayerApex.Facts.IsMilitaryTunnelLab, (byte)obj2, true, true);
				component.Stats.MaxRangeToSpawnLoc = this.MaxRangeToSpawnLoc;
				if (!this.SpawnHostile)
				{
					component.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
					component.SetFact(NPCPlayerApex.Facts.Speed, 0, true, true);
				}
				if (this._mgr == null)
				{
					this._mgr = base.GetComponentInParent<AiLocationManager>();
				}
				if (this._mgr != null)
				{
					component.AiContext.AiLocationManager = this._mgr;
				}
			}
		}

		protected override void Spawn(int numToSpawn)
		{
			if (!ConVar.AI.npc_enable)
			{
				return;
			}
			if (base.currentPopulation == this.maxPopulation)
			{
				this._lastSpawnCallHadMaxAliveMembers = true;
				this._lastSpawnCallHadAliveMembers = true;
				return;
			}
			if (this._lastSpawnCallHadMaxAliveMembers)
			{
				this._nextForcedRespawn = UnityEngine.Time.time + 2200f;
			}
			if (UnityEngine.Time.time < this._nextForcedRespawn)
			{
				if (base.currentPopulation == 0 && this._lastSpawnCallHadAliveMembers)
				{
					this._lastSpawnCallHadMaxAliveMembers = false;
					this._lastSpawnCallHadAliveMembers = false;
					return;
				}
				if (base.currentPopulation > 0)
				{
					this._lastSpawnCallHadMaxAliveMembers = false;
					this._lastSpawnCallHadAliveMembers = base.currentPopulation > 0;
					return;
				}
			}
			this._lastSpawnCallHadMaxAliveMembers = false;
			this._lastSpawnCallHadAliveMembers = base.currentPopulation > 0;
			base.Spawn(numToSpawn);
		}
	}
}