using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using Apex.Serialization;
using ConVar;
using Rust.Ai;
using Rust.AI;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistAStar.Reasoners;
using Rust.Ai.HTN.ScientistAStar.Sensors;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.ScientistAStar
{
	public class ScientistAStarDomain : HTNDomain
	{
		[ReadOnly]
		[SerializeField]
		private bool _isRegisteredWithAgency;

		[Header("Context")]
		[SerializeField]
		private ScientistAStarContext _context;

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
			},
			new AtNextAStarWaypointLocationReasoner()
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

		private HTNUtilityAiClient _aiClient;

		private ScientistAStarDefinition _scientistDefinition;

		private Vector3 missOffset;

		private float missToHeadingAlignmentTime;

		private float repeatMissTime;

		private bool recalculateMissOffset = true;

		private bool isMissing;

		private Vector3 _lastNavigationHeading = Vector3.zero;

		[Header("Pathfinding")]
		[ReadOnly]
		public BasePath Path;

		[ReadOnly]
		public List<BasePathNode> CurrentPath;

		[ReadOnly]
		public int CurrentPathIndex;

		[ReadOnly]
		public bool PathLooping;

		[ReadOnly]
		public BasePathNode FinalDestination;

		[ReadOnly]
		public float StoppingDistance = 1f;

		public ScientistAStarDomain.OnPlanAborted OnPlanAbortedEvent;

		public ScientistAStarDomain.OnPlanCompleted OnPlanCompletedEvent;

		public bool HasPath
		{
			get
			{
				if (this.CurrentPath == null)
				{
					return false;
				}
				return this.CurrentPath.Count > 0;
			}
		}

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

		public ScientistAStarContext ScientistContext
		{
			get
			{
				return this._context;
			}
		}

		public ScientistAStarDefinition ScientistDefinition
		{
			get
			{
				if (this._scientistDefinition == null)
				{
					this._scientistDefinition = this._context.Body.AiDefinition as ScientistAStarDefinition;
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
				BaseEntity parentEntity = this._context.Body.GetParentEntity();
				if (parentEntity == null)
				{
					return this._spawnPosition;
				}
				return parentEntity.transform.TransformPoint(this._spawnPosition);
			}
		}

		public float SqrStoppingDistance
		{
			get
			{
				return this.StoppingDistance * this.StoppingDistance;
			}
		}

		public ScientistAStarDomain()
		{
		}

		private bool _SetDestination(Vector3 destination)
		{
			float single;
			Stack<BasePathNode> basePathNodes;
			BasePathNode closestToPoint = this.Path.GetClosestToPoint(destination);
			if (closestToPoint == null || closestToPoint.transform == null)
			{
				return false;
			}
			BasePathNode basePathNode = this.Path.GetClosestToPoint(base.transform.position);
			if (basePathNode == null || basePathNode.transform == null)
			{
				return false;
			}
			this._context.Memory.HasTargetDestination = true;
			this._context.Memory.TargetDestination = destination;
			if (basePathNode == closestToPoint || (basePathNode.transform.position - closestToPoint.transform.position).sqrMagnitude <= this.SqrStoppingDistance)
			{
				this.CurrentPath.Clear();
				this.CurrentPath.Add(closestToPoint);
				this.CurrentPathIndex = -1;
				this.PathLooping = false;
				this.FinalDestination = closestToPoint;
				return true;
			}
			if (!AStarPath.FindPath(basePathNode, closestToPoint, out basePathNodes, out single))
			{
				return false;
			}
			this.CurrentPath.Clear();
			while (basePathNodes.Count > 0)
			{
				this.CurrentPath.Add(basePathNodes.Pop());
			}
			this.CurrentPathIndex = -1;
			this.PathLooping = false;
			this.FinalDestination = closestToPoint;
			return true;
		}

		protected override void AbortPlan()
		{
			base.AbortPlan();
			ScientistAStarDomain.OnPlanAborted onPlanAbortedEvent = this.OnPlanAbortedEvent;
			if (onPlanAbortedEvent != null)
			{
				onPlanAbortedEvent(this);
			}
			else
			{
			}
			this._context.SetFact(Facts.MaintainCover, 0, true, true, true);
			this._context.Body.modelState.ducked = false;
		}

		public bool AdvancePathMovement()
		{
			if (!this.HasPath)
			{
				return false;
			}
			if (this.AtCurrentPathNode() || this.CurrentPathIndex == -1)
			{
				this.CurrentPathIndex = this.GetLoopedIndex(this.CurrentPathIndex + 1);
			}
			if (this.PathComplete())
			{
				this.ClearPath();
				return false;
			}
			Vector3 vector3 = this.IdealPathPosition();
			float single = Vector3.Distance(vector3, this.CurrentPath[this.CurrentPathIndex].transform.position);
			float single1 = Vector3.Distance(base.transform.position, vector3);
			float single2 = Mathf.InverseLerp(8f, 0f, single1);
			vector3 = vector3 + (ScientistAStarDomain.Direction2D(this.CurrentPath[this.CurrentPathIndex].transform.position, vector3) * Mathf.Min(single, single2 * 20f));
			this.SetDestination(vector3);
			return true;
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

		public bool AtCurrentPathNode()
		{
			if (base.transform == null || this.CurrentPath == null)
			{
				return false;
			}
			if (this.CurrentPathIndex < 0 || this.CurrentPathIndex >= this.CurrentPath.Count)
			{
				return false;
			}
			if (this.CurrentPath[this.CurrentPathIndex] == null || this.CurrentPath[this.CurrentPathIndex].transform == null)
			{
				return false;
			}
			Vector3 item = base.transform.position - this.CurrentPath[this.CurrentPathIndex].transform.position;
			return item.sqrMagnitude <= this.SqrStoppingDistance;
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

		public void ClearPath()
		{
			this.CurrentPath.Clear();
			this.CurrentPathIndex = -1;
		}

		public Vector3 ClosestPointAlongPath(Vector3 start, Vector3 end, Vector3 fromPos)
		{
			Vector3 vector3 = start;
			Vector3 vector31 = end;
			Vector3 vector32 = fromPos;
			Vector3 vector33 = vector31 - vector3;
			float single = Vector3.Dot(vector33, vector32 - vector3);
			float single1 = Vector3.SqrMagnitude(vector31 - vector3);
			float single2 = Mathf.Clamp01(single / single1);
			return vector3 + (vector33 * single2);
		}

		protected override void CompletePlan()
		{
			base.CompletePlan();
			ScientistAStarDomain.OnPlanCompleted onPlanCompletedEvent = this.OnPlanCompletedEvent;
			if (onPlanCompletedEvent != null)
			{
				onPlanCompletedEvent(this);
			}
			else
			{
			}
			this._context.SetFact(Facts.MaintainCover, 0, true, true, true);
			this._context.Body.modelState.ducked = false;
		}

		public static Vector3 Direction2D(Vector3 aimAt, Vector3 aimFrom)
		{
			Vector3 vector3 = new Vector3(aimAt.x, 0f, aimAt.z) - new Vector3(aimFrom.x, 0f, aimFrom.z);
			return vector3.normalized;
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

		public Vector3 GetCurrentPathDestination()
		{
			if (!this.HasPath)
			{
				return base.transform.position;
			}
			if (this.AtCurrentPathNode() || this.CurrentPathIndex == -1)
			{
				this.CurrentPathIndex = this.GetLoopedIndex(this.CurrentPathIndex + 1);
			}
			if (this.CurrentPath[this.CurrentPathIndex] == null)
			{
				UnityEngine.Debug.LogWarning("Scientist CurrentPathIndex was NULL (perhaps the path owner was destroyed but this was not?");
				return base.transform.position;
			}
			return this.CurrentPath[this.CurrentPathIndex].transform.position;
		}

		public bool GetEngagementPath(ref List<BasePathNode> nodes)
		{
			BasePathNode closestToPoint = this.Path.GetClosestToPoint(base.transform.position);
			if (closestToPoint == null || closestToPoint.transform == null)
			{
				return false;
			}
			Vector3 vector3 = closestToPoint.transform.position - base.transform.position;
			Vector3 vector31 = vector3.normalized;
			if (Vector3.Dot(base.transform.forward, vector31) > 0f)
			{
				nodes.Add(closestToPoint);
				if (!closestToPoint.straightaway)
				{
					return true;
				}
			}
			return this.GetPathToClosestTurnableNode(closestToPoint, base.transform.forward, ref nodes);
		}

		public BasePathNode GetFinalDestination()
		{
			if (!this.HasPath)
			{
				return null;
			}
			return this.FinalDestination;
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
			if (this._context.GetFact(Facts.IsNavigating) > 0 && this.HasPath && this.FinalDestination != null)
			{
				Vector3 finalDestination = this.FinalDestination.transform.position - this._context.BodyPosition;
				this._lastNavigationHeading = finalDestination.normalized;
				return this._lastNavigationHeading;
			}
			if (this._lastNavigationHeading.sqrMagnitude > 0f)
			{
				return this._lastNavigationHeading;
			}
			return this._context.Body.eyes.rotation.eulerAngles.normalized;
		}

		public override Vector3 GetHomeDirection()
		{
			Vector3 spawnPosition = this.SpawnPosition - this._context.BodyPosition;
			if (spawnPosition.SqrMagnitudeXZ() >= 0.01f)
			{
				return spawnPosition.normalized;
			}
			return this._context.Body.eyes.rotation.eulerAngles.normalized;
		}

		public int GetLoopedIndex(int index)
		{
			if (!this.HasPath)
			{
				UnityEngine.Debug.LogWarning("Warning, GetLoopedIndex called without a path");
				return 0;
			}
			if (!this.PathLooping)
			{
				return Mathf.Clamp(index, 0, this.CurrentPath.Count - 1);
			}
			if (index >= this.CurrentPath.Count)
			{
				return index % this.CurrentPath.Count;
			}
			if (index >= 0)
			{
				return index;
			}
			return this.CurrentPath.Count - Mathf.Abs(index % this.CurrentPath.Count);
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
			float single;
			if (!this.HasPath)
			{
				return this._context.BodyPosition;
			}
			Vector3 currentPathDestination = this.GetCurrentPathDestination() - base.transform.position;
			if (currentPathDestination.sqrMagnitude <= this.SqrStoppingDistance)
			{
				return this._context.BodyPosition;
			}
			currentPathDestination.Normalize();
			single = (this._context.IsFact(Facts.IsDucking) ? this.ScientistDefinition.Movement.DuckSpeed : this.ScientistDefinition.Movement.WalkSpeed);
			float single1 = (delta > 0.1f ? 0.1f : delta);
			float acceleration = this.ScientistDefinition.Movement.Acceleration;
			float single2 = single * single1 - 0.5f * acceleration * single1 * single1;
			return this._context.BodyPosition + (currentPathDestination * single2);
		}

		public bool GetPathToClosestTurnableNode(BasePathNode start, Vector3 forward, ref List<BasePathNode> nodes)
		{
			Vector3 vector3;
			float single = Single.NegativeInfinity;
			BasePathNode basePathNode = null;
			foreach (BasePathNode basePathNode1 in start.linked)
			{
				vector3 = basePathNode1.transform.position - start.transform.position;
				float single1 = Vector3.Dot(forward, vector3.normalized);
				if (single1 <= single)
				{
					continue;
				}
				single = single1;
				basePathNode = basePathNode1;
			}
			if (basePathNode == null)
			{
				return false;
			}
			nodes.Add(basePathNode);
			if (!basePathNode.straightaway)
			{
				return true;
			}
			vector3 = basePathNode.transform.position - start.transform.position;
			return this.GetPathToClosestTurnableNode(basePathNode, vector3.normalized, ref nodes);
		}

		private IEnumerator HoldTriggerLogic(BaseProjectile proj, float startTime, float triggerDownInterval)
		{
			ScientistAStarDomain scientistAStarDomain = null;
			scientistAStarDomain._isFiring = true;
			scientistAStarDomain._lastFirearmUsageTime = startTime + triggerDownInterval + proj.attackSpacing;
			scientistAStarDomain._context.IncrementFact(Facts.Vulnerability, 1, true, true, true);
			do
			{
				if (UnityEngine.Time.time - startTime >= triggerDownInterval || !scientistAStarDomain._context.IsBodyAlive() || !scientistAStarDomain._context.IsFact(Facts.CanSeeEnemy))
				{
					break;
				}
				proj.ServerUse(ConVar.AI.npc_htn_player_base_damage_modifier);
				yield return CoroutineEx.waitForSeconds(proj.repeatDelay);
			}
			while (proj.primaryMagazine.contents > 0);
			scientistAStarDomain._isFiring = false;
		}

		public Vector3 IdealPathPosition()
		{
			if (!this.HasPath)
			{
				return base.transform.position;
			}
			int loopedIndex = this.GetLoopedIndex(this.CurrentPathIndex - 1);
			if (loopedIndex == this.CurrentPathIndex)
			{
				return this.CurrentPath[this.CurrentPathIndex].transform.position;
			}
			return this.ClosestPointAlongPath(this.CurrentPath[loopedIndex].transform.position, this.CurrentPath[this.CurrentPathIndex].transform.position, base.transform.position);
		}

		public bool IndexValid(int index)
		{
			if (!this.HasPath)
			{
				return false;
			}
			if (index < 0)
			{
				return false;
			}
			return index < this.CurrentPath.Count;
		}

		public override void Initialize(BaseEntity body)
		{
			if (this._aiClient == null || this._aiClient.ai == null || this._aiClient.ai.id != AINameMap.HTNDomainScientistAStar)
			{
				this._aiClient = new HTNUtilityAiClient(AINameMap.HTNDomainScientistAStar, this);
			}
			if (this._context == null || this._context.Body != body)
			{
				this._context = new ScientistAStarContext(body as HTNPlayer, this);
			}
			this._spawnPosition = this._context.Body.transform.localPosition;
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

		public void InstallPath(BasePath path)
		{
			this.Path = path;
			this.CurrentPath = new List<BasePathNode>();
			this.CurrentPathIndex = -1;
			this._context.Memory.TargetDestination = this._context.BodyPosition;
			this.FinalDestination = null;
		}

		public bool IsAtDestination()
		{
			Vector3 targetDestination = base.transform.position - this._context.Memory.TargetDestination;
			return targetDestination.sqrMagnitude <= this.SqrStoppingDistance;
		}

		public bool IsAtFinalDestination()
		{
			if (this.FinalDestination == null)
			{
				return true;
			}
			Vector3 finalDestination = base.transform.position - this.FinalDestination.transform.position;
			return finalDestination.sqrMagnitude <= this.SqrStoppingDistance;
		}

		public bool IsPathValid()
		{
			if (!this._context.IsBodyAlive())
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
			if (single <= this._context.Body.AiDefinition.Engagement.SqrCloseRangeFirearm(this.GetFirearm()) + 2f)
			{
				return heading;
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

		public bool PathComplete()
		{
			if (!this.HasPath || this.FinalDestination == null)
			{
				return true;
			}
			Vector3 finalDestination = this.FinalDestination.transform.position - base.transform.position;
			return finalDestination.sqrMagnitude <= this.SqrStoppingDistance;
		}

		public Vector3 PathDirection(int index)
		{
			if (!this.HasPath || this.CurrentPath.Count <= 1)
			{
				return base.transform.forward;
			}
			index = this.GetLoopedIndex(index);
			Vector3 item = Vector3.zero;
			Vector3 vector3 = Vector3.zero;
			if (!this.PathLooping)
			{
				item = (index - 1 >= 0 ? this.CurrentPath[index - 1].transform.position : base.transform.position);
				vector3 = this.CurrentPath[index].transform.position;
			}
			else
			{
				int loopedIndex = this.GetLoopedIndex(index - 1);
				item = this.CurrentPath[loopedIndex].transform.position;
				vector3 = this.CurrentPath[this.GetLoopedIndex(index)].transform.position;
			}
			return (vector3 - item).normalized;
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
			if (single <= this.ScientistContext.Body.AiDefinition.Engagement.SqrMediumRange)
			{
				if (allowCloseRange)
				{
					return true;
				}
				float sqrCloseRange = this.ScientistContext.Body.AiDefinition.Engagement.SqrCloseRange;
			}
			return true;
		}

		public override void Pause()
		{
			this.PauseNavigation();
		}

		public void PauseNavigation()
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
			ScientistAStarDomain scientistAStarDomain = null;
			scientistAStarDomain._context.SetFact(Facts.IsReloading, true, true, true, true);
			proj.ServerReload();
			yield return CoroutineEx.waitForSeconds(proj.reloadTime);
			scientistAStarDomain._context.SetFact(Facts.IsReloading, false, true, true, true);
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
		}

		public bool SetDestination(Vector3 destination)
		{
			if (this._SetDestination(destination))
			{
				this._context.SetFact(Facts.PathStatus, (byte)1, true, false, true);
				return true;
			}
			this._context.SetFact(Facts.PathStatus, (byte)3, true, false, true);
			return false;
		}

		public override float SqrDistanceToSpawn()
		{
			return (this._context.BodyPosition - this.SpawnPosition).sqrMagnitude;
		}

		public void StopNavigating()
		{
			this._context.Memory.HasTargetDestination = false;
			this._context.SetFact(Facts.PathStatus, (byte)0, true, false, true);
		}

		public override void Tick(float time)
		{
			base.Tick(time);
			this.TickFirearm(time);
			this._context.Memory.Forget(this._context.Body.AiDefinition.Memory.ForgetTime);
		}

		public override void TickDestinationTracker()
		{
			if (!this.IsPathValid())
			{
				this._context.Memory.AddFailedDestination(this._context.Memory.TargetDestination);
				this._context.Memory.HasTargetDestination = false;
				this._context.SetFact(Facts.PathStatus, (byte)3, true, false, true);
			}
			if (this._context.Memory.HasTargetDestination && this.PathComplete())
			{
				this._context.Memory.HasTargetDestination = false;
				this._context.SetFact(Facts.PathStatus, (byte)2, true, false, true);
			}
			if (this._context.Memory.HasTargetDestination && this.HasPath)
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
				ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(this._context, ItemType.ProjectileWeapon);
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

		public class AStarApplyFirearmOrder : OperatorBase<ScientistAStarContext>
		{
			public AStarApplyFirearmOrder()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistAStarContext context)
			{
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class AStarApplyFrustration : OperatorBase<ScientistAStarContext>
		{
			public AStarApplyFrustration()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistAStarContext context)
			{
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class AStarArrivedAtLocation : OperatorBase<ScientistAStarContext>
		{
			public AStarArrivedAtLocation()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistAStarContext context)
			{
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class AStarCanNavigateAwayFromAnimal : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarCanNavigateAwayFromAnimal()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (!ScientistAStarDomain.AStarCanNavigateAwayFromAnimal.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(ScientistAStarContext c)
			{
				Vector3 destination = ScientistAStarDomain.AStarNavigateAwayFromAnimal.GetDestination(c);
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

		public class AStarCanNavigateAwayFromExplosive : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarCanNavigateAwayFromExplosive()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (!ScientistAStarDomain.AStarCanNavigateAwayFromExplosive.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(ScientistAStarContext c)
			{
				Vector3 destination = ScientistAStarDomain.AStarNavigateAwayFromExplosive.GetDestination(c);
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

		public class AStarCanNavigateToCoverLocation : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			private CoverTactic _preferredTactic;

			public AStarCanNavigateToCoverLocation()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (!ScientistAStarDomain.AStarCanNavigateToCoverLocation.Try(this._preferredTactic, c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(CoverTactic tactic, ScientistAStarContext c)
			{
				Vector3 coverPosition = ScientistAStarDomain.AStarNavigateToCover.GetCoverPosition(tactic, c);
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

		public class AStarCanNavigateToLastKnownPositionOfPrimaryEnemyTarget : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarCanNavigateToLastKnownPositionOfPrimaryEnemyTarget()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (c.HasVisitedLastKnownEnemyPlayerLocation)
				{
					return this.score;
				}
				Vector3 destination = ScientistAStarDomain.AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
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

		public class AStarCanNavigateToPreferredFightingRange : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			private bool CanNot;

			public AStarCanNavigateToPreferredFightingRange()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				Vector3 preferredFightingPosition = ScientistAStarDomain.AStarNavigateToPreferredFightingRange.GetPreferredFightingPosition(c);
				if ((preferredFightingPosition - c.BodyPosition).sqrMagnitude < 0.01f)
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

		public class AStarCanNavigateToWaypoint : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarCanNavigateToWaypoint()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				Vector3 nextWaypointPosition = ScientistAStarDomain.AStarNavigateToWaypoint.GetNextWaypointPosition(c);
				if (!c.Memory.IsValid(nextWaypointPosition))
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarCanRememberPrimaryEnemyTarget : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarCanRememberPrimaryEnemyTarget()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarCanThrowAtLastKnownLocation : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarCanThrowAtLastKnownLocation()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (!ScientistAStarDomain.AStarCanThrowAtLastKnownLocation.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(ScientistAStarContext c)
			{
				if (!ConVar.AI.npc_use_thrown_weapons)
				{
					return false;
				}
				if (c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return false;
				}
				if (UnityEngine.Time.time - ScientistAStarDomain.AStarUseThrowableWeapon.LastTimeThrown < 10f)
				{
					return false;
				}
				Vector3 destination = ScientistAStarDomain.AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(c);
				if ((destination - c.BodyPosition).sqrMagnitude < 0.1f)
				{
					return false;
				}
				Vector3 eyeOffset = destination + PlayerEyes.EyeOffset;
				Vector3 player = c.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player.transform.position + PlayerEyes.EyeOffset;
				if ((eyeOffset - player).sqrMagnitude > 5f)
				{
					return false;
				}
				Vector3 bodyPosition = c.BodyPosition + PlayerEyes.EyeOffset;
				if (Mathf.Abs(Vector3.Dot((bodyPosition - eyeOffset).normalized, (bodyPosition - player).normalized)) < 0.75f)
				{
					return false;
				}
				if (!c.Body.IsVisible(eyeOffset, Single.PositiveInfinity))
				{
					return false;
				}
				return true;
			}
		}

		public class AStarCanUseWeaponAtCurrentRange : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarCanUseWeaponAtCurrentRange()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (!ScientistAStarDomain.AStarCanUseWeaponAtCurrentRange.Try(c))
				{
					return 0f;
				}
				return this.score;
			}

			public static bool Try(ScientistAStarContext c)
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

		public class AStarChangeFirearmOrder : EffectBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public FirearmOrders Order;

			public AStarChangeFirearmOrder()
			{
			}

			public override void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.FirearmOrder, this.Order, temporary);
					return;
				}
				context.SetFact(Facts.FirearmOrder, this.Order, true, true, true);
			}

			public override void Reverse(ScientistAStarContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.FirearmOrder);
					return;
				}
				context.SetFact(Facts.FirearmOrder, context.GetPreviousFact(Facts.FirearmOrder), true, true, true);
			}
		}

		public class AStarDuck : OperatorBase<ScientistAStarContext>
		{
			public AStarDuck()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				context.Body.modelState.ducked = false;
			}

			public override void Execute(ScientistAStarContext context)
			{
				context.Body.modelState.ducked = true;
				ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class AStarDuckTimed : OperatorBase<ScientistAStarContext>
		{
			[ApexSerialization]
			private float _duckTimeMin;

			[ApexSerialization]
			private float _duckTimeMax;

			public AStarDuckTimed()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				context.Body.StopCoroutine(this.AsyncTimer(context, 0f));
				this.Reset(context);
			}

			private IEnumerator AsyncTimer(ScientistAStarContext context, float time)
			{
				ScientistAStarDomain.AStarDuckTimed aStarDuckTimed = null;
				yield return CoroutineEx.waitForSeconds(time);
				aStarDuckTimed.Reset(context);
			}

			public override void Execute(ScientistAStarContext context)
			{
				context.Body.modelState.ducked = true;
				context.SetFact(Facts.IsDucking, true, true, true, true);
				ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

			private void Reset(ScientistAStarContext context)
			{
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsDucking))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class AStarFutureCoverState : EffectBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public CoverTactic Tactic;

			public AStarFutureCoverState()
			{
			}

			public override void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
			{
				CoverState coverState;
				CoverState coverState1;
				CoverPoint cover = ScientistAStarDomain.AStarNavigateToCover.GetCover(this.Tactic, context);
				if (fromPlanner)
				{
					ScientistAStarContext scientistAStarContext = context;
					if (cover == null)
					{
						coverState1 = CoverState.None;
					}
					else
					{
						coverState1 = (cover.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full);
					}
					scientistAStarContext.PushFactChangeDuringPlanning(Facts.CoverState, coverState1, temporary);
					return;
				}
				ScientistAStarContext scientistAStarContext1 = context;
				if (cover == null)
				{
					coverState = CoverState.None;
				}
				else
				{
					coverState = (cover.NormalCoverType == CoverPoint.CoverType.Partial ? CoverState.Partial : CoverState.Full);
				}
				scientistAStarContext1.SetFact(Facts.CoverState, coverState, true, true, true);
			}

			public override void Reverse(ScientistAStarContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.CoverState);
					return;
				}
				context.SetFact(Facts.CoverState, context.GetPreviousFact(Facts.CoverState), true, true, true);
			}
		}

		public class AStarHasFirearmOrder : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public FirearmOrders Order;

			public AStarHasFirearmOrder()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				return this.score;
			}
		}

		public class AStarHasItem : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public AStarHasItem()
			{
			}

			public override float Score(ScientistAStarContext c)
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

		public class AStarHasWorldState : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public AStarHasWorldState()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (c.GetWorldState(this.Fact) != this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarHasWorldStateAmmo : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public AmmoState Value;

			public AStarHasWorldStateAmmo()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (c.GetWorldState(Facts.AmmoState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarHasWorldStateBool : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public AStarHasWorldStateBool()
			{
			}

			public override float Score(ScientistAStarContext c)
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

		public class AStarHasWorldStateCoverState : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public CoverState Value;

			public AStarHasWorldStateCoverState()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (c.GetWorldState(Facts.CoverState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarHasWorldStateEnemyRange : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public EnemyRange Value;

			public AStarHasWorldStateEnemyRange()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (c.GetWorldState(Facts.EnemyRange) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarHasWorldStateGreaterThan : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public AStarHasWorldStateGreaterThan()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (c.GetWorldState(this.Fact) <= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarHasWorldStateHealth : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public HealthState Value;

			public AStarHasWorldStateHealth()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (c.GetWorldState(Facts.HealthState) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarHasWorldStateLessThan : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public AStarHasWorldStateLessThan()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (c.GetWorldState(this.Fact) >= this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarHealEffect : EffectBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public HealthState Health;

			public AStarHealEffect()
			{
			}

			public override void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.HealthState, this.Health, temporary);
					return;
				}
				context.SetFact(Facts.HealthState, this.Health, true, true, true);
			}

			public override void Reverse(ScientistAStarContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.HealthState);
					return;
				}
				context.SetFact(Facts.HealthState, context.GetPreviousFact(Facts.HealthState), true, true, true);
			}
		}

		public class AStarHoldItemOfType : OperatorBase<ScientistAStarContext>
		{
			[ApexSerialization]
			private ItemType _item;

			[ApexSerialization]
			private float _switchTime;

			public AStarHoldItemOfType()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				this._item = (ItemType)context.GetPreviousFact(Facts.HeldItemType);
				ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, this._item);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			public override void Execute(ScientistAStarContext context)
			{
				ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, this._item);
				context.Body.StartCoroutine(this.WaitAsync(context));
			}

			public static void SwitchToItem(ScientistAStarContext context, ItemType _item)
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

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsWaiting))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator WaitAsync(ScientistAStarContext context)
			{
				ScientistAStarDomain.AStarHoldItemOfType aStarHoldItemOfType = null;
				context.SetFact(Facts.IsWaiting, true, true, true, true);
				yield return CoroutineEx.waitForSeconds(aStarHoldItemOfType._switchTime);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}
		}

		public class AStarHoldItemOfTypeEffect : EffectBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public AStarHoldItemOfTypeEffect()
			{
			}

			public override void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.HeldItemType, this.Value, temporary);
					return;
				}
				context.SetFact(Facts.HeldItemType, this.Value, true, true, true);
			}

			public override void Reverse(ScientistAStarContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.HeldItemType);
					return;
				}
				context.SetFact(Facts.HeldItemType, context.GetPreviousFact(Facts.HeldItemType), true, true, true);
			}
		}

		public class AStarHoldLocation : OperatorBase<ScientistAStarContext>
		{
			public AStarHoldLocation()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistAStarContext context)
			{
				ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				return OperatorStateType.Running;
			}
		}

		public class AStarHoldLocationTimed : OperatorBase<ScientistAStarContext>
		{
			[ApexSerialization]
			private float _duckTimeMin;

			[ApexSerialization]
			private float _duckTimeMax;

			public AStarHoldLocationTimed()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			private IEnumerator AsyncTimer(ScientistAStarContext context, float time)
			{
				yield return CoroutineEx.waitForSeconds(time);
				context.SetFact(Facts.IsWaiting, false, true, true, true);
			}

			public override void Execute(ScientistAStarContext context)
			{
				ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsWaiting))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class AStarIdle_JustStandAround : OperatorBase<ScientistAStarContext>
		{
			public AStarIdle_JustStandAround()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsIdle, false, true, true, true);
			}

			public override void Execute(ScientistAStarContext context)
			{
				this.ResetWorldState(context);
				context.SetFact(Facts.IsIdle, true, true, true, true);
				context.Domain.ReloadFirearm();
			}

			private void ResetWorldState(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.SetFact(Facts.IsNavigating, false, true, true, true);
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				return OperatorStateType.Running;
			}
		}

		public class AStarIsHoldingItem : ContextualScorerBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public ItemType Value;

			public AStarIsHoldingItem()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (c.GetWorldState(Facts.HeldItemType) != (byte)this.Value)
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarIsNavigatingEffect : EffectBase<ScientistAStarContext>
		{
			public AStarIsNavigatingEffect()
			{
			}

			public override void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.IsNavigating, 1, temporary);
					return;
				}
				context.PreviousWorldState[5] = context.WorldState[5];
				context.WorldState[5] = 1;
			}

			public override void Reverse(ScientistAStarContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.IsNavigating);
					return;
				}
				context.WorldState[5] = context.PreviousWorldState[5];
			}
		}

		public class AStarIsNavigationAllowed : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarIsNavigationAllowed()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				if (!ScientistAStarDomain.AStarIsNavigationBlocked.CanNavigate(c))
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarIsNavigationBlocked : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarIsNavigationBlocked()
			{
			}

			public static bool CanNavigate(ScientistAStarContext c)
			{
				return false;
			}

			public override float Score(ScientistAStarContext c)
			{
				if (ScientistAStarDomain.AStarIsNavigationBlocked.CanNavigate(c))
				{
					return 0f;
				}
				return this.score;
			}
		}

		public class AStarIsNotNavigatingEffect : EffectBase<ScientistAStarContext>
		{
			public AStarIsNotNavigatingEffect()
			{
			}

			public override void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
			{
				ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, fromPlanner, temporary);
			}

			public static void ApplyStatic(ScientistAStarContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(Facts.IsNavigating, (byte)0, temporary);
					return;
				}
				context.PreviousWorldState[5] = context.WorldState[5];
				context.WorldState[5] = 0;
			}

			public override void Reverse(ScientistAStarContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(Facts.IsNavigating);
					return;
				}
				context.WorldState[5] = context.PreviousWorldState[5];
			}
		}

		public class AStarIsReloadingAllowed : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarIsReloadingAllowed()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				return this.score;
			}
		}

		public class AStarIsReloadingBlocked : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarIsReloadingBlocked()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				return 0f;
			}
		}

		public class AStarIsShootingAllowed : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarIsShootingAllowed()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				return this.score;
			}
		}

		public class AStarIsShootingBlocked : ContextualScorerBase<ScientistAStarContext>
		{
			public AStarIsShootingBlocked()
			{
			}

			public override float Score(ScientistAStarContext c)
			{
				return 0f;
			}
		}

		public class AStarLookAround : OperatorBase<ScientistAStarContext>
		{
			[ApexSerialization]
			private float _lookAroundTime;

			public AStarLookAround()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override void Execute(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsLookingAround, true, true, true, true);
				context.Body.StartCoroutine(this.LookAroundAsync(context));
			}

			private IEnumerator LookAroundAsync(ScientistAStarContext context)
			{
				ScientistAStarDomain.AStarLookAround aStarLookAround = null;
				yield return CoroutineEx.waitForSeconds(aStarLookAround._lookAroundTime);
				if (context.IsFact(Facts.CanSeeEnemy))
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.SetFact(Facts.IsLookingAround, false, true, true, true);
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsLookingAround))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class AStarNavigateAwayFromAnimal : ScientistAStarDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsAvoidingAnimalOnComplete;

			public AStarNavigateAwayFromAnimal()
			{
			}

			protected override Vector3 _GetDestination(ScientistAStarContext context)
			{
				return ScientistAStarDomain.AStarNavigateAwayFromAnimal.GetDestination(context);
			}

			public static Vector3 GetDestination(ScientistAStarContext context)
			{
				if (context.Memory.PrimaryKnownAnimal.Animal == null)
				{
					return context.BodyPosition;
				}
				Vector3 bodyPosition = context.BodyPosition - context.Memory.PrimaryKnownAnimal.Animal.transform.position;
				Vector3 vector3 = bodyPosition.normalized;
				return context.BodyPosition + (vector3 * 10f);
			}

			protected override void OnPathComplete(ScientistAStarContext context)
			{
				if (this.DisableIsAvoidingAnimalOnComplete)
				{
					context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
				}
				context.Domain.StoppingDistance = 1f;
			}

			protected override void OnPathFailed(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsAvoidingAnimal, false, true, true, true);
				context.Domain.StoppingDistance = 1f;
			}

			protected override void OnPreStart(ScientistAStarContext context)
			{
				context.Domain.StoppingDistance = 0.25f;
			}

			protected override void OnStart(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsAvoidingAnimal, true, true, true, true);
			}
		}

		public class AStarNavigateAwayFromExplosive : ScientistAStarDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsAvoidingExplosiveOnComplete;

			public AStarNavigateAwayFromExplosive()
			{
			}

			protected override Vector3 _GetDestination(ScientistAStarContext context)
			{
				return ScientistAStarDomain.AStarNavigateAwayFromExplosive.GetDestination(context);
			}

			public static Vector3 GetDestination(ScientistAStarContext context)
			{
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
				if (entity == null)
				{
					return context.BodyPosition;
				}
				vector3.Normalize();
				return context.BodyPosition + (vector3 * 10f);
			}

			protected override void OnPathComplete(ScientistAStarContext context)
			{
				if (this.DisableIsAvoidingExplosiveOnComplete)
				{
					context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
				}
				context.Domain.StoppingDistance = 1f;
			}

			protected override void OnPathFailed(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsAvoidingExplosive, false, true, true, true);
				context.Domain.StoppingDistance = 1f;
			}

			protected override void OnPreStart(ScientistAStarContext context)
			{
				context.Domain.StoppingDistance = 0.25f;
			}

			protected override void OnStart(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsAvoidingExplosive, true, true, true, true);
			}
		}

		public class AStarNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer : ScientistAStarDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public AStarNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(ScientistAStarContext context)
			{
				return ScientistAStarDomain.AStarNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetContinuousDestinationFromBody(ScientistAStarContext context)
			{
				if (context.Memory.LastClosestEdgeNormal.sqrMagnitude < 0.01f)
				{
					return context.BodyPosition;
				}
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (primaryKnownEnemyPlayer.PlayerInfo.Player == null)
				{
					return context.BodyPosition;
				}
				Vector3 body = context.Body.estimatedVelocity.normalized;
				if (body.sqrMagnitude < 0.01f)
				{
					body = context.Body.estimatedVelocity.normalized;
				}
				if (body.sqrMagnitude < 0.01f)
				{
					body = primaryKnownEnemyPlayer.LastKnownHeading;
				}
				return context.BodyPosition + (body * 2f);
			}

			public static Vector3 GetDestination(ScientistAStarContext context)
			{
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (primaryKnownEnemyPlayer.PlayerInfo.Player != null)
				{
					Vector3 lastKnownPosition = primaryKnownEnemyPlayer.LastKnownPosition + (primaryKnownEnemyPlayer.LastKnownHeading * 2f);
					BasePathNode closestToPoint = context.Domain.Path.GetClosestToPoint(lastKnownPosition);
					if (closestToPoint != null && closestToPoint.transform != null)
					{
						return closestToPoint.transform.position;
					}
				}
				return context.BodyPosition;
			}

			private void OnContinuePath(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				Vector3 continuousDestinationFromBody = ScientistAStarDomain.AStarNavigateInDirectionOfLastKnownHeadingOfPrimaryEnemyPlayer.GetContinuousDestinationFromBody(context);
				if ((context.BodyPosition - continuousDestinationFromBody).sqrMagnitude <= 0.2f)
				{
					return;
				}
				this.OnPreStart(context);
				context.Domain.SetDestination(continuousDestinationFromBody);
				this.OnStart(context);
			}

			protected override void OnPathComplete(ScientistAStarContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.StoppingDistance = 1f;
			}

			protected override void OnPathFailed(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.StoppingDistance = 1f;
			}

			protected override void OnPreStart(ScientistAStarContext context)
			{
				context.Domain.StoppingDistance = 0.25f;
			}

			protected override void OnStart(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				OperatorStateType operatorStateType = base.Tick(context, task);
				return operatorStateType;
			}
		}

		public class AStarNavigateToCover : ScientistAStarDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private CoverTactic _preferredTactic;

			public AStarNavigateToCover()
			{
			}

			private static Vector3 _GetCoverPosition(CoverTactic tactic, ScientistAStarContext context)
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
				return context.BodyPosition;
			}

			protected override Vector3 _GetDestination(ScientistAStarContext context)
			{
				return ScientistAStarDomain.AStarNavigateToCover.GetCoverPosition(this._preferredTactic, context);
			}

			public static CoverPoint GetCover(CoverTactic tactic, ScientistAStarContext context)
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

			public static Vector3 GetCoverPosition(CoverTactic tactic, ScientistAStarContext context)
			{
				return ScientistAStarDomain.AStarNavigateToCover._GetCoverPosition(tactic, context);
			}

			protected override void OnPathComplete(ScientistAStarContext context)
			{
				context.SetFact(Facts.CoverTactic, CoverTactic.None, true, true, true);
			}

			protected override void OnPathFailed(ScientistAStarContext context)
			{
				context.SetFact(Facts.CoverTactic, CoverTactic.None, true, true, true);
			}
		}

		public class AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer : ScientistAStarDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(ScientistAStarContext context)
			{
				return ScientistAStarDomain.AStarNavigateToLastKnownLocationOfPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetDestination(ScientistAStarContext context)
			{
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (primaryKnownEnemyPlayer.PlayerInfo.Player != null && !context.HasVisitedLastKnownEnemyPlayerLocation)
				{
					BasePathNode closestToPoint = context.Domain.Path.GetClosestToPoint(primaryKnownEnemyPlayer.LastKnownPosition);
					if (closestToPoint != null && closestToPoint.transform != null)
					{
						return closestToPoint.transform.position;
					}
				}
				return context.BodyPosition;
			}

			protected override void OnPathComplete(ScientistAStarContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.StoppingDistance = 1f;
				context.HasVisitedLastKnownEnemyPlayerLocation = true;
			}

			protected override void OnPathFailed(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.StoppingDistance = 1f;
				context.HasVisitedLastKnownEnemyPlayerLocation = false;
			}

			protected override void OnPreStart(ScientistAStarContext context)
			{
				context.Domain.StoppingDistance = 0.25f;
			}

			protected override void OnStart(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}
		}

		public class AStarNavigateToNextAStarWaypoint : ScientistAStarDomain.BaseNavigateTo
		{
			private static int index;

			static AStarNavigateToNextAStarWaypoint()
			{
			}

			public AStarNavigateToNextAStarWaypoint()
			{
			}

			protected override Vector3 _GetDestination(ScientistAStarContext context)
			{
				return ScientistAStarDomain.AStarNavigateToNextAStarWaypoint.GetDestination(context);
			}

			public static Vector3 GetDestination(ScientistAStarContext context)
			{
				Vector3 item = context.Domain.Path.nodes[ScientistAStarDomain.AStarNavigateToNextAStarWaypoint.index].transform.position;
				ScientistAStarDomain.AStarNavigateToNextAStarWaypoint.index++;
				if (ScientistAStarDomain.AStarNavigateToNextAStarWaypoint.index >= context.Domain.Path.nodes.Count)
				{
					ScientistAStarDomain.AStarNavigateToNextAStarWaypoint.index = 0;
				}
				return item;
			}

			protected override void OnPathComplete(ScientistAStarContext context)
			{
				context.Domain.StoppingDistance = 1f;
			}

			protected override void OnPathFailed(ScientistAStarContext context)
			{
				context.Domain.StoppingDistance = 1f;
			}

			protected override void OnPreStart(ScientistAStarContext context)
			{
				context.Domain.StoppingDistance = 0.8f;
			}

			protected override void OnStart(ScientistAStarContext context)
			{
			}
		}

		public class AStarNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer : ScientistAStarDomain.BaseNavigateTo
		{
			[ApexSerialization]
			private bool DisableIsSearchingOnComplete;

			public AStarNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer()
			{
			}

			protected override Vector3 _GetDestination(ScientistAStarContext context)
			{
				return ScientistAStarDomain.AStarNavigateToPositionWhereWeLastSawPrimaryEnemyPlayer.GetDestination(context);
			}

			public static Vector3 GetDestination(ScientistAStarContext context)
			{
				BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = context.Memory.PrimaryKnownEnemyPlayer;
				if (primaryKnownEnemyPlayer.PlayerInfo.Player != null)
				{
					return primaryKnownEnemyPlayer.OurLastPositionWhenLastSeen;
				}
				return context.BodyPosition;
			}

			protected override void OnPathComplete(ScientistAStarContext context)
			{
				if (this.DisableIsSearchingOnComplete)
				{
					context.SetFact(Facts.IsSearching, false, true, true, true);
				}
				context.Domain.StoppingDistance = 1f;
			}

			protected override void OnPathFailed(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsSearching, false, true, true, true);
				context.Domain.StoppingDistance = 1f;
			}

			protected override void OnPreStart(ScientistAStarContext context)
			{
				context.Domain.StoppingDistance = 0.25f;
			}

			protected override void OnStart(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsSearching, true, true, true, true);
			}
		}

		public class AStarNavigateToPreferredFightingRange : ScientistAStarDomain.BaseNavigateTo
		{
			public AStarNavigateToPreferredFightingRange()
			{
			}

			protected override Vector3 _GetDestination(ScientistAStarContext context)
			{
				return ScientistAStarDomain.AStarNavigateToPreferredFightingRange.GetPreferredFightingPosition(context);
			}

			public static Vector3 GetPreferredFightingPosition(ScientistAStarContext context)
			{
				Vector3 vector3;
				Vector3 player;
				if (UnityEngine.Time.time - context.Memory.CachedPreferredDistanceDestinationTime < 0.01f)
				{
					return context.Memory.CachedPreferredDistanceDestination;
				}
				NpcPlayerInfo primaryEnemyPlayerTarget = context.GetPrimaryEnemyPlayerTarget();
				if (primaryEnemyPlayerTarget.Player == null)
				{
					return context.BodyPosition;
				}
				AttackEntity firearm = context.Domain.GetFirearm();
				float single = context.Body.AiDefinition.Engagement.CenterOfMediumRangeFirearm(firearm);
				if (primaryEnemyPlayerTarget.SqrDistance >= single * single)
				{
					player = primaryEnemyPlayerTarget.Player.transform.position - context.BodyPosition;
					vector3 = player.normalized;
				}
				else
				{
					player = context.BodyPosition - primaryEnemyPlayerTarget.Player.transform.position;
					vector3 = player.normalized;
				}
				return context.BodyPosition + (vector3 * single);
			}
		}

		public class AStarNavigateToWaypoint : ScientistAStarDomain.BaseNavigateTo
		{
			public AStarNavigateToWaypoint()
			{
			}

			protected override Vector3 _GetDestination(ScientistAStarContext context)
			{
				return ScientistAStarDomain.AStarNavigateToWaypoint.GetNextWaypointPosition(context);
			}

			public static Vector3 GetNextWaypointPosition(ScientistAStarContext context)
			{
				return context.BodyPosition + (Vector3.forward * 10f);
			}
		}

		public class AStarReloadFirearmOperator : OperatorBase<ScientistAStarContext>
		{
			public AStarReloadFirearmOperator()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistAStarContext context)
			{
				context.Domain.ReloadFirearm();
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsReloading))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class AStarStand : OperatorBase<ScientistAStarContext>
		{
			public AStarStand()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				context.Body.StopCoroutine(this.AsyncTimer(context, 0f));
				this.Reset(context);
			}

			private IEnumerator AsyncTimer(ScientistAStarContext context, float time)
			{
				yield return CoroutineEx.waitForSeconds(time);
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
				yield return CoroutineEx.waitForSeconds(time * 2f);
				context.SetFact(Facts.IsStandingUp, false, true, true, true);
			}

			public override void Execute(ScientistAStarContext context)
			{
				context.SetFact(Facts.IsStandingUp, true, true, true, true);
				context.Body.StartCoroutine(this.AsyncTimer(context, 0.2f));
			}

			private void Reset(ScientistAStarContext context)
			{
				context.Body.modelState.ducked = false;
				context.SetFact(Facts.IsDucking, false, true, true, true);
				context.SetFact(Facts.IsStandingUp, false, true, true, true);
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsStandingUp))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class AStarStopMoving : OperatorBase<ScientistAStarContext>
		{
			public AStarStopMoving()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
			}

			public override void Execute(ScientistAStarContext context)
			{
				ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}
		}

		public class AStarUseMedicalTool : OperatorBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public HealthState Health;

			public AStarUseMedicalTool()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsApplyingMedical, false, true, true, true);
				ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, (ItemType)context.GetPreviousFact(Facts.HeldItemType));
			}

			public override void Execute(ScientistAStarContext context)
			{
				context.Body.StartCoroutine(this.UseItem(context));
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsApplyingMedical))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator UseItem(ScientistAStarContext context)
			{
				ScientistAStarDomain.AStarUseMedicalTool aStarUseMedicalTool = null;
				Item activeItem = context.Body.GetActiveItem();
				if (activeItem != null)
				{
					MedicalTool heldEntity = activeItem.GetHeldEntity() as MedicalTool;
					if (heldEntity != null)
					{
						context.SetFact(Facts.IsApplyingMedical, true, true, true, true);
						heldEntity.ServerUse();
						if (aStarUseMedicalTool.Health == HealthState.FullHealth)
						{
							context.Body.Heal(context.Body.MaxHealth());
						}
						yield return CoroutineEx.waitForSeconds(heldEntity.repeatDelay * 4f);
					}
				}
				context.SetFact(Facts.IsApplyingMedical, false, true, true, true);
				ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, context.GetPreviousFact(Facts.HeldItemType));
			}
		}

		public class AStarUseThrowableWeapon : OperatorBase<ScientistAStarContext>
		{
			[ApexSerialization]
			private NpcOrientation _orientation;

			public static float LastTimeThrown;

			static AStarUseThrowableWeapon()
			{
			}

			public AStarUseThrowableWeapon()
			{
			}

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
				ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, (ItemType)context.GetPreviousFact(Facts.HeldItemType));
			}

			public override void Execute(ScientistAStarContext context)
			{
				if (context.Memory.PrimaryKnownEnemyPlayer.PlayerInfo.Player != null)
				{
					context.Body.StartCoroutine(this.UseItem(context));
				}
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				if (context.IsFact(Facts.IsThrowingWeapon))
				{
					return OperatorStateType.Running;
				}
				base.ApplyExpectedEffects(context, task);
				return OperatorStateType.Complete;
			}

			private IEnumerator UseItem(ScientistAStarContext context)
			{
				ScientistAStarDomain.AStarUseThrowableWeapon aStarUseThrowableWeapon = null;
				Item activeItem = context.Body.GetActiveItem();
				if (activeItem != null)
				{
					ThrownWeapon heldEntity = activeItem.GetHeldEntity() as ThrownWeapon;
					if (heldEntity != null)
					{
						context.SetFact(Facts.IsThrowingWeapon, true, true, true, true);
						ScientistAStarDomain.AStarUseThrowableWeapon.LastTimeThrown = UnityEngine.Time.time;
						context.OrientationType = aStarUseThrowableWeapon._orientation;
						context.Body.ForceOrientationTick();
						yield return null;
						heldEntity.ServerThrow(context.Memory.PrimaryKnownEnemyPlayer.LastKnownPosition);
						yield return null;
					}
					heldEntity = null;
				}
				context.SetFact(Facts.IsThrowingWeapon, false, true, true, true);
				ScientistAStarDomain.AStarHoldItemOfType.SwitchToItem(context, ItemType.ProjectileWeapon);
			}
		}

		public class AStarWorldStateBoolEffect : EffectBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public bool Value;

			public AStarWorldStateBoolEffect()
			{
			}

			public override void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(ScientistAStarContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class AStarWorldStateEffect : EffectBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public AStarWorldStateEffect()
			{
			}

			public override void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					context.PushFactChangeDuringPlanning(this.Fact, this.Value, temporary);
					return;
				}
				context.SetFact(this.Fact, this.Value, true, true, true);
			}

			public override void Reverse(ScientistAStarContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public class AStarWorldStateIncrementEffect : EffectBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public Facts Fact;

			[ApexSerialization]
			public byte Value;

			public AStarWorldStateIncrementEffect()
			{
			}

			public override void Apply(ScientistAStarContext context, bool fromPlanner, bool temporary)
			{
				if (fromPlanner)
				{
					byte num = context.PeekFactChangeDuringPlanning(this.Fact);
					context.PushFactChangeDuringPlanning(this.Fact, (int)(num + this.Value), temporary);
					return;
				}
				context.SetFact(this.Fact, (int)(context.GetFact(this.Fact) + this.Value), true, true, true);
			}

			public override void Reverse(ScientistAStarContext context, bool fromPlanner)
			{
				if (fromPlanner)
				{
					context.PopFactChangeDuringPlanning(this.Fact);
					return;
				}
				context.WorldState[(int)this.Fact] = context.PreviousWorldState[(int)this.Fact];
			}
		}

		public abstract class BaseNavigateTo : OperatorBase<ScientistAStarContext>
		{
			[ApexSerialization]
			public bool RunUntilArrival;

			protected BaseNavigateTo()
			{
			}

			protected abstract Vector3 _GetDestination(ScientistAStarContext context);

			public override void Abort(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
				context.Domain.StopNavigating();
			}

			public override void Execute(ScientistAStarContext context)
			{
				this.OnPreStart(context);
				context.ReserveCoverPoint(null);
				context.Domain.SetDestination(this._GetDestination(context));
				if (!this.RunUntilArrival)
				{
					context.OnWorldStateChangedEvent += new ScientistAStarContext.WorldStateChangedEvent(this.TrackWorldState);
				}
				this.OnStart(context);
			}

			protected virtual void OnPathComplete(ScientistAStarContext context)
			{
			}

			protected virtual void OnPathFailed(ScientistAStarContext context)
			{
			}

			protected virtual void OnPreStart(ScientistAStarContext context)
			{
			}

			protected virtual void OnStart(ScientistAStarContext context)
			{
			}

			public override OperatorStateType Tick(ScientistAStarContext context, PrimitiveTaskSelector task)
			{
				switch (context.GetFact(Facts.PathStatus))
				{
					case 0:
					case 2:
					{
						ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
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

			private void TrackWorldState(ScientistAStarContext context, Facts fact, byte oldValue, byte newValue)
			{
				if (fact == Facts.PathStatus)
				{
					if (newValue == 2)
					{
						context.OnWorldStateChangedEvent -= new ScientistAStarContext.WorldStateChangedEvent(this.TrackWorldState);
						ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
						base.ApplyExpectedEffects(context, context.CurrentTask);
						context.Domain.StopNavigating();
						this.OnPathComplete(context);
						return;
					}
					if (newValue == 3)
					{
						context.OnWorldStateChangedEvent -= new ScientistAStarContext.WorldStateChangedEvent(this.TrackWorldState);
						ScientistAStarDomain.AStarIsNotNavigatingEffect.ApplyStatic(context, false, false);
						context.Domain.StopNavigating();
						this.OnPathFailed(context);
					}
				}
			}
		}

		public delegate void OnPlanAborted(ScientistAStarDomain domain);

		public delegate void OnPlanCompleted(ScientistAStarDomain domain);
	}
}