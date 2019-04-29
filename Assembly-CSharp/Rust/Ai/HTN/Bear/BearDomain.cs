using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using ConVar;
using Rust.Ai;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Bear.Reasoners;
using Rust.Ai.HTN.Bear.Sensors;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.Bear
{
	public class BearDomain : HTNDomain
	{
		[ReadOnly]
		[SerializeField]
		private bool _isRegisteredWithAgency;

		[Header("Context")]
		[SerializeField]
		private Rust.Ai.HTN.Bear.BearContext _context;

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
			new BearPlayersInRangeSensor()
			{
				TickFrequency = 0.5f
			},
			new BearPlayersOutsideRangeSensor()
			{
				TickFrequency = 0.1f
			},
			new BearPlayersDistanceSensor()
			{
				TickFrequency = 0.1f
			},
			new BearPlayersViewAngleSensor()
			{
				TickFrequency = 0.1f
			},
			new BearEnemyPlayersInRangeSensor()
			{
				TickFrequency = 0.1f
			},
			new BearEnemyPlayersLineOfSightSensor()
			{
				TickFrequency = 0.25f
			},
			new BearEnemyPlayersHearingSensor()
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
			new EnemyPlayerHearingReasoner()
			{
				TickFrequency = 0.1f
			},
			new EnemyTargetReasoner()
			{
				TickFrequency = 0.1f
			},
			new PlayersInRangeReasoner()
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
			new HealthReasoner()
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
			new ReturnHomeReasoner()
			{
				TickFrequency = 5f
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
				TickFrequency = 0.1f
			},
			new EnemyRangeReasoner()
			{
				TickFrequency = 0.1f
			}
		};

		private HTNUtilityAiClient _aiClient;

		private Rust.Ai.HTN.Bear.BearDefinition _bearDefinition;

		private static Vector3[] pathCornerCache;

		private static NavMeshPath _pathCache;

		public BearDomain.OnPlanAborted OnPlanAbortedEvent;

		public BearDomain.OnPlanCompleted OnPlanCompletedEvent;

		public Rust.Ai.HTN.Bear.BearContext BearContext
		{
			get
			{
				return this._context;
			}
		}

		public Rust.Ai.HTN.Bear.BearDefinition BearDefinition
		{
			get
			{
				if (this._bearDefinition == null)
				{
					this._bearDefinition = this._context.Body.AiDefinition as Rust.Ai.HTN.Bear.BearDefinition;
				}
				return this._bearDefinition;
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

		static BearDomain()
		{
			BearDomain.pathCornerCache = new Vector3[128];
			BearDomain._pathCache = null;
		}

		public BearDomain()
		{
		}

		protected override void AbortPlan()
		{
			base.AbortPlan();
			BearDomain.OnPlanAborted onPlanAbortedEvent = this.OnPlanAbortedEvent;
			if (onPlanAbortedEvent != null)
			{
				onPlanAbortedEvent(this);
			}
			else
			{
			}
			if (this.BearContext.IsFact(Facts.IsStandingUp))
			{
				this.BearContext.Body.ClientRPC<string, int>(null, "PlayAnimationBool", "standing", 0);
				this.BearContext.Body.ClientRPC<string>(null, "PlayAnimationTrigger", "standDown");
				this.BearContext.SetFact(Facts.IsStandingUp, false, true, true, true);
			}
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

		protected override void CompletePlan()
		{
			base.CompletePlan();
			BearDomain.OnPlanCompleted onPlanCompletedEvent = this.OnPlanCompletedEvent;
			if (onPlanCompletedEvent != null)
			{
				onPlanCompletedEvent(this);
			}
			else
			{
			}
			if (this.BearContext.IsFact(Facts.IsStandingUp))
			{
				this.BearContext.Body.ClientRPC<string, int>(null, "PlayAnimationBool", "standing", 0);
				this.BearContext.Body.ClientRPC<string>(null, "PlayAnimationTrigger", "standDown");
				this.BearContext.SetFact(Facts.IsStandingUp, false, true, true, true);
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

		public override void ForceProjectileOrientation()
		{
		}

		public override IAIContext GetContext(Guid aiId)
		{
			return this._context;
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

		public override Vector3 GetNextPosition(float delta)
		{
			if (!(this.NavAgent == null) && this.NavAgent.isOnNavMesh && this.NavAgent.hasPath)
			{
				return this.NavAgent.nextPosition;
			}
			return this._context.BodyPosition;
		}

		public override void Initialize(BaseEntity body)
		{
			if (this._aiClient == null || this._aiClient.ai == null || this._aiClient.ai.id != AINameMap.HTNDomainAnimalBear)
			{
				this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainAnimalBear, this);
			}
			if (this._context == null || this._context.Body != body)
			{
				this._context = new Rust.Ai.HTN.Bear.BearContext(body as HTNAnimal, this);
			}
			if (this._navAgent == null)
			{
				this._navAgent = base.GetComponent<NavMeshAgent>();
			}
			if (this._navAgent)
			{
				this._navAgent.updateRotation = false;
				this._navAgent.updatePosition = false;
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
			if (single > this.BearContext.Body.AiDefinition.Engagement.SqrMediumRange || !allowCloseRange && single < this.BearContext.Body.AiDefinition.Engagement.SqrCloseRange)
			{
				return true;
			}
			float single1 = Mathf.Sqrt(single);
			if (BearDomain._pathCache == null)
			{
				BearDomain._pathCache = new NavMeshPath();
			}
			if (NavMesh.CalculatePath(from, to, this.NavAgent.areaMask, BearDomain._pathCache))
			{
				int cornersNonAlloc = BearDomain._pathCache.GetCornersNonAlloc(BearDomain.pathCornerCache);
				if (BearDomain._pathCache.status == NavMeshPathStatus.PathComplete && cornersNonAlloc > 1 && Mathf.Abs(single1 - this.PathDistance(cornersNonAlloc, ref BearDomain.pathCornerCache, single1 + ConVar.AI.npc_cover_path_vs_straight_dist_max_diff)) > ConVar.AI.npc_cover_path_vs_straight_dist_max_diff)
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
			this._context.Memory.Forget(this._context.Body.AiDefinition.Memory.ForgetTime);
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
			BearDomain bearDomain = null;
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
				if (!(bearDomain.NavAgent != null) || bearDomain.NavAgent.isOnNavMesh)
				{
					bearDomain.NavAgent.enabled = true;
					bearDomain.NavAgent.stoppingDistance = 1f;
					yield break;
				}
				if (NavMesh.SamplePosition(bearDomain._context.Body.transform.position, out navMeshHit, bearDomain.NavAgent.height * single1, bearDomain.NavAgent.areaMask))
				{
					bearDomain._context.Body.transform.position = navMeshHit.position;
					bearDomain.NavAgent.Warp(bearDomain._context.Body.transform.position);
					bearDomain.NavAgent.enabled = true;
					bearDomain.NavAgent.stoppingDistance = 1f;
					bearDomain.UpdateNavmeshOffset();
					yield break;
				}
				yield return CoroutineEx.waitForSecondsRealtime(single);
				single1 *= 1.5f;
				num++;
			}
			int areaFromName = NavMesh.GetAreaFromName("Walkable");
			if ((bearDomain.NavAgent.areaMask & 1 << (areaFromName & 31)) != 0)
			{
				if (bearDomain._context.Body.transform != null && !bearDomain._context.Body.IsDestroyed)
				{
					UnityEngine.Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[] { bearDomain.name });
					bearDomain._context.Body.Kill(BaseNetworkable.DestroyMode.None);
				}
				yield break;
			}
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
			bearDomain.NavAgent.agentTypeID = settingsByIndex.agentTypeID;
			bearDomain.NavAgent.areaMask = 1 << (areaFromName & 31);
			yield return bearDomain.TryForceToNavmesh();
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

		public abstract class BaseNavigateTo : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public bool RunUntilArrival;

			protected BaseNavigateTo()
			{
			}

			protected abstract Vector3 _GetDestination(Rust.Ai.HTN.Bear.BearContext context);

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				context.Domain.StopNavigating();
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
				this.OnPreStart(context);
				context.Domain.SetDestination(this._GetDestination(context));
				if (!this.RunUntilArrival)
				{
					context.OnWorldStateChangedEvent += new Rust.Ai.HTN.Bear.BearContext.WorldStateChangedEvent(this.TrackWorldState);
				}
				this.OnStart(context);
			}

			protected virtual void OnPathComplete(Rust.Ai.HTN.Bear.BearContext context)
			{
			}

			protected virtual void OnPathFailed(Rust.Ai.HTN.Bear.BearContext context)
			{
			}

			protected virtual void OnPreStart(Rust.Ai.HTN.Bear.BearContext context)
			{
			}

			protected virtual void OnStart(Rust.Ai.HTN.Bear.BearContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				switch (context.GetFact(Facts.PathStatus))
				{
					case 0:
					case 2:
					{
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

			private void TrackWorldState(Rust.Ai.HTN.Bear.BearContext context, Facts fact, byte oldValue, byte newValue)
			{
				if (fact == Facts.PathStatus)
				{
					if (newValue == 2)
					{
						context.OnWorldStateChangedEvent -= new Rust.Ai.HTN.Bear.BearContext.WorldStateChangedEvent(this.TrackWorldState);
						base.ApplyExpectedEffects(context, context.CurrentTask);
						context.Domain.StopNavigating();
						this.OnPathComplete(context);
						return;
					}
					if (newValue == 3)
					{
						context.OnWorldStateChangedEvent -= new Rust.Ai.HTN.Bear.BearContext.WorldStateChangedEvent(this.TrackWorldState);
						context.Domain.StopNavigating();
						this.OnPathFailed(context);
					}
				}
			}
		}

		public class BearApplyFrustration : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			public BearApplyFrustration()
			{
			}

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class BearArrivedAtLocation : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			public BearArrivedAtLocation()
			{
			}

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class BearCanAttackAtCurrentRange : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			public BearCanAttackAtCurrentRange()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
			{
				if (!BearDomain.BearCanAttackAtCurrentRange.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Bear.BearContext c)
			{
				if (c.GetFact(Facts.EnemyRange) == 0)
				{
					return true;
				}
				return false;
			}
		}

		public class BearCanNavigateAwayFromAnimal : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			public BearCanNavigateAwayFromAnimal()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
			{
				if (!BearDomain.BearCanNavigateAwayFromAnimal.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(Rust.Ai.HTN.Bear.BearContext c)
			{
				Vector3 destination = BearDomain.BearNavigateAwayFromAnimal.GetDestination(c);
				if (!c.Domain.AllowedMovementDestination(destination))
				{
					return false;
				}
				if ((destination - c.Body.transform.position).sqrMagnitude < 0.1f)
				{
					return false;
				}
				return c.Memory.IsValid(destination);
			}
		}

		public class BearCanNavigateToLastKnownPositionOfPrimaryEnemyTarget : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			public BearCanNavigateToLastKnownPositionOfPrimaryEnemyTarget()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
			{
				if (c.HasVisitedLastKnownEnemyPlayerLocation)
				{
					return this.score;
				}
				Vector3 destination = BearDomain.BearNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
				if (!c.Domain.AllowedMovementDestination(destination))
				{
					return 0f;
				}
				if ((destination - c.Body.transform.position).sqrMagnitude < 0.1f)
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

		public class BearCanNavigateToPreferredFightingRange : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			private bool CanNot;

			public BearCanNavigateToPreferredFightingRange()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
			{
				Vector3 preferredFightingPosition = BearDomain.BearNavigateToPreferredFightingRange.GetPreferredFightingPosition(c);
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

		public class BearCanRememberPrimaryEnemyTarget : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			public BearCanRememberPrimaryEnemyTarget()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
			{
				if (c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class BearHasWorldState : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public BearHasWorldState()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
			{
				if (c.GetWorldState(this.Fact) != this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class BearHasWorldStateBool : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public BearHasWorldStateBool()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
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

		public class BearHasWorldStateEnemyRange : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public EnemyRange Value;

			public BearHasWorldStateEnemyRange()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
			{
				if (c.GetWorldState(Facts.EnemyRange) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class BearHasWorldStateGreaterThan : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public BearHasWorldStateGreaterThan()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
			{
				if (c.GetWorldState(this.Fact) <= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class BearHasWorldStateHealth : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public HealthState Value;

			public BearHasWorldStateHealth()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
			{
				if (c.GetWorldState(Facts.HealthState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class BearHasWorldStateLessThan : ContextualScorerBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public BearHasWorldStateLessThan()
			{
			}

			public override float Score(Rust.Ai.HTN.Bear.BearContext c)
			{
				if (c.GetWorldState(this.Fact) >= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class BearHealEffect : EffectBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public HealthState Health;

			public BearHealEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.HealthState, this.Health, temporary);
					return;
				}
				context.SetFact(Facts.HealthState, this.Health, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.HealthState);
					return;
				}
				context.SetFact(Facts.HealthState, context.GetPreviousFact(Facts.HealthState), true, true, true);
			}
		}

		public class BearIdle_JustStandAround : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			public BearIdle_JustStandAround()
			{
			}

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsIdle, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
				this.ResetWorldState(context);
				context.SetFact(Facts.IsIdle, true, true, true, true);
			}

			private void ResetWorldState(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.SetFact(Facts.IsNavigating, false, true, true, true);
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				return OperatorStateType.Running;
			}
		}

		public class BearIsNavigatingEffect : EffectBase<Rust.Ai.HTN.Bear.BearContext>
		{
			public BearIsNavigatingEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.IsNavigating, 1, temporary);
					return;
				}
				context.PreviousWorldState[5] = context.WorldState[5];
				context.WorldState[5] = 1;
			}

			public override void Reverse(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.IsNavigating);
					return;
				}
				context.WorldState[5] = context.PreviousWorldState[5];
			}
		}

		public class BearIsNotNavigatingEffect : EffectBase<Rust.Ai.HTN.Bear.BearContext>
		{
			public BearIsNotNavigatingEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner, bool temporary)
			{
				BearDomain.BearIsNotNavigatingEffect.ApplyStatic(context, fromPlanner, temporary);
			}

			public static void ApplyStatic(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.IsNavigating, (byte)0, temporary);
					return;
				}
				context.PreviousWorldState[5] = context.WorldState[5];
				context.WorldState[5] = 0;
			}

			public override void Reverse(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.IsNavigating);
					return;
				}
				context.WorldState[5] = context.PreviousWorldState[5];
			}
		}

		public class BearLookAround : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			private float _lookAroundTime;

			public BearLookAround()
			{
			}

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.SetFact(Facts.IsLookingAround, true, true, true, true);
				context.Body.StartCoroutine(this.LookAroundAsync(context));
			}

			private IEnumerator LookAroundAsync(Rust.Ai.HTN.Bear.BearContext context)
			{
				BearDomain.BearLookAround bearLookAround = null;
				yield return CoroutineEx.waitForSeconds(bearLookAround._lookAroundTime);
				if (context.IsFact(Facts.CanSeeEnemy))
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsLookingAround))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class BearNavigateAwayFromAnimal : BearDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsAvoidingAnimalOnComplete;

			public BearNavigateAwayFromAnimal()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Bear.BearContext context)
			{
				return BearDomain.BearNavigateAwayFromAnimal.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Bear.BearContext context)
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

			protected override void OnPathComplete(Rust.Ai.HTN.Bear.BearContext context)
			{
				if (this.DisableIsAvoidingAnimalOnComplete)
				{
					context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.SetFact(Facts.IsAvoidingAnimal, true, true, true, true);
			}
		}

		public class BearNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer : BearDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public BearNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Bear.BearContext context)
			{
				return BearDomain.BearNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetContinuousDestinationFromBody(Rust.Ai.HTN.Bear.BearContext context)
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

			public static Vector3 GetDestination(Rust.Ai.HTN.Bear.BearContext context)
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

			private void OnContinuePath(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				Vector3 continuousDestinationFromBody = BearDomain.BearNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetContinuousDestinationFromBody(context);
				if ((context.Body.transform.position - continuousDestinationFromBody).sqrMagnitude <= 0.2f)
				{
					return;
				}
				this.OnPreStart(context);
				context.Domain.SetDestination(continuousDestinationFromBody);
				this.OnStart(context);
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Bear.BearContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
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

		public class BearNavigateToLastKnownLocationOfPrimaryEnemyPlayer : BearDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public BearNavigateToLastKnownLocationOfPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Bear.BearContext context)
			{
				return BearDomain.BearNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Bear.BearContext context)
			{
				NavMeshHit navMeshHit;
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (primaryKnownEnemyPlayer.PlayerInfo.Player != null && !context.HasVisitedLastKnownEnemyPlayerLocation && NavMesh.FindClosestEdge(primaryKnownEnemyPlayer.LastKnownPosition, out navMeshHit, context.Domain.NavAgent.areaMask))
				{
					return navMeshHit.position;
				}
				return context.Body.transform.position;
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Bear.BearContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
				context.HasVisitedLastKnownEnemyPlayerLocation = true;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
				context.HasVisitedLastKnownEnemyPlayerLocation = false;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}
		}

		public class BearNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer : BearDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public BearNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Bear.BearContext context)
			{
				return BearDomain.BearNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetDestination(Rust.Ai.HTN.Bear.BearContext context)
			{
				NavMeshHit navMeshHit;
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (!(primaryKnownEnemyPlayer.PlayerInfo.Player != null) || !NavMesh.FindClosestEdge(primaryKnownEnemyPlayer.OurLastPositionWhenLastSeen, out navMeshHit, context.Domain.NavAgent.areaMask))
				{
					return context.Body.transform.position;
				}
				return context.Domain.ToAllowedMovementDestination(navMeshHit.position);
			}

			protected override void OnPathComplete(Rust.Ai.HTN.Bear.BearContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPathFailed(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.NavAgent.stoppingDistance = 1f;
			}

			protected override void OnPreStart(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.Domain.NavAgent.stoppingDistance = 0.1f;
			}

			protected override void OnStart(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}
		}

		public class BearNavigateToPreferredFightingRange : BearDomain.BaseNavigateTo
		{
			public BearNavigateToPreferredFightingRange()
			{
			}

			protected override Vector3 _GetDestination(Rust.Ai.HTN.Bear.BearContext context)
			{
				return BearDomain.BearNavigateToPreferredFightingRange.GetPreferredFightingPosition(context);
			}

			public static Vector3 GetPreferredFightingPosition(Rust.Ai.HTN.Bear.BearContext context)
			{
				Vector3 vector3;
				Vector3 player;
				NavMeshHit navMeshHit;
				if (UnityEngine.Time.time - context.Memory.CachedPreferredDistanceDestinationTime < 0.01f)
				{
					return context.Memory.CachedPreferredDistanceDestination;
				}
				NpcPlayerInfo primaryEnemyPlayerTarget = context.GetPrimaryEnemyPlayerTarget();
				if (primaryEnemyPlayerTarget.Player != null)
				{
					float closeRange = context.Body.AiDefinition.Engagement.CloseRange;
					if (primaryEnemyPlayerTarget.SqrDistance >= closeRange * closeRange)
					{
						player = primaryEnemyPlayerTarget.Player.transform.position - context.Body.transform.position;
						vector3 = player.normalized;
					}
					else
					{
						player = context.Body.transform.position - primaryEnemyPlayerTarget.Player.transform.position;
						vector3 = player.normalized;
					}
					Vector3 body = context.Body.transform.position + (vector3 * closeRange);
					Vector3 vector31 = body;
					for (int i = 0; i < 10; i++)
					{
						if (!NavMesh.SamplePosition(vector31 + (Vector3.up * 0.1f), out navMeshHit, 2f * context.Domain.NavAgent.height, -1))
						{
							context.Memory.AddFailedDestination(vector31);
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
						Vector2 vector2 = UnityEngine.Random.insideUnitCircle * 5f;
						vector31 = body + new Vector3(vector2.x, 0f, vector2.y);
					}
				}
				return context.Body.transform.position;
			}
		}

		public class BearPlayAnimationBool : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			private float _timeout;

			[ApexSerialization]
			private string animationStr;

			[ApexSerialization]
			private bool animationValue;

			public BearPlayAnimationBool()
			{
			}

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsTransitioning, false, true, true, true);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Bear.BearContext context, float time)
			{
				context.SetFact(Facts.IsTransitioning, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(time);
				context.SetFact(Facts.IsTransitioning, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.Domain.StopNavigating();
				context.Body.ClientRPC<string, bool>(null, "PlayAnimationBool", this.animationStr, this.animationValue);
				context.Body.StartCoroutine(this.AsyncTimer(context, this._timeout));
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsTransitioning))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class BearPlayAnimationInt : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			private float _timeout;

			[ApexSerialization]
			private string animationStr;

			[ApexSerialization]
			private int animationValue;

			public BearPlayAnimationInt()
			{
			}

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsTransitioning, false, true, true, true);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Bear.BearContext context, float time)
			{
				context.SetFact(Facts.IsTransitioning, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(time);
				context.SetFact(Facts.IsTransitioning, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.Domain.StopNavigating();
				context.Body.ClientRPC<string, int>(null, "PlayAnimationInt", this.animationStr, this.animationValue);
				context.Body.StartCoroutine(this.AsyncTimer(context, this._timeout));
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsTransitioning))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class BearPlayAnimationTrigger : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			private float _timeout;

			[ApexSerialization]
			private string animationStr;

			public BearPlayAnimationTrigger()
			{
			}

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsTransitioning, false, true, true, true);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Bear.BearContext context, float time)
			{
				context.SetFact(Facts.IsTransitioning, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(time);
				context.SetFact(Facts.IsTransitioning, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.Domain.StopNavigating();
				context.Body.ClientRPC<string>(null, "PlayAnimationTrigger", this.animationStr);
				context.Body.StartCoroutine(this.AsyncTimer(context, this._timeout));
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsTransitioning))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class BearStandDown : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			private float _standDownTime;

			public BearStandDown()
			{
			}

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsTransitioning, false, true, true, true);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Bear.BearContext context, float time)
			{
				context.SetFact(Facts.IsTransitioning, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(time);
				context.SetFact(Facts.IsTransitioning, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.Domain.StopNavigating();
				context.Body.ClientRPC<string>(null, "PlayAnimationTrigger", "standDown");
				context.Body.StartCoroutine(this.AsyncTimer(context, this._standDownTime));
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsTransitioning))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class BearStandUp : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			private float _standUpTime;

			public BearStandUp()
			{
			}

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				context.Body.ClientRPC<string, int>(null, "PlayAnimationBool", "standing", 0);
				context.Body.ClientRPC<string>(null, "PlayAnimationTrigger", "standDown");
				context.SetFact(Facts.IsTransitioning, false, true, true, true);
			}

			private IEnumerator AsyncTimer(Rust.Ai.HTN.Bear.BearContext context, float time)
			{
				context.SetFact(Facts.IsTransitioning, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(time);
				context.SetFact(Facts.IsTransitioning, false, true, true, true);
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.Domain.StopNavigating();
				context.Body.ClientRPC<string>(null, "PlayAnimationTrigger", "standUp");
				context.Body.StartCoroutine(this.AsyncTimer(context, this._standUpTime));
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsTransitioning))
				{
					return OperatorStateType.Running;
				}
				context.Body.ClientRPC<string, int>(null, "PlayAnimationBool", "standing", 1);
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class BearStopMoving : OperatorBase<Rust.Ai.HTN.Bear.BearContext>
		{
			public BearStopMoving()
			{
			}

			public override void Abort(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(Rust.Ai.HTN.Bear.BearContext context)
			{
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(Rust.Ai.HTN.Bear.BearContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class BearWorldStateBoolEffect : EffectBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public BearWorldStateBoolEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class BearWorldStateEffect : EffectBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public BearWorldStateEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class BearWorldStateIncrementEffect : EffectBase<Rust.Ai.HTN.Bear.BearContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public BearWorldStateIncrementEffect()
			{
			}

			public override void Apply(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					byte num = context.PeekFactChangeDuringPlanning(this.Fact);
					context.PushFactChangeDuringPlanning(this.Fact, (int)(num + this.Value), temporary);
					return;
				}
				context.SetFact(this.Fact, (int)(context.GetFact(this.Fact) + this.Value), true, true, true);
			}

			public override void Reverse(Rust.Ai.HTN.Bear.BearContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public delegate void OnPlanAborted(BearDomain domain);

		public delegate void OnPlanCompleted(BearDomain domain);
	}
}