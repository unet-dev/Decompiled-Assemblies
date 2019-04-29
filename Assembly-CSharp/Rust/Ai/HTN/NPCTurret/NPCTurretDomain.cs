using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using ConVar;
using Rust.Ai;
using Rust.Ai.HTN;
using Rust.Ai.HTN.NPCTurret.Reasoners;
using Rust.Ai.HTN.NPCTurret.Sensors;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.NPCTurret
{
	public class NPCTurretDomain : HTNDomain
	{
		[ReadOnly]
		[SerializeField]
		private bool _isRegisteredWithAgency;

		[Header("Context")]
		[SerializeField]
		private Rust.Ai.HTN.NPCTurret.NPCTurretContext _context;

		[Header("Navigation")]
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
				TickFrequency = 0.25f
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
			new AnimalReasoner()
			{
				TickFrequency = 0.25f
			},
			new AlertnessReasoner()
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

		[ReadOnly]
		[SerializeField]
		public bool BurstAtLongRange;

		private HTNUtilityAiClient _aiClient;

		private Vector3 missOffset;

		private float missToHeadingAlignmentTime;

		private float repeatMissTime;

		private bool recalculateMissOffset = true;

		private bool isMissing;

		public NPCTurretDomain.OnPlanAborted OnPlanAbortedEvent;

		public NPCTurretDomain.OnPlanCompleted OnPlanCompletedEvent;

		public override NavMeshAgent NavAgent
		{
			get
			{
				return null;
			}
		}

		public override BaseNpcContext NpcContext
		{
			get
			{
				return this._context;
			}
		}

		public Rust.Ai.HTN.NPCTurret.NPCTurretContext NPCTurretContext
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

		public NPCTurretDomain()
		{
		}

		protected override void AbortPlan()
		{
			base.AbortPlan();
			NPCTurretDomain.OnPlanAborted onPlanAbortedEvent = this.OnPlanAbortedEvent;
			if (onPlanAbortedEvent == null)
			{
				return;
			}
			onPlanAbortedEvent(this);
		}

		public override bool AllowedMovementDestination(Vector3 destination)
		{
			return false;
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
			NPCTurretDomain.OnPlanCompleted onPlanCompletedEvent = this.OnPlanCompletedEvent;
			if (onPlanCompletedEvent == null)
			{
				return;
			}
			onPlanCompletedEvent(this);
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
			attackEnt.ServerUse(ConVar.AI.npc_htn_player_base_damage_modifier);
			this._lastFirearmUsageTime = time + attackEnt.attackSpacing * 0.5f;
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
			return this._context.Body.eyes.rotation.eulerAngles.normalized;
		}

		public override Vector3 GetHomeDirection()
		{
			return this._context.Body.eyes.rotation.eulerAngles.normalized;
		}

		private Vector3 GetMissVector(Vector3 heading, Vector3 target, Vector3 origin, float maxTime, float repeatTime, float missOffsetMultiplier)
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
				this.missOffset *= missOffsetMultiplier;
				this.missToHeadingAlignmentTime = single + maxTime;
				this.repeatMissTime = this.missToHeadingAlignmentTime + repeatTime;
				this.recalculateMissOffset = false;
				this.isMissing = true;
			}
			Vector3 vector3 = (target + this.missOffset) - origin;
			float single1 = Mathf.Max(this.missToHeadingAlignmentTime - single, 0f);
			float single2 = (Mathf.Approximately(single1, 0f) ? 1f : 1f - Mathf.Min(single1 / maxTime, 1f));
			if (!Mathf.Approximately(single2, 1f))
			{
				return Vector3.Lerp(vector3.normalized, heading, single2);
			}
			this.recalculateMissOffset = true;
			this.isMissing = false;
			return Vector3.Lerp(vector3.normalized, heading, 0.5f + UnityEngine.Random.@value * 0.5f);
		}

		public override Vector3 GetNextPosition(float delta)
		{
			return this._context.BodyPosition;
		}

		private IEnumerator HoldTriggerLogic(BaseProjectile proj, float startTime, float triggerDownInterval)
		{
			NPCTurretDomain nPCTurretDomain = null;
			nPCTurretDomain._isFiring = true;
			nPCTurretDomain._lastFirearmUsageTime = startTime + triggerDownInterval + proj.attackSpacing;
			float single = (nPCTurretDomain.BurstAtLongRange ? 0.75f : 1f);
			while (UnityEngine.Time.time - startTime < triggerDownInterval && nPCTurretDomain._context.IsBodyAlive() && nPCTurretDomain._context.IsFact(Facts.CanSeeEnemy))
			{
				proj.ServerUse(ConVar.AI.npc_htn_player_base_damage_modifier * single);
				yield return CoroutineEx.waitForSeconds(proj.repeatDelay);
				if (proj.primaryMagazine.contents <= 0)
				{
					break;
				}
				if (!nPCTurretDomain.BurstAtLongRange)
				{
					continue;
				}
				single *= 0.15f;
			}
			nPCTurretDomain._isFiring = false;
		}

		public override void Initialize(BaseEntity body)
		{
			if (this._aiClient == null || this._aiClient.ai == null || this._aiClient.ai.id != AINameMap.HTNDomainNPCTurret)
			{
				this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainNPCTurret, this);
			}
			if (this._context == null || this._context.Body != body)
			{
				this._context = new Rust.Ai.HTN.NPCTurret.NPCTurretContext(body as HTNPlayer, this);
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
			float single1 = 2f;
			float single2 = 1f;
			if (this.ReducedLongRangeAccuracy && single > this._context.Body.AiDefinition.Engagement.SqrMediumRangeFirearm(attackEntity))
			{
				fact *= 0.05f;
				single1 = 5f;
				single2 = 5f;
			}
			return this.GetMissVector(heading, target, origin, single1, fact * 2f, single2);
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
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
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
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
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
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
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
			if (!this._context.IsFact(Facts.CanSeeEnemy))
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
					this._context.IncrementFact(Facts.Alertness, 1, true, true, true);
					flag = true;
					goto Label0;
				}
			Label0:
				if (!flag)
				{
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

		public override void Pause()
		{
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
			NPCTurretDomain nPCTurretDomain = null;
			nPCTurretDomain._context.SetFact(Facts.IsReloading, true, true, true, true);
			proj.ServerReload();
			yield return CoroutineEx.waitForSeconds(proj.reloadTime);
			nPCTurretDomain._context.SetFact(Facts.IsReloading, false, true, true, true);
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
			float radius = info.Radius * 0.1f;
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
			float radius = info.Radius * 0.1f;
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
		}

		public override float SqrDistanceToSpawn()
		{
			return 0f;
		}

		public override void Tick(float time)
		{
			base.Tick(time);
			this.TickFirearm(time);
			this._context.Memory.Forget(this._context.Body.AiDefinition.Memory.ForgetTime);
		}

		public override void TickDestinationTracker()
		{
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
				NPCTurretDomain.NPCTurretHoldItemOfType.SwitchToItem(this._context, ItemType.ProjectileWeapon);
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

		public class NPCTurretApplyFirearmOrder : OperatorBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			public NPCTurretApplyFirearmOrder()
			{
			}

			public override void Abort(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.NPCTurret.NPCTurretContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class NPCTurretCanRememberPrimaryEnemyTarget : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			public NPCTurretCanRememberPrimaryEnemyTarget()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
			{
				if (c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class NPCTurretChangeFirearmOrder : EffectBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public FirearmOrders Order;

			public NPCTurretChangeFirearmOrder()
			{
			}

			public override void Apply(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.FirearmOrder, this.Order, temporary);
					return;
				}
				context.SetFact(Facts.FirearmOrder, this.Order, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.FirearmOrder);
					return;
				}
				context.SetFact(Facts.FirearmOrder, context.GetPreviousFact(Facts.FirearmOrder), true, true, true);
			}
		}

		public class NPCTurretHasFirearmOrder : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public FirearmOrders Order;

			public NPCTurretHasFirearmOrder()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
			{
				return this.score;
			}
		}

		public class NPCTurretHasItem : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public NPCTurretHasItem()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
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

		public class NPCTurretHasWorldState : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public NPCTurretHasWorldState()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
			{
				if (c.GetWorldState(this.Fact) != this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class NPCTurretHasWorldStateAmmo : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public AmmoState Value;

			public NPCTurretHasWorldStateAmmo()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
			{
				if (c.GetWorldState(Facts.AmmoState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class NPCTurretHasWorldStateBool : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public NPCTurretHasWorldStateBool()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
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

		public class NPCTurretHasWorldStateEnemyRange : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public EnemyRange Value;

			public NPCTurretHasWorldStateEnemyRange()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
			{
				if (c.GetWorldState(Facts.EnemyRange) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class NPCTurretHasWorldStateGreaterThan : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public NPCTurretHasWorldStateGreaterThan()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
			{
				if (c.GetWorldState(this.Fact) <= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class NPCTurretHasWorldStateHealth : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public HealthState Value;

			public NPCTurretHasWorldStateHealth()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
			{
				if (c.GetWorldState(Facts.HealthState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class NPCTurretHasWorldStateLessThan : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public NPCTurretHasWorldStateLessThan()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
			{
				if (c.GetWorldState(this.Fact) >= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class NPCTurretHealEffect : EffectBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public HealthState Health;

			public NPCTurretHealEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.HealthState, this.Health, temporary);
					return;
				}
				context.SetFact(Facts.HealthState, this.Health, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.HealthState);
					return;
				}
				context.SetFact(Facts.HealthState, context.GetPreviousFact(Facts.HealthState), true, true, true);
			}
		}

		public class NPCTurretHoldItemOfType : OperatorBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			private ItemType _item;

			[ApexSerialization]
			private float _switchTime;

			public NPCTurretHoldItemOfType()
			{
			}

			public override void Abort(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, PrimitiveTaskSelector task)
			{
				this._item = (ItemType)context.GetPreviousFact(Facts.HeldItemType);
				NPCTurretDomain.NPCTurretHoldItemOfType.SwitchToItem(context, this._item);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.NPCTurret.NPCTurretContext context)
			{
				NPCTurretDomain.NPCTurretHoldItemOfType.SwitchToItem(context, this._item);
				context.Body.StartCoroutine(this.WaitAsync(context));
			}

			public static void SwitchToItem(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, ItemType _item)
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

			public override OperatorStateType Tick(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsWaiting))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator WaitAsync(Rust.Ai.HTN.NPCTurret.NPCTurretContext context)
			{
				NPCTurretDomain.NPCTurretHoldItemOfType nPCTurretHoldItemOfType = null;
				context.SetFact(Facts.IsWaiting, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(nPCTurretHoldItemOfType._switchTime);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}
		}

		public class NPCTurretHoldItemOfTypeEffect : EffectBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public NPCTurretHoldItemOfTypeEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.HeldItemType, this.Value, temporary);
					return;
				}
				context.SetFact(Facts.HeldItemType, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.HeldItemType);
					return;
				}
				context.SetFact(Facts.HeldItemType, context.GetPreviousFact(Facts.HeldItemType), true, true, true);
			}
		}

		public class NPCTurretIdle_JustStandAround : OperatorBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			public NPCTurretIdle_JustStandAround()
			{
			}

			public override void Abort(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsIdle, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.NPCTurret.NPCTurretContext context)
			{
				this.ResetWorldState(context);
				context.SetFact(Facts.IsIdle, true, true, true, true);
				context.Domain.ReloadFirearm();
			}

			private void ResetWorldState(Rust.Ai.HTN.NPCTurret.NPCTurretContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, PrimitiveTaskSelector task)
			{
				return OperatorStateType.Running;
			}
		}

		public class NPCTurretIsHoldingItem : ContextualScorerBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public NPCTurretIsHoldingItem()
			{
			}

			public override float Score(Rust.Ai.HTN.NPCTurret.NPCTurretContext c)
			{
				if (c.GetWorldState(Facts.HeldItemType) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class NPCTurretReloadFirearmOperator : OperatorBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			public NPCTurretReloadFirearmOperator()
			{
			}

			public override void Abort(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.NPCTurret.NPCTurretContext context)
			{
				context.Domain.ReloadFirearm();
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsReloading))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class NPCTurretWorldStateBoolEffect : EffectBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public NPCTurretWorldStateBoolEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class NPCTurretWorldStateEffect : EffectBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public NPCTurretWorldStateEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class NPCTurretWorldStateIncrementEffect : EffectBase<Rust.Ai.HTN.NPCTurret.NPCTurretContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public NPCTurretWorldStateIncrementEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					byte num = context.PeekFactChangeDuringPlanning(this.Fact);
					context.PushFactChangeDuringPlanning(this.Fact, (int)(num + this.Value), temporary);
					return;
				}
				context.SetFact(this.Fact, (int)(context.GetFact(this.Fact) + this.Value), true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.NPCTurret.NPCTurretContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public delegate void OnPlanAborted(NPCTurretDomain domain);

		public delegate void OnPlanCompleted(NPCTurretDomain domain);
	}
}