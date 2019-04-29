using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using ConVar;
using Rust;
using Rust.Ai;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Murderer.Reasoners;
using Rust.Ai.HTN.Murderer.Sensors;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.Murderer
{
	public class MurdererDomain : HTNDomain
	{
		[ReadOnly]
		[SerializeField]
		private bool _isRegisteredWithAgency;

		[Header("Context")]
		[SerializeField]
		private Rust.Ai.HTN.Murderer.MurdererContext _context;

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
				TickFrequency = 0.2f
			},
			new PlayersViewAngleSensor()
			{
				TickFrequency = 0.25f
			},
			new EnemyPlayersInRangeSensor()
			{
				TickFrequency = 0.2f
			},
			new EnemyPlayersLineOfSightSensor()
			{
				TickFrequency = 0.25f,
				MaxVisible = 1
			},
			new EnemyPlayersHearingSensor()
			{
				TickFrequency = 0.1f
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
				TickFrequency = 0.2f
			},
			new EnemyPlayerHearingReasoner()
			{
				TickFrequency = 0.2f
			},
			new EnemyTargetReasoner()
			{
				TickFrequency = 0.2f
			},
			new FireTacticReasoner()
			{
				TickFrequency = 0.2f
			},
			new OrientationReasoner()
			{
				TickFrequency = 0.01f
			},
			new PreferredFightingRangeReasoner()
			{
				TickFrequency = 0.2f
			},
			new AtLastKnownEnemyPlayerLocationReasoner()
			{
				TickFrequency = 0.2f
			},
			new HealthReasoner()
			{
				TickFrequency = 0.2f
			},
			new VulnerabilityReasoner()
			{
				TickFrequency = 0.2f
			},
			new FrustrationReasoner()
			{
				TickFrequency = 0.25f
			},
			new ReturnHomeReasoner()
			{
				TickFrequency = 1f
			},
			new AtHomeLocationReasoner()
			{
				TickFrequency = 5f
			},
			new AnimalReasoner()
			{
				TickFrequency = 0.25f
			},
			new AlertnessReasoner()
			{
				TickFrequency = 0.2f
			},
			new EnemyRangeReasoner()
			{
				TickFrequency = 0.2f
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

		private Rust.Ai.HTN.Murderer.MurdererDefinition _murdererDefinition;

		private Vector3 missOffset;

		private float missToHeadingAlignmentTime;

		private float repeatMissTime;

		private bool recalculateMissOffset = true;

		private bool isMissing;

		private bool _passPathValidity;

		private static Vector3[] pathCornerCache;

		private static NavMeshPath _pathCache;

		public MurdererDomain.OnPlanAborted OnPlanAbortedEvent;

		public MurdererDomain.OnPlanCompleted OnPlanCompletedEvent;

		public Rust.Ai.HTN.Murderer.MurdererContext MurdererContext
		{
			get
			{
				return this._context;
			}
		}

		public Rust.Ai.HTN.Murderer.MurdererDefinition MurdererDefinition
		{
			get
			{
				if (this._murdererDefinition == null)
				{
					this._murdererDefinition = this._context.Body.AiDefinition as Rust.Ai.HTN.Murderer.MurdererDefinition;
				}
				return this._murdererDefinition;
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

		static MurdererDomain()
		{
			MurdererDomain.pathCornerCache = new Vector3[128];
			MurdererDomain._pathCache = null;
		}

		public MurdererDomain()
		{
		}

		protected override void AbortPlan()
		{
			base.AbortPlan();
			MurdererDomain.OnPlanAborted onPlanAbortedEvent = this.OnPlanAbortedEvent;
			if (onPlanAbortedEvent != null)
			{
				onPlanAbortedEvent(this);
			}
			else
			{
			}
			this._context.SetFact(Facts.MaintainCover, 0, true, true, true);
			this._context.SetFact(Facts.IsRoaming, 0, true, true, true);
			this._context.SetFact(Facts.IsSearching, 0, true, true, true);
			this._context.SetFact(Facts.IsReturningHome, 0, true, true, true);
			this._context.Body.modelState.ducked = false;
			MurdererDomain.MurdererHoldItemOfType.SwitchToItem(this._context, ItemType.MeleeWeapon);
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
			MurdererDomain.OnPlanCompleted onPlanCompletedEvent = this.OnPlanCompletedEvent;
			if (onPlanCompletedEvent != null)
			{
				onPlanCompletedEvent(this);
			}
			else
			{
			}
			this._context.SetFact(Facts.MaintainCover, 0, true, true, true);
			this._context.SetFact(Facts.IsRoaming, 0, true, true, true);
			this._context.SetFact(Facts.IsSearching, 0, true, true, true);
			this._context.SetFact(Facts.IsReturningHome, 0, true, true, true);
			this._context.Body.modelState.ducked = false;
			MurdererDomain.MurdererHoldItemOfType.SwitchToItem(this._context, ItemType.MeleeWeapon);
		}

		private IEnumerator DelayedForcedThink()
		{
			MurdererDomain murdererDomain = null;
			while (!murdererDomain._context.IsFact(Facts.IsRoaming) && !murdererDomain._context.IsFact(Facts.HasEnemyTarget))
			{
				yield return CoroutineEx.waitForSeconds(3f);
				murdererDomain.Think();
			}
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
			float single2 = this.MurdererDefinition.MissFunction.Evaluate((Mathf.Approximately(single1, 0f) ? 1f : 1f - Mathf.Min(single1 / maxTime, 1f)));
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
			MurdererDomain murdererDomain = null;
			murdererDomain._isFiring = true;
			murdererDomain._lastFirearmUsageTime = startTime + triggerDownInterval + proj.attackSpacing;
			murdererDomain._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
			do
			{
				if (UnityEngine.Time.time - startTime >= triggerDownInterval || !murdererDomain._context.IsBodyAlive() || !murdererDomain._context.IsFact(Facts.CanSeeEnemy))
				{
					break;
				}
				if (murdererDomain._context.EnemyPlayersInLineOfSight.Count > 3)
				{
					proj.ServerUse((1f + UnityEngine.Random.@value * 0.5f) * ConVar.AI.npc_htn_player_base_damage_modifier);
				}
				else if (!(murdererDomain._context.PrimaryEnemyPlayerInLineOfSight.Player != null) || murdererDomain._context.PrimaryEnemyPlayerInLineOfSight.Player.healthFraction >= 0.2f)
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
			murdererDomain._isFiring = false;
		}

		public override void Initialize(BaseEntity body)
		{
			if (this._aiClient == null || this._aiClient.ai == null || this._aiClient.ai.id != AINameMap.HTNDomainMurderer)
			{
				this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainMurderer, this);
			}
			if (this._context == null || this._context.Body != body)
			{
				this._context = new Rust.Ai.HTN.Murderer.MurdererContext(body as HTNPlayer, this);
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
			base.StartCoroutine(this.DelayedForcedThink());
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
			if (this._context.Memory.HasTargetDestination && !this._context.Domain.NavAgent.pathPending && (this._context.Domain.NavAgent.pathStatus != NavMeshPathStatus.PathComplete || (this._context.Domain.NavAgent.destination - this._context.Memory.TargetDestination).sqrMagnitude > 0.01f || float.IsInfinity(this._context.Domain.NavAgent.remainingDistance) || (this._context.OrientationType == NpcOrientation.PrimaryTargetBody || this._context.OrientationType == NpcOrientation.PrimaryTargetHead) && this._context.Domain.NavAgent.remainingDistance <= this._context.Domain.NavAgent.stoppingDistance && !this._context.IsFact(Facts.AtLocationPreferredFightingRange)))
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
				this._context.ReservedCoverPoint.CoverIsCompromised(ConVar.AI.npc_cover_compromised_cooldown);
				this._context.ReserveCoverPoint(null);
			}
		}

		public override void OnPreHurt(HitInfo info)
		{
			if (!info.isHeadshot)
			{
				if (info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc || info.InitiatorPlayer == null && info.Initiator != null && info.Initiator.IsNpc)
				{
					info.damageTypes.ScaleAll(Halloween.scarecrow_body_dmg_modifier);
					return;
				}
				info.damageTypes.ScaleAll(2f);
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
			if (single > this.MurdererContext.Body.AiDefinition.Engagement.SqrMediumRange || !allowCloseRange && single < this.MurdererContext.Body.AiDefinition.Engagement.SqrCloseRange)
			{
				return true;
			}
			float single1 = Mathf.Sqrt(single);
			if (MurdererDomain._pathCache == null)
			{
				MurdererDomain._pathCache = new NavMeshPath();
			}
			if (NavMesh.CalculatePath(from, to, this.NavAgent.areaMask, MurdererDomain._pathCache))
			{
				int cornersNonAlloc = MurdererDomain._pathCache.GetCornersNonAlloc(MurdererDomain.pathCornerCache);
				if (MurdererDomain._pathCache.status == NavMeshPathStatus.PathComplete && cornersNonAlloc > 1 && Mathf.Abs(single1 - this.PathDistance(cornersNonAlloc, ref MurdererDomain.pathCornerCache, single1 + ConVar.AI.npc_cover_path_vs_straight_dist_max_diff)) > ConVar.AI.npc_cover_path_vs_straight_dist_max_diff)
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
			MurdererDomain murdererDomain = null;
			murdererDomain._context.SetFact(Facts.IsReloading, true, true, true, true);
			proj.ServerReload();
			yield return CoroutineEx.waitForSeconds(proj.reloadTime);
			murdererDomain._context.SetFact(Facts.IsReloading, false, true, true, true);
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

		public bool SetDestination(Vector3 destination, bool passPathValidity = false)
		{
			this._passPathValidity = passPathValidity;
			if (this.NavAgent == null || !this.NavAgent.isOnNavMesh)
			{
				this._context.SetFact(Facts.PathStatus, (byte)3, true, false, true);
				return false;
			}
			destination = this.ToAllowedMovementDestination(destination);
			this._context.Memory.HasTargetDestination = true;
			this._context.Memory.TargetDestination = destination;
			this._context.Domain.NavAgent.destination = destination;
			if (this._passPathValidity || this.IsPathValid())
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
			if (this._context.IsFact(Facts.CanSeeEnemy) || this._context.IsFact(Facts.IsSearching))
			{
				this._navAgent.speed = this._context.Body.AiDefinition.Movement.RunSpeed;
			}
			else
			{
				this._navAgent.speed = this._context.Body.AiDefinition.Movement.DuckSpeed;
			}
			if (this._context.Body != null && this._context.Memory != null)
			{
				this._context.Body.SetFlag(BaseEntity.Flags.Reserved3, (this._context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null ? false : this._context.Body.IsAlive()), false, true);
			}
		}

		public override void TickDestinationTracker()
		{
			if (this.NavAgent == null || !this.NavAgent.isOnNavMesh)
			{
				this._context.SetFact(Facts.PathStatus, (byte)0, true, false, true);
				return;
			}
			if (!this._passPathValidity && !this.IsPathValid())
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
				this._context.Body.modelState.aiming = this._isFiring;
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
					if (this._context.GetFact(Facts.HeldItemType) != 2)
					{
						return;
					}
					else
					{
						break;
					}
				}
			}
			this._context.Body.modelState.aiming = true;
		}

		private void TickFirearm(float time, float interval)
		{
			AttackEntity firearm = this.ReloadFirearmIfEmpty();
			if (firearm == null || !(firearm is BaseMelee) || this._context.GetFact(Facts.HeldItemType) == 2)
			{
				MurdererDomain.MurdererHoldItemOfType.SwitchToItem(this._context, ItemType.MeleeWeapon);
				firearm = this.GetFirearm();
			}
			if (firearm == null)
			{
				return;
			}
			BaseMelee baseMelee = firearm as BaseMelee;
			if (baseMelee == null || baseMelee.effectiveRange > 2f)
			{
				this._context.Body.modelState.aiming = false;
			}
			else
			{
				this._context.Body.modelState.aiming = true;
			}
			if (time - this._lastFirearmUsageTime < interval)
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
			BaseProjectile baseProjectile = firearm as BaseProjectile;
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
					this.FireSingle(firearm, time);
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
			MurdererDomain murdererDomain = null;
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
				if (!(murdererDomain.NavAgent != null) || murdererDomain.NavAgent.isOnNavMesh)
				{
					murdererDomain.NavAgent.enabled = true;
					murdererDomain.NavAgent.stoppingDistance = 1f;
					yield break;
				}
				if (NavMesh.SamplePosition(murdererDomain._context.Body.transform.position, out navMeshHit, murdererDomain.NavAgent.height * single1, murdererDomain.NavAgent.areaMask))
				{
					murdererDomain._context.Body.transform.position = navMeshHit.position;
					murdererDomain.NavAgent.Warp(murdererDomain._context.Body.transform.position);
					murdererDomain.NavAgent.enabled = true;
					murdererDomain.NavAgent.stoppingDistance = 1f;
					murdererDomain.UpdateNavmeshOffset();
					yield break;
				}
				yield return CoroutineEx.waitForSecondsRealtime(single);
				single1 *= 1.5f;
				num++;
			}
			int areaFromName = NavMesh.GetAreaFromName("Walkable");
			if ((murdererDomain.NavAgent.areaMask & 1 << (areaFromName & 31)) != 0)
			{
				if (murdererDomain._context.Body.transform != null && !murdererDomain._context.Body.IsDestroyed)
				{
					UnityEngine.Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[] { murdererDomain.name });
					murdererDomain._context.Body.Kill(BaseNetworkable.DestroyMode.None);
				}
				yield break;
			}
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
			murdererDomain.NavAgent.agentTypeID = settingsByIndex.agentTypeID;
			murdererDomain.NavAgent.areaMask = 1 << (areaFromName & 31);
			yield return murdererDomain.TryForceToNavmesh();
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

		public class MurdererApplyFirearmOrder : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererApplyFirearmOrder()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class MurdererApplyFrustration : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererApplyFrustration()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class MurdererArrivedAtLocation : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererArrivedAtLocation()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public abstract class MurdererBaseNavigateTo : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public bool RunUntilArrival;

			protected MurdererBaseNavigateTo()
			{
			}

			protected abstract Vector3 _GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context);

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				this.OnPreStart(context);
				context.ReserveCoverPoint(null);
				context.Domain.SetDestination(this._GetDestination(context), false);
				if (!this.RunUntilArrival)
				{
					context.OnWorldStateChangedEvent += new Rust.Ai.HTN.Murderer.MurdererContext.WorldStateChangedEvent(this.TrackWorldState);
				}
				this.OnStart(context);
			}

			protected virtual void OnPathComplete(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
			}

			protected virtual void OnPathFailed(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
			}

			protected virtual void OnPreStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
			}

			protected virtual void OnStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				switch (context.GetFact(Facts.PathStatus))
				{
					case 0:
					case 2:
					{
						MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

			protected void TrackWorldState(Rust.Ai.HTN.Murderer.MurdererContext context, Facts fact, byte oldValue, byte newValue)
			{
				if (fact == Facts.PathStatus)
				{
					if (newValue == 2)
					{
						context.OnWorldStateChangedEvent -= new Rust.Ai.HTN.Murderer.MurdererContext.WorldStateChangedEvent(this.TrackWorldState);
						MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
						base.ApplyExpectedEffects(context, context.CurrentTask);
						context.Domain.StopNavigating();
						this.OnPathComplete(context);
						return;
					}
					if (newValue == 3)
					{
						context.OnWorldStateChangedEvent -= new Rust.Ai.HTN.Murderer.MurdererContext.WorldStateChangedEvent(this.TrackWorldState);
						MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
						context.Domain.StopNavigating();
						this.OnPathFailed(context);
					}
				}
			}
		}

		public class MurdererCanNavigateAwayFromAnimal : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererCanNavigateAwayFromAnimal()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (!MurdererDomain.MurdererCanNavigateAwayFromAnimal.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				Vector3 destination = MurdererDomain.MurdererNavigateAwayFromAnimal.GetDestination(c);
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

		public class MurdererCanNavigateAwayFromExplosive : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererCanNavigateAwayFromExplosive()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (!MurdererDomain.MurdererCanNavigateAwayFromExplosive.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				Vector3 destination = MurdererDomain.MurdererNavigateAwayFromExplosive.GetDestination(c);
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

		public class MurdererCanNavigateCloserToPrimaryPlayerTarget : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererCanNavigateCloserToPrimaryPlayerTarget()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (!MurdererDomain.MurdererCanNavigateCloserToPrimaryPlayerTarget.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if ((MurdererDomain.MurdererNavigateCloserToPrimaryPlayerTarget.GetDestination(c) - c.BodyPosition).SqrMagnitudeXZ() < 1f)
				{
					return false;
				}
				return true;
			}
		}

		public class MurdererCanNavigateToLastKnownPositionOfPrimaryEnemyTarget : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererCanNavigateToLastKnownPositionOfPrimaryEnemyTarget()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (c.HasVisitedLastKnownEnemyPlayerLocation)
				{
					return this.score;
				}
				Vector3 destination = MurdererDomain.MurdererNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
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

		public class MurdererCanNavigateToPreferredFightingRange : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			private bool CanNot;

			public MurdererCanNavigateToPreferredFightingRange()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				Vector3 preferredFightingPosition = MurdererDomain.MurdererNavigateToPreferredFightingRange.GetPreferredFightingPosition(c);
				if ((preferredFightingPosition - c.Body.transform.position).sqrMagnitude < 0.01f)
				{
					if (!this.CanNot)
					{
						return 0f;
					}
					return this.score;
				}
				bool flag = c.Memory.IsValid(preferredFightingPosition);
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

		public class MurdererCanNavigateToRoamLocation : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererCanNavigateToRoamLocation()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (!MurdererDomain.MurdererCanNavigateToRoamLocation.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if ((MurdererDomain.MurdererRoamToRandomLocation.GetDestination(c) - c.BodyPosition).SqrMagnitudeXZ() < 1f)
				{
					return false;
				}
				return true;
			}
		}

		public class MurdererCanRememberPrimaryEnemyTarget : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererCanRememberPrimaryEnemyTarget()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class MurdererCanThrowAtLastKnownLocation : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererCanThrowAtLastKnownLocation()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (!MurdererDomain.MurdererCanThrowAtLastKnownLocation.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (!ConVar.AI.npc_use_thrown_weapons || !Halloween.scarecrows_throw_beancans)
				{
					return false;
				}
				if (c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return false;
				}
				if (UnityEngine.Time.time - MurdererDomain.MurdererUseThrowableWeapon.LastTimeThrown < Halloween.scarecrow_throw_beancan_global_delay)
				{
					return false;
				}
				Vector3 destination = MurdererDomain.MurdererNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
				if ((destination - c.BodyPosition).sqrMagnitude < 0.1f)
				{
					destination = c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.transform.position;
					if ((destination - c.BodyPosition).sqrMagnitude < 0.1f)
					{
						return false;
					}
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

		public class MurdererCanUseWeaponAtCurrentRange : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererCanUseWeaponAtCurrentRange()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (!MurdererDomain.MurdererCanUseWeaponAtCurrentRange.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Murderer.MurdererContext c)
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

		public class MurdererChangeFirearmOrder : EffectBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public FirearmOrders Order;

			public MurdererChangeFirearmOrder()
			{
			}

			public override void Apply(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.FirearmOrder, this.Order, temporary);
					return;
				}
				context.SetFact(Facts.FirearmOrder, this.Order, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.FirearmOrder);
					return;
				}
				context.SetFact(Facts.FirearmOrder, context.GetPreviousFact(Facts.FirearmOrder), true, true, true);
			}
		}

		public class MurdererChasePrimaryPlayerTarget : MurdererDomain.MurdererBaseNavigateTo
		{
			public MurdererChasePrimaryPlayerTarget()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				return MurdererDomain.MurdererChasePrimaryPlayerTarget.GetPreferredFightingPosition(context);
			}

			public static Vector3 GetPreferredFightingPosition(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				return MurdererDomain.MurdererNavigateToPreferredFightingRange.GetPreferredFightingPosition(context);
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Domain.NavAgent.stoppingDistance = Halloween.scarecrow_chase_stopping_distance;
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				if (context.Memory == null || context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null || context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.transform == null || context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.IsDestroyed || context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.IsWounded() || context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.IsDead())
				{
					return OperatorStateType.Aborted;
				}
				Vector3 vector3 = this._GetDestination(context);
				if (context.Memory != null && context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player != null)
				{
					if (context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.estimatedSpeed2D >= 0.01f)
					{
						context.Domain.NavAgent.stoppingDistance = Halloween.scarecrow_chase_stopping_distance;
					}
					else
					{
						context.Domain.NavAgent.stoppingDistance = 1f;
					}
					if ((context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.transform.position - vector3).SqrMagnitudeXZ() > 0.5f)
					{
						context.Domain.SetDestination(vector3, false);
					}
				}
				return base.Tick(context, task);
			}
		}

		public class MurdererDuck : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererDuck()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				context.Body.modelState.ducked = false;
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Body.modelState.ducked = true;
				MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class MurdererDuckTimed : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			private float _duckTimeMin;

			[ApexSerialization]
			private float _duckTimeMax;

			public MurdererDuckTimed()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				context.Body.StopCoroutine(this.AsyncTimer(context, 0f));
				this.Reset(context);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Murderer.MurdererContext context, float time)
			{
				MurdererDomain.MurdererDuckTimed murdererDuckTimed = null;
				yield return CoroutineEx.waitForSeconds(time);
				murdererDuckTimed.Reset(context);
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Body.modelState.ducked = true;
				context.SetFact(Facts.IsDucking, true, true, true, true);
				MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

			private void Reset(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsDucking))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class MurdererHasFirearmOrder : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public FirearmOrders Order;

			public MurdererHasFirearmOrder()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				return this.score;
			}
		}

		public class MurdererHasItem : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public MurdererHasItem()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (!MurdererDomain.MurdererHasItem.Test(c, this.Value))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Test(Rust.Ai.HTN.Murderer.MurdererContext c, ItemType Value)
			{
				bool flag;
				c.Body.inventory.AllItemsNoAlloc(ref BaseNpcContext.InventoryLookupCache);
				List<Item>.Enumerator enumerator = BaseNpcContext.InventoryLookupCache.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Item current = enumerator.Current;
						if (Value == ItemType.HealingItem && current.info.category == ItemCategory.Medical)
						{
							flag = true;
							return flag;
						}
						else if (Value == ItemType.MeleeWeapon && (current.info.category == ItemCategory.Weapon || current.info.category == ItemCategory.Tool || current.info.category == ItemCategory.Misc) && current.GetHeldEntity() is BaseMelee)
						{
							flag = true;
							return flag;
						}
						else if (Value == ItemType.ProjectileWeapon && current.info.category == ItemCategory.Weapon && current.GetHeldEntity() is BaseProjectile)
						{
							flag = true;
							return flag;
						}
						else if (Value == ItemType.ThrowableWeapon && current.info.category == ItemCategory.Weapon && current.GetHeldEntity() is ThrownWeapon)
						{
							flag = true;
							return flag;
						}
						else if (Value != ItemType.LightSourceItem || current.info.category != ItemCategory.Tool)
						{
							if (Value != ItemType.ResearchItem || current.info.category != ItemCategory.Tool)
							{
								continue;
							}
							flag = true;
							return flag;
						}
						else
						{
							flag = true;
							return flag;
						}
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
		}

		public class MurdererHasWorldState : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public MurdererHasWorldState()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (c.GetWorldState(this.Fact) != this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class MurdererHasWorldStateBool : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public MurdererHasWorldStateBool()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
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

		public class MurdererHasWorldStateEnemyRange : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public EnemyRange Value;

			public MurdererHasWorldStateEnemyRange()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (c.GetWorldState(Facts.EnemyRange) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class MurdererHasWorldStateGreaterThan : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public MurdererHasWorldStateGreaterThan()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (c.GetWorldState(this.Fact) <= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class MurdererHasWorldStateHealth : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public HealthState Value;

			public MurdererHasWorldStateHealth()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (c.GetWorldState(Facts.HealthState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class MurdererHasWorldStateLessThan : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public MurdererHasWorldStateLessThan()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (c.GetWorldState(this.Fact) >= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class MurdererHoldItemOfType : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			private ItemType _item;

			[ApexSerialization]
			private float _switchTime;

			public MurdererHoldItemOfType()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				MurdererDomain.MurdererHoldItemOfType.SwitchToItem(context, (ItemType)context.GetPreviousFact(Facts.HeldItemType));
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				MurdererDomain.MurdererHoldItemOfType.SwitchToItem(context, this._item);
				context.Body.StartCoroutine(this.WaitAsync(context));
			}

			public static void SwitchToItem(Rust.Ai.HTN.Murderer.MurdererContext context, ItemType _item)
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
					else if (_item == ItemType.MeleeWeapon && (inventoryLookupCache.info.category == ItemCategory.Weapon || inventoryLookupCache.info.category == ItemCategory.Tool || inventoryLookupCache.info.category == ItemCategory.Misc) && inventoryLookupCache.GetHeldEntity() is BaseMelee)
					{
						context.Body.UpdateActiveItem(inventoryLookupCache.uid);
						context.SetFact(Facts.HeldItemType, _item, true, true, true);
						Chainsaw heldEntity = inventoryLookupCache.GetHeldEntity() as Chainsaw;
						if (heldEntity)
						{
							heldEntity.ServerNPCStart();
						}
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

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsWaiting))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator WaitAsync(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				MurdererDomain.MurdererHoldItemOfType murdererHoldItemOfType = null;
				context.SetFact(Facts.IsWaiting, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(murdererHoldItemOfType._switchTime);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}
		}

		public class MurdererHoldItemOfTypeEffect : EffectBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public MurdererHoldItemOfTypeEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.HeldItemType, this.Value, temporary);
					return;
				}
				context.SetFact(Facts.HeldItemType, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.HeldItemType);
					return;
				}
				context.SetFact(Facts.HeldItemType, context.GetPreviousFact(Facts.HeldItemType), true, true, true);
			}
		}

		public class MurdererHoldLocation : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererHoldLocation()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				return OperatorStateType.Running;
			}
		}

		public class MurdererHoldLocationTimed : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			private float _duckTimeMin;

			[ApexSerialization]
			private float _duckTimeMax;

			public MurdererHoldLocationTimed()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Murderer.MurdererContext context, float time)
			{
				yield return CoroutineEx.waitForSeconds(time);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsWaiting))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class MurdererIdle_JustStandAround : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererIdle_JustStandAround()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsIdle, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				this.ResetWorldState(context);
				context.SetFact(Facts.IsIdle, true, true, true, true);
				context.Domain.ReloadFirearm();
			}

			private void ResetWorldState(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.SetFact(Facts.IsNavigating, false, true, true, true);
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				return OperatorStateType.Running;
			}
		}

		public class MurdererIsHoldingItem : ContextualScorerBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public MurdererIsHoldingItem()
			{
			}

			public override float Score(Rust.Ai.HTN.Murderer.MurdererContext c)
			{
				if (c.GetWorldState(Facts.HeldItemType) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class MurdererIsNavigatingEffect : EffectBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererIsNavigatingEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.IsNavigating, 1, temporary);
					return;
				}
				context.PreviousWorldState[5] = context.WorldState[5];
				context.WorldState[5] = 1;
			}

			public override void Reverse(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.IsNavigating);
					return;
				}
				context.WorldState[5] = context.PreviousWorldState[5];
			}
		}

		public class MurdererIsNotNavigatingEffect : EffectBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererIsNotNavigatingEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner, bool temporary)
			{
				MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, fromPlanner, temporary);
			}

			public static void ApplyStatic(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.IsNavigating, (byte)0, temporary);
					return;
				}
				context.PreviousWorldState[5] = context.WorldState[5];
				context.WorldState[5] = 0;
			}

			public override void Reverse(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.IsNavigating);
					return;
				}
				context.WorldState[5] = context.PreviousWorldState[5];
			}
		}

		public class MurdererLookAround : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			private float _lookAroundTime;

			public MurdererLookAround()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsLookingAround, true, true, true, true);
				context.Body.StartCoroutine(this.LookAroundAsync(context));
			}

			private IEnumerator LookAroundAsync(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				MurdererDomain.MurdererLookAround murdererLookAround = null;
				yield return CoroutineEx.waitForSeconds(murdererLookAround._lookAroundTime);
				if (context.IsFact(Facts.CanSeeEnemy))
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsLookingAround))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class MurdererNavigateAwayFromAnimal : MurdererDomain.MurdererBaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsAvoidingAnimalOnComplete;

			public MurdererNavigateAwayFromAnimal()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				return MurdererDomain.MurdererNavigateAwayFromAnimal.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
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

			protected override void OnPathComplete(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				if (this.DisableIsAvoidingAnimalOnComplete)
				{
					context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsAvoidingAnimal, true, true, true, true);
			}
		}

		public class MurdererNavigateAwayFromExplosive : MurdererDomain.MurdererBaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsAvoidingExplosiveOnComplete;

			public MurdererNavigateAwayFromExplosive()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				return MurdererDomain.MurdererNavigateAwayFromExplosive.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
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

			protected override void OnPathComplete(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				if (this.DisableIsAvoidingExplosiveOnComplete)
				{
					context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsAvoidingExplosive, true, true, true, true);
			}
		}

		public class MurdererNavigateCloserToPrimaryPlayerTarget : MurdererDomain.MurdererBaseNavigateTo
		{
			public MurdererNavigateCloserToPrimaryPlayerTarget()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				return MurdererDomain.MurdererNavigateCloserToPrimaryPlayerTarget.GetDestination(context);
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				this.OnPreStart(context);
				context.ReserveCoverPoint(null);
				context.Domain.SetDestination(this._GetDestination(context), true);
				if (!this.RunUntilArrival)
				{
					context.OnWorldStateChangedEvent += new Rust.Ai.HTN.Murderer.MurdererContext.WorldStateChangedEvent(this.TrackWorldState);
				}
				this.OnStart(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				NpcPlayerInfo primaryEnemyPlayerTarget = context.GetPrimaryEnemyPlayerTarget();
				if (primaryEnemyPlayerTarget.Player != null)
				{
					return primaryEnemyPlayerTarget.Player.transform.position;
				}
				return context.Body.transform.position;
			}
		}

		public class MurdererNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer : MurdererDomain.MurdererBaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public MurdererNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				return MurdererDomain.MurdererNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetContinuousDestinationFromBody(Rust.Ai.HTN.Murderer.MurdererContext context)
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

			public static Vector3 GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
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

			private void OnContinuePath(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				Vector3 continuousDestinationFromBody = MurdererDomain.MurdererNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetContinuousDestinationFromBody(context);
				if ((context.Body.transform.position - continuousDestinationFromBody).sqrMagnitude <= 0.2f)
				{
					return;
				}
				this.OnPreStart(context);
				context.Domain.SetDestination(continuousDestinationFromBody, false);
				this.OnStart(context);
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
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

		public class MurdererNavigateToLastKnownLocationOfPrimaryEnemyPlayer : MurdererDomain.MurdererBaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public MurdererNavigateToLastKnownLocationOfPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				return MurdererDomain.MurdererNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				NavMeshHit navMeshHit;
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (primaryKnownEnemyPlayer.PlayerInfo.Player != null && !context.HasVisitedLastKnownEnemyPlayerLocation && NavMesh.FindClosestEdge(primaryKnownEnemyPlayer.LastKnownPosition, out navMeshHit, context.Domain.NavAgent.areaMask))
				{
					return navMeshHit.position;
				}
				return context.Body.transform.position;
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
				context.HasVisitedLastKnownEnemyPlayerLocation = true;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
				context.HasVisitedLastKnownEnemyPlayerLocation = false;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}
		}

		public class MurdererNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer : MurdererDomain.MurdererBaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public MurdererNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				return MurdererDomain.MurdererNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				NavMeshHit navMeshHit;
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (!(primaryKnownEnemyPlayer.PlayerInfo.Player != null) || !NavMesh.FindClosestEdge(primaryKnownEnemyPlayer.OurLastPositionWhenLastSeen, out navMeshHit, context.Domain.NavAgent.areaMask))
				{
					return context.Body.transform.position;
				}
				return context.Domain.ToAllowedMovementDestination(navMeshHit.position);
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}
		}

		public class MurdererNavigateToPreferredFightingRange : MurdererDomain.MurdererBaseNavigateTo
		{
			public MurdererNavigateToPreferredFightingRange()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				return MurdererDomain.MurdererNavigateToPreferredFightingRange.GetPreferredFightingPosition(context);
			}

			public static Vector3 GetPreferredFightingPosition(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				Vector3 vector3;
				Vector3 body;
				NavMeshHit navMeshHit;
				if (UnityEngine.Time.time - context.Memory.CachedPreferredDistanceDestinationTime < 0.01f)
				{
					return context.Memory.CachedPreferredDistanceDestination;
				}
				NpcPlayerInfo primaryEnemyPlayerTarget = context.GetPrimaryEnemyPlayerTarget();
				if (primaryEnemyPlayerTarget.Player != null)
				{
					float single = 1.5f;
					AttackEntity firearm = context.Domain.GetFirearm();
					if (firearm != null)
					{
						single = (firearm.effectiveRangeType != NPCPlayerApex.WeaponTypeEnum.CloseRange ? context.Body.AiDefinition.Engagement.CenterOfMediumRangeFirearm(firearm) : context.Body.AiDefinition.Engagement.CenterOfCloseRangeFirearm(firearm));
					}
					float single1 = single * single;
					if (primaryEnemyPlayerTarget.Player.estimatedSpeed2D <= 5f)
					{
						single -= 0.1f;
						if (primaryEnemyPlayerTarget.SqrDistance > single1)
						{
							body = context.Body.transform.position - primaryEnemyPlayerTarget.Player.transform.position;
							vector3 = body.normalized;
						}
						else
						{
							body = primaryEnemyPlayerTarget.Player.transform.position - context.Body.transform.position;
							vector3 = body.normalized;
						}
					}
					else
					{
						single += 1.5f;
						if (primaryEnemyPlayerTarget.SqrDistance > single1)
						{
							body = primaryEnemyPlayerTarget.Player.transform.position - context.Body.transform.position;
							vector3 = body.normalized;
						}
						else
						{
							body = context.Body.transform.position - primaryEnemyPlayerTarget.Player.transform.position;
							vector3 = body.normalized;
						}
						if (Vector3.Dot(primaryEnemyPlayerTarget.Player.estimatedVelocity, vector3) < 0f)
						{
							if (primaryEnemyPlayerTarget.SqrDistance > single1)
							{
								body = context.Body.transform.position - primaryEnemyPlayerTarget.Player.transform.position;
								vector3 = body.normalized;
							}
							else
							{
								body = primaryEnemyPlayerTarget.Player.transform.position - context.Body.transform.position;
								vector3 = body.normalized;
							}
						}
					}
					Vector3 player = primaryEnemyPlayerTarget.Player.transform.position + (vector3 * single);
					if (!NavMesh.SamplePosition(player + (Vector3.up * 0.1f), out navMeshHit, 2f * context.Domain.NavAgent.height, -1))
					{
						context.Memory.AddFailedDestination(player);
					}
					else
					{
						Vector3 allowedMovementDestination = context.Domain.ToAllowedMovementDestination(navMeshHit.position);
						if (context.Memory.IsValid(allowedMovementDestination))
						{
							context.Memory.CachedPreferredDistanceDestination = allowedMovementDestination;
							context.Memory.CachedPreferredDistanceDestinationTime = UnityEngine.Time.time;
							return allowedMovementDestination;
						}
					}
				}
				return context.Body.transform.position;
			}
		}

		public class MurdererRoamToRandomLocation : MurdererDomain.MurdererBaseNavigateTo
		{
			public MurdererRoamToRandomLocation()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				return MurdererDomain.MurdererRoamToRandomLocation.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				NavMeshHit navMeshHit;
				if (UnityEngine.Time.time - context.Memory.CachedRoamDestinationTime < 0.01f)
				{
					return context.Memory.CachedRoamDestination;
				}
				uint num = (uint)((float)Mathf.Abs(context.Body.GetInstanceID()) + UnityEngine.Time.time);
				for (int i = 0; i < 10; i++)
				{
					Vector2 vector2 = SeedRandom.Value2D(num) * 20f;
					if (vector2.x < 0f)
					{
						vector2.x -= 10f;
					}
					if (vector2.x > 0f)
					{
						vector2.x += 10f;
					}
					if (vector2.y < 0f)
					{
						vector2.y -= 10f;
					}
					if (vector2.y > 0f)
					{
						vector2.y += 10f;
					}
					Vector3 bodyPosition = context.BodyPosition + new Vector3(vector2.x, 0f, vector2.y);
					if (TerrainMeta.HeightMap != null)
					{
						bodyPosition.y = TerrainMeta.HeightMap.GetHeight(bodyPosition);
					}
					if (NavMesh.FindClosestEdge(bodyPosition, out navMeshHit, context.Domain.NavAgent.areaMask))
					{
						bodyPosition = navMeshHit.position;
						if (WaterLevel.GetWaterDepth(bodyPosition) <= 0.01f)
						{
							context.Memory.CachedRoamDestination = bodyPosition;
							context.Memory.CachedRoamDestinationTime = UnityEngine.Time.time;
							return bodyPosition;
						}
					}
					else if (NavMesh.SamplePosition(bodyPosition, out navMeshHit, 5f, context.Domain.NavAgent.areaMask))
					{
						bodyPosition = navMeshHit.position;
						if (WaterLevel.GetWaterDepth(bodyPosition) <= 0.01f)
						{
							context.Memory.CachedRoamDestination = bodyPosition;
							context.Memory.CachedRoamDestinationTime = UnityEngine.Time.time;
							return bodyPosition;
						}
					}
				}
				return context.Body.transform.position;
			}

			protected override void OnStart(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsRoaming, 1, true, true, true);
			}
		}

		public class MurdererStand : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererStand()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				context.Body.StopCoroutine(this.AsyncTimer(context, 0f));
				this.Reset(context);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Murderer.MurdererContext context, float time)
			{
				yield return CoroutineEx.waitForSeconds(time);
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
				yield return CoroutineEx.waitForSeconds(time * 2f);
				context.SetFact(Facts.IsStandingUp, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.SetFact(Facts.IsStandingUp, true, true, true, true);
				context.Body.StartCoroutine(this.AsyncTimer(context, 0.2f));
			}

			private void Reset(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
				context.SetFact(Facts.IsStandingUp, false, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsStandingUp))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class MurdererStopMoving : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			public MurdererStopMoving()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				MurdererDomain.MurdererIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class MurdererUseThrowableWeapon : OperatorBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			private NpcOrientation _orientation;

			public static float LastTimeThrown;

			static MurdererUseThrowableWeapon()
			{
			}

			public MurdererUseThrowableWeapon()
			{
			}

			public override void Abort(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
				MurdererDomain.MurdererHoldItemOfType.SwitchToItem(context, (ItemType)context.GetPreviousFact(Facts.HeldItemType));
			}

			public override void Execute(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				if (context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player != null)
				{
					context.Body.StartCoroutine(this.UseItem(context));
				}
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Murderer.MurdererContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsThrowingWeapon))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator UseItem(Rust.Ai.HTN.Murderer.MurdererContext context)
			{
				MurdererDomain.MurdererUseThrowableWeapon murdererUseThrowableWeapon = null;
				Item activeItem = context.Body.GetActiveItem();
				if (activeItem == null)
				{
					MurdererDomain.MurdererUseThrowableWeapon.LastTimeThrown = UnityEngine.Time.time;
				}
				else
				{
					MurdererDomain.MurdererUseThrowableWeapon.LastTimeThrown = UnityEngine.Time.time;
					ThrownWeapon heldEntity = activeItem.GetHeldEntity() as ThrownWeapon;
					if (heldEntity != null)
					{
						context.SetFact(Facts.IsThrowingWeapon, true, true, true, true);
						yield return CoroutineEx.waitForSeconds(1f + UnityEngine.Random.@value);
						context.OrientationType = murdererUseThrowableWeapon._orientation;
						context.Body.ForceOrientationTick();
						yield return null;
						heldEntity.ServerThrow(context.Memory.PrimaryKnownEnemyPlayer.LastKnownPosition);
						MurdererDomain.MurdererHoldItemOfType.SwitchToItem(context, ItemType.MeleeWeapon);
						yield return CoroutineEx.waitForSeconds(1f);
					}
					heldEntity = null;
				}
				context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
				MurdererDomain.MurdererHoldItemOfType.SwitchToItem(context, ItemType.MeleeWeapon);
			}
		}

		public class MurdererWorldStateBoolEffect : EffectBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public MurdererWorldStateBoolEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class MurdererWorldStateEffect : EffectBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public MurdererWorldStateEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class MurdererWorldStateIncrementEffect : EffectBase<Rust.Ai.HTN.Murderer.MurdererContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public MurdererWorldStateIncrementEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					byte num = context.PeekFactChangeDuringPlanning(this.Fact);
					context.PushFactChangeDuringPlanning(this.Fact, (int)(num + this.Value), temporary);
					return;
				}
				context.SetFact(this.Fact, (int)(context.GetFact(this.Fact) + this.Value), true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Murderer.MurdererContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public delegate void OnPlanAborted(MurdererDomain domain);

		public delegate void OnPlanCompleted(MurdererDomain domain);
	}
}