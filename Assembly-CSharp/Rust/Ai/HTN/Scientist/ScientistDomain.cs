using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using ConVar;
using Rust.Ai;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Scientist.Reasoners;
using Rust.Ai.HTN.Scientist.Sensors;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.Scientist
{
	public class ScientistDomain : HTNDomain
	{
		[ReadOnly]
		[SerializeField]
		private bool _isRegisteredWithAgency;

		[Header("Context")]
		[SerializeField]
		private Rust.Ai.HTN.Scientist.ScientistContext _context;

		[Header("Navigation")]
		[ReadOnly]
		[SerializeField]
		private NavMeshAgent _navAgent;

		[ReadOnly]
		[SerializeField]
		private Vector3 _spawnPosition;

		[Header("Sensors")]
		[ReadOnly]
		[SerializeField]
		private List<INpcSensor> _sensors = new List<INpcSensor>()
		{
			new PlayersInRangeSensor()
			{
				TickFrequency = 0.5f
			},
			new PlayersOutsideRangeSensor()
			{
				TickFrequency = 0.1f
			},
			new PlayersDistanceSensor()
			{
				TickFrequency = 0.1f
			},
			new PlayersViewAngleSensor()
			{
				TickFrequency = 0.1f
			},
			new EnemyPlayersInRangeSensor()
			{
				TickFrequency = 0.1f
			},
			new EnemyPlayersLineOfSightSensor()
			{
				TickFrequency = 0.25f,
				MaxVisible = 5
			},
			new EnemyPlayersHearingSensor()
			{
				TickFrequency = 0.1f
			},
			new CoverPointsInRangeSensor()
			{
				TickFrequency = 1f
			},
			new AnimalsInRangeSensor()
			{
				TickFrequency = 1f
			},
			new AnimalDistanceSensor()
			{
				TickFrequency = 0.25f
			}
		};

		[Header("Reasoners")]
		[ReadOnly]
		[SerializeField]
		private List<INpcReasoner> _reasoners = new List<INpcReasoner>()
		{
			new EnemyPlayerLineOfSightReasoner()
			{
				TickFrequency = 0.1f
			},
			new EnemyPlayerHearingReasoner()
			{
				TickFrequency = 0.1f
			},
			new EnemyTargetReasoner()
			{
				TickFrequency = 0.1f
			},
			new FireTacticReasoner()
			{
				TickFrequency = 0.1f
			},
			new OrientationReasoner()
			{
				TickFrequency = 0.01f
			},
			new PreferredFightingRangeReasoner()
			{
				TickFrequency = 0.1f
			},
			new AtLastKnownEnemyPlayerLocationReasoner()
			{
				TickFrequency = 0.1f
			},
			new FirearmPoseReasoner()
			{
				TickFrequency = 0.1f
			},
			new HealthReasoner()
			{
				TickFrequency = 0.1f
			},
			new AmmoReasoner()
			{
				TickFrequency = 0.1f
			},
			new VulnerabilityReasoner()
			{
				TickFrequency = 0.1f
			},
			new FrustrationReasoner()
			{
				TickFrequency = 0.25f
			},
			new CoverPointsReasoner()
			{
				TickFrequency = 0.5f
			},
			new AtCoverLocationReasoner()
			{
				TickFrequency = 0.1f
			},
			new MaintainCoverReasoner()
			{
				TickFrequency = 0.1f
			},
			new ReturnHomeReasoner()
			{
				TickFrequency = 5f
			},
			new AtHomeLocationReasoner()
			{
				TickFrequency = 5f
			},
			new ExplosivesReasoner()
			{
				TickFrequency = 0.1f
			},
			new AnimalReasoner()
			{
				TickFrequency = 0.25f
			},
			new AlertnessReasoner()
			{
				TickFrequency = 0.1f
			},
			new EnemyRangeReasoner()
			{
				TickFrequency = 0.1f
			}
		};

		[Header("Firearm Utility")]
		[ReadOnly]
		[SerializeField]
		private float _lastFirearmUsageTime;

		[ReadOnly]
		[SerializeField]
		private bool _isFiring;

		[ReadOnly]
		[SerializeField]
		public bool ReducedLongRangeAccuracy;

		private HTNUtilityAiClient _aiClient;

		private Rust.Ai.HTN.Scientist.ScientistDefinition _scientistDefinition;

		private Vector3 missOffset;

		private float missToHeadingAlignmentTime;

		private float repeatMissTime;

		private bool recalculateMissOffset = true;

		private bool isMissing;

		private static Vector3[] pathCornerCache;

		private static NavMeshPath _pathCache;

		public ScientistDomain.OnPlanAborted OnPlanAbortedEvent;

		public ScientistDomain.OnPlanCompleted OnPlanCompletedEvent;

		public override NavMeshAgent NavAgent
		{
			get
			{
				return this._navAgent;
			}
		}

		public override BaseNpcContext NpcContext
		{
			get
			{
				return this._context;
			}
		}

		public override IUtilityAI PlannerAi
		{
			get
			{
				return this._aiClient.ai;
			}
		}

		public override IUtilityAIClient PlannerAiClient
		{
			get
			{
				return this._aiClient;
			}
		}

		public override IHTNContext PlannerContext
		{
			get
			{
				return this._context;
			}
		}

		public override List<INpcReasoner> Reasoners
		{
			get
			{
				return this._reasoners;
			}
		}

		public Rust.Ai.HTN.Scientist.ScientistContext ScientistContext
		{
			get
			{
				return this._context;
			}
		}

		public Rust.Ai.HTN.Scientist.ScientistDefinition ScientistDefinition
		{
			get
			{
				if (this._scientistDefinition == null)
				{
					this._scientistDefinition = this._context.Body.AiDefinition as Rust.Ai.HTN.Scientist.ScientistDefinition;
				}
				return this._scientistDefinition;
			}
		}

		public override List<INpcSensor> Sensors
		{
			get
			{
				return this._sensors;
			}
		}

		public Vector3 SpawnPosition
		{
			get
			{
				return this._spawnPosition;
			}
		}

		static ScientistDomain()
		{
			ScientistDomain.pathCornerCache = new Vector3[128];
			ScientistDomain._pathCache = null;
		}

		public ScientistDomain()
		{
		}

		protected override void AbortPlan()
		{
			base.AbortPlan();
			ScientistDomain.OnPlanAborted onPlanAbortedEvent = this.OnPlanAbortedEvent;
			if (onPlanAbortedEvent != null)
			{
				onPlanAbortedEvent(this);
			}
			else
			{
			}
			this._context.SetFact(Facts.MaintainCover, 0, false, true, true);
			this._context.Body.modelState.ducked = false;
			this._context.SetFact(Facts.IsDucking, false, false, true, true);
		}

		public override bool AllowedMovementDestination(Vector3 destination)
		{
			if (this.Movement == HTNDomain.MovementRule.FreeMove)
			{
				return true;
			}
			if (this.Movement == HTNDomain.MovementRule.NeverMove)
			{
				return false;
			}
			Vector3 spawnPosition = this.SpawnPosition - destination;
			return spawnPosition.sqrMagnitude <= base.SqrMovementRadius + 0.1f;
		}

		private bool CanUseFirearmAtRange(float sqrRange)
		{
			AttackEntity firearm = this.GetFirearm();
			if (firearm == null)
			{
				return false;
			}
			if (sqrRange <= this._context.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(firearm))
			{
				return true;
			}
			if (sqrRange <= this._context.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(firearm))
			{
				return firearm.CanUseAtMediumRange;
			}
			return firearm.CanUseAtLongRange;
		}

		protected override void CompletePlan()
		{
			base.CompletePlan();
			ScientistDomain.OnPlanCompleted onPlanCompletedEvent = this.OnPlanCompletedEvent;
			if (onPlanCompletedEvent != null)
			{
				onPlanCompletedEvent(this);
			}
			else
			{
			}
			this._context.SetFact(Facts.MaintainCover, 0, false, true, true);
			this._context.Body.modelState.ducked = false;
			this._context.SetFact(Facts.IsDucking, false, false, true, true);
		}

		public override void Dispose()
		{
			HTNUtilityAiClient hTNUtilityAiClient = this._aiClient;
			if (hTNUtilityAiClient != null)
			{
				hTNUtilityAiClient.Kill();
			}
			else
			{
			}
			this.RemoveAgency();
		}

		private void FireBurst(BaseProjectile proj, float time)
		{
			if (proj == null)
			{
				return;
			}
			base.StartCoroutine(this.HoldTriggerLogic(proj, time, UnityEngine.Random.Range(proj.attackLengthMin, proj.attackLengthMax)));
		}

		private void FireFullAuto(BaseProjectile proj, float time)
		{
			if (proj == null)
			{
				return;
			}
			base.StartCoroutine(this.HoldTriggerLogic(proj, time, 4f));
		}

		private void FireSingle(AttackEntity attackEnt, float time)
		{
			if (this._context.EnemyPlayersInLineOfSight.Count > 3)
			{
				attackEnt.ServerUse((1f + UnityEngine.Random.@value * 0.5f) * ConVar.AI.npc_htn_player_base_damage_modifier);
			}
			else if (!(this._context.PrimaryEnemyPlayerInLineOfSight.Player != null) || this._context.PrimaryEnemyPlayerInLineOfSight.Player.healthFraction >= 0.2f)
			{
				attackEnt.ServerUse(ConVar.AI.npc_htn_player_base_damage_modifier);
			}
			else
			{
				attackEnt.ServerUse((0.1f + UnityEngine.Random.@value * 0.5f) * ConVar.AI.npc_htn_player_base_damage_modifier);
			}
			this._lastFirearmUsageTime = time + attackEnt.attackSpacing * (0.5f + UnityEngine.Random.@value * 0.5f);
			this._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
		}

		public override void ForceProjectileOrientation()
		{
			if (this._context.OrientationType != NpcOrientation.LookAtAnimal && this._context.OrientationType != NpcOrientation.PrimaryTargetBody && this._context.OrientationType != NpcOrientation.PrimaryTargetHead)
			{
				if (this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player != null)
				{
					if (!this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.BodyVisible && this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.HeadVisible)
					{
						this._context.OrientationType = NpcOrientation.PrimaryTargetHead;
						return;
					}
					this._context.OrientationType = NpcOrientation.PrimaryTargetBody;
					return;
				}
				if (this._context.Memory.PrimaryKnownAnimal.Animal != null)
				{
					this._context.OrientationType = NpcOrientation.LookAtAnimal;
				}
			}
		}

		public float GetAllowedCoverRangeSqr()
		{
			float sqrMovementRadius = 225f;
			if (this.Movement == HTNDomain.MovementRule.RestrainedMove && this.MovementRadius < 15f)
			{
				sqrMovementRadius = base.SqrMovementRadius;
			}
			return sqrMovementRadius;
		}

		public override IAIContext GetContext(Guid aiId)
		{
			return this._context;
		}

		public AttackEntity GetFirearm()
		{
			return this._context.Body.GetHeldEntity() as AttackEntity;
		}

		public BaseProjectile GetFirearmProj()
		{
			AttackEntity firearm = this.GetFirearm();
			if (!firearm)
			{
				return null;
			}
			return firearm as BaseProjectile;
		}

		public override Vector3 GetHeadingDirection()
		{
			if (!(this.NavAgent != null) || !this.NavAgent.isOnNavMesh || this._context.GetFact(Facts.IsNavigating) <= 0)
			{
				return this._context.Body.transform.forward;
			}
			return this.NavAgent.desiredVelocity.normalized;
		}

		public override Vector3 GetHomeDirection()
		{
			Vector3 spawnPosition = this.SpawnPosition - this._context.BodyPosition;
			if (spawnPosition.SqrMagnitudeXZ() >= 0.01f)
			{
				return spawnPosition.normalized;
			}
			return this._context.Body.transform.forward;
		}

		private Vector3 GetMissVector(Vector3 heading, Vector3 target, Vector3 origin, float maxTime, float repeatTime)
		{
			float single = UnityEngine.Time.time;
			if (!this.isMissing && this.repeatMissTime < single)
			{
				if (!this.recalculateMissOffset)
				{
					return heading;
				}
				this.missOffset = Vector3.zero;
				this.missOffset.x = (UnityEngine.Random.@value > 0.5f ? 1f : -1f);
				this.missOffset *= ConVar.AI.npc_deliberate_miss_offset_multiplier;
				this.missToHeadingAlignmentTime = single + maxTime;
				this.repeatMissTime = this.missToHeadingAlignmentTime + repeatTime;
				this.recalculateMissOffset = false;
				this.isMissing = true;
			}
			Vector3 vector3 = (target + this.missOffset) - origin;
			float single1 = Mathf.Max(this.missToHeadingAlignmentTime - single, 0f);
			float single2 = this.ScientistDefinition.MissFunction.Evaluate((Mathf.Approximately(single1, 0f) ? 1f : 1f - Mathf.Min(single1 / maxTime, 1f)));
			if (!Mathf.Approximately(single2, 1f))
			{
				return Vector3.Lerp(vector3.normalized, heading, single2);
			}
			this.recalculateMissOffset = true;
			this.isMissing = false;
			float single3 = Mathf.Min(1f, ConVar.AI.npc_deliberate_hit_randomizer);
			return Vector3.Lerp(vector3.normalized, heading, 1f - single3 + UnityEngine.Random.@value * single3);
		}

		public override Vector3 GetNextPosition(float delta)
		{
			if (!(this.NavAgent == null) && this.NavAgent.isOnNavMesh && this.NavAgent.hasPath)
			{
				return this.NavAgent.nextPosition;
			}
			return this._context.BodyPosition;
		}

		private IEnumerator HoldTriggerLogic(BaseProjectile proj, float startTime, float triggerDownInterval)
		{
			ScientistDomain scientistDomain = null;
			float single;
			scientistDomain._isFiring = true;
			scientistDomain._lastFirearmUsageTime = startTime + triggerDownInterval + proj.attackSpacing;
			scientistDomain._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
			int count = scientistDomain._context.EnemyPlayersInLineOfSight.Count;
			single = (count <= 2 ? 0f : 1.5f - 1f / (float)count * 3f);
			float single1 = single;
			do
			{
				if (UnityEngine.Time.time - startTime >= triggerDownInterval || !scientistDomain._context.IsBodyAlive() || !scientistDomain._context.IsFact(Facts.CanSeeEnemy))
				{
					break;
				}
				if (count > 2)
				{
					proj.ServerUse((1f + UnityEngine.Random.@value * single1) * ConVar.AI.npc_htn_player_base_damage_modifier);
				}
				else if (!(scientistDomain._context.PrimaryEnemyPlayerInLineOfSight.Player != null) || scientistDomain._context.PrimaryEnemyPlayerInLineOfSight.Player.healthFraction >= 0.2f)
				{
					proj.ServerUse(ConVar.AI.npc_htn_player_base_damage_modifier);
				}
				else
				{
					proj.ServerUse((0.1f + UnityEngine.Random.@value * 0.5f) * ConVar.AI.npc_htn_player_base_damage_modifier);
				}
				yield return CoroutineEx.waitForSeconds(proj.repeatDelay);
			}
			while (proj.primaryMagazine.contents > 0);
			scientistDomain._isFiring = false;
		}

		public override void Initialize(BaseEntity body)
		{
			if (this._aiClient == null || this._aiClient.ai == null || this._aiClient.ai.id != AINameMap.HTNDomainScientistMilitaryTunnel)
			{
				this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainScientistMilitaryTunnel, this);
			}
			if (this._context == null || this._context.Body != body)
			{
				this._context = new Rust.Ai.HTN.Scientist.ScientistContext(body as HTNPlayer, this);
			}
			if (this._navAgent == null)
			{
				this._navAgent = base.GetComponent<NavMeshAgent>();
			}
			if (this._navAgent)
			{
				this._navAgent.updateRotation = false;
				this._navAgent.updatePosition = false;
				this._navAgent.speed = this._context.Body.AiDefinition.Movement.DuckSpeed;
			}
			this._spawnPosition = body.transform.position;
			this._aiClient.Initialize();
			this._context.Body.Resume();
			this.InitializeAgency();
		}

		private void InitializeAgency()
		{
			if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || !ConVar.AI.npc_enable)
			{
				return;
			}
			if (!this._isRegisteredWithAgency)
			{
				this._isRegisteredWithAgency = true;
				SingletonComponent<AiManager>.Instance.HTNAgency.Add(this._context.Body);
			}
		}

		public bool IsPathValid()
		{
			if (!this._context.IsBodyAlive())
			{
				return false;
			}
			if (this._context.Memory.HasTargetDestination && !this._context.Domain.NavAgent.pathPending && (this._context.Domain.NavAgent.pathStatus != NavMeshPathStatus.PathComplete || (this._context.Domain.NavAgent.destination - this._context.Memory.TargetDestination).sqrMagnitude > 0.01f))
			{
				return false;
			}
			if (!this.AllowedMovementDestination(this._context.Memory.TargetDestination))
			{
				return false;
			}
			return true;
		}

		public Vector3 ModifyFirearmAim(Vector3 heading, Vector3 target, Vector3 origin, float swayModifier = 1f)
		{
			if (!ConVar.AI.npc_use_new_aim_system)
			{
				AttackEntity firearm = this.GetFirearm();
				if (firearm)
				{
					return firearm.ModifyAIAim(heading, swayModifier);
				}
			}
			float single = (target - origin).sqrMagnitude;
			float fact = (float)this._context.GetFact(Facts.Alertness);
			if (fact > 10f)
			{
				fact = 10f;
			}
			AttackEntity attackEntity = this.GetFirearm();
			if (single <= this._context.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(attackEntity) + 2f)
			{
				return heading;
			}
			int count = this._context.EnemyPlayersInLineOfSight.Count;
			if (count > 2 && UnityEngine.Random.@value < 0.9f - 1f / (float)count * 2f)
			{
				return heading;
			}
			if (this.ReducedLongRangeAccuracy && single > this._context.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(attackEntity))
			{
				fact *= 0.5f;
			}
			if (this._context.PrimaryEnemyPlayerInLineOfSight.Player != null && (this._context.PrimaryEnemyPlayerInLineOfSight.Player.modelState.jumped || !this._context.PrimaryEnemyPlayerInLineOfSight.BodyVisible && this._context.PrimaryEnemyPlayerInLineOfSight.HeadVisible))
			{
				fact *= 0.5f;
			}
			return this.GetMissVector(heading, target, origin, ConVar.AI.npc_deliberate_miss_to_hit_alignment_time, fact * ConVar.AI.npc_alertness_to_aim_modifier);
		}

		private void OnExplosionSensation(ref Sensation info)
		{
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer != null && initiatorPlayer != this._context.Body)
			{
				bool flag = false;
				foreach (NpcPlayerInfo enemyPlayersInRange in this._context.EnemyPlayersInRange)
				{
					if (!this.RememberExplosion(ref info, enemyPlayersInRange, initiatorPlayer))
					{
						continue;
					}
					this._context.IncrementFact(Facts.Vulnerability, (this._context.IsFact(Facts.CanSeeEnemy) ? 1 : 2), true, true, true);
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					this._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
				}
			}
		}

		private void OnGunshotSensation(ref Sensation info)
		{
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer != null && initiatorPlayer != this._context.Body)
			{
				bool flag = false;
				foreach (NpcPlayerInfo enemyPlayersInRange in this._context.EnemyPlayersInRange)
				{
					if (!this.RememberGunshot(ref info, enemyPlayersInRange, initiatorPlayer))
					{
						continue;
					}
					if (this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null || this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == initiatorPlayer)
					{
						this._context.Memory.RememberPrimaryEnemyPlayer(initiatorPlayer);
					}
					this._context.IncrementFact(Facts.Vulnerability, (this._context.IsFact(Facts.CanSeeEnemy) ? 0 : 1), true, true, true);
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					this._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
					List<NpcPlayerInfo> playersOutsideDetectionRange = this._context.PlayersOutsideDetectionRange;
					NpcPlayerInfo npcPlayerInfo = new NpcPlayerInfo()
					{
						Player = initiatorPlayer,
						Time = UnityEngine.Time.time
					};
					playersOutsideDetectionRange.Add(npcPlayerInfo);
				}
			}
		}

		public override void OnHurt(HitInfo info)
		{
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer != null && initiatorPlayer != this._context.Body)
			{
				bool flag = false;
				foreach (NpcPlayerInfo enemyPlayersInRange in this._context.EnemyPlayersInRange)
				{
					if (!this.RememberPlayerThatHurtUs(enemyPlayersInRange, initiatorPlayer))
					{
						continue;
					}
					if (this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
					{
						this._context.Memory.RememberPrimaryEnemyPlayer(initiatorPlayer);
					}
					this._context.IncrementFact(Facts.Vulnerability, (this._context.IsFact(Facts.CanSeeEnemy) ? 1 : 10), true, true, true);
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					this._context.IncrementFact(Facts.Vulnerability, 10, true, true, true);
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
					List<NpcPlayerInfo> playersOutsideDetectionRange = this._context.PlayersOutsideDetectionRange;
					NpcPlayerInfo npcPlayerInfo = new NpcPlayerInfo()
					{
						Player = initiatorPlayer,
						Time = UnityEngine.Time.time
					};
					playersOutsideDetectionRange.Add(npcPlayerInfo);
				}
			}
			if (this._context.ReservedCoverPoint != null && this._context.IsFact(Facts.AtLocationCover))
			{
				this._context.SetFact(Facts.MaintainCover, true, false, true, true);
				this._context.ReservedCoverPoint.CoverIsCompromised(ConVar.AI.npc_cover_compromised_cooldown);
				this._context.ReserveCoverPoint(null);
			}
		}

		public override void OnSensation(Sensation sensation)
		{
			switch (sensation.Type)
			{
				case SensationType.Gunshot:
				{
					this.OnGunshotSensation(ref sensation);
					return;
				}
				case SensationType.ThrownWeapon:
				{
					this.OnThrownWeaponSensation(ref sensation);
					return;
				}
				case SensationType.Explosion:
				{
					this.OnExplosionSensation(ref sensation);
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private void OnThrownWeaponSensation(ref Sensation info)
		{
			this.RememberEntityOfInterest(ref info);
			if (!this._context.IsFact(Facts.CanSeeEnemy) || !this._context.IsFact(Facts.CanHearEnemy))
			{
				return;
			}
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer != null && initiatorPlayer != this._context.Body)
			{
				bool flag = false;
				foreach (NpcPlayerInfo enemyPlayersInRange in this._context.EnemyPlayersInRange)
				{
					if (!this.RememberThrownItem(ref info, enemyPlayersInRange, initiatorPlayer))
					{
						continue;
					}
					if (this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
					{
						this._context.Memory.RememberPrimaryEnemyPlayer(initiatorPlayer);
					}
					this._context.IncrementFact(Facts.Vulnerability, (this._context.IsFact(Facts.CanSeeEnemy) ? 0 : 1), true, true, true);
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
					this._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
					List<NpcPlayerInfo> playersOutsideDetectionRange = this._context.PlayersOutsideDetectionRange;
					NpcPlayerInfo npcPlayerInfo = new NpcPlayerInfo()
					{
						Player = initiatorPlayer,
						Time = UnityEngine.Time.time
					};
					playersOutsideDetectionRange.Add(npcPlayerInfo);
				}
			}
		}

		private float PathDistance(int count, ref Vector3[] path, float maxDistance)
		{
			if (count < 2)
			{
				return 0f;
			}
			Vector3 vector3 = path[0];
			float single = 0f;
			for (int i = 0; i < count; i++)
			{
				Vector3 vector31 = path[i];
				single += Vector3.Distance(vector3, vector31);
				vector3 = vector31;
				if (single > maxDistance)
				{
					return single;
				}
			}
			return single;
		}

		public bool PathDistanceIsValid(Vector3 from, Vector3 to, bool allowCloseRange = false)
		{
			float single = (from - to).sqrMagnitude;
			if (single > this.ScientistContext.Body.AiDefinition.Engagement.SqrMediumRange || !allowCloseRange && single < this.ScientistContext.Body.AiDefinition.Engagement.SqrCloseRange)
			{
				return true;
			}
			float single1 = Mathf.Sqrt(single);
			if (ScientistDomain._pathCache == null)
			{
				ScientistDomain._pathCache = new NavMeshPath();
			}
			if (NavMesh.CalculatePath(from, to, this.NavAgent.areaMask, ScientistDomain._pathCache))
			{
				int cornersNonAlloc = ScientistDomain._pathCache.GetCornersNonAlloc(ScientistDomain.pathCornerCache);
				if (ScientistDomain._pathCache.status == NavMeshPathStatus.PathComplete && cornersNonAlloc > 1 && Mathf.Abs(single1 - this.PathDistance(cornersNonAlloc, ref ScientistDomain.pathCornerCache, single1 + ConVar.AI.npc_cover_path_vs_straight_dist_max_diff)) > ConVar.AI.npc_cover_path_vs_straight_dist_max_diff)
				{
					return false;
				}
			}
			return true;
		}

		public override void Pause()
		{
			this.PauseNavigation();
		}

		public void PauseNavigation()
		{
			if (this.NavAgent != null && this.NavAgent.enabled)
			{
				this.NavAgent.enabled = false;
			}
		}

		public BaseProjectile ReloadFirearm()
		{
			BaseProjectile firearmProj = this.GetFirearmProj();
			this.ReloadFirearm(firearmProj);
			return firearmProj;
		}

		public void ReloadFirearm(BaseProjectile proj)
		{
			if (proj && this._context.IsBodyAlive() && proj.primaryMagazine.contents < proj.primaryMagazine.capacity)
			{
				base.StartCoroutine(this.ReloadHandler(proj));
			}
		}

		public AttackEntity ReloadFirearmIfEmpty()
		{
			AttackEntity firearm = this.GetFirearm();
			if (firearm)
			{
				this.ReloadFirearmIfEmpty(firearm as BaseProjectile);
			}
			return firearm;
		}

		public void ReloadFirearmIfEmpty(BaseProjectile proj)
		{
			if (proj && proj.primaryMagazine.contents <= 0)
			{
				this.ReloadFirearm(proj);
			}
		}

		public BaseProjectile ReloadFirearmProjIfEmpty()
		{
			BaseProjectile firearmProj = this.GetFirearmProj();
			this.ReloadFirearmIfEmpty(firearmProj);
			return firearmProj;
		}

		private IEnumerator ReloadHandler(BaseProjectile proj)
		{
			ScientistDomain scientistDomain = null;
			scientistDomain._context.SetFact(Facts.IsReloading, true, true, true, true);
			proj.ServerReload();
			yield return CoroutineEx.waitForSeconds(proj.reloadTime);
			scientistDomain._context.SetFact(Facts.IsReloading, false, true, true, true);
		}

		private void RememberEntityOfInterest(ref Sensation info)
		{
			if (info.UsedEntity != null)
			{
				this._context.Memory.RememberEntityOfInterest(this._context.Body, info.UsedEntity, UnityEngine.Time.time, info.UsedEntity.name);
			}
		}

		private bool RememberExplosion(ref Sensation info, NpcPlayerInfo player, BasePlayer initiator)
		{
			return false;
		}

		private bool RememberGunshot(ref Sensation info, NpcPlayerInfo player, BasePlayer initiator)
		{
			if (player.Player != initiator)
			{
				return false;
			}
			float radius = info.Radius * 0.05f;
			this._context.Memory.RememberEnemyPlayer(this._context.Body, ref player, UnityEngine.Time.time, radius, "GUNSHOT!");
			return true;
		}

		private bool RememberPlayerThatHurtUs(NpcPlayerInfo player, BasePlayer initiator)
		{
			if (player.Player != initiator)
			{
				return false;
			}
			float noiseRadius = 0f;
			NpcPlayerInfo npcPlayerInfo = player;
			BaseProjectile heldEntity = initiator.GetHeldEntity() as BaseProjectile;
			if (heldEntity != null)
			{
				noiseRadius = heldEntity.NoiseRadius * 0.1f;
				if (heldEntity.IsSilenced())
				{
					noiseRadius *= 3f;
				}
			}
			this._context.Memory.RememberEnemyPlayer(this._context.Body, ref npcPlayerInfo, UnityEngine.Time.time, noiseRadius, "HURT!");
			return true;
		}

		private bool RememberThrownItem(ref Sensation info, NpcPlayerInfo player, BasePlayer initiator)
		{
			if (player.Player != initiator)
			{
				return false;
			}
			float radius = info.Radius * 0.05f;
			this._context.Memory.RememberEnemyPlayer(this._context.Body, ref player, UnityEngine.Time.time, radius, "THROW!");
			return true;
		}

		private void RemoveAgency()
		{
			if (SingletonComponent<AiManager>.Instance == null)
			{
				return;
			}
			if (this._isRegisteredWithAgency)
			{
				this._isRegisteredWithAgency = false;
				SingletonComponent<AiManager>.Instance.HTNAgency.Remove(this._context.Body);
			}
		}

		public override void ResetState()
		{
			base.ResetState();
			this._lastFirearmUsageTime = 0f;
			this._isFiring = false;
		}

		public override void Resume()
		{
			this.ResumeNavigation();
		}

		public void ResumeNavigation()
		{
			if (this.NavAgent == null)
			{
				return;
			}
			if (!this.NavAgent.isOnNavMesh)
			{
				base.StartCoroutine(this.TryForceToNavmesh());
				return;
			}
			this.NavAgent.enabled = true;
			this.NavAgent.stoppingDistance = 1f;
			this.UpdateNavmeshOffset();
		}

		public bool SetDestination(Vector3 destination)
		{
			if (this.NavAgent == null || !this.NavAgent.isOnNavMesh)
			{
				this._context.SetFact(Facts.PathStatus, (byte)3, true, false, true);
				return false;
			}
			destination = this.ToAllowedMovementDestination(destination);
			this._context.Memory.HasTargetDestination = true;
			this._context.Memory.TargetDestination = destination;
			this._context.Domain.NavAgent.destination = destination;
			if (this.IsPathValid())
			{
				this._context.Domain.NavAgent.isStopped = false;
				this._context.SetFact(Facts.PathStatus, (byte)1, true, false, true);
				return true;
			}
			this._context.Memory.AddFailedDestination(this._context.Memory.TargetDestination);
			this._context.Domain.NavAgent.isStopped = true;
			this._context.SetFact(Facts.PathStatus, (byte)3, true, false, true);
			return false;
		}

		public override float SqrDistanceToSpawn()
		{
			return (this._context.BodyPosition - this.SpawnPosition).sqrMagnitude;
		}

		public void StopNavigating()
		{
			if (this.NavAgent != null && this.NavAgent.isOnNavMesh)
			{
				this.NavAgent.isStopped = true;
			}
			this._context.Memory.HasTargetDestination = false;
			this._context.SetFact(Facts.PathStatus, (byte)0, true, false, true);
		}

		public override void Tick(float time)
		{
			base.Tick(time);
			this.TickFirearm(time);
			this._context.Memory.Forget(this._context.Body.AiDefinition.Memory.ForgetTime);
			if (this._context.IsFact(Facts.IsIdle) || this._context.IsFact(Facts.IsDucking) || !this._context.IsFact(Facts.HasEnemyTarget) && !this._context.IsFact(Facts.NearbyAnimal) && !this._context.IsFact(Facts.NearbyExplosives))
			{
				this._navAgent.speed = this._context.Body.AiDefinition.Movement.DuckSpeed;
				return;
			}
			Vector3 body = this._context.Body.transform.forward;
			Vector3 vector3 = this._navAgent.desiredVelocity;
			float single = Vector3.Dot(body, vector3.normalized);
			if (single <= 0.5f)
			{
				this._navAgent.speed = this._context.Body.AiDefinition.Movement.WalkSpeed;
				return;
			}
			float single1 = (single - 0.5f) * 2f;
			this._navAgent.speed = Mathf.Lerp(this._context.Body.AiDefinition.Movement.WalkSpeed, this._context.Body.AiDefinition.Movement.RunSpeed, single1);
		}

		public override void TickDestinationTracker()
		{
			if (this.NavAgent == null || !this.NavAgent.isOnNavMesh)
			{
				this._context.SetFact(Facts.PathStatus, (byte)0, true, false, true);
				return;
			}
			if (!this.IsPathValid())
			{
				this._context.Memory.AddFailedDestination(this._context.Memory.TargetDestination);
				this._context.Domain.NavAgent.isStopped = true;
				this._context.Memory.HasTargetDestination = false;
				this._context.SetFact(Facts.PathStatus, (byte)3, true, false, true);
			}
			if (this._context.Memory.HasTargetDestination && this._context.Domain.NavAgent.remainingDistance <= this._context.Domain.NavAgent.stoppingDistance)
			{
				this._context.Memory.HasTargetDestination = false;
				this._context.SetFact(Facts.PathStatus, (byte)2, true, false, true);
			}
			if (this._context.Memory.HasTargetDestination && this.NavAgent.hasPath)
			{
				this._context.SetFact(Facts.PathStatus, (byte)1, true, false, true);
				return;
			}
			this._context.SetFact(Facts.PathStatus, (byte)0, true, false, true);
		}

		private void TickFirearm(float time)
		{
			if (this._context.GetFact(Facts.HasEnemyTarget) == 0 || this._isFiring || !this._context.IsBodyAlive())
			{
				return;
			}
			switch (this._context.GetFact(Facts.FirearmOrder))
			{
				case 1:
				{
					this.TickFirearm(time, 0f);
					return;
				}
				case 2:
				{
					this.TickFirearm(time, 0.2f);
					return;
				}
				case 3:
				{
					this.TickFirearm(time, 0.5f);
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private void TickFirearm(float time, float interval)
		{
			AttackEntity attackEntity = this.ReloadFirearmIfEmpty();
			if (attackEntity == null || !(attackEntity is BaseProjectile))
			{
				ScientistDomain.HoldItemOfType.SwitchToItem(this._context, ItemType.ProjectileWeapon);
			}
			if (time - this._lastFirearmUsageTime < interval)
			{
				return;
			}
			if (attackEntity == null)
			{
				return;
			}
			NpcPlayerInfo primaryEnemyPlayerTarget = this._context.GetPrimaryEnemyPlayerTarget();
			if (primaryEnemyPlayerTarget.Player == null || !primaryEnemyPlayerTarget.BodyVisible && !primaryEnemyPlayerTarget.HeadVisible)
			{
				return;
			}
			if (!this.CanUseFirearmAtRange(primaryEnemyPlayerTarget.SqrDistance))
			{
				return;
			}
			BaseProjectile baseProjectile = attackEntity as BaseProjectile;
			if (baseProjectile && baseProjectile.NextAttackTime > time)
			{
				return;
			}
			switch (this._context.GetFact(Facts.FireTactic))
			{
				case 0:
				{
					this.FireBurst(baseProjectile, time);
					return;
				}
				case 2:
				{
					this.FireFullAuto(baseProjectile, time);
					return;
				}
				default:
				{
					this.FireSingle(attackEntity, time);
					return;
				}
			}
		}

		protected override void TickReasoner(INpcReasoner reasoner, float deltaTime, float time)
		{
			reasoner.Tick(this._context.Body, deltaTime, time);
		}

		protected override void TickSensor(INpcSensor sensor, float deltaTime, float time)
		{
			sensor.Tick(this._context.Body, deltaTime, time);
		}

		public Vector3 ToAllowedMovementDestination(Vector3 destination)
		{
			if (!this.AllowedMovementDestination(destination))
			{
				Vector3 vector3 = (destination - this._context.Domain.SpawnPosition).normalized;
				destination = this._context.Domain.SpawnPosition + (vector3 * this.MovementRadius);
			}
			return destination;
		}

		private IEnumerator TryForceToNavmesh()
		{
			ScientistDomain scientistDomain = null;
			NavMeshHit navMeshHit;
			yield return null;
			int num = 0;
			float single = 1f;
			float single1 = 2f;
			if (SingletonComponent<DynamicNavMesh>.Instance != null)
			{
				while (SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
				{
					yield return CoroutineEx.waitForSecondsRealtime(single);
					single += 0.5f;
				}
			}
			single = 1f;
			while (num < 3)
			{
				if (!(scientistDomain.NavAgent != null) || scientistDomain.NavAgent.isOnNavMesh)
				{
					scientistDomain.NavAgent.enabled = true;
					scientistDomain.NavAgent.stoppingDistance = 1f;
					yield break;
				}
				if (NavMesh.SamplePosition(scientistDomain._context.Body.transform.position, out navMeshHit, scientistDomain.NavAgent.height * single1, scientistDomain.NavAgent.areaMask))
				{
					scientistDomain._context.Body.transform.position = navMeshHit.position;
					scientistDomain.NavAgent.Warp(scientistDomain._context.Body.transform.position);
					scientistDomain.NavAgent.enabled = true;
					scientistDomain.NavAgent.stoppingDistance = 1f;
					scientistDomain.UpdateNavmeshOffset();
					yield break;
				}
				yield return CoroutineEx.waitForSecondsRealtime(single);
				single1 *= 1.5f;
				num++;
			}
			int areaFromName = NavMesh.GetAreaFromName("Walkable");
			if ((scientistDomain.NavAgent.areaMask & 1 << (areaFromName & 31)) != 0)
			{
				if (scientistDomain._context.Body.transform != null && !scientistDomain._context.Body.IsDestroyed)
				{
					UnityEngine.Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[] { scientistDomain.name });
					scientistDomain._context.Body.Kill(BaseNetworkable.DestroyMode.None);
				}
				yield break;
			}
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
			scientistDomain.NavAgent.agentTypeID = settingsByIndex.agentTypeID;
			scientistDomain.NavAgent.areaMask = 1 << (areaFromName & 31);
			yield return scientistDomain.TryForceToNavmesh();
		}

		private void UpdateNavmeshOffset()
		{
			float bodyPosition = this._spawnPosition.y - this._context.BodyPosition.y;
			if (bodyPosition < 0f)
			{
				bodyPosition = Mathf.Max(bodyPosition, -0.25f);
				this.NavAgent.baseOffset = bodyPosition;
			}
		}

		public class ApplyFirearmOrder : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public ApplyFirearmOrder()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class ApplyFrustration : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public ApplyFrustration()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class ArrivedAtLocation : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public ArrivedAtLocation()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public abstract class BaseNavigateTo : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public bool RunUntilArrival;

			protected BaseNavigateTo()
			{
			}

			protected abstract Vector3 _GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context);

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				this.OnPreStart(context);
				context.ReserveCoverPoint(null);
				context.Domain.SetDestination(this._GetDestination(context));
				if (!this.RunUntilArrival)
				{
					context.OnWorldStateChangedEvent += new Rust.Ai.HTN.Scientist.ScientistContext.WorldStateChangedEvent(this.TrackWorldState);
				}
				this.OnStart(context);
			}

			protected virtual void OnPathComplete(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
			}

			protected virtual void OnPathFailed(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
			}

			protected virtual void OnPreStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
			}

			protected virtual void OnStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				switch (context.GetFact(Facts.PathStatus))
				{
					case 0:
					case 2:
					{
						ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
						base.ApplyExpectedEffects(context, task);
						context.Domain.StopNavigating();
						this.OnPathComplete(context);
						return OperatorStateType.Complete;
					}
					case 1:
					{
						if (this.RunUntilArrival)
						{
							return OperatorStateType.Running;
						}
						return OperatorStateType.Complete;
					}
					default:
					{
						context.Domain.StopNavigating();
						this.OnPathFailed(context);
						return OperatorStateType.Aborted;
					}
				}
			}

			private void TrackWorldState(Rust.Ai.HTN.Scientist.ScientistContext context, Facts fact, byte oldValue, byte newValue)
			{
				if (fact == Facts.PathStatus)
				{
					if (newValue == 2)
					{
						context.OnWorldStateChangedEvent -= new Rust.Ai.HTN.Scientist.ScientistContext.WorldStateChangedEvent(this.TrackWorldState);
						ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
						base.ApplyExpectedEffects(context, context.CurrentTask);
						context.Domain.StopNavigating();
						this.OnPathComplete(context);
						return;
					}
					if (newValue == 3)
					{
						context.OnWorldStateChangedEvent -= new Rust.Ai.HTN.Scientist.ScientistContext.WorldStateChangedEvent(this.TrackWorldState);
						ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
						context.Domain.StopNavigating();
						this.OnPathFailed(context);
					}
				}
			}
		}

		public class BlockNavigationEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public BlockNavigationEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
			}
		}

		public class BlockReloadingEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public BlockReloadingEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
			}
		}

		public class BlockShootingEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public BlockShootingEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
			}
		}

		public class CanNavigateAwayFromAnimal : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public CanNavigateAwayFromAnimal()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (!ScientistDomain.CanNavigateAwayFromAnimal.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				Vector3 destination = ScientistDomain.NavigateAwayFromAnimal.GetDestination(c);
				if (!c.Domain.AllowedMovementDestination(destination))
				{
					return false;
				}
				if ((destination - c.BodyPosition).sqrMagnitude < 0.1f)
				{
					return false;
				}
				return c.Memory.IsValid(destination);
			}
		}

		public class CanNavigateAwayFromExplosive : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public CanNavigateAwayFromExplosive()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (!ScientistDomain.CanNavigateAwayFromExplosive.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				Vector3 destination = ScientistDomain.NavigateAwayFromExplosive.GetDestination(c);
				if (!c.Domain.AllowedMovementDestination(destination))
				{
					return false;
				}
				if ((destination - c.BodyPosition).sqrMagnitude < 0.1f)
				{
					return false;
				}
				return c.Memory.IsValid(destination);
			}
		}

		public class CanNavigateToCoverLocation : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			private CoverTactic _preferredTactic;

			public CanNavigateToCoverLocation()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (!ScientistDomain.CanNavigateToCoverLocation.Try(this._preferredTactic, c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(CoverTactic tactic, Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				Vector3 coverPosition = ScientistDomain.NavigateToCover.GetCoverPosition(tactic, c);
				if (!c.Domain.AllowedMovementDestination(coverPosition))
				{
					return false;
				}
				if ((coverPosition - c.BodyPosition).sqrMagnitude < 0.1f)
				{
					return false;
				}
				return c.Memory.IsValid(coverPosition);
			}
		}

		public class CanNavigateToLastKnownPositionOfPrimaryEnemyTarget : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public CanNavigateToLastKnownPositionOfPrimaryEnemyTarget()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.HasVisitedLastKnownEnemyPlayerLocation)
				{
					return this.score;
				}
				Vector3 destination = ScientistDomain.NavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
				if (!c.Domain.AllowedMovementDestination(destination))
				{
					return 0f;
				}
				if ((destination - c.BodyPosition).sqrMagnitude < 0.1f)
				{
					return 0f;
				}
				if (!c.Memory.IsValid(destination))
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class CanNavigateToPreferredFightingRange : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			private bool CanNot;

			public CanNavigateToPreferredFightingRange()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				Vector3 preferredFightingPosition = ScientistDomain.NavigateToPreferredFightingRange.GetPreferredFightingPosition(c, false);
				if ((preferredFightingPosition - c.Body.transform.position).sqrMagnitude < 0.01f)
				{
					if (!this.CanNot)
					{
						return 0f;
					}
					return this.score;
				}
				bool flag = c.Memory.IsValid(preferredFightingPosition);
				if (flag)
				{
					flag = c.Domain.AllowedMovementDestination(preferredFightingPosition);
				}
				if (this.CanNot)
				{
					if (flag)
					{
						return 0f;
					}
					return this.score;
				}
				if (!flag)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class CanNavigateToWaypoint : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public CanNavigateToWaypoint()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				Vector3 nextWaypointPosition = ScientistDomain.NavigateToWaypoint.GetNextWaypointPosition(c);
				if (!c.Memory.IsValid(nextWaypointPosition))
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class CanRememberPrimaryEnemyTarget : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public CanRememberPrimaryEnemyTarget()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class CanThrowAtLastKnownLocation : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public CanThrowAtLastKnownLocation()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (!ScientistDomain.CanThrowAtLastKnownLocation.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (!ConVar.AI.npc_use_thrown_weapons)
				{
					return false;
				}
				if (c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return false;
				}
				if (UnityEngine.Time.time - ScientistDomain.UseThrowableWeapon.LastTimeThrown < 8f)
				{
					return false;
				}
				Vector3 destination = ScientistDomain.NavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
				if ((destination - c.BodyPosition).sqrMagnitude < 0.1f)
				{
					return false;
				}
				Vector3 eyeOffset = destination + PlayerEyes.EyeOffset;
				Vector3 player = c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.transform.position + PlayerEyes.EyeOffset;
				if ((eyeOffset - player).sqrMagnitude > 8f)
				{
					return false;
				}
				Vector3 bodyPosition = c.BodyPosition + PlayerEyes.EyeOffset;
				if (Mathf.Abs(Vector3.Dot((bodyPosition - eyeOffset).normalized, (bodyPosition - player).normalized)) < 0.75f)
				{
					return false;
				}
				if (!c.Body.IsVisible(eyeOffset, Single.PositiveInfinity) && (!c.Memory.PrimaryKnownEnemyPlayer.HeadVisibleWhenLastNoticed || c.Memory.PrimaryKnownEnemyPlayer.BodyVisibleWhenLastNoticed))
				{
					return false;
				}
				return true;
			}
		}

		public class CanUseWeaponAtCurrentRange : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public CanUseWeaponAtCurrentRange()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (!ScientistDomain.CanUseWeaponAtCurrentRange.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				AttackEntity firearm = c.Domain.GetFirearm();
				if (firearm == null)
				{
					return false;
				}
				EnemyRange fact = (EnemyRange)c.GetFact(Facts.EnemyRange);
				if (fact == EnemyRange.LongRange)
				{
					return firearm.CanUseAtLongRange;
				}
				if (fact == EnemyRange.MediumRange)
				{
					return firearm.CanUseAtMediumRange;
				}
				if (fact != EnemyRange.OutOfRange)
				{
					return true;
				}
				return firearm.CanUseAtLongRange;
			}
		}

		public class ChangeFirearmOrder : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public FirearmOrders Order;

			public ChangeFirearmOrder()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.FirearmOrder, this.Order, temporary);
					return;
				}
				context.SetFact(Facts.FirearmOrder, this.Order, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.FirearmOrder);
					return;
				}
				context.SetFact(Facts.FirearmOrder, context.GetPreviousFact(Facts.FirearmOrder), true, true, true);
			}
		}

		public class Duck : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public Duck()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				context.Body.modelState.ducked = false;
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Body.modelState.ducked = true;
				ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class DuckTimed : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			private float _duckTimeMin;

			[ApexSerialization]
			private float _duckTimeMax;

			public DuckTimed()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				context.Body.StopCoroutine(this.AsyncTimer(context, 0f));
				this.Reset(context);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Scientist.ScientistContext context, float time)
			{
				ScientistDomain.DuckTimed duckTimed = null;
				yield return CoroutineEx.waitForSeconds(time);
				duckTimed.Reset(context);
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Body.modelState.ducked = true;
				context.SetFact(Facts.IsDucking, true, true, true, true);
				ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
				if (this._duckTimeMin > this._duckTimeMax)
				{
					float single = this._duckTimeMin;
					this._duckTimeMin = this._duckTimeMax;
					this._duckTimeMax = single;
				}
				float single1 = UnityEngine.Random.@value * (this._duckTimeMax - this._duckTimeMin) + this._duckTimeMin;
				context.Body.StartCoroutine(this.AsyncTimer(context, single1));
			}

			private void Reset(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsDucking))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class FutureCoverState : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public CoverTactic Tactic;

			public FutureCoverState()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
				CoverState coverState;
				CoverState coverState1;
				CoverPoint cover = ScientistDomain.NavigateToCover.GetCover(this.Tactic, context);
				if (fromPlanner)
				{
					Rust.Ai.HTN.Scientist.ScientistContext scientistContext = context;
					if (cover == null)
					{
						coverState1 = CoverState.None;
					}
					else
					{
						coverState1 = (cover.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full);
					}
					scientistContext.PushFactChangeDuringPlanning(Facts.CoverState, coverState1, temporary);
					return;
				}
				Rust.Ai.HTN.Scientist.ScientistContext scientistContext1 = context;
				if (cover == null)
				{
					coverState = CoverState.None;
				}
				else
				{
					coverState = (cover.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full);
				}
				scientistContext1.SetFact(Facts.CoverState, coverState, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.CoverState);
					return;
				}
				context.SetFact(Facts.CoverState, context.GetPreviousFact(Facts.CoverState), true, true, true);
			}
		}

		public class HasFirearmOrder : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public FirearmOrders Order;

			public HasFirearmOrder()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				return this.score;
			}
		}

		public class HasItem : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public HasItem()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				float single;
				c.Body.inventory.AllItemsNoAlloc(ref BaseNpcContext.InventoryLookupCache);
				List<Item>.Enumerator enumerator = BaseNpcContext.InventoryLookupCache.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Item current = enumerator.Current;
						if (this.Value == ItemType.HealingItem && current.info.category == ItemCategory.Medical)
						{
							single = this.score;
							return single;
						}
						else if (this.Value == ItemType.MeleeWeapon && current.info.category == ItemCategory.Weapon && current.GetHeldEntity() is BaseMelee)
						{
							single = this.score;
							return single;
						}
						else if (this.Value == ItemType.ProjectileWeapon && current.info.category == ItemCategory.Weapon && current.GetHeldEntity() is BaseProjectile)
						{
							single = this.score;
							return single;
						}
						else if (this.Value == ItemType.ThrowableWeapon && current.info.category == ItemCategory.Weapon && current.GetHeldEntity() is ThrownWeapon)
						{
							single = this.score;
							return single;
						}
						else if (this.Value != ItemType.LightSourceItem || current.info.category != ItemCategory.Tool)
						{
							if (this.Value != ItemType.ResearchItem || current.info.category != ItemCategory.Tool)
							{
								continue;
							}
							single = this.score;
							return single;
						}
						else
						{
							single = this.score;
							return single;
						}
					}
					return 0f;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return single;
			}
		}

		public class HasWorldState : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public HasWorldState()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.GetWorldState(this.Fact) != this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class HasWorldStateAmmo : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public AmmoState Value;

			public HasWorldStateAmmo()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.GetWorldState(Facts.AmmoState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class HasWorldStateBool : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public HasWorldStateBool()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				object obj;
				if (this.Value)
				{
					obj = 1;
				}
				else
				{
					obj = null;
				}
				byte num = (byte)obj;
				if (c.GetWorldState(this.Fact) != num)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class HasWorldStateCoverState : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public CoverState Value;

			public HasWorldStateCoverState()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.GetWorldState(Facts.CoverState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class HasWorldStateEnemyRange : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public EnemyRange Value;

			public HasWorldStateEnemyRange()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.GetWorldState(Facts.EnemyRange) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class HasWorldStateGreaterThan : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public HasWorldStateGreaterThan()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.GetWorldState(this.Fact) <= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class HasWorldStateHealth : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public HealthState Value;

			public HasWorldStateHealth()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.GetWorldState(Facts.HealthState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class HasWorldStateLessThan : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public HasWorldStateLessThan()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.GetWorldState(this.Fact) >= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class HealEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public HealthState Health;

			public HealEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.HealthState, this.Health, temporary);
					return;
				}
				context.SetFact(Facts.HealthState, this.Health, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.HealthState);
					return;
				}
				context.SetFact(Facts.HealthState, context.GetPreviousFact(Facts.HealthState), true, true, true);
			}
		}

		public class HoldItemOfType : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			private ItemType _item;

			[ApexSerialization]
			private float _switchTime;

			public HoldItemOfType()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				ScientistDomain.HoldItemOfType.SwitchToItem(context, (ItemType)context.GetPreviousFact(Facts.HeldItemType));
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				ScientistDomain.HoldItemOfType.SwitchToItem(context, this._item);
				context.Body.StartCoroutine(this.WaitAsync(context));
			}

			public static void SwitchToItem(Rust.Ai.HTN.Scientist.ScientistContext context, ItemType _item)
			{
				context.Body.inventory.AllItemsNoAlloc(ref BaseNpcContext.InventoryLookupCache);
				foreach (Item inventoryLookupCache in BaseNpcContext.InventoryLookupCache)
				{
					if (_item == ItemType.HealingItem && inventoryLookupCache.info.category == ItemCategory.Medical && inventoryLookupCache.CanBeHeld())
					{
						context.Body.UpdateActiveItem(inventoryLookupCache.uid);
						context.SetFact(Facts.HeldItemType, _item, true, true, true);
						return;
					}
					else if (_item == ItemType.MeleeWeapon && inventoryLookupCache.info.category == ItemCategory.Weapon && inventoryLookupCache.GetHeldEntity() is BaseMelee)
					{
						context.Body.UpdateActiveItem(inventoryLookupCache.uid);
						context.SetFact(Facts.HeldItemType, _item, true, true, true);
						return;
					}
					else if (_item == ItemType.ProjectileWeapon && inventoryLookupCache.info.category == ItemCategory.Weapon && inventoryLookupCache.GetHeldEntity() is BaseProjectile)
					{
						context.Body.UpdateActiveItem(inventoryLookupCache.uid);
						context.SetFact(Facts.HeldItemType, _item, true, true, true);
						return;
					}
					else if (_item == ItemType.ThrowableWeapon && inventoryLookupCache.info.category == ItemCategory.Weapon && inventoryLookupCache.GetHeldEntity() is ThrownWeapon)
					{
						context.Body.UpdateActiveItem(inventoryLookupCache.uid);
						context.SetFact(Facts.HeldItemType, _item, true, true, true);
						return;
					}
					else if (_item != ItemType.LightSourceItem || inventoryLookupCache.info.category != ItemCategory.Tool || !inventoryLookupCache.CanBeHeld())
					{
						if (_item != ItemType.ResearchItem || inventoryLookupCache.info.category != ItemCategory.Tool || !inventoryLookupCache.CanBeHeld())
						{
							continue;
						}
						context.Body.UpdateActiveItem(inventoryLookupCache.uid);
						context.SetFact(Facts.HeldItemType, _item, true, true, true);
						return;
					}
					else
					{
						context.Body.UpdateActiveItem(inventoryLookupCache.uid);
						context.SetFact(Facts.HeldItemType, _item, true, true, true);
						return;
					}
				}
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsWaiting))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator WaitAsync(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				ScientistDomain.HoldItemOfType holdItemOfType = null;
				context.SetFact(Facts.IsWaiting, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(holdItemOfType._switchTime);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}
		}

		public class HoldItemOfTypeEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public HoldItemOfTypeEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.HeldItemType, this.Value, temporary);
					return;
				}
				context.SetFact(Facts.HeldItemType, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.HeldItemType);
					return;
				}
				context.SetFact(Facts.HeldItemType, context.GetPreviousFact(Facts.HeldItemType), true, true, true);
			}
		}

		public class HoldLocation : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public HoldLocation()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				return OperatorStateType.Running;
			}
		}

		public class HoldLocationTimed : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			private float _duckTimeMin;

			[ApexSerialization]
			private float _duckTimeMax;

			public HoldLocationTimed()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Scientist.ScientistContext context, float time)
			{
				yield return CoroutineEx.waitForSeconds(time);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
				context.SetFact(Facts.IsWaiting, true, true, true, true);
				if (this._duckTimeMin > this._duckTimeMax)
				{
					float single = this._duckTimeMin;
					this._duckTimeMin = this._duckTimeMax;
					this._duckTimeMax = single;
				}
				float single1 = UnityEngine.Random.@value * (this._duckTimeMax - this._duckTimeMin) + this._duckTimeMin;
				context.Body.StartCoroutine(this.AsyncTimer(context, single1));
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsWaiting))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class Idle_JustStandAround : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public Idle_JustStandAround()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsIdle, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				this.ResetWorldState(context);
				context.SetFact(Facts.IsIdle, true, true, true, true);
				context.Domain.ReloadFirearm();
			}

			private void ResetWorldState(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.SetFact(Facts.IsNavigating, false, true, true, true);
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				return OperatorStateType.Running;
			}
		}

		public class IsHoldingItem : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public IsHoldingItem()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.GetWorldState(Facts.HeldItemType) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class IsNavigatingEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public IsNavigatingEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.IsNavigating, 1, temporary);
					return;
				}
				context.PreviousWorldState[5] = context.WorldState[5];
				context.WorldState[5] = 1;
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.IsNavigating);
					return;
				}
				context.WorldState[5] = context.PreviousWorldState[5];
			}
		}

		public class IsNavigationAllowed : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public IsNavigationAllowed()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (!ScientistDomain.IsNavigationBlocked.CanNavigate(c))
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class IsNavigationBlocked : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public IsNavigationBlocked()
			{
			}

			public static bool CanNavigate(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (c.Domain.NavAgent != null && c.Domain.NavAgent.isOnNavMesh)
				{
					return true;
				}
				return false;
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				if (ScientistDomain.IsNavigationBlocked.CanNavigate(c))
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class IsNotNavigatingEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public IsNotNavigatingEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
				ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, fromPlanner, temporary);
			}

			public static void ApplyStatic(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.IsNavigating, (byte)0, temporary);
					return;
				}
				context.PreviousWorldState[5] = context.WorldState[5];
				context.WorldState[5] = 0;
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.IsNavigating);
					return;
				}
				context.WorldState[5] = context.PreviousWorldState[5];
			}
		}

		public class IsReloadingAllowed : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public IsReloadingAllowed()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				return this.score;
			}
		}

		public class IsReloadingBlocked : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public IsReloadingBlocked()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				return 0f;
			}
		}

		public class IsShootingAllowed : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public IsShootingAllowed()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				return this.score;
			}
		}

		public class IsShootingBlocked : ContextualScorerBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public IsShootingBlocked()
			{
			}

			public override float Score(Rust.Ai.HTN.Scientist.ScientistContext c)
			{
				return 0f;
			}
		}

		public class LookAround : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			private float _lookAroundTime;

			public LookAround()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsLookingAround, true, true, true, true);
				context.Body.StartCoroutine(this.LookAroundAsync(context));
			}

			private IEnumerator LookAroundAsync(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				ScientistDomain.LookAround lookAround = null;
				yield return CoroutineEx.waitForSeconds(lookAround._lookAroundTime);
				if (context.IsFact(Facts.CanSeeEnemy))
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsLookingAround))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class NavigateAwayFromAnimal : ScientistDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsAvoidingAnimalOnComplete;

			public NavigateAwayFromAnimal()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				return ScientistDomain.NavigateAwayFromAnimal.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				NavMeshHit navMeshHit;
				if (context.Memory.PrimaryKnownAnimal.Animal != null)
				{
					Vector3 bodyPosition = context.BodyPosition - context.Memory.PrimaryKnownAnimal.Animal.transform.position;
					Vector3 vector3 = bodyPosition.normalized;
					if (NavMesh.FindClosestEdge(context.BodyPosition + (vector3 * 10f), out navMeshHit, context.Domain.NavAgent.areaMask))
					{
						context.Memory.LastClosestEdgeNormal = navMeshHit.normal;
						return navMeshHit.position;
					}
				}
				return context.Body.transform.position;
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				if (this.DisableIsAvoidingAnimalOnComplete)
				{
					context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsAvoidingAnimal, true, true, true, true);
			}
		}

		public class NavigateAwayFromExplosive : ScientistDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsAvoidingExplosiveOnComplete;

			public NavigateAwayFromExplosive()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				return ScientistDomain.NavigateAwayFromExplosive.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				NavMeshHit navMeshHit;
				BaseEntity entity = null;
				Vector3 vector3 = Vector3.zero;
				float single = Single.MaxValue;
				for (int i = 0; i < context.Memory.KnownTimedExplosives.Count; i++)
				{
					BaseNpcMemory.EntityOfInterestInfo item = context.Memory.KnownTimedExplosives[i];
					if (item.Entity != null)
					{
						Vector3 bodyPosition = context.BodyPosition - item.Entity.transform.position;
						float single1 = bodyPosition.sqrMagnitude;
						if (single1 < single)
						{
							vector3 = bodyPosition;
							single = single1;
							entity = item.Entity;
						}
					}
				}
				if (entity != null)
				{
					vector3.Normalize();
					if (NavMesh.FindClosestEdge(context.BodyPosition + (vector3 * 10f), out navMeshHit, context.Domain.NavAgent.areaMask))
					{
						context.Memory.LastClosestEdgeNormal = navMeshHit.normal;
						return navMeshHit.position;
					}
				}
				return context.Body.transform.position;
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				if (this.DisableIsAvoidingExplosiveOnComplete)
				{
					context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsAvoidingExplosive, true, true, true, true);
			}
		}

		public class NavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer : ScientistDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public NavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				return ScientistDomain.NavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetContinuousDestinationFromBody(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				NavMeshHit navMeshHit;
				if (context.Memory.LastClosestEdgeNormal.sqrMagnitude < 0.01f)
				{
					return context.Body.transform.position;
				}
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (primaryKnownEnemyPlayer.PlayerInfo.Player != null)
				{
					Vector3 body = context.Body.estimatedVelocity.normalized;
					if (body.sqrMagnitude < 0.01f)
					{
						body = context.Body.estimatedVelocity.normalized;
					}
					if (body.sqrMagnitude < 0.01f)
					{
						body = primaryKnownEnemyPlayer.LastKnownHeading;
					}
					if (NavMesh.FindClosestEdge(context.Body.transform.position + (body * 2f), out navMeshHit, context.Domain.NavAgent.areaMask))
					{
						if (Vector3.Dot(context.Memory.LastClosestEdgeNormal, navMeshHit.normal) > 0.9f)
						{
							return navMeshHit.position;
						}
						context.Memory.LastClosestEdgeNormal = navMeshHit.normal;
						return navMeshHit.position + (navMeshHit.normal * 0.25f);
					}
				}
				return context.Body.transform.position;
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				NavMeshHit navMeshHit;
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (!(primaryKnownEnemyPlayer.PlayerInfo.Player != null) || !NavMesh.FindClosestEdge(primaryKnownEnemyPlayer.LastKnownPosition + (primaryKnownEnemyPlayer.LastKnownHeading * 2f), out navMeshHit, context.Domain.NavAgent.areaMask))
				{
					return context.Body.transform.position;
				}
				Vector3 vector3 = navMeshHit.position;
				context.Memory.LastClosestEdgeNormal = navMeshHit.normal;
				return vector3;
			}

			private void OnContinuePath(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				Vector3 continuousDestinationFromBody = ScientistDomain.NavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetContinuousDestinationFromBody(context);
				if ((context.Body.transform.position - continuousDestinationFromBody).sqrMagnitude <= 0.2f)
				{
					return;
				}
				this.OnPreStart(context);
				context.Domain.SetDestination(continuousDestinationFromBody);
				this.OnStart(context);
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				OperatorStateType operatorStateType = base.Tick(context, task);
				if (operatorStateType != OperatorStateType.Running)
				{
					return operatorStateType;
				}
				if (context.Domain.NavAgent.remainingDistance < context.Domain.NavAgent.stoppingDistance + 0.5f)
				{
					this.OnContinuePath(context, task);
				}
				return operatorStateType;
			}
		}

		public class NavigateToCover : ScientistDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private CoverTactic _preferredTactic;

			public NavigateToCover()
			{
			}

			private static Vector3 _GetCoverPosition(CoverTactic tactic, Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				switch (tactic)
				{
					case CoverTactic.Advance:
					{
						if (context.BestAdvanceCover != null && context.BestAdvanceCover.IsValidFor(context.Body))
						{
							context.SetFact(Facts.CoverTactic, CoverTactic.Advance, true, true, true);
							context.ReserveCoverPoint(context.BestAdvanceCover);
							return context.BestAdvanceCover.Position;
						}
						if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor(context.Body))
						{
							context.SetFact(Facts.CoverTactic, CoverTactic.Flank, true, true, true);
							context.ReserveCoverPoint(context.BestFlankCover);
							return context.BestFlankCover.Position;
						}
						if (context.BestRetreatCover == null || !context.BestRetreatCover.IsValidFor(context.Body))
						{
							break;
						}
						context.SetFact(Facts.CoverTactic, CoverTactic.Retreat, true, true, true);
						context.ReserveCoverPoint(context.BestRetreatCover);
						return context.BestRetreatCover.Position;
					}
					case CoverTactic.Retreat:
					{
						if (context.BestRetreatCover != null && context.BestRetreatCover.IsValidFor(context.Body))
						{
							context.SetFact(Facts.CoverTactic, CoverTactic.Retreat, true, true, true);
							context.ReserveCoverPoint(context.BestRetreatCover);
							return context.BestRetreatCover.Position;
						}
						if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor(context.Body))
						{
							context.SetFact(Facts.CoverTactic, CoverTactic.Flank, true, true, true);
							context.ReserveCoverPoint(context.BestFlankCover);
							return context.BestFlankCover.Position;
						}
						if (context.BestAdvanceCover == null || !context.BestAdvanceCover.IsValidFor(context.Body))
						{
							break;
						}
						context.SetFact(Facts.CoverTactic, CoverTactic.Advance, true, true, true);
						context.ReserveCoverPoint(context.BestAdvanceCover);
						return context.BestAdvanceCover.Position;
					}
					case CoverTactic.Flank:
					{
						if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor(context.Body))
						{
							context.SetFact(Facts.CoverTactic, CoverTactic.Flank, true, true, true);
							context.ReserveCoverPoint(context.BestFlankCover);
							return context.BestFlankCover.Position;
						}
						if (context.BestRetreatCover != null && context.BestRetreatCover.IsValidFor(context.Body))
						{
							context.SetFact(Facts.CoverTactic, CoverTactic.Retreat, true, true, true);
							context.ReserveCoverPoint(context.BestRetreatCover);
							return context.BestRetreatCover.Position;
						}
						if (context.BestAdvanceCover == null || !context.BestAdvanceCover.IsValidFor(context.Body))
						{
							break;
						}
						context.SetFact(Facts.CoverTactic, CoverTactic.Advance, true, true, true);
						context.ReserveCoverPoint(context.BestAdvanceCover);
						return context.BestAdvanceCover.Position;
					}
					case CoverTactic.Closest:
					{
						if (context.ClosestCover == null || !context.ClosestCover.IsValidFor(context.Body))
						{
							break;
						}
						context.SetFact(Facts.CoverTactic, CoverTactic.Closest, true, true, true);
						context.ReserveCoverPoint(context.ClosestCover);
						return context.ClosestCover.Position;
					}
				}
				return context.Body.transform.position;
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				return ScientistDomain.NavigateToCover.GetCoverPosition(this._preferredTactic, context);
			}

			public static CoverPoint GetCover(CoverTactic tactic, Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				switch (tactic)
				{
					case CoverTactic.Advance:
					{
						if (context.BestAdvanceCover != null && context.BestAdvanceCover.IsValidFor(context.Body))
						{
							return context.BestAdvanceCover;
						}
						if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor(context.Body))
						{
							return context.BestFlankCover;
						}
						if (context.BestRetreatCover == null || !context.BestRetreatCover.IsValidFor(context.Body))
						{
							break;
						}
						return context.BestRetreatCover;
					}
					case CoverTactic.Retreat:
					{
						if (context.BestRetreatCover != null && context.BestRetreatCover.IsValidFor(context.Body))
						{
							return context.BestRetreatCover;
						}
						if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor(context.Body))
						{
							return context.BestFlankCover;
						}
						if (context.BestAdvanceCover == null || !context.BestAdvanceCover.IsValidFor(context.Body))
						{
							break;
						}
						return context.BestAdvanceCover;
					}
					case CoverTactic.Flank:
					{
						if (context.BestFlankCover != null && context.BestFlankCover.IsValidFor(context.Body))
						{
							return context.BestFlankCover;
						}
						if (context.BestRetreatCover != null && context.BestRetreatCover.IsValidFor(context.Body))
						{
							return context.BestRetreatCover;
						}
						if (context.BestAdvanceCover == null || !context.BestAdvanceCover.IsValidFor(context.Body))
						{
							break;
						}
						return context.BestAdvanceCover;
					}
					case CoverTactic.Closest:
					{
						if (context.ClosestCover == null || !context.ClosestCover.IsValidFor(context.Body))
						{
							break;
						}
						return context.ClosestCover;
					}
				}
				return null;
			}

			public static Vector3 GetCoverPosition(CoverTactic tactic, Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				NavMeshHit navMeshHit;
				if (UnityEngine.Time.time - context.Memory.CachedCoverDestinationTime < 0.01f)
				{
					return context.Memory.CachedCoverDestination;
				}
				Vector3 vector3 = ScientistDomain.NavigateToCover._GetCoverPosition(tactic, context);
				Vector3 vector31 = vector3;
				for (int i = 0; i < 10; i++)
				{
					bool flag = false;
					if (NavMesh.FindClosestEdge(vector31, out navMeshHit, context.Domain.NavAgent.areaMask))
					{
						Vector3 vector32 = navMeshHit.position;
						if (context.Memory.IsValid(vector32))
						{
							context.Memory.CachedCoverDestination = vector32;
							context.Memory.CachedCoverDestinationTime = UnityEngine.Time.time;
							return vector32;
						}
						flag = true;
					}
					if (NavMesh.SamplePosition(vector31, out navMeshHit, 2f * context.Domain.NavAgent.height, context.Domain.NavAgent.areaMask))
					{
						Vector3 allowedMovementDestination = context.Domain.ToAllowedMovementDestination(navMeshHit.position);
						if (context.Memory.IsValid(allowedMovementDestination))
						{
							context.Memory.CachedCoverDestination = allowedMovementDestination;
							context.Memory.CachedCoverDestinationTime = UnityEngine.Time.time;
							return allowedMovementDestination;
						}
						flag = true;
					}
					if (!flag)
					{
						context.Memory.AddFailedDestination(vector31);
					}
					Vector2 vector2 = UnityEngine.Random.insideUnitCircle * 5f;
					vector31 = vector3 + new Vector3(vector2.x, 0f, vector2.y);
				}
				return context.BodyPosition;
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.CoverTactic, CoverTactic.None, true, true, true);
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.CoverTactic, CoverTactic.None, true, true, true);
			}
		}

		public class NavigateToLastKnownLocationOfPrimaryEnemyPlayer : ScientistDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public NavigateToLastKnownLocationOfPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				return ScientistDomain.NavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				NavMeshHit navMeshHit;
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (primaryKnownEnemyPlayer.PlayerInfo.Player != null && !context.HasVisitedLastKnownEnemyPlayerLocation && NavMesh.FindClosestEdge(primaryKnownEnemyPlayer.LastKnownPosition, out navMeshHit, context.Domain.NavAgent.areaMask))
				{
					return navMeshHit.position;
				}
				return context.Body.transform.position;
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
				context.HasVisitedLastKnownEnemyPlayerLocation = true;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
				context.HasVisitedLastKnownEnemyPlayerLocation = false;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}
		}

		public class NavigateToPositionWhereWeLastSawPrimaryEnemyPlayer : ScientistDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public NavigateToPositionWhereWeLastSawPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				return ScientistDomain.NavigateToPositionWhereWeLastSawPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				NavMeshHit navMeshHit;
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (!(primaryKnownEnemyPlayer.PlayerInfo.Player != null) || !NavMesh.FindClosestEdge(primaryKnownEnemyPlayer.OurLastPositionWhenLastSeen, out navMeshHit, context.Domain.NavAgent.areaMask))
				{
					return context.Body.transform.position;
				}
				return context.Domain.ToAllowedMovementDestination(navMeshHit.position);
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}
		}

		public class NavigateToPreferredFightingRange : ScientistDomain.BaseNavigateTo
		{
			public NavigateToPreferredFightingRange()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				return ScientistDomain.NavigateToPreferredFightingRange.GetPreferredFightingPosition(context, false);
			}

			public static Vector3 GetPreferredFightingPosition(Rust.Ai.HTN.Scientist.ScientistContext context, bool snapToAllowedRange = true)
			{
				Vector3 player;
				float single;
				NavMeshHit navMeshHit;
				if (UnityEngine.Time.time - context.Memory.CachedPreferredDistanceDestinationTime < 0.01f)
				{
					return context.Memory.CachedPreferredDistanceDestination;
				}
				NpcPlayerInfo primaryEnemyPlayerTarget = context.GetPrimaryEnemyPlayerTarget();
				if (primaryEnemyPlayerTarget.Player != null)
				{
					Vector3 bodyPosition = context.BodyPosition;
					if (context.GetFact(Facts.Frustration) > ConVar.AI.npc_htn_player_frustration_threshold)
					{
						AttackEntity firearm = context.Domain.GetFirearm();
						float preferredRange = PreferredFightingRangeReasoner.GetPreferredRange(context, ref primaryEnemyPlayerTarget, firearm);
						if (primaryEnemyPlayerTarget.SqrDistance >= preferredRange * preferredRange)
						{
							player = primaryEnemyPlayerTarget.Player.transform.position - context.Body.transform.position;
							single = player.magnitude;
							player.Normalize();
						}
						else
						{
							player = context.Body.transform.position - primaryEnemyPlayerTarget.Player.transform.position;
							single = player.magnitude;
							player.Normalize();
						}
						float single1 = single - preferredRange;
						bodyPosition = context.Body.transform.position + (player * single1);
					}
					else
					{
						bodyPosition = ScientistDomain.NavigateToCover.GetCoverPosition(CoverTactic.Closest, context);
					}
					Vector3 vector3 = bodyPosition;
					for (int i = 0; i < 10; i++)
					{
						if (!NavMesh.SamplePosition(vector3 + (Vector3.up * 0.1f), out navMeshHit, 2f * context.Domain.NavAgent.height, -1))
						{
							context.Memory.AddFailedDestination(vector3);
						}
						else
						{
							Vector3 vector31 = navMeshHit.position;
							if (snapToAllowedRange)
							{
								context.Domain.ToAllowedMovementDestination(vector31);
							}
							if (context.Memory.IsValid(vector31))
							{
								context.Memory.CachedPreferredDistanceDestination = vector31;
								context.Memory.CachedPreferredDistanceDestinationTime = UnityEngine.Time.time;
								return vector31;
							}
						}
						Vector2 vector2 = UnityEngine.Random.insideUnitCircle * 5f;
						vector3 = bodyPosition + new Vector3(vector2.x, 0f, vector2.y);
					}
				}
				return context.Body.transform.position;
			}
		}

		public class NavigateToWaypoint : ScientistDomain.BaseNavigateTo
		{
			public NavigateToWaypoint()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				return ScientistDomain.NavigateToWaypoint.GetNextWaypointPosition(context);
			}

			public static Vector3 GetNextWaypointPosition(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				return context.Body.transform.position + (Vector3.forward * 10f);
			}
		}

		public delegate void OnPlanAborted(ScientistDomain domain);

		public delegate void OnPlanCompleted(ScientistDomain domain);

		public class ReloadFirearmOperator : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public ReloadFirearmOperator()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Domain.ReloadFirearm();
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsReloading))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class Stand : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public Stand()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				context.Body.StopCoroutine(this.AsyncTimer(context, 0f));
				this.Reset(context);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Scientist.ScientistContext context, float time)
			{
				context.SetFact(Facts.IsStandingUp, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(time);
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
				yield return CoroutineEx.waitForSeconds(time);
				context.SetFact(Facts.IsStandingUp, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Body.StartCoroutine(this.AsyncTimer(context, 0.3f));
			}

			private void Reset(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
				context.SetFact(Facts.IsStandingUp, false, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsStandingUp))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class StopMoving : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public StopMoving()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				ScientistDomain.IsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class TimeBlockNavigationEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			[FriendlyName("Time (Seconds)")]
			public float Time;

			public TimeBlockNavigationEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
			}
		}

		public class UnblockNavigationEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public UnblockNavigationEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
			}
		}

		public class UnblockReloadingEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public UnblockReloadingEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
			}
		}

		public class UnblockShootingEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			public UnblockShootingEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
			}
		}

		public class UseMedicalTool : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public HealthState Health;

			public UseMedicalTool()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsApplyingMedical, false, true, true, true);
				ScientistDomain.HoldItemOfType.SwitchToItem(context, (ItemType)context.GetPreviousFact(Facts.HeldItemType));
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				context.Body.StartCoroutine(this.UseItem(context));
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsApplyingMedical))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator UseItem(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				ScientistDomain.UseMedicalTool useMedicalTool = null;
				Item activeItem = context.Body.GetActiveItem();
				if (activeItem != null)
				{
					MedicalTool heldEntity = activeItem.GetHeldEntity() as MedicalTool;
					if (heldEntity != null)
					{
						context.SetFact(Facts.IsApplyingMedical, true, true, true, true);
						heldEntity.ServerUse();
						if (useMedicalTool.Health == HealthState.FullHealth)
						{
							context.Body.Heal(context.Body.MaxHealth());
						}
						yield return CoroutineEx.waitForSeconds(heldEntity.repeatDelay * 4f);
					}
				}
				context.SetFact(Facts.IsApplyingMedical, false, true, true, true);
				ScientistDomain.HoldItemOfType.SwitchToItem(context, context.GetPreviousFact(Facts.HeldItemType));
			}
		}

		public class UseThrowableWeapon : OperatorBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			private NpcOrientation _orientation;

			public static float LastTimeThrown;

			static UseThrowableWeapon()
			{
			}

			public UseThrowableWeapon()
			{
			}

			public override void Abort(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
				ScientistDomain.HoldItemOfType.SwitchToItem(context, (ItemType)context.GetPreviousFact(Facts.HeldItemType));
			}

			public override void Execute(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				if (context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player != null)
				{
					context.Body.StartCoroutine(this.UseItem(context));
				}
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Scientist.ScientistContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsThrowingWeapon))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator UseItem(Rust.Ai.HTN.Scientist.ScientistContext context)
			{
				ScientistDomain.UseThrowableWeapon useThrowableWeapon = null;
				Item activeItem = context.Body.GetActiveItem();
				if (activeItem != null)
				{
					ThrownWeapon heldEntity = activeItem.GetHeldEntity() as ThrownWeapon;
					if (heldEntity != null)
					{
						context.SetFact(Facts.IsThrowingWeapon, true, true, true, true);
						ScientistDomain.UseThrowableWeapon.LastTimeThrown = UnityEngine.Time.time;
						context.OrientationType = useThrowableWeapon._orientation;
						context.Body.ForceOrientationTick();
						yield return null;
						heldEntity.ServerThrow(context.Memory.PrimaryKnownEnemyPlayer.LastKnownPosition);
						yield return null;
					}
					heldEntity = null;
				}
				context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
				ScientistDomain.HoldItemOfType.SwitchToItem(context, ItemType.ProjectileWeapon);
			}
		}

		public class WorldStateBoolEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public WorldStateBoolEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class WorldStateEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public WorldStateEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class WorldStateIncrementEffect : EffectBase<Rust.Ai.HTN.Scientist.ScientistContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public WorldStateIncrementEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					byte num = context.PeekFactChangeDuringPlanning(this.Fact);
					context.PushFactChangeDuringPlanning(this.Fact, (int)(num + this.Value), temporary);
					return;
				}
				context.SetFact(this.Fact, (int)(context.GetFact(this.Fact) + this.Value), true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Scientist.ScientistContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}
	}
}