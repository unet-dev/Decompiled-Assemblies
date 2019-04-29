using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using ConVar;
using Rust.Ai;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistJunkpile.Reasoners;
using Rust.Ai.HTN.ScientistJunkpile.Sensors;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.ScientistJunkpile
{
	public class ScientistJunkpileDomain : HTNDomain
	{
		[ReadOnly]
		[SerializeField]
		private bool _isRegisteredWithAgency;

		[ReadOnly]
		[SerializeField]
		private static List<ScientistJunkpileDomain> _allJunkpileNPCs;

		[Header("Context")]
		[SerializeField]
		private ScientistJunkpileContext _context;

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
				TickFrequency = 1f
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
				TickFrequency = 0.25f
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
			new EnemyPlayerMarkTooCloseReasoner()
			{
				TickFrequency = 0.1f
			},
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
			},
			new FollowWaypointsReasoner()
			{
				TickFrequency = 0.125f
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

		private ScientistJunkpileDefinition _scientistJunkpileDefinition;

		private Vector3 missOffset;

		private float missToHeadingAlignmentTime;

		private float repeatMissTime;

		private bool recalculateMissOffset = true;

		private bool isMissing;

		private static Vector3[] pathCornerCache;

		private static NavMeshPath _pathCache;

		public ScientistJunkpileDomain.OnPlanAborted OnPlanAbortedEvent;

		public ScientistJunkpileDomain.OnPlanCompleted OnPlanCompletedEvent;

		public static List<ScientistJunkpileDomain> AllJunkpileNPCs
		{
			get
			{
				return ScientistJunkpileDomain._allJunkpileNPCs;
			}
		}

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

		public ScientistJunkpileContext ScientistContext
		{
			get
			{
				return this._context;
			}
		}

		public ScientistJunkpileDefinition ScientistDefinition
		{
			get
			{
				if (this._scientistJunkpileDefinition == null)
				{
					this._scientistJunkpileDefinition = this._context.Body.AiDefinition as ScientistJunkpileDefinition;
				}
				return this._scientistJunkpileDefinition;
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

		static ScientistJunkpileDomain()
		{
			ScientistJunkpileDomain.pathCornerCache = new Vector3[128];
			ScientistJunkpileDomain._pathCache = null;
		}

		public ScientistJunkpileDomain()
		{
		}

		protected override void AbortPlan()
		{
			base.AbortPlan();
			ScientistJunkpileDomain.OnPlanAborted onPlanAbortedEvent = this.OnPlanAbortedEvent;
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
			this._context.SetFact(Facts.IsUsingTool, 0, false, true, true);
			this._context.SetFact(Facts.IsNavigating, 0, false, true, true);
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

		protected override bool CanTickReasoner(float deltaTime, INpcReasoner reasoner)
		{
			if (this._context.Memory.MarkedEnemies.Count != 0 || !(this._context.Domain.ScientistContext.Memory.PrimaryKnownAnimal.Animal == null))
			{
				return deltaTime >= reasoner.TickFrequency;
			}
			return deltaTime >= reasoner.TickFrequency * 4f;
		}

		protected override bool CanTickSensor(float deltaTime, INpcSensor sensor)
		{
			if (this._context.Memory.MarkedEnemies.Count != 0 || !(this._context.Domain.ScientistContext.Memory.PrimaryKnownAnimal.Animal == null))
			{
				return deltaTime >= sensor.TickFrequency;
			}
			return deltaTime >= sensor.TickFrequency * 4f;
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
			ScientistJunkpileDomain.OnPlanCompleted onPlanCompletedEvent = this.OnPlanCompletedEvent;
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
			this._context.SetFact(Facts.IsUsingTool, 0, false, true, true);
			this._context.SetFact(Facts.IsNavigating, 0, false, true, true);
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
			ScientistJunkpileDomain scientistJunkpileDomain = null;
			scientistJunkpileDomain._isFiring = true;
			scientistJunkpileDomain._lastFirearmUsageTime = startTime + triggerDownInterval + proj.attackSpacing;
			scientistJunkpileDomain._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
			do
			{
				if (UnityEngine.Time.time - startTime >= triggerDownInterval || !scientistJunkpileDomain._context.IsBodyAlive() || !scientistJunkpileDomain._context.IsFact(Facts.CanSeeEnemy) || scientistJunkpileDomain._context.Domain.ScientistContext.Memory.PrimaryKnownAnimal.Animal == null)
				{
					break;
				}
				if (scientistJunkpileDomain._context.EnemyPlayersInLineOfSight.Count > 3)
				{
					proj.ServerUse((1f + UnityEngine.Random.@value * 0.5f) * ConVar.AI.npc_htn_player_base_damage_modifier);
				}
				else if (!(scientistJunkpileDomain._context.PrimaryEnemyPlayerInLineOfSight.Player != null) || scientistJunkpileDomain._context.PrimaryEnemyPlayerInLineOfSight.Player.healthFraction >= 0.2f)
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
			scientistJunkpileDomain._isFiring = false;
		}

		public override void Initialize(BaseEntity body)
		{
			if (this._aiClient == null || this._aiClient.ai == null || this._aiClient.ai.id != AINameMap.HTNDomainScientistJunkpile)
			{
				this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainScientistJunkpile, this);
			}
			if (this._context == null || this._context.Body != body)
			{
				this._context = new ScientistJunkpileContext(body as HTNPlayer, this);
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
				if (ScientistJunkpileDomain._allJunkpileNPCs == null)
				{
					ScientistJunkpileDomain._allJunkpileNPCs = new List<ScientistJunkpileDomain>();
				}
				ScientistJunkpileDomain._allJunkpileNPCs.Add(this);
				if (this._context.Junkpile != null)
				{
					this._context.Junkpile.AddNpc(this._context.Body);
				}
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
			if (this.ReducedLongRangeAccuracy && single > this._context.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(attackEntity))
			{
				fact *= 0.5f;
			}
			if (this._context.PrimaryEnemyPlayerInLineOfSight.Player != null && (this._context.PrimaryEnemyPlayerInLineOfSight.Player.modelState.jumped || !this._context.PrimaryEnemyPlayerInLineOfSight.BodyVisible && this._context.PrimaryEnemyPlayerInLineOfSight.HeadVisible))
			{
				fact *= 0.5f;
			}
			return this.GetMissVector(heading, target, origin, ConVar.AI.npc_deliberate_miss_to_hit_alignment_time * 1.1f, fact * ConVar.AI.npc_alertness_to_aim_modifier);
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
				this._context.Memory.MarkEnemy(initiatorPlayer);
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
			if (ScientistJunkpileDomain._pathCache == null)
			{
				ScientistJunkpileDomain._pathCache = new NavMeshPath();
			}
			if (NavMesh.CalculatePath(from, to, this.NavAgent.areaMask, ScientistJunkpileDomain._pathCache))
			{
				int cornersNonAlloc = ScientistJunkpileDomain._pathCache.GetCornersNonAlloc(ScientistJunkpileDomain.pathCornerCache);
				if (ScientistJunkpileDomain._pathCache.status == NavMeshPathStatus.PathComplete && cornersNonAlloc > 1 && Mathf.Abs(single1 - this.PathDistance(cornersNonAlloc, ref ScientistJunkpileDomain.pathCornerCache, single1 + ConVar.AI.npc_cover_path_vs_straight_dist_max_diff)) > ConVar.AI.npc_cover_path_vs_straight_dist_max_diff)
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
			ScientistJunkpileDomain scientistJunkpileDomain = null;
			scientistJunkpileDomain._context.SetFact(Facts.IsReloading, true, true, true, true);
			proj.ServerReload();
			yield return CoroutineEx.waitForSeconds(proj.reloadTime);
			scientistJunkpileDomain._context.SetFact(Facts.IsReloading, false, true, true, true);
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
				List<ScientistJunkpileDomain> scientistJunkpileDomains = ScientistJunkpileDomain._allJunkpileNPCs;
				if (scientistJunkpileDomains == null)
				{
					return;
				}
				scientistJunkpileDomains.Remove(this);
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
			if (this._isFiring || !this._context.IsBodyAlive())
			{
				return;
			}
			if (this._context.IsFact(Facts.IsUsingTool) && TOD_Sky.Instance != null && !this._context.IsFact(Facts.HasEnemyTarget) && this._context.Domain.ScientistContext.Memory.PrimaryKnownAnimal.Animal == null)
			{
				ItemType fact = (ItemType)this._context.GetFact(Facts.HeldItemType);
				if (TOD_Sky.Instance.IsNight && fact != ItemType.LightSourceItem)
				{
					ScientistJunkpileDomain.JunkpileHoldItemOfType.SwitchToItem(this._context, ItemType.LightSourceItem);
					return;
				}
				if (TOD_Sky.Instance.IsDay && fact != ItemType.ResearchItem)
				{
					ScientistJunkpileDomain.JunkpileHoldItemOfType.SwitchToItem(this._context, ItemType.ResearchItem);
				}
				return;
			}
			if (!this._context.IsFact(Facts.HasEnemyTarget) && this._context.Domain.ScientistContext.Memory.PrimaryKnownAnimal.Animal == null)
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
				ScientistJunkpileDomain.JunkpileHoldItemOfType.SwitchToItem(this._context, ItemType.ProjectileWeapon);
			}
			if (time - this._lastFirearmUsageTime < interval)
			{
				return;
			}
			if (attackEntity == null)
			{
				return;
			}
			AnimalInfo primaryKnownAnimal = this._context.Memory.PrimaryKnownAnimal;
			NpcPlayerInfo primaryEnemyPlayerTarget = this._context.GetPrimaryEnemyPlayerTarget();
			if (primaryEnemyPlayerTarget.Player == null || !primaryEnemyPlayerTarget.BodyVisible && !primaryEnemyPlayerTarget.HeadVisible)
			{
				if (primaryKnownAnimal.Animal == null)
				{
					return;
				}
				if (!this.CanUseFirearmAtRange(primaryKnownAnimal.SqrDistance))
				{
					return;
				}
			}
			else if (!this.CanUseFirearmAtRange(primaryEnemyPlayerTarget.SqrDistance))
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
			ScientistJunkpileDomain scientistJunkpileDomain = null;
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
				if (!(scientistJunkpileDomain.NavAgent != null) || scientistJunkpileDomain.NavAgent.isOnNavMesh)
				{
					scientistJunkpileDomain.NavAgent.enabled = true;
					scientistJunkpileDomain.NavAgent.stoppingDistance = 1f;
					yield break;
				}
				if (NavMesh.SamplePosition(scientistJunkpileDomain._context.Body.transform.position, out navMeshHit, scientistJunkpileDomain.NavAgent.height * single1, scientistJunkpileDomain.NavAgent.areaMask))
				{
					scientistJunkpileDomain._context.Body.transform.position = navMeshHit.position;
					scientistJunkpileDomain.NavAgent.Warp(scientistJunkpileDomain._context.Body.transform.position);
					scientistJunkpileDomain.NavAgent.enabled = true;
					scientistJunkpileDomain.NavAgent.stoppingDistance = 1f;
					scientistJunkpileDomain.UpdateNavmeshOffset();
					scientistJunkpileDomain.SetDestination(navMeshHit.position + Vector3.forward);
					yield break;
				}
				yield return CoroutineEx.waitForSecondsRealtime(single);
				single1 *= 1.5f;
				num++;
			}
			int areaFromName = NavMesh.GetAreaFromName("Walkable");
			if ((scientistJunkpileDomain.NavAgent.areaMask & 1 << (areaFromName & 31)) != 0)
			{
				if (scientistJunkpileDomain._context.Body.transform != null && !scientistJunkpileDomain._context.Body.IsDestroyed)
				{
					UnityEngine.Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[] { scientistJunkpileDomain.name });
					scientistJunkpileDomain._context.Body.Kill(BaseNetworkable.DestroyMode.None);
				}
				yield break;
			}
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
			scientistJunkpileDomain.NavAgent.agentTypeID = settingsByIndex.agentTypeID;
			scientistJunkpileDomain.NavAgent.areaMask = 1 << (areaFromName & 31);
			yield return scientistJunkpileDomain.TryForceToNavmesh();
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

		public abstract class BaseNavigateTo : OperatorBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public bool RunUntilArrival;

			protected BaseNavigateTo()
			{
			}

			protected abstract Vector3 _GetDestination(ScientistJunkpileContext context);

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				ScientistJunkpileDomain.JunkpileIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				this.OnPreStart(context);
				context.ReserveCoverPoint(null);
				context.Domain.SetDestination(this._GetDestination(context));
				if (!this.RunUntilArrival)
				{
					context.OnWorldStateChangedEvent += new ScientistJunkpileContext.WorldStateChangedEvent(this.TrackWorldState);
				}
				this.OnStart(context);
			}

			protected virtual void OnPathComplete(ScientistJunkpileContext context)
			{
			}

			protected virtual void OnPathFailed(ScientistJunkpileContext context)
			{
			}

			protected virtual void OnPreStart(ScientistJunkpileContext context)
			{
			}

			protected virtual void OnStart(ScientistJunkpileContext context)
			{
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				switch (context.GetFact(Facts.PathStatus))
				{
					case 0:
					case 2:
					{
						ScientistJunkpileDomain.JunkpileIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

			private void TrackWorldState(ScientistJunkpileContext context, Facts fact, byte oldValue, byte newValue)
			{
				if (fact == Facts.PathStatus)
				{
					if (newValue == 2)
					{
						context.OnWorldStateChangedEvent -= new ScientistJunkpileContext.WorldStateChangedEvent(this.TrackWorldState);
						ScientistJunkpileDomain.JunkpileIsNotNavigatingEffect.ApplyStatic(context, false, false);
						base.ApplyExpectedEffects(context, context.CurrentTask);
						context.Domain.StopNavigating();
						this.OnPathComplete(context);
						return;
					}
					if (newValue == 3)
					{
						context.OnWorldStateChangedEvent -= new ScientistJunkpileContext.WorldStateChangedEvent(this.TrackWorldState);
						ScientistJunkpileDomain.JunkpileIsNotNavigatingEffect.ApplyStatic(context, false, false);
						context.Domain.StopNavigating();
						this.OnPathFailed(context);
					}
				}
			}
		}

		public class JunkpileApplyFirearmOrder : OperatorBase<ScientistJunkpileContext>
		{
			public JunkpileApplyFirearmOrder()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistJunkpileContext context)
			{
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class JunkpileApplyFrustration : OperatorBase<ScientistJunkpileContext>
		{
			public JunkpileApplyFrustration()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistJunkpileContext context)
			{
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class JunkpileArrivedAtLocation : OperatorBase<ScientistJunkpileContext>
		{
			public JunkpileArrivedAtLocation()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistJunkpileContext context)
			{
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class JunkpileBlockNavigationEffect : EffectBase<ScientistJunkpileContext>
		{
			public JunkpileBlockNavigationEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
			}
		}

		public class JunkpileBlockReloadingEffect : EffectBase<ScientistJunkpileContext>
		{
			public JunkpileBlockReloadingEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
			}
		}

		public class JunkpileBlockShootingEffect : EffectBase<ScientistJunkpileContext>
		{
			public JunkpileBlockShootingEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
			}
		}

		public class JunkpileCanNavigateAwayFromAnimal : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileCanNavigateAwayFromAnimal()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (!ScientistJunkpileDomain.JunkpileCanNavigateAwayFromAnimal.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(ScientistJunkpileContext c)
			{
				Vector3 destination = ScientistJunkpileDomain.JunkpileNavigateAwayFromAnimal.GetDestination(c);
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

		public class JunkpileCanNavigateAwayFromExplosive : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileCanNavigateAwayFromExplosive()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (!ScientistJunkpileDomain.JunkpileCanNavigateAwayFromExplosive.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(ScientistJunkpileContext c)
			{
				Vector3 destination = ScientistJunkpileDomain.JunkpileNavigateAwayFromExplosive.GetDestination(c);
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

		public class JunkpileCanNavigateToCoverLocation : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			private CoverTactic _preferredTactic;

			public JunkpileCanNavigateToCoverLocation()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (!ScientistJunkpileDomain.JunkpileCanNavigateToCoverLocation.Try(this._preferredTactic, c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(CoverTactic tactic, ScientistJunkpileContext c)
			{
				Vector3 coverPosition = ScientistJunkpileDomain.JunkpileNavigateToCover.GetCoverPosition(tactic, c);
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

		public class JunkpileCanNavigateToLastKnownPositionOfPrimaryEnemyTarget : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileCanNavigateToLastKnownPositionOfPrimaryEnemyTarget()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (c.HasVisitedLastKnownEnemyPlayerLocation)
				{
					return this.score;
				}
				Vector3 destination = ScientistJunkpileDomain.JunkpileNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
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

		public class JunkpileCanNavigateToPreferredFightingRange : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			private bool CanNot;

			public JunkpileCanNavigateToPreferredFightingRange()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				Vector3 preferredFightingPosition = ScientistJunkpileDomain.JunkpileNavigateToPreferredFightingRange.GetPreferredFightingPosition(c, false);
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

		public class JunkpileCanNavigateToWaypoint : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileCanNavigateToWaypoint()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				Vector3 nextWaypointPosition = ScientistJunkpileDomain.JunkpileNavigateToWaypoint.GetNextWaypointPosition(c);
				if (!c.Memory.IsValid(nextWaypointPosition))
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileCanRememberPrimaryEnemyTarget : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileCanRememberPrimaryEnemyTarget()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileCanThrowAtLastKnownLocation : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileCanThrowAtLastKnownLocation()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (!ScientistJunkpileDomain.JunkpileCanThrowAtLastKnownLocation.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(ScientistJunkpileContext c)
			{
				if (!ConVar.AI.npc_use_thrown_weapons)
				{
					return false;
				}
				if (c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return false;
				}
				if (UnityEngine.Time.time - ScientistJunkpileDomain.JunkpileUseThrowableWeapon.LastTimeThrown < 8f)
				{
					return false;
				}
				Vector3 destination = ScientistJunkpileDomain.JunkpileNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
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

		public class JunkpileCanUseWeaponAtCurrentRange : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileCanUseWeaponAtCurrentRange()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (!ScientistJunkpileDomain.JunkpileCanUseWeaponAtCurrentRange.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(ScientistJunkpileContext c)
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

		public class JunkpileChangeFirearmOrder : EffectBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public FirearmOrders Order;

			public JunkpileChangeFirearmOrder()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.FirearmOrder, this.Order, temporary);
					return;
				}
				context.SetFact(Facts.FirearmOrder, this.Order, true, true, true);
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.FirearmOrder);
					return;
				}
				context.SetFact(Facts.FirearmOrder, context.GetPreviousFact(Facts.FirearmOrder), true, true, true);
			}
		}

		public class JunkpileDuck : OperatorBase<ScientistJunkpileContext>
		{
			public JunkpileDuck()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				context.Body.modelState.ducked = false;
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				context.Body.modelState.ducked = true;
				ScientistJunkpileDomain.JunkpileIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class JunkpileDuckTimed : OperatorBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			private float _duckTimeMin;

			[ApexSerialization]
			private float _duckTimeMax;

			public JunkpileDuckTimed()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				context.Body.StopCoroutine(this.AsyncTimer(context, 0f));
				this.Reset(context);
			}

			private IEnumerator AsyncTimer(ScientistJunkpileContext context, float time)
			{
				ScientistJunkpileDomain.JunkpileDuckTimed junkpileDuckTimed = null;
				yield return CoroutineEx.waitForSeconds(time);
				junkpileDuckTimed.Reset(context);
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				context.Body.modelState.ducked = true;
				context.SetFact(Facts.IsDucking, true, true, true, true);
				ScientistJunkpileDomain.JunkpileIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

			private void Reset(ScientistJunkpileContext context)
			{
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsDucking))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class JunkpileFutureCoverState : EffectBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public CoverTactic Tactic;

			public JunkpileFutureCoverState()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
				CoverState coverState;
				CoverState coverState1;
				CoverPoint cover = ScientistJunkpileDomain.JunkpileNavigateToCover.GetCover(this.Tactic, context);
				if (fromPlanner)
				{
					ScientistJunkpileContext scientistJunkpileContext = context;
					if (cover == null)
					{
						coverState1 = CoverState.None;
					}
					else
					{
						coverState1 = (cover.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full);
					}
					scientistJunkpileContext.PushFactChangeDuringPlanning(Facts.CoverState, coverState1, temporary);
					return;
				}
				ScientistJunkpileContext scientistJunkpileContext1 = context;
				if (cover == null)
				{
					coverState = CoverState.None;
				}
				else
				{
					coverState = (cover.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full);
				}
				scientistJunkpileContext1.SetFact(Facts.CoverState, coverState, true, true, true);
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.CoverState);
					return;
				}
				context.SetFact(Facts.CoverState, context.GetPreviousFact(Facts.CoverState), true, true, true);
			}
		}

		public class JunkpileHasFirearmOrder : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public FirearmOrders Order;

			public JunkpileHasFirearmOrder()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				return this.score;
			}
		}

		public class JunkpileHasItem : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public JunkpileHasItem()
			{
			}

			public override float Score(ScientistJunkpileContext c)
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

		public class JunkpileHasWorldState : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public JunkpileHasWorldState()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (c.GetWorldState(this.Fact) != this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileHasWorldStateAmmo : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public AmmoState Value;

			public JunkpileHasWorldStateAmmo()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (c.GetWorldState(Facts.AmmoState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileHasWorldStateBool : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public JunkpileHasWorldStateBool()
			{
			}

			public override float Score(ScientistJunkpileContext c)
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

		public class JunkpileHasWorldStateCoverState : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public CoverState Value;

			public JunkpileHasWorldStateCoverState()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (c.GetWorldState(Facts.CoverState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileHasWorldStateEnemyRange : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public EnemyRange Value;

			public JunkpileHasWorldStateEnemyRange()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (c.GetWorldState(Facts.EnemyRange) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileHasWorldStateGreaterThan : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public JunkpileHasWorldStateGreaterThan()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (c.GetWorldState(this.Fact) <= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileHasWorldStateHealth : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public HealthState Value;

			public JunkpileHasWorldStateHealth()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (c.GetWorldState(Facts.HealthState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileHasWorldStateLessThan : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public JunkpileHasWorldStateLessThan()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (c.GetWorldState(this.Fact) >= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileHealEffect : EffectBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public HealthState Health;

			public JunkpileHealEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.HealthState, this.Health, temporary);
					return;
				}
				context.SetFact(Facts.HealthState, this.Health, true, true, true);
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.HealthState);
					return;
				}
				context.SetFact(Facts.HealthState, context.GetPreviousFact(Facts.HealthState), true, true, true);
			}
		}

		public class JunkpileHoldItemOfType : OperatorBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			private ItemType _item;

			[ApexSerialization]
			private float _switchTime;

			public JunkpileHoldItemOfType()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				ScientistJunkpileDomain.JunkpileHoldItemOfType.SwitchToItem(context, (ItemType)context.GetPreviousFact(Facts.HeldItemType));
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				ScientistJunkpileDomain.JunkpileHoldItemOfType.SwitchToItem(context, this._item);
				context.Body.StartCoroutine(this.WaitAsync(context));
			}

			public static void SwitchToItem(ScientistJunkpileContext context, ItemType _item)
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
					else if (_item != ItemType.ThrowableWeapon || inventoryLookupCache.info.category != ItemCategory.Weapon || !(inventoryLookupCache.GetHeldEntity() is ThrownWeapon))
					{
						HeldEntity heldEntity = inventoryLookupCache.GetHeldEntity() as HeldEntity;
						if (_item != ItemType.LightSourceItem || inventoryLookupCache.info.category != ItemCategory.Tool || !inventoryLookupCache.CanBeHeld() || !(heldEntity != null) || heldEntity.toolType != NPCPlayerApex.ToolTypeEnum.Lightsource)
						{
							if (_item != ItemType.ResearchItem || inventoryLookupCache.info.category != ItemCategory.Tool || !inventoryLookupCache.CanBeHeld() || !(heldEntity != null) || heldEntity.toolType != NPCPlayerApex.ToolTypeEnum.Research)
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
					else
					{
						context.Body.UpdateActiveItem(inventoryLookupCache.uid);
						context.SetFact(Facts.HeldItemType, _item, true, true, true);
						return;
					}
				}
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsWaiting))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator WaitAsync(ScientistJunkpileContext context)
			{
				ScientistJunkpileDomain.JunkpileHoldItemOfType junkpileHoldItemOfType = null;
				context.SetFact(Facts.IsWaiting, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(junkpileHoldItemOfType._switchTime);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}
		}

		public class JunkpileHoldItemOfTypeEffect : EffectBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public JunkpileHoldItemOfTypeEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.HeldItemType, this.Value, temporary);
					return;
				}
				context.SetFact(Facts.HeldItemType, this.Value, true, true, true);
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.HeldItemType);
					return;
				}
				context.SetFact(Facts.HeldItemType, context.GetPreviousFact(Facts.HeldItemType), true, true, true);
			}
		}

		public class JunkpileHoldLocation : OperatorBase<ScientistJunkpileContext>
		{
			public JunkpileHoldLocation()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				ScientistJunkpileDomain.JunkpileIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				return OperatorStateType.Running;
			}
		}

		public class JunkpileHoldLocationTimed : OperatorBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			private float _duckTimeMin;

			[ApexSerialization]
			private float _duckTimeMax;

			public JunkpileHoldLocationTimed()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			private IEnumerator AsyncTimer(ScientistJunkpileContext context, float time)
			{
				yield return CoroutineEx.waitForSeconds(time);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				ScientistJunkpileDomain.JunkpileIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsWaiting))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class JunkpileIdle_JustStandAround : OperatorBase<ScientistJunkpileContext>
		{
			public JunkpileIdle_JustStandAround()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsIdle, false, true, true, true);
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				this.ResetWorldState(context);
				context.SetFact(Facts.IsIdle, true, true, true, true);
				context.Domain.ReloadFirearm();
			}

			private void ResetWorldState(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.SetFact(Facts.IsNavigating, false, true, true, true);
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				return OperatorStateType.Running;
			}
		}

		public class JunkpileIsHoldingItem : ContextualScorerBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public JunkpileIsHoldingItem()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (c.GetWorldState(Facts.HeldItemType) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileIsNavigatingEffect : EffectBase<ScientistJunkpileContext>
		{
			public JunkpileIsNavigatingEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.IsNavigating, 1, temporary);
					return;
				}
				context.PreviousWorldState[5] = context.WorldState[5];
				context.WorldState[5] = 1;
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.IsNavigating);
					return;
				}
				context.WorldState[5] = context.PreviousWorldState[5];
			}
		}

		public class JunkpileIsNavigationAllowed : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileIsNavigationAllowed()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (!ScientistJunkpileDomain.JunkpileIsNavigationBlocked.CanNavigate(c))
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileIsNavigationBlocked : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileIsNavigationBlocked()
			{
			}

			public static bool CanNavigate(ScientistJunkpileContext c)
			{
				if (c.Domain.NavAgent != null && c.Domain.NavAgent.isOnNavMesh)
				{
					return true;
				}
				return false;
			}

			public override float Score(ScientistJunkpileContext c)
			{
				if (ScientistJunkpileDomain.JunkpileIsNavigationBlocked.CanNavigate(c))
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class JunkpileIsNotNavigatingEffect : EffectBase<ScientistJunkpileContext>
		{
			public JunkpileIsNotNavigatingEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
				ScientistJunkpileDomain.JunkpileIsNotNavigatingEffect.ApplyStatic(context, fromPlanner, temporary);
			}

			public static void ApplyStatic(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.IsNavigating, (byte)0, temporary);
					return;
				}
				context.PreviousWorldState[5] = context.WorldState[5];
				context.WorldState[5] = 0;
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.IsNavigating);
					return;
				}
				context.WorldState[5] = context.PreviousWorldState[5];
			}
		}

		public class JunkpileIsReloadingAllowed : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileIsReloadingAllowed()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				return this.score;
			}
		}

		public class JunkpileIsReloadingBlocked : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileIsReloadingBlocked()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				return 0f;
			}
		}

		public class JunkpileIsShootingAllowed : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileIsShootingAllowed()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				return this.score;
			}
		}

		public class JunkpileIsShootingBlocked : ContextualScorerBase<ScientistJunkpileContext>
		{
			public JunkpileIsShootingBlocked()
			{
			}

			public override float Score(ScientistJunkpileContext c)
			{
				return 0f;
			}
		}

		public class JunkpileLookAround : OperatorBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			private float _lookAroundTime;

			public JunkpileLookAround()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsLookingAround, true, true, true, true);
				context.Body.StartCoroutine(this.LookAroundAsync(context));
			}

			private IEnumerator LookAroundAsync(ScientistJunkpileContext context)
			{
				ScientistJunkpileDomain.JunkpileLookAround junkpileLookAround = null;
				yield return CoroutineEx.waitForSeconds(junkpileLookAround._lookAroundTime);
				if (context.IsFact(Facts.CanSeeEnemy))
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsLookingAround))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class JunkpileNavigateAwayFromAnimal : ScientistJunkpileDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsAvoidingAnimalOnComplete;

			public JunkpileNavigateAwayFromAnimal()
			{
			}

			protected override Vector3 _GetDestination(ScientistJunkpileContext context)
			{
				return ScientistJunkpileDomain.JunkpileNavigateAwayFromAnimal.GetDestination(context);
			}

			public static Vector3 GetDestination(ScientistJunkpileContext context)
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

			protected override void OnPathComplete(ScientistJunkpileContext context)
			{
				if (this.DisableIsAvoidingAnimalOnComplete)
				{
					context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(ScientistJunkpileContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.5f;
			}

			protected override void OnStart(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsAvoidingAnimal, true, true, true, true);
			}
		}

		public class JunkpileNavigateAwayFromExplosive : ScientistJunkpileDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsAvoidingExplosiveOnComplete;

			public JunkpileNavigateAwayFromExplosive()
			{
			}

			protected override Vector3 _GetDestination(ScientistJunkpileContext context)
			{
				return ScientistJunkpileDomain.JunkpileNavigateAwayFromExplosive.GetDestination(context);
			}

			public static Vector3 GetDestination(ScientistJunkpileContext context)
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

			protected override void OnPathComplete(ScientistJunkpileContext context)
			{
				if (this.DisableIsAvoidingExplosiveOnComplete)
				{
					context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(ScientistJunkpileContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.5f;
			}

			protected override void OnStart(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsAvoidingExplosive, true, true, true, true);
			}
		}

		public class JunkpileNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer : ScientistJunkpileDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public JunkpileNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(ScientistJunkpileContext context)
			{
				return ScientistJunkpileDomain.JunkpileNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetContinuousDestinationFromBody(ScientistJunkpileContext context)
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

			public static Vector3 GetDestination(ScientistJunkpileContext context)
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

			private void OnContinuePath(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				Vector3 continuousDestinationFromBody = ScientistJunkpileDomain.JunkpileNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetContinuousDestinationFromBody(context);
				if ((context.Body.transform.position - continuousDestinationFromBody).sqrMagnitude <= 0.2f)
				{
					return;
				}
				this.OnPreStart(context);
				context.Domain.SetDestination(continuousDestinationFromBody);
				this.OnStart(context);
			}

			protected override void OnPathComplete(ScientistJunkpileContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(ScientistJunkpileContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.5f;
			}

			protected override void OnStart(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
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

		public class JunkpileNavigateToCover : ScientistJunkpileDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private CoverTactic _preferredTactic;

			public JunkpileNavigateToCover()
			{
			}

			private static Vector3 _GetCoverPosition(CoverTactic tactic, ScientistJunkpileContext context)
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

			protected override Vector3 _GetDestination(ScientistJunkpileContext context)
			{
				return ScientistJunkpileDomain.JunkpileNavigateToCover.GetCoverPosition(this._preferredTactic, context);
			}

			public static CoverPoint GetCover(CoverTactic tactic, ScientistJunkpileContext context)
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

			public static Vector3 GetCoverPosition(CoverTactic tactic, ScientistJunkpileContext context)
			{
				NavMeshHit navMeshHit;
				if (UnityEngine.Time.time - context.Memory.CachedCoverDestinationTime < 0.01f)
				{
					return context.Memory.CachedCoverDestination;
				}
				Vector3 vector3 = ScientistJunkpileDomain.JunkpileNavigateToCover._GetCoverPosition(tactic, context);
				Vector3 vector31 = vector3;
				if ((vector31 - context.Memory.CachedCoverDestination).sqrMagnitude < 2f)
				{
					return context.Memory.CachedCoverDestination;
				}
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

			protected override void OnPathComplete(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.CoverTactic, CoverTactic.None, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.CoverTactic, CoverTactic.None, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(ScientistJunkpileContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 1f;
			}
		}

		public class JunkpileNavigateToLastKnownLocationOfPrimaryEnemyPlayer : ScientistJunkpileDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public JunkpileNavigateToLastKnownLocationOfPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(ScientistJunkpileContext context)
			{
				return ScientistJunkpileDomain.JunkpileNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetDestination(ScientistJunkpileContext context)
			{
				NavMeshHit navMeshHit;
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (primaryKnownEnemyPlayer.PlayerInfo.Player != null && !context.HasVisitedLastKnownEnemyPlayerLocation && NavMesh.FindClosestEdge(primaryKnownEnemyPlayer.LastKnownPosition, out navMeshHit, context.Domain.NavAgent.areaMask))
				{
					return navMeshHit.position;
				}
				return context.Body.transform.position;
			}

			protected override void OnPathComplete(ScientistJunkpileContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
				context.HasVisitedLastKnownEnemyPlayerLocation = true;
			}

			protected override void OnPathFailed(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
				context.HasVisitedLastKnownEnemyPlayerLocation = false;
			}

			protected override void OnPreStart(ScientistJunkpileContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.5f;
			}

			protected override void OnStart(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}
		}

		public class JunkpileNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer : ScientistJunkpileDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public JunkpileNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(ScientistJunkpileContext context)
			{
				return ScientistJunkpileDomain.JunkpileNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetDestination(ScientistJunkpileContext context)
			{
				NavMeshHit navMeshHit;
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (!(primaryKnownEnemyPlayer.PlayerInfo.Player != null) || !NavMesh.FindClosestEdge(primaryKnownEnemyPlayer.OurLastPositionWhenLastSeen, out navMeshHit, context.Domain.NavAgent.areaMask))
				{
					return context.Body.transform.position;
				}
				return context.Domain.ToAllowedMovementDestination(navMeshHit.position);
			}

			protected override void OnPathComplete(ScientistJunkpileContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(ScientistJunkpileContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.5f;
			}

			protected override void OnStart(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}
		}

		public class JunkpileNavigateToPreferredFightingRange : ScientistJunkpileDomain.BaseNavigateTo
		{
			public JunkpileNavigateToPreferredFightingRange()
			{
			}

			protected override Vector3 _GetDestination(ScientistJunkpileContext context)
			{
				return ScientistJunkpileDomain.JunkpileNavigateToPreferredFightingRange.GetPreferredFightingPosition(context, false);
			}

			public static Vector3 GetPreferredFightingPosition(ScientistJunkpileContext context, bool snapToAllowedRange = true)
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
						float single1 = context.Body.AiDefinition.Engagement.CenterOfMediumRangeFirearm(firearm);
						if (primaryEnemyPlayerTarget.SqrDistance >= single1 * single1)
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
						float single2 = single - single1;
						bodyPosition = context.Body.transform.position + (player * single2);
					}
					else
					{
						bodyPosition = ScientistJunkpileDomain.JunkpileNavigateToCover.GetCoverPosition(CoverTactic.Closest, context);
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

		public class JunkpileNavigateToWaypoint : ScientistJunkpileDomain.BaseNavigateTo
		{
			public JunkpileNavigateToWaypoint()
			{
			}

			protected override Vector3 _GetDestination(ScientistJunkpileContext context)
			{
				return ScientistJunkpileDomain.JunkpileNavigateToWaypoint.GetNextWaypointPosition(context);
			}

			public static Vector3 GetNextWaypointPosition(ScientistJunkpileContext context)
			{
				return context.Body.transform.position + (Vector3.forward * 10f);
			}
		}

		public class JunkpileReloadFirearmOperator : OperatorBase<ScientistJunkpileContext>
		{
			public JunkpileReloadFirearmOperator()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				context.Domain.ReloadFirearm();
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsReloading))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class JunkpileStand : OperatorBase<ScientistJunkpileContext>
		{
			public JunkpileStand()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				context.Body.StopCoroutine(this.AsyncTimer(context, 0f));
				this.Reset(context);
			}

			private IEnumerator AsyncTimer(ScientistJunkpileContext context, float time)
			{
				yield return CoroutineEx.waitForSeconds(time);
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
				yield return CoroutineEx.waitForSeconds(time * 2f);
				context.SetFact(Facts.IsStandingUp, false, true, true, true);
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				context.SetFact(Facts.IsStandingUp, true, true, true, true);
				context.Body.StartCoroutine(this.AsyncTimer(context, 0.2f));
			}

			private void Reset(ScientistJunkpileContext context)
			{
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
				context.SetFact(Facts.IsStandingUp, false, true, true, true);
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsStandingUp))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class JunkpileStopMoving : OperatorBase<ScientistJunkpileContext>
		{
			public JunkpileStopMoving()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				ScientistJunkpileDomain.JunkpileIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class JunkpileTimeBlockNavigationEffect : EffectBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			[FriendlyName("Time (Seconds)")]
			public float Time;

			public JunkpileTimeBlockNavigationEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
			}
		}

		public class JunkpileUnblockNavigationEffect : EffectBase<ScientistJunkpileContext>
		{
			public JunkpileUnblockNavigationEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
			}
		}

		public class JunkpileUnblockReloadingEffect : EffectBase<ScientistJunkpileContext>
		{
			public JunkpileUnblockReloadingEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
			}
		}

		public class JunkpileUnblockShootingEffect : EffectBase<ScientistJunkpileContext>
		{
			public JunkpileUnblockShootingEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
			}
		}

		public class JunkpileUseMedicalTool : OperatorBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public HealthState Health;

			public JunkpileUseMedicalTool()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsApplyingMedical, false, true, true, true);
				ScientistJunkpileDomain.JunkpileHoldItemOfType.SwitchToItem(context, (ItemType)context.GetPreviousFact(Facts.HeldItemType));
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				context.Body.StartCoroutine(this.UseItem(context));
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsApplyingMedical))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator UseItem(ScientistJunkpileContext context)
			{
				ScientistJunkpileDomain.JunkpileUseMedicalTool junkpileUseMedicalTool = null;
				Item activeItem = context.Body.GetActiveItem();
				if (activeItem != null)
				{
					MedicalTool heldEntity = activeItem.GetHeldEntity() as MedicalTool;
					if (heldEntity != null)
					{
						context.SetFact(Facts.IsApplyingMedical, true, true, true, true);
						heldEntity.ServerUse();
						if (junkpileUseMedicalTool.Health == HealthState.FullHealth)
						{
							context.Body.Heal(context.Body.MaxHealth());
						}
						yield return CoroutineEx.waitForSeconds(heldEntity.repeatDelay * 4f);
					}
				}
				context.SetFact(Facts.IsApplyingMedical, false, true, true, true);
				ScientistJunkpileDomain.JunkpileHoldItemOfType.SwitchToItem(context, context.GetPreviousFact(Facts.HeldItemType));
			}
		}

		public class JunkpileUseThrowableWeapon : OperatorBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			private NpcOrientation _orientation;

			public static float LastTimeThrown;

			static JunkpileUseThrowableWeapon()
			{
			}

			public JunkpileUseThrowableWeapon()
			{
			}

			public override void Abort(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
				ScientistJunkpileDomain.JunkpileHoldItemOfType.SwitchToItem(context, (ItemType)context.GetPreviousFact(Facts.HeldItemType));
			}

			public override void Execute(ScientistJunkpileContext context)
			{
				if (context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player != null)
				{
					context.Body.StartCoroutine(this.UseItem(context));
				}
			}

			public override OperatorStateType Tick(ScientistJunkpileContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsThrowingWeapon))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator UseItem(ScientistJunkpileContext context)
			{
				ScientistJunkpileDomain.JunkpileUseThrowableWeapon junkpileUseThrowableWeapon = null;
				Item activeItem = context.Body.GetActiveItem();
				if (activeItem != null)
				{
					ThrownWeapon heldEntity = activeItem.GetHeldEntity() as ThrownWeapon;
					if (heldEntity != null)
					{
						context.SetFact(Facts.IsThrowingWeapon, true, true, true, true);
						ScientistJunkpileDomain.JunkpileUseThrowableWeapon.LastTimeThrown = UnityEngine.Time.time;
						context.OrientationType = junkpileUseThrowableWeapon._orientation;
						context.Body.ForceOrientationTick();
						yield return null;
						heldEntity.ServerThrow(context.Memory.PrimaryKnownEnemyPlayer.LastKnownPosition);
						yield return null;
					}
					heldEntity = null;
				}
				context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
				ScientistJunkpileDomain.JunkpileHoldItemOfType.SwitchToItem(context, ItemType.ProjectileWeapon);
			}
		}

		public class JunkpileWorldStateBoolEffect : EffectBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public JunkpileWorldStateBoolEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class JunkpileWorldStateEffect : EffectBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public JunkpileWorldStateEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class JunkpileWorldStateIncrementEffect : EffectBase<ScientistJunkpileContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public JunkpileWorldStateIncrementEffect()
			{
			}

			public override void Apply(ScientistJunkpileContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					byte num = context.PeekFactChangeDuringPlanning(this.Fact);
					context.PushFactChangeDuringPlanning(this.Fact, (int)(num + this.Value), temporary);
					return;
				}
				context.SetFact(this.Fact, (int)(context.GetFact(this.Fact) + this.Value), true, true, true);
			}

			public override void Reverse(ScientistJunkpileContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public delegate void OnPlanAborted(ScientistJunkpileDomain domain);

		public delegate void OnPlanCompleted(ScientistJunkpileDomain domain);
	}
}