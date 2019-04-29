using Apex.AI;
using Apex.AI.Components;
using Apex.LoadBalancing;
using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Rust.Ai;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class BaseNpc : BaseCombatEntity, IContextProvider, IAIAgent, ILoadBalanced
{
	public int agentTypeIndex;

	public bool NewAI;

	private Vector3 stepDirection;

	private float maxFleeTime;

	private float fleeHealthThresholdPercentage = 1f;

	private float blockEnemyTargetingTimeout = Single.NegativeInfinity;

	private float blockFoodTargetingTimeout = Single.NegativeInfinity;

	private float aggroTimeout = Single.NegativeInfinity;

	private float lastAggroChanceResult;

	private float lastAggroChanceCalcTime;

	private const float aggroChanceRecalcTimeout = 5f;

	private float eatTimeout = Single.NegativeInfinity;

	private float wakeUpBlockMoveTimeout = Single.NegativeInfinity;

	private BaseEntity blockTargetingThisEnemy;

	public float waterDepth;

	public bool swimming;

	public bool wasSwimming;

	private readonly static AnimationCurve speedFractionResponse;

	private bool _traversingNavMeshLink;

	private OffMeshLinkData _currentNavMeshLink;

	private string _currentNavMeshLinkName;

	private float _currentNavMeshLinkTraversalTime;

	private float _currentNavMeshLinkTraversalTimeDelta;

	private Quaternion _currentNavMeshLinkOrientation;

	private Vector3 _currentNavMeshLinkEndPos;

	private float nextAttackTime;

	[InspectorFlags]
	[SerializeField]
	public TerrainTopology.Enum topologyPreference = TerrainTopology.Enum.Forest | TerrainTopology.Enum.Forestside;

	[NonSerialized]
	public Transform ChaseTransform;

	[Header("BaseNpc")]
	public GameObjectRef CorpsePrefab;

	public BaseNpc.AiStatistics Stats;

	public Vector3 AttackOffset;

	public float AttackDamage = 20f;

	public DamageType AttackDamageType = DamageType.Bite;

	[Tooltip("Stamina to use per attack")]
	public float AttackCost = 0.1f;

	[Tooltip("How often can we attack")]
	public float AttackRate = 1f;

	[Tooltip("Maximum Distance for an attack")]
	public float AttackRange = 1f;

	public NavMeshAgent NavAgent;

	[SerializeField]
	private UtilityAIComponent utilityAiComponent;

	public LayerMask movementMask = 429990145;

	[NonSerialized]
	public BaseContext AiContext;

	private bool _isDormant;

	private float lastSetDestinationTime;

	public StateTimer BusyTimer;

	public float Sleep;

	public VitalLevel Stamina;

	public VitalLevel Energy;

	public VitalLevel Hydration;

	[InspectorFlags]
	public BaseNpc.AiFlags aiFlags;

	[NonSerialized]
	public byte[] CurrentFacts = new byte[Enum.GetValues(typeof(BaseNpc.Facts)).Length];

	[Header("NPC Senses")]
	public int ForgetUnseenEntityTime = 10;

	public float SensesTickRate = 0.5f;

	[NonSerialized]
	public BaseEntity[] SensesResults = new BaseEntity[64];

	private List<NavPointSample> navPointSamples = new List<NavPointSample>(8);

	private float lastTickTime;

	private float playerTargetDecisionStartTime;

	private float animalTargetDecisionStartTime;

	private bool isAlreadyCheckingPathPending;

	private int numPathPendingAttempts;

	private float accumPathPendingDelay;

	public const float TickRate = 0.1f;

	private Vector3 lastStuckPos;

	public float stuckDuration;

	public float lastStuckTime;

	public float idleDuration;

	private float nextFlinchTime;

	private float _lastHeardGunshotTime = Single.NegativeInfinity;

	public bool AgencyUpdateRequired
	{
		get;
		set;
	}

	public int AgentTypeIndex
	{
		get
		{
			return this.agentTypeIndex;
		}
		set
		{
			this.agentTypeIndex = value;
		}
	}

	bool Apex.LoadBalancing.ILoadBalanced.repeat
	{
		get
		{
			return true;
		}
	}

	public Vector3 AttackPosition
	{
		get
		{
			return this.ServerPosition + base.transform.TransformDirection(this.AttackOffset);
		}
	}

	public BaseEntity AttackTarget
	{
		get;
		set;
	}

	public Memory.SeenInfo AttackTargetMemory
	{
		get;
		set;
	}

	public float AttackTargetVisibleFor
	{
		get
		{
			return 0f;
		}
	}

	public bool AutoBraking
	{
		get
		{
			if (!this.IsNavRunning())
			{
				return false;
			}
			return this.GetNavAgent.autoBraking;
		}
		set
		{
			if (this.IsNavRunning())
			{
				this.GetNavAgent.autoBraking = value;
			}
		}
	}

	public BaseCombatEntity CombatTarget
	{
		get
		{
			return this.AttackTarget as BaseCombatEntity;
		}
	}

	public Vector3 CrouchedAttackPosition
	{
		get
		{
			return this.AttackPosition;
		}
	}

	public Vector3 CurrentAimAngles
	{
		get
		{
			return base.transform.forward;
		}
	}

	public float currentBehaviorDuration
	{
		get
		{
			return 0f;
		}
	}

	public BaseNpc.Behaviour CurrentBehaviour
	{
		get;
		set;
	}

	public Vector3 Destination
	{
		get
		{
			if (this.IsNavRunning())
			{
				return this.GetNavAgent.destination;
			}
			return this.Entity.ServerPosition;
		}
		set
		{
			if (this.IsNavRunning())
			{
				this.GetNavAgent.destination = value;
				this.lastSetDestinationTime = UnityEngine.Time.time;
			}
		}
	}

	public BaseCombatEntity Entity
	{
		get
		{
			return this;
		}
	}

	public BaseEntity FoodTarget
	{
		get;
		set;
	}

	public float GetAttackCost
	{
		get
		{
			return this.AttackCost;
		}
	}

	public Vector3 GetAttackOffset
	{
		get
		{
			return this.AttackOffset;
		}
	}

	public float GetAttackRange
	{
		get
		{
			return this.AttackRange;
		}
	}

	public float GetAttackRate
	{
		get
		{
			return this.AttackRate;
		}
	}

	public float GetEnergy
	{
		get
		{
			return this.Energy.Level;
		}
	}

	public float GetLastStuckTime
	{
		get
		{
			return this.lastStuckTime;
		}
	}

	public NavMeshAgent GetNavAgent
	{
		get
		{
			if (base.isClient)
			{
				return null;
			}
			if (this.NavAgent == null)
			{
				this.NavAgent = base.GetComponent<NavMeshAgent>();
				if (this.NavAgent == null)
				{
					UnityEngine.Debug.LogErrorFormat("{0} has no nav agent!", new object[] { base.name });
				}
			}
			return this.NavAgent;
		}
	}

	public float GetSleep
	{
		get
		{
			return this.Sleep;
		}
	}

	public float GetStamina
	{
		get
		{
			return this.Stamina.Level;
		}
	}

	public BaseNpc.AiStatistics GetStats
	{
		get
		{
			return this.Stats;
		}
	}

	public float GetStuckDuration
	{
		get
		{
			return this.stuckDuration;
		}
	}

	public bool HasPath
	{
		get
		{
			if (!this.IsNavRunning())
			{
				return false;
			}
			return this.GetNavAgent.hasPath;
		}
	}

	public bool IsChasing
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Chasing);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Chasing, value);
		}
	}

	public bool IsDormant
	{
		get
		{
			return this._isDormant;
		}
		set
		{
			this._isDormant = value;
			if (this._isDormant)
			{
				this.StopMoving();
				this.Pause();
				return;
			}
			if (!(this.GetNavAgent == null) && !AiManager.nav_disable)
			{
				this.Resume();
				return;
			}
			this.IsDormant = true;
		}
	}

	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	public bool IsOnOffmeshLinkAndReachedNewCoord
	{
		get;
		set;
	}

	public bool IsSitting
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Sitting);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Sitting, value);
		}
	}

	public bool IsSleeping
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Sleeping);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Sleeping, value);
		}
	}

	public bool IsStopped
	{
		get
		{
			if (!this.IsNavRunning())
			{
				return true;
			}
			return this.GetNavAgent.isStopped;
		}
		set
		{
			if (this.IsNavRunning())
			{
				if (value)
				{
					this.GetNavAgent.destination = this.ServerPosition;
				}
				this.GetNavAgent.isStopped = value;
			}
		}
	}

	public bool IsStuck
	{
		get
		{
			return JustDecompileGenerated_get_IsStuck();
		}
		set
		{
			JustDecompileGenerated_set_IsStuck(value);
		}
	}

	private bool JustDecompileGenerated_IsStuck_k__BackingField;

	public bool JustDecompileGenerated_get_IsStuck()
	{
		return this.JustDecompileGenerated_IsStuck_k__BackingField;
	}

	public void JustDecompileGenerated_set_IsStuck(bool value)
	{
		this.JustDecompileGenerated_IsStuck_k__BackingField = value;
	}

	public Vector3 LastHeardGunshotDirection
	{
		get;
		set;
	}

	public float LastSetDestinationTime
	{
		get
		{
			return this.lastSetDestinationTime;
		}
	}

	public float SecondsSinceLastHeardGunshot
	{
		get
		{
			return UnityEngine.Time.time - this._lastHeardGunshotTime;
		}
	}

	public float SecondsSinceLastSetDestination
	{
		get
		{
			return UnityEngine.Time.time - this.lastSetDestinationTime;
		}
	}

	public Vector3 SpawnPosition
	{
		get;
		set;
	}

	public float TargetSpeed
	{
		get;
		set;
	}

	public float TimeAtDestination
	{
		get
		{
			return 0f;
		}
	}

	static BaseNpc()
	{
		BaseNpc.speedFractionResponse = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	}

	public BaseNpc()
	{
	}

	public virtual void AddCalories(float amount)
	{
		this.Energy.Add(amount / 1000f);
	}

	private void AggroClosestEnemy()
	{
		float single = Single.MaxValue;
		BasePlayer item = null;
		BaseNpc baseNpc = null;
		this.AiContext.AIAgent.AttackTarget = null;
		Vector3 vector3 = Vector3.zero;
		float single1 = 0f;
		float single2 = 0f;
		foreach (BasePlayer player in this.AiContext.Players)
		{
			if (player.IsDead() || player.IsDestroyed || this.blockTargetingThisEnemy != null && player.net != null && this.blockTargetingThisEnemy.net != null && player.net.ID == this.blockTargetingThisEnemy.net.ID)
			{
				continue;
			}
			Vector3 serverPosition = player.ServerPosition - this.ServerPosition;
			float single3 = serverPosition.sqrMagnitude;
			single1 = single1 + Mathf.Min(Mathf.Sqrt(single3), this.Stats.VisionRange) / this.Stats.VisionRange;
			if (single3 >= single)
			{
				continue;
			}
			single = single3;
			item = player;
			baseNpc = null;
			vector3 = serverPosition;
			if (single > this.AttackRange)
			{
				continue;
			}
			goto Label0;
		}
	Label0:
		if (single > this.AttackRange)
		{
			foreach (BaseNpc npc in this.AiContext.Npcs)
			{
				if (npc.IsDead() || npc.IsDestroyed || this.Stats.Family == npc.Stats.Family)
				{
					continue;
				}
				Vector3 serverPosition1 = npc.ServerPosition - this.ServerPosition;
				float single4 = serverPosition1.sqrMagnitude;
				single2 = single2 + Mathf.Min(Mathf.Sqrt(single4), this.Stats.VisionRange) / this.Stats.VisionRange;
				if (single4 >= single)
				{
					continue;
				}
				single = single4;
				baseNpc = npc;
				item = null;
				vector3 = serverPosition1;
				if (single >= this.AttackRange)
				{
					continue;
				}
				goto Label1;
			}
		}
	Label1:
		if (single > this.AttackRange)
		{
			if (this.AiContext.PlayersBehindUs.Count > 0)
			{
				item = this.AiContext.PlayersBehindUs[0];
				baseNpc = null;
			}
			else if (this.AiContext.NpcsBehindUs.Count > 0)
			{
				item = null;
				baseNpc = this.AiContext.NpcsBehindUs[0];
			}
		}
		if (this.AiContext.EnemyPlayer == null || this.AiContext.EnemyPlayer.IsDestroyed || this.AiContext.EnemyPlayer.IsDead() || single1 > this.AiContext.LastEnemyPlayerScore + this.DecisionMomentumPlayerTarget())
		{
			this.AiContext.EnemyPlayer = item;
			this.AiContext.LastEnemyPlayerScore = single1;
			this.playerTargetDecisionStartTime = UnityEngine.Time.time;
		}
		else if (item == null && this.DecisionMomentumPlayerTarget() < 0.01f)
		{
			this.AiContext.EnemyPlayer = item;
			this.AiContext.LastEnemyPlayerScore = 0f;
			this.playerTargetDecisionStartTime = 0f;
		}
		if (this.AiContext.EnemyNpc == null || this.AiContext.EnemyNpc.IsDestroyed || this.AiContext.EnemyNpc.IsDead() || single2 > this.AiContext.LastEnemyNpcScore + this.DecisionMomentumAnimalTarget())
		{
			this.AiContext.EnemyNpc = baseNpc;
			this.AiContext.LastEnemyNpcScore = single2;
			this.animalTargetDecisionStartTime = UnityEngine.Time.time;
		}
		else if (baseNpc == null && this.DecisionMomentumAnimalTarget() < 0.01f)
		{
			this.AiContext.EnemyNpc = baseNpc;
			this.AiContext.LastEnemyNpcScore = 0f;
			this.animalTargetDecisionStartTime = 0f;
		}
		if (!(item != null) && !(baseNpc != null))
		{
			this.SetFact(BaseNpc.Facts.HasEnemy, 0, true, true);
			this.SetFact(BaseNpc.Facts.EnemyRange, 3, true, true);
			this.SetFact(BaseNpc.Facts.AfraidRange, 1, true, true);
			return;
		}
		this.SetFact(BaseNpc.Facts.HasEnemy, 1, true, true);
		if (item == null)
		{
			this.AiContext.AIAgent.AttackTarget = baseNpc;
		}
		else
		{
			this.AiContext.AIAgent.AttackTarget = item;
		}
		float single5 = vector3.magnitude;
		BaseNpc.EnemyRangeEnum enemyRangeEnum = this.ToEnemyRangeEnum(single5);
		BaseNpc.AfraidRangeEnum afraidRangeEnum = this.ToAfraidRangeEnum(single5);
		this.SetFact(BaseNpc.Facts.EnemyRange, (byte)enemyRangeEnum, true, true);
		this.SetFact(BaseNpc.Facts.AfraidRange, (byte)afraidRangeEnum, true, true);
		this.TryAggro(enemyRangeEnum);
	}

	private static bool AiCaresAbout(BaseEntity ent)
	{
		if (ent is BasePlayer)
		{
			return true;
		}
		if (ent is BaseNpc)
		{
			return true;
		}
		if (!AI.animal_ignore_food)
		{
			if (ent is WorldItem)
			{
				return true;
			}
			if (ent is BaseCorpse)
			{
				return true;
			}
			if (ent is CollectibleEntity)
			{
				return true;
			}
		}
		return false;
	}

	float? Apex.LoadBalancing.ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
	{
		if (base.IsDestroyed || this == null || base.transform == null)
		{
			AnimalSensesLoadBalancer.animalSensesLoadBalancer.Remove(this);
			return new float?(nextInterval);
		}
		using (TimeWarning timeWarning = TimeWarning.New("Animal.TickSenses", 0.1f))
		{
			this.TickSenses();
		}
		using (timeWarning = TimeWarning.New("Animal.TickBehaviourState", 0.1f))
		{
			this.TickBehaviourState();
		}
		return new float?(UnityEngine.Random.@value * 0.1f + 0.1f);
	}

	public bool AttackReady()
	{
		return UnityEngine.Time.realtimeSinceStartup >= this.nextAttackTime;
	}

	public bool BlockEnemyTargeting(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 0)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanTargetEnemies, 0, true, true);
		this.blockEnemyTargetingTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		this.blockTargetingThisEnemy = this.AttackTarget;
		return true;
	}

	public bool BlockFoodTargeting(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanTargetFood, 0, true, true);
		this.blockFoodTargetingTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	public bool BusyTimerActive()
	{
		return this.BusyTimer.IsActive;
	}

	private bool CheckHealthThresholdToFlee()
	{
		object obj;
		if (base.healthFraction > this.Stats.HealthThresholdForFleeing)
		{
			if (this.Stats.HealthThresholdForFleeing < 1f)
			{
				this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, 0, true, true);
				return false;
			}
			if (this.GetFact(BaseNpc.Facts.HasEnemy) == 1)
			{
				this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, 0, true, true);
				return false;
			}
		}
		bool healthThresholdFleeChance = UnityEngine.Random.@value < this.Stats.HealthThresholdFleeChance;
		if (healthThresholdFleeChance)
		{
			obj = 1;
		}
		else
		{
			obj = null;
		}
		this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, (byte)obj, true, true);
		return healthThresholdFleeChance;
	}

	private void CompleteNavMeshLink()
	{
		this.NavAgent.ActivateCurrentOffMeshLink(true);
		this.NavAgent.CompleteOffMeshLink();
		this.NavAgent.isStopped = false;
		this.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
	}

	public override string DebugText()
	{
		string str = base.DebugText();
		str = string.Concat(str, string.Format("\nBehaviour: {0}", this.CurrentBehaviour));
		str = string.Concat(str, string.Format("\nAttackTarget: {0}", this.AttackTarget));
		str = string.Concat(str, string.Format("\nFoodTarget: {0}", this.FoodTarget));
		str = string.Concat(str, string.Format("\nSleep: {0:0.00}", this.Sleep));
		if (this.AiContext != null)
		{
			str = string.Concat(str, string.Format("\nVisible Ents: {0}", this.AiContext.Memory.Visible.Count));
		}
		return str;
	}

	private float DecisionMomentumAnimalTarget()
	{
		float single = UnityEngine.Time.time - this.animalTargetDecisionStartTime;
		if (single > 1f)
		{
			return 0f;
		}
		return single;
	}

	private float DecisionMomentumPlayerTarget()
	{
		float single = UnityEngine.Time.time - this.playerTargetDecisionStartTime;
		if (single > 1f)
		{
			return 0f;
		}
		return single;
	}

	private void DelayedTargetPathStatus()
	{
		this.accumPathPendingDelay += 0.1f;
		this.isAlreadyCheckingPathPending = false;
		this.SetTargetPathStatus(this.accumPathPendingDelay);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.NewAI)
		{
			AnimalSensesLoadBalancer.animalSensesLoadBalancer.Remove(this);
		}
	}

	public virtual void Eat()
	{
		if (!this.FoodTarget)
		{
			return;
		}
		this.BusyTimer.Activate(0.5f, null);
		this.FoodTarget.Eat(this, 0.5f);
		this.StartEating(UnityEngine.Random.@value * 5f + 0.5f);
		base.ClientRPC<Vector3>(null, "Eat", this.FoodTarget.transform.position);
	}

	public virtual float FearLevel(BaseEntity ent)
	{
		float single = 0f;
		BaseNpc baseNpc = ent as BaseNpc;
		if (baseNpc != null && baseNpc.Stats.Size > this.Stats.Size)
		{
			if (baseNpc.WantsToAttack(this) > 0.25f)
			{
				single += 0.2f;
			}
			if (baseNpc.AttackTarget == this)
			{
				single += 0.3f;
			}
			if (baseNpc.CurrentBehaviour == BaseNpc.Behaviour.Attack)
			{
				single *= 1.5f;
			}
			if (baseNpc.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
			{
				single *= 0.1f;
			}
		}
		if (ent is BasePlayer)
		{
			single += 1f;
		}
		return single;
	}

	public float GetActiveAggressionRangeSqr()
	{
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			return this.Stats.DeaggroRange * this.Stats.DeaggroRange;
		}
		return this.Stats.AggressionRange * this.Stats.AggressionRange;
	}

	public IAIContext GetContext(Guid aiId)
	{
		if (this.AiContext == null)
		{
			this.SetupAiContext();
		}
		return this.AiContext;
	}

	public byte GetFact(BaseNpc.Facts fact)
	{
		return this.CurrentFacts[(int)fact];
	}

	public byte GetFact(NPCPlayerApex.Facts fact)
	{
		return (byte)0;
	}

	public byte GetPathStatus()
	{
		if (!this.IsNavRunning())
		{
			return (byte)2;
		}
		return (byte)this.NavAgent.pathStatus;
	}

	public float GetWantsToAttack(BaseEntity target)
	{
		object obj = Interface.CallHook("IOnNpcTarget", this, target);
		if (obj is float)
		{
			return (float)obj;
		}
		return this.WantsToAttack(target);
	}

	private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
	{
		if (!this._traversingNavMeshLink && !this.HandleNavMeshLinkTraversalStart(delta))
		{
			return;
		}
		this.HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
		if (!this.IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
		{
			this._currentNavMeshLinkTraversalTimeDelta += delta;
		}
	}

	private bool HandleNavMeshLinkTraversalStart(float delta)
	{
		OffMeshLinkData navAgent = this.NavAgent.currentOffMeshLinkData;
		if (!navAgent.valid || !navAgent.activated || navAgent.offMeshLink == null)
		{
			return false;
		}
		Vector3 vector3 = (navAgent.endPos - navAgent.startPos).normalized;
		vector3.y = 0f;
		Vector3 navAgent1 = this.NavAgent.desiredVelocity;
		navAgent1.y = 0f;
		if (Vector3.Dot(navAgent1, vector3) < 0.1f)
		{
			this.CompleteNavMeshLink();
			return false;
		}
		this._currentNavMeshLink = navAgent;
		this._currentNavMeshLinkName = this._currentNavMeshLink.linkType.ToString();
		if (!navAgent.offMeshLink.biDirectional)
		{
			this._currentNavMeshLinkEndPos = navAgent.endPos;
			this._currentNavMeshLinkOrientation = Quaternion.LookRotation((navAgent.endPos + (Vector3.up * (navAgent.startPos.y - navAgent.endPos.y))) - navAgent.startPos);
		}
		else if ((navAgent.endPos - this.ServerPosition).sqrMagnitude >= 0.05f)
		{
			this._currentNavMeshLinkEndPos = navAgent.endPos;
			this._currentNavMeshLinkOrientation = Quaternion.LookRotation((navAgent.endPos + (Vector3.up * (navAgent.startPos.y - navAgent.endPos.y))) - navAgent.startPos);
		}
		else
		{
			this._currentNavMeshLinkEndPos = navAgent.startPos;
			this._currentNavMeshLinkOrientation = Quaternion.LookRotation((navAgent.startPos + (Vector3.up * (navAgent.endPos.y - navAgent.startPos.y))) - navAgent.endPos);
		}
		this._traversingNavMeshLink = true;
		this.NavAgent.ActivateCurrentOffMeshLink(false);
		this.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		float single = Mathf.Max(this.NavAgent.speed, 2.8f);
		Vector3 vector31 = this._currentNavMeshLink.startPos - this._currentNavMeshLink.endPos;
		this._currentNavMeshLinkTraversalTime = vector31.magnitude / single;
		this._currentNavMeshLinkTraversalTimeDelta = 0f;
		if (!(this._currentNavMeshLinkName == "OpenDoorLink") && !(this._currentNavMeshLinkName == "JumpRockLink"))
		{
		}
		return true;
	}

	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		if (this._currentNavMeshLinkName == "OpenDoorLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		if (this._currentNavMeshLinkName == "JumpRockLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		if (this._currentNavMeshLinkName == "JumpFoundationLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
	}

	public bool HasAiFlag(BaseNpc.AiFlags f)
	{
		return (this.aiFlags & f) == f;
	}

	public virtual float HateLevel(BaseEntity ent)
	{
		return 0f;
	}

	public override void Hurt(HitInfo info)
	{
		if (info.Initiator != null && this.AiContext != null)
		{
			this.AiContext.Memory.Update(info.Initiator, 0f);
			if (this.blockTargetingThisEnemy != null && this.blockTargetingThisEnemy.net != null && info.Initiator.net != null && this.blockTargetingThisEnemy.net.ID == info.Initiator.net.ID)
			{
				this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
			}
			if (this.GetFact(BaseNpc.Facts.HasEnemy) != 0)
			{
				this.TryAggro(BaseNpc.EnemyRangeEnum.AggroRange);
			}
			else
			{
				this.WantsToFlee();
			}
		}
		base.Hurt(info);
	}

	public void InitFacts()
	{
		this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
		this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
	}

	private bool IsAfraid()
	{
		if (this.GetFact(BaseNpc.Facts.AfraidRange) == 0)
		{
			if (this.AiContext.EnemyNpc != null && this.IsAfraidOf(this.AiContext.EnemyNpc.Stats.Family))
			{
				this.SetFact(BaseNpc.Facts.IsAfraid, 1, true, true);
				return true;
			}
			if (this.AiContext.EnemyPlayer != null && this.IsAfraidOf(this.AiContext.EnemyPlayer.Family))
			{
				this.SetFact(BaseNpc.Facts.IsAfraid, 1, true, true);
				return true;
			}
		}
		this.SetFact(BaseNpc.Facts.IsAfraid, 0, true, true);
		return false;
	}

	private bool IsAfraidOf(BaseNpc.AiStatistics.FamilyEnum family)
	{
		BaseNpc.AiStatistics.FamilyEnum[] isAfraidOf = this.Stats.IsAfraidOf;
		for (int i = 0; i < (int)isAfraidOf.Length; i++)
		{
			if (family == isAfraidOf[i])
			{
				return true;
			}
		}
		return false;
	}

	private byte IsMoving()
	{
		object obj;
		if (!this.IsNavRunning() || !this.NavAgent.hasPath || this.NavAgent.remainingDistance <= this.NavAgent.stoppingDistance || this.IsStuck || this.GetFact(BaseNpc.Facts.Speed) == 0)
		{
			obj = null;
		}
		else
		{
			obj = 1;
		}
		return (byte)obj;
	}

	private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
	{
		if (this._currentNavMeshLinkTraversalTimeDelta < this._currentNavMeshLinkTraversalTime)
		{
			return false;
		}
		moveToPosition = this._currentNavMeshLink.endPos;
		this._traversingNavMeshLink = false;
		this._currentNavMeshLink = new OffMeshLinkData();
		this._currentNavMeshLinkTraversalTime = 0f;
		this._currentNavMeshLinkTraversalTimeDelta = 0f;
		this._currentNavMeshLinkName = string.Empty;
		this._currentNavMeshLinkOrientation = Quaternion.identity;
		this.CompleteNavMeshLink();
		return true;
	}

	public bool IsNavRunning()
	{
		if (AiManager.nav_disable || !(this.GetNavAgent != null) || !this.GetNavAgent.enabled)
		{
			return false;
		}
		return this.GetNavAgent.isOnNavMesh;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseNPC != null)
		{
			this.aiFlags = (BaseNpc.AiFlags)info.msg.baseNPC.flags;
		}
	}

	public override float MaxVelocity()
	{
		return this.Stats.Speed;
	}

	public void OnBecomeStuck()
	{
		this.IsStuck = true;
	}

	public void OnBecomeUnStuck()
	{
		this.IsStuck = false;
	}

	private void OnFactChanged(BaseNpc.Facts fact, byte oldValue, byte newValue)
	{
		if (fact <= BaseNpc.Facts.IsAggro)
		{
			switch (fact)
			{
				case BaseNpc.Facts.CanTargetEnemies:
				{
					if (newValue != 1)
					{
						return;
					}
					this.blockTargetingThisEnemy = null;
					return;
				}
				case BaseNpc.Facts.Health:
				case BaseNpc.Facts.IsTired:
				{
					return;
				}
				case BaseNpc.Facts.Speed:
				{
					BaseNpc.SpeedEnum speedEnum = (BaseNpc.SpeedEnum)newValue;
					if (speedEnum == BaseNpc.SpeedEnum.StandStill)
					{
						this.StopMoving();
						this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
						return;
					}
					if (speedEnum != BaseNpc.SpeedEnum.Walk)
					{
						this.IsStopped = false;
						return;
					}
					this.IsStopped = false;
					this.CurrentBehaviour = BaseNpc.Behaviour.Wander;
					return;
				}
				case BaseNpc.Facts.IsSleeping:
				{
					if (newValue > 0)
					{
						this.CurrentBehaviour = BaseNpc.Behaviour.Sleep;
						this.SetFact(BaseNpc.Facts.CanTargetEnemies, 0, false, true);
						this.SetFact(BaseNpc.Facts.CanTargetFood, 0, true, true);
						return;
					}
					this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
					this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
					this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
					this.WakeUpBlockMove(this.Stats.WakeupBlockMoveTime);
					this.TickSenses();
					return;
				}
				default:
				{
					if (fact == BaseNpc.Facts.IsAggro)
					{
						break;
					}
					else
					{
						return;
					}
				}
			}
			if (newValue > 0)
			{
				this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
				return;
			}
			this.BlockEnemyTargeting(this.Stats.DeaggroCooldown);
			return;
		}
		else if (fact != BaseNpc.Facts.FoodRange)
		{
			if (fact != BaseNpc.Facts.IsEating)
			{
				return;
			}
			if (newValue == 0)
			{
				this.FoodTarget = null;
				return;
			}
		}
		else if (newValue == 0)
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.Eat;
			return;
		}
	}

	public override void OnKilled(HitInfo hitInfo = null)
	{
		UnityEngine.Assertions.Assert.IsTrue(base.isServer, "OnKilled called on client!");
		BaseCorpse baseCorpse = base.DropCorpse(this.CorpsePrefab.resourcePath);
		if (baseCorpse)
		{
			baseCorpse.Spawn();
			baseCorpse.TakeChildren(this);
		}
		base.Invoke(new Action(this.KillMessage), 0.5f);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("BaseNpc.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void OnSensation(Sensation sensation)
	{
		if (this.AiContext == null)
		{
			return;
		}
		if (sensation.Type <= SensationType.ThrownWeapon)
		{
			this.OnSenseGunshot(sensation);
		}
	}

	protected virtual void OnSenseGunshot(Sensation sensation)
	{
		this.AiContext.Memory.AddDanger(sensation.Position, 1f);
		this._lastHeardGunshotTime = UnityEngine.Time.time;
		Vector3 position = sensation.Position - base.transform.localPosition;
		this.LastHeardGunshotDirection = position.normalized;
		if (this.CurrentBehaviour != BaseNpc.Behaviour.Attack)
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.Flee;
		}
	}

	public void Pause()
	{
		if (this.GetNavAgent != null && this.GetNavAgent.enabled)
		{
			this.GetNavAgent.enabled = false;
		}
		if (this.utilityAiComponent == null)
		{
			this.utilityAiComponent = this.Entity.GetComponent<UtilityAIComponent>();
		}
		if (this.utilityAiComponent != null)
		{
			this.utilityAiComponent.Pause();
			this.utilityAiComponent.enabled = false;
		}
	}

	public List<NavPointSample> RequestNavPointSamplesInCircle(NavPointSampler.SampleCount sampleCount, float radius, NavPointSampler.SampleFeatures features = 0)
	{
		this.navPointSamples.Clear();
		Vector3 serverPosition = this.ServerPosition;
		NavPointSampler.SampleScoreParams sampleScoreParam = new NavPointSampler.SampleScoreParams()
		{
			WaterMaxDepth = this.Stats.MaxWaterDepth,
			Agent = this,
			Features = features
		};
		NavPointSampler.SampleCircle(sampleCount, serverPosition, radius, sampleScoreParam, ref this.navPointSamples);
		return this.navPointSamples;
	}

	public List<NavPointSample> RequestNavPointSamplesInCircleWaterDepthOnly(NavPointSampler.SampleCount sampleCount, float radius, float waterDepth)
	{
		this.navPointSamples.Clear();
		Vector3 serverPosition = this.ServerPosition;
		NavPointSampler.SampleScoreParams sampleScoreParam = new NavPointSampler.SampleScoreParams()
		{
			WaterMaxDepth = waterDepth,
			Agent = this
		};
		NavPointSampler.SampleCircleWaterDepthOnly(sampleCount, serverPosition, radius, sampleScoreParam, ref this.navPointSamples);
		return this.navPointSamples;
	}

	public void Resume()
	{
		if (!this.GetNavAgent.isOnNavMesh)
		{
			base.StartCoroutine(this.TryForceToNavmesh());
			return;
		}
		this.GetNavAgent.enabled = true;
		if (this.utilityAiComponent == null)
		{
			this.utilityAiComponent = this.Entity.GetComponent<UtilityAIComponent>();
		}
		if (this.utilityAiComponent != null)
		{
			this.utilityAiComponent.enabled = true;
			this.utilityAiComponent.Resume();
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseNPC = Facepunch.Pool.Get<BaseNPC>();
		info.msg.baseNPC.flags = (int)this.aiFlags;
	}

	private void SelectClosestFood()
	{
		BaseNpc.FoodRangeEnum foodRangeEnum;
		float single = Single.MaxValue;
		Vector3 vector3 = Vector3.zero;
		bool flag = false;
		foreach (BaseEntity visible in this.AiContext.Memory.Visible)
		{
			if (visible.IsDestroyed || !this.WantsToEat(visible))
			{
				continue;
			}
			Vector3 serverPosition = visible.ServerPosition - this.ServerPosition;
			float single1 = serverPosition.sqrMagnitude;
			if (single1 >= single)
			{
				continue;
			}
			single = single1;
			this.FoodTarget = visible;
			vector3 = serverPosition;
			flag = true;
			if (single > 0.1f)
			{
				continue;
			}
			if (!flag)
			{
				this.FoodTarget = null;
				this.SetFact(BaseNpc.Facts.FoodRange, 2, true, true);
				return;
			}
			foodRangeEnum = this.ToFoodRangeEnum(vector3.magnitude);
			this.SetFact(BaseNpc.Facts.FoodRange, (byte)foodRangeEnum, true, true);
			return;
		}
		if (!flag)
		{
			this.FoodTarget = null;
			this.SetFact(BaseNpc.Facts.FoodRange, 2, true, true);
			return;
		}
		foodRangeEnum = this.ToFoodRangeEnum(vector3.magnitude);
		this.SetFact(BaseNpc.Facts.FoodRange, (byte)foodRangeEnum, true, true);
	}

	private void SelectEnemy()
	{
		if (this.AiContext.Players.Count != 0 || this.AiContext.Npcs.Count != 0 || this.AiContext.PlayersBehindUs.Count != 0 || this.AiContext.NpcsBehindUs.Count != 0)
		{
			this.AggroClosestEnemy();
			return;
		}
		this.AiContext.EnemyNpc = null;
		this.AiContext.EnemyPlayer = null;
		this.SetFact(BaseNpc.Facts.HasEnemy, 0, true, true);
		this.SetFact(BaseNpc.Facts.EnemyRange, 3, true, true);
		this.SetFact(BaseNpc.Facts.IsAggro, 0, false, true);
	}

	private void SelectFood()
	{
		if (this.AiContext.Memory.Visible.Count != 0)
		{
			this.SelectClosestFood();
			return;
		}
		this.FoodTarget = null;
		this.SetFact(BaseNpc.Facts.FoodRange, 2, true, true);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.NavAgent == null)
		{
			this.NavAgent = base.GetComponent<NavMeshAgent>();
		}
		if (this.NavAgent != null)
		{
			this.NavAgent.updateRotation = false;
			this.NavAgent.updatePosition = false;
		}
		this.IsStuck = false;
		this.AgencyUpdateRequired = false;
		this.IsOnOffmeshLinkAndReachedNewCoord = false;
		base.InvokeRandomized(new Action(this.TickAi), 0.1f, 0.1f, 0.00500000035f);
		this.Sleep = UnityEngine.Random.Range(0.5f, 1f);
		this.Stamina.Level = UnityEngine.Random.Range(0.1f, 1f);
		this.Energy.Level = UnityEngine.Random.Range(0.5f, 1f);
		this.Hydration.Level = UnityEngine.Random.Range(0.5f, 1f);
		if (this.NewAI)
		{
			this.InitFacts();
			this.fleeHealthThresholdPercentage = this.Stats.HealthThresholdForFleeing;
			AnimalSensesLoadBalancer.animalSensesLoadBalancer.Add(this);
		}
	}

	public void SetAiFlag(BaseNpc.AiFlags f, bool set)
	{
		BaseNpc.AiFlags aiFlag = this.aiFlags;
		if (!set)
		{
			this.aiFlags &= ~f;
		}
		else
		{
			this.aiFlags |= f;
		}
		if (aiFlag != this.aiFlags && base.isServer)
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	public void SetBusyFor(float dur)
	{
		this.BusyTimer.Activate(dur, null);
	}

	public void SetFact(BaseNpc.Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true)
	{
		byte currentFacts = this.CurrentFacts[(int)fact];
		this.CurrentFacts[(int)fact] = value;
		if (triggerCallback && value != currentFacts)
		{
			this.OnFactChanged(fact, currentFacts, value);
		}
	}

	public void SetFact(NPCPlayerApex.Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true)
	{
	}

	public void SetTargetPathStatus(float pendingDelay = 0.05f)
	{
		if (this.isAlreadyCheckingPathPending)
		{
			return;
		}
		if (this.NavAgent.pathPending && this.numPathPendingAttempts < 10)
		{
			this.isAlreadyCheckingPathPending = true;
			base.Invoke(new Action(this.DelayedTargetPathStatus), pendingDelay);
			return;
		}
		this.numPathPendingAttempts = 0;
		this.accumPathPendingDelay = 0f;
		this.SetFact(BaseNpc.Facts.PathToTargetStatus, this.GetPathStatus(), true, true);
	}

	protected virtual void SetupAiContext()
	{
		this.AiContext = new BaseContext(this);
	}

	public bool StartAggro(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.IsAggro, 1, true, true);
		this.aggroTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	public virtual void StartAttack()
	{
		if (!this.AttackTarget)
		{
			return;
		}
		if (!this.AttackReady())
		{
			return;
		}
		if (Interface.CallHook("CanNpcAttack", this, this.AttackTarget) != null)
		{
			return;
		}
		if ((this.AttackTarget.ServerPosition - this.ServerPosition).magnitude > this.AttackRange)
		{
			return;
		}
		this.nextAttackTime = UnityEngine.Time.realtimeSinceStartup + this.AttackRate;
		BaseCombatEntity combatTarget = this.CombatTarget;
		if (!combatTarget)
		{
			return;
		}
		combatTarget.Hurt(this.AttackDamage, this.AttackDamageType, this, true);
		this.Stamina.Use(this.AttackCost);
		this.BusyTimer.Activate(0.5f, null);
		base.SignalBroadcast(BaseEntity.Signal.Attack, null);
		base.ClientRPC<Vector3>(null, "Attack", this.AttackTarget.ServerPosition);
	}

	public void StartAttack(AttackOperator.AttackType type, BaseCombatEntity target)
	{
		if (target == null)
		{
			return;
		}
		if (this.GetFact(BaseNpc.Facts.IsAttackReady) == 0)
		{
			return;
		}
		Vector3 serverPosition = target.ServerPosition - this.ServerPosition;
		float single = serverPosition.magnitude;
		if (single > this.AttackRange)
		{
			return;
		}
		if (single > 0.001f)
		{
			this.ServerRotation = Quaternion.LookRotation(serverPosition.normalized);
		}
		this.nextAttackTime = UnityEngine.Time.realtimeSinceStartup + this.AttackRate;
		target.Hurt(this.AttackDamage, this.AttackDamageType, this, true);
		this.Stamina.Use(this.AttackCost);
		base.SignalBroadcast(BaseEntity.Signal.Attack, null);
		base.ClientRPC<Vector3>(null, "Attack", target.ServerPosition);
	}

	public bool StartEating(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.IsEating) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.IsEating, 1, true, true);
		this.eatTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	public virtual void Startled()
	{
		base.ClientRPC<Vector3>(null, "Startled", base.transform.position);
	}

	public void StopMoving()
	{
		this.IsStopped = true;
		this.ChaseTransform = null;
		this.SetFact(BaseNpc.Facts.PathToTargetStatus, 0, true, true);
	}

	private void TickAggro()
	{
		bool flag = false;
		bool flag1 = true;
		if (!float.IsInfinity(base.SecondsSinceDealtDamage))
		{
			BaseCombatEntity attackTarget = this.AttackTarget as BaseCombatEntity;
			if (!(attackTarget != null) || !(attackTarget.lastAttacker != null) || this.net == null || attackTarget.lastAttacker.net == null)
			{
				flag = UnityEngine.Time.realtimeSinceStartup > this.aggroTimeout;
			}
			else
			{
				flag = (attackTarget.lastAttacker.net.ID != this.net.ID ? false : base.SecondsSinceDealtDamage > this.Stats.DeaggroChaseTime);
			}
		}
		else
		{
			flag = UnityEngine.Time.realtimeSinceStartup > this.aggroTimeout;
		}
		if (!flag)
		{
			if (this.AiContext.EnemyNpc != null && (this.AiContext.EnemyNpc.IsDead() || this.AiContext.EnemyNpc.IsDestroyed))
			{
				flag = true;
				flag1 = false;
			}
			else if (this.AiContext.EnemyPlayer != null && (this.AiContext.EnemyPlayer.IsDead() || this.AiContext.EnemyPlayer.IsDestroyed))
			{
				flag = true;
				flag1 = false;
			}
		}
		if (flag)
		{
			this.SetFact(BaseNpc.Facts.IsAggro, 0, flag1, true);
		}
	}

	public void TickAi()
	{
		if (!AI.think)
		{
			return;
		}
		if (TerrainMeta.WaterMap == null)
		{
			this.wasSwimming = false;
			this.swimming = false;
			this.waterDepth = 0f;
		}
		else
		{
			this.waterDepth = TerrainMeta.WaterMap.GetDepth(this.ServerPosition);
			this.wasSwimming = this.swimming;
			this.swimming = this.waterDepth > this.Stats.WaterLevelNeck * 0.25f;
		}
		using (TimeWarning timeWarning = TimeWarning.New("TickNavigation", 0.1f))
		{
			this.TickNavigation();
		}
		if (!AiManager.ai_dormant || this.GetNavAgent.enabled)
		{
			using (timeWarning = TimeWarning.New("TickMetabolism", 0.1f))
			{
				this.TickSleep();
				this.TickMetabolism();
				this.TickSpeed();
			}
		}
	}

	private void TickBehaviourState()
	{
		if (this.GetFact(BaseNpc.Facts.WantsToFlee) == 1 && this.IsNavRunning() && this.NavAgent.pathStatus == NavMeshPathStatus.PathComplete && UnityEngine.Time.realtimeSinceStartup - (this.maxFleeTime - this.Stats.MaxFleeTime) > 0.5f)
		{
			this.TickFlee();
		}
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 0)
		{
			this.TickBlockEnemyTargeting();
		}
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			this.TickBlockFoodTargeting();
		}
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			this.TickAggro();
		}
		if (this.GetFact(BaseNpc.Facts.IsEating) == 1)
		{
			this.TickEating();
		}
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 1)
		{
			this.TickWakeUpBlockMove();
		}
	}

	private void TickBlockEnemyTargeting()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 1)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.blockEnemyTargetingTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
		}
	}

	private void TickBlockFoodTargeting()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 1)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.blockFoodTargetingTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
		}
	}

	private void TickChase()
	{
		Vector3 chaseTransform = this.ChaseTransform.position;
		Vector3 vector3 = base.transform.position - chaseTransform;
		if ((double)vector3.magnitude < 5)
		{
			chaseTransform = chaseTransform + (vector3.normalized * this.AttackOffset.z);
		}
		if ((this.NavAgent.destination - chaseTransform).sqrMagnitude > 0.0100000007f)
		{
			this.NavAgent.SetDestination(chaseTransform);
		}
	}

	private void TickEating()
	{
		if (this.GetFact(BaseNpc.Facts.IsEating) == 0)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.eatTimeout)
		{
			this.SetFact(BaseNpc.Facts.IsEating, 0, true, true);
		}
	}

	private void TickEnemyAwareness()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) != 0 || !(this.blockTargetingThisEnemy == null))
		{
			this.SelectEnemy();
			return;
		}
		this.AiContext.EnemyNpc = null;
		this.AiContext.EnemyPlayer = null;
		this.SetFact(BaseNpc.Facts.HasEnemy, 0, true, true);
		this.SetFact(BaseNpc.Facts.EnemyRange, 3, true, true);
		this.SetFact(BaseNpc.Facts.IsAggro, 0, false, true);
	}

	private void TickFlee()
	{
		bool flag = UnityEngine.Time.realtimeSinceStartup > this.maxFleeTime;
		if (flag || this.IsNavRunning() && this.NavAgent.remainingDistance <= this.NavAgent.stoppingDistance + 1f)
		{
			if (!flag && this.IsAfraid())
			{
				NavigateToOperator.FleeEnemy(this.AiContext);
				return;
			}
			this.SetFact(BaseNpc.Facts.WantsToFlee, 0, true, true);
			this.SetFact(BaseNpc.Facts.IsFleeing, 0, true, true);
			this.Stats.HealthThresholdForFleeing = base.healthFraction * this.fleeHealthThresholdPercentage;
		}
	}

	private void TickFollowPath(ref Vector3 moveToPosition)
	{
		moveToPosition = this.NavAgent.nextPosition;
		this.stepDirection = this.NavAgent.desiredVelocity.normalized;
	}

	private void TickFoodAwareness()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) != 0)
		{
			this.SelectFood();
			return;
		}
		this.FoodTarget = null;
		this.SetFact(BaseNpc.Facts.FoodRange, 2, true, true);
	}

	private void TickHearing()
	{
		this.SetFact(BaseNpc.Facts.LoudNoiseNearby, 0, true, true);
	}

	private void TickIdle()
	{
		if (this.CurrentBehaviour != BaseNpc.Behaviour.Idle)
		{
			this.idleDuration = 0f;
			return;
		}
		this.idleDuration += 0.1f;
	}

	protected virtual void TickMetabolism()
	{
		float single = 0.000166666665f;
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			single *= 0.01f;
		}
		if (this.NavAgent.desiredVelocity.sqrMagnitude > 0.1f)
		{
			single *= 2f;
		}
		this.Energy.Add(single * 0.1f * -1f);
		if (this.Stamina.TimeSinceUsed > 5f)
		{
			this.Stamina.Add(0.1f * 0.06666667f);
		}
		float secondsSinceAttacked = base.SecondsSinceAttacked;
	}

	public void TickNavigation()
	{
		if (!AI.move)
		{
			return;
		}
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			this.StopMoving();
			return;
		}
		Vector3 vector3 = base.transform.position;
		this.stepDirection = Vector3.zero;
		if (this.ChaseTransform)
		{
			this.TickChase();
		}
		if (this.NavAgent.isOnOffMeshLink)
		{
			this.HandleNavMeshLinkTraversal(0.1f, ref vector3);
		}
		else if (this.NavAgent.hasPath)
		{
			this.TickFollowPath(ref vector3);
		}
		if (!this.ValidateNextPosition(ref vector3))
		{
			return;
		}
		this.UpdatePositionAndRotation(vector3);
		this.TickIdle();
		this.TickStuck();
	}

	public void TickNavigationWater()
	{
		if (!AI.move)
		{
			return;
		}
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			this.StopMoving();
			return;
		}
		Vector3 waterLevelNeck = base.transform.position;
		this.stepDirection = Vector3.zero;
		if (this.ChaseTransform)
		{
			this.TickChase();
		}
		if (this.NavAgent.isOnOffMeshLink)
		{
			this.HandleNavMeshLinkTraversal(0.1f, ref waterLevelNeck);
		}
		else if (this.NavAgent.hasPath)
		{
			this.TickFollowPath(ref waterLevelNeck);
		}
		if (!this.ValidateNextPosition(ref waterLevelNeck))
		{
			return;
		}
		waterLevelNeck.y = 0f - this.Stats.WaterLevelNeck;
		this.UpdatePositionAndRotation(waterLevelNeck);
		this.TickIdle();
		this.TickStuck();
	}

	private void TickSenses()
	{
		if (BaseEntity.Query.Server == null || this.AiContext == null || this.IsDormant)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.lastTickTime + this.SensesTickRate)
		{
			this.TickVision();
			this.TickHearing();
			this.TickSmell();
			this.AiContext.Memory.Forget((float)this.ForgetUnseenEntityTime);
			this.lastTickTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.TickEnemyAwareness();
		if (!AI.animal_ignore_food)
		{
			this.TickFoodAwareness();
		}
		this.UpdateSelfFacts();
	}

	protected virtual void TickSleep()
	{
		if (this.CurrentBehaviour != BaseNpc.Behaviour.Sleep)
		{
			this.IsSleeping = false;
			this.Sleep -= 2.77777781E-05f;
		}
		else
		{
			this.IsSleeping = true;
			this.Sleep += 0.000333333359f;
		}
		this.Sleep = Mathf.Clamp01(this.Sleep);
	}

	private void TickSmell()
	{
	}

	private void TickSpeed()
	{
		float speed = this.Stats.Speed;
		if (this.NewAI)
		{
			speed = (this.swimming ? this.ToSpeed(BaseNpc.SpeedEnum.Walk) : this.TargetSpeed);
			speed = speed * (0.5f + base.healthFraction * 0.5f);
			this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, speed, 0.5f);
			this.NavAgent.angularSpeed = this.Stats.TurnSpeed;
			this.NavAgent.acceleration = this.Stats.Acceleration;
			return;
		}
		speed = speed * (0.5f + base.healthFraction * 0.5f);
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Idle)
		{
			speed *= 0.2f;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Eat)
		{
			speed *= 0.3f;
		}
		float single = Mathf.Min(this.NavAgent.speed / this.Stats.Speed, 1f);
		single = BaseNpc.speedFractionResponse.Evaluate(single);
		Vector3 vector3 = base.transform.forward;
		Vector3 navAgent = this.NavAgent.nextPosition - this.ServerPosition;
		float single1 = 1f - 0.9f * Vector3.Angle(vector3, navAgent.normalized) / 180f * single * single;
		speed *= single1;
		this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, speed, 0.5f);
		this.NavAgent.angularSpeed = this.Stats.TurnSpeed * (1.1f - single);
		this.NavAgent.acceleration = this.Stats.Acceleration;
	}

	public void TickStuck()
	{
		if (!this.IsNavRunning() || this.NavAgent.isStopped || (this.lastStuckPos - this.ServerPosition).sqrMagnitude >= 0.0625f || !this.AttackReady())
		{
			this.stuckDuration = 0f;
			this.lastStuckPos = this.ServerPosition;
			if (UnityEngine.Time.time - this.lastStuckTime > 5f)
			{
				this.lastStuckTime = 0f;
				this.OnBecomeUnStuck();
			}
		}
		else
		{
			this.stuckDuration += 0.1f;
			if (this.stuckDuration >= 5f && Mathf.Approximately(this.lastStuckTime, 0f))
			{
				this.lastStuckTime = UnityEngine.Time.time;
				this.OnBecomeStuck();
				return;
			}
		}
	}

	private void TickVision()
	{
		this.AiContext.Players.Clear();
		this.AiContext.Npcs.Clear();
		this.AiContext.PlayersBehindUs.Clear();
		this.AiContext.NpcsBehindUs.Clear();
		if (BaseEntity.Query.Server == null)
		{
			return;
		}
		if (this.GetFact(BaseNpc.Facts.IsSleeping) == 1)
		{
			return;
		}
		int inSphere = BaseEntity.Query.Server.GetInSphere(base.transform.position, this.Stats.VisionRange, this.SensesResults, new Func<BaseEntity, bool>(BaseNpc.AiCaresAbout));
		if (inSphere == 0)
		{
			return;
		}
		for (int i = 0; i < inSphere; i++)
		{
			BaseEntity sensesResults = this.SensesResults[i];
			if (!(sensesResults == null) && !(sensesResults == this) && sensesResults.isServer && !(sensesResults.transform == null) && !sensesResults.IsDestroyed)
			{
				if (BaseNpc.WithinVisionCone(this, sensesResults))
				{
					BasePlayer basePlayer = sensesResults as BasePlayer;
					if (basePlayer == null)
					{
						BaseNpc baseNpc = sensesResults as BaseNpc;
						if (baseNpc != null)
						{
							this.AiContext.Npcs.Add(baseNpc);
						}
					}
					else if (!AI.ignoreplayers)
					{
						Vector3 attackPosition = this.AiContext.AIAgent.AttackPosition;
						if ((basePlayer.IsVisible(attackPosition, basePlayer.CenterPoint(), Single.PositiveInfinity) || basePlayer.IsVisible(attackPosition, basePlayer.eyes.position, Single.PositiveInfinity) ? false : !basePlayer.IsVisible(attackPosition, basePlayer.transform.position, Single.PositiveInfinity)))
						{
							goto Label0;
						}
						this.AiContext.Players.Add(sensesResults as BasePlayer);
					}
					else
					{
						goto Label0;
					}
					this.AiContext.Memory.Update(sensesResults, 0f);
				}
				else
				{
					BasePlayer basePlayer1 = sensesResults as BasePlayer;
					if (basePlayer1 == null)
					{
						BaseNpc baseNpc1 = sensesResults as BaseNpc;
						if (baseNpc1 != null && (baseNpc1.ServerPosition - this.ServerPosition).sqrMagnitude <= (this.AttackRange + 2f) * (this.AttackRange + 2f))
						{
							this.AiContext.NpcsBehindUs.Add(baseNpc1);
						}
					}
					else if (!AI.ignoreplayers && (basePlayer1.ServerPosition - this.ServerPosition).sqrMagnitude <= (this.AttackRange + 2f) * (this.AttackRange + 2f))
					{
						this.AiContext.PlayersBehindUs.Add(basePlayer1);
					}
				}
			}
		Label0:
		}
	}

	private void TickWakeUpBlockMove()
	{
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 0)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.wakeUpBlockMoveTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanNotMove, 0, true, true);
		}
	}

	public BaseNpc.AfraidRangeEnum ToAfraidRangeEnum(float range)
	{
		if (range <= this.Stats.AfraidRange)
		{
			return BaseNpc.AfraidRangeEnum.InAfraidRange;
		}
		return BaseNpc.AfraidRangeEnum.OutOfRange;
	}

	public BaseNpc.EnemyRangeEnum ToEnemyRangeEnum(float range)
	{
		if (range <= this.AttackRange)
		{
			return BaseNpc.EnemyRangeEnum.AttackRange;
		}
		if (range <= this.Stats.AggressionRange)
		{
			return BaseNpc.EnemyRangeEnum.AggroRange;
		}
		if (range >= this.Stats.DeaggroRange && this.GetFact(BaseNpc.Facts.IsAggro) > 0)
		{
			return BaseNpc.EnemyRangeEnum.OutOfRange;
		}
		if (range <= this.Stats.VisionRange)
		{
			return BaseNpc.EnemyRangeEnum.AwareRange;
		}
		return BaseNpc.EnemyRangeEnum.OutOfRange;
	}

	public BaseNpc.FoodRangeEnum ToFoodRangeEnum(float range)
	{
		if (range <= 0.5f)
		{
			return BaseNpc.FoodRangeEnum.EatRange;
		}
		if (range <= this.Stats.VisionRange)
		{
			return BaseNpc.FoodRangeEnum.AwareRange;
		}
		return BaseNpc.FoodRangeEnum.OutOfRange;
	}

	public BaseNpc.HealthEnum ToHealthEnum(float healthNormalized)
	{
		if (healthNormalized >= 0.75f)
		{
			return BaseNpc.HealthEnum.Fine;
		}
		if (healthNormalized >= 0.25f)
		{
			return BaseNpc.HealthEnum.Medium;
		}
		return BaseNpc.HealthEnum.Low;
	}

	public byte ToIsTired(float energyNormalized)
	{
		bool fact = this.GetFact(BaseNpc.Facts.IsSleeping) == 1;
		if (!fact && energyNormalized < 0.1f)
		{
			return (byte)1;
		}
		if (fact && energyNormalized < 0.5f)
		{
			return (byte)1;
		}
		return (byte)0;
	}

	public NavMeshPathStatus ToPathStatus(byte value)
	{
		return (NavMeshPathStatus)value;
	}

	public int TopologyPreference()
	{
		return (int)this.topologyPreference;
	}

	public float ToSpeed(NPCPlayerApex.SpeedEnum speed)
	{
		return 0f;
	}

	public float ToSpeed(BaseNpc.SpeedEnum speed)
	{
		if (speed == BaseNpc.SpeedEnum.StandStill)
		{
			return 0f;
		}
		if (speed != BaseNpc.SpeedEnum.Walk)
		{
			return this.Stats.Speed;
		}
		return 0.18f * this.Stats.Speed;
	}

	public BaseNpc.SpeedEnum ToSpeedEnum(float speed)
	{
		if (speed <= 0.01f)
		{
			return BaseNpc.SpeedEnum.StandStill;
		}
		if (speed <= 0.18f)
		{
			return BaseNpc.SpeedEnum.Walk;
		}
		return BaseNpc.SpeedEnum.Run;
	}

	public bool TryAggro(BaseNpc.EnemyRangeEnum range)
	{
		if (Mathf.Approximately(this.Stats.Hostility, 0f) && Mathf.Approximately(this.Stats.Defensiveness, 0f))
		{
			return false;
		}
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 0 && (range == BaseNpc.EnemyRangeEnum.AggroRange || range == BaseNpc.EnemyRangeEnum.AttackRange))
		{
			float single = (range == BaseNpc.EnemyRangeEnum.AttackRange ? 1f : this.Stats.Defensiveness);
			single = Mathf.Max(single, this.Stats.Hostility);
			if (UnityEngine.Time.realtimeSinceStartup > this.lastAggroChanceCalcTime + 5f)
			{
				this.lastAggroChanceResult = UnityEngine.Random.@value;
				this.lastAggroChanceCalcTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (this.lastAggroChanceResult < single)
			{
				return this.StartAggro(this.Stats.DeaggroChaseTime);
			}
		}
		return false;
	}

	private IEnumerator TryForceToNavmesh()
	{
		BaseNpc component = null;
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
		while (num < 4)
		{
			if (component.GetNavAgent.isOnNavMesh)
			{
				component.GetNavAgent.enabled = true;
				if (component.utilityAiComponent == null)
				{
					component.utilityAiComponent = component.Entity.GetComponent<UtilityAIComponent>();
				}
				if (component.utilityAiComponent != null)
				{
					component.utilityAiComponent.enabled = true;
					component.utilityAiComponent.Resume();
				}
				yield break;
			}
			if (NavMesh.SamplePosition(component.ServerPosition, out navMeshHit, component.GetNavAgent.height * single1, component.GetNavAgent.areaMask))
			{
				component.ServerPosition = navMeshHit.position;
				component.GetNavAgent.Warp(component.ServerPosition);
				component.GetNavAgent.enabled = true;
				if (component.utilityAiComponent == null)
				{
					component.utilityAiComponent = component.Entity.GetComponent<UtilityAIComponent>();
				}
				if (component.utilityAiComponent != null)
				{
					component.utilityAiComponent.enabled = true;
					component.utilityAiComponent.Resume();
				}
				yield break;
			}
			yield return CoroutineEx.waitForSecondsRealtime(single);
			single1 *= 1.5f;
			single *= 1.5f;
			num++;
		}
		UnityEngine.Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[] { component.name });
		component.DieInstantly();
	}

	public void UpdateAiRotation()
	{
		Vector3 chaseTransform;
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			return;
		}
		if (this._traversingNavMeshLink)
		{
			if (this.ChaseTransform == null)
			{
				chaseTransform = (this.AttackTarget == null ? this.NavAgent.destination - this.ServerPosition : this.AttackTarget.ServerPosition - this.ServerPosition);
			}
			else
			{
				chaseTransform = this.ChaseTransform.localPosition - this.ServerPosition;
			}
			if (chaseTransform.sqrMagnitude > 1f)
			{
				chaseTransform = this._currentNavMeshLinkEndPos - this.ServerPosition;
			}
			if (chaseTransform.sqrMagnitude > 0.001f)
			{
				this.ServerRotation = this._currentNavMeshLinkOrientation;
				return;
			}
		}
		else if ((this.NavAgent.destination - this.ServerPosition).sqrMagnitude > 1f)
		{
			Vector3 vector3 = this.stepDirection;
			if (vector3.sqrMagnitude > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(vector3);
				return;
			}
		}
		if (this.ChaseTransform && this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
		{
			Vector3 chaseTransform1 = this.ChaseTransform.localPosition - this.ServerPosition;
			float single = chaseTransform1.sqrMagnitude;
			if (single < 9f && single > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(chaseTransform1.normalized);
				return;
			}
		}
		else if (this.AttackTarget && this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
		{
			Vector3 serverPosition = this.AttackTarget.ServerPosition - this.ServerPosition;
			float single1 = serverPosition.sqrMagnitude;
			if (single1 < 9f && single1 > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(serverPosition.normalized);
				return;
			}
		}
	}

	public void UpdateDestination(Vector3 position)
	{
		if (this.IsStopped)
		{
			this.IsStopped = false;
		}
		if ((this.Destination - position).sqrMagnitude > 0.0100000007f)
		{
			this.Destination = position;
		}
		this.ChaseTransform = null;
	}

	public void UpdateDestination(Transform tx)
	{
		this.IsStopped = false;
		this.ChaseTransform = tx;
	}

	private void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		this.ServerPosition = moveToPosition;
		this.UpdateAiRotation();
	}

	private void UpdateSelfFacts()
	{
		object obj;
		object obj1;
		object obj2;
		object obj3;
		this.SetFact(BaseNpc.Facts.Health, (byte)this.ToHealthEnum(base.healthFraction), true, true);
		this.SetFact(BaseNpc.Facts.IsTired, this.ToIsTired(this.Sleep), true, true);
		if (UnityEngine.Time.realtimeSinceStartup >= this.nextAttackTime)
		{
			obj = 1;
		}
		else
		{
			obj = null;
		}
		this.SetFact(BaseNpc.Facts.IsAttackReady, (byte)obj, true, true);
		if (UnityEngine.Time.realtimeSinceStartup < this.AiContext.NextRoamTime || !this.IsNavRunning())
		{
			obj1 = null;
		}
		else
		{
			obj1 = 1;
		}
		this.SetFact(BaseNpc.Facts.IsRoamReady, (byte)obj1, true, true);
		this.SetFact(BaseNpc.Facts.Speed, (byte)this.ToSpeedEnum(this.TargetSpeed / this.Stats.Speed), true, true);
		if (this.Energy.Level < 0.25f)
		{
			obj2 = 1;
		}
		else
		{
			obj2 = null;
		}
		this.SetFact(BaseNpc.Facts.IsHungry, (byte)obj2, true, true);
		if (float.IsNegativeInfinity(base.SecondsSinceAttacked) || base.SecondsSinceAttacked >= this.Stats.AttackedMemoryTime)
		{
			obj3 = null;
		}
		else
		{
			obj3 = 1;
		}
		this.SetFact(BaseNpc.Facts.AttackedLately, (byte)obj3, true, true);
		this.SetFact(BaseNpc.Facts.IsMoving, this.IsMoving(), true, true);
		if (this.CheckHealthThresholdToFlee() || this.IsAfraid())
		{
			this.WantsToFlee();
		}
	}

	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		if (ValidBounds.Test(moveToPosition) || !(base.transform != null) || base.IsDestroyed)
		{
			return true;
		}
		UnityEngine.Debug.Log(string.Concat(new object[] { "Invalid NavAgent Position: ", this, " ", moveToPosition, " (destroying)" }));
		base.Kill(BaseNetworkable.DestroyMode.None);
		return false;
	}

	public bool WakeUpBlockMove(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanNotMove, 1, true, true);
		this.wakeUpBlockMoveTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	internal float WantsToAttack(BaseEntity target)
	{
		if (target == null)
		{
			return 0f;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			return 0f;
		}
		if (!target.HasAnyTrait(BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Human))
		{
			return 0f;
		}
		if (target.GetType() != base.GetType())
		{
			return 1f;
		}
		return 1f - this.Stats.Tolerance;
	}

	public virtual bool WantsToEat(BaseEntity best)
	{
		object obj = Interface.CallHook("CanNpcEat", this, best);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (!best.HasTrait(BaseEntity.TraitFlag.Food))
		{
			return false;
		}
		if (best.HasTrait(BaseEntity.TraitFlag.Alive))
		{
			return false;
		}
		return true;
	}

	private void WantsToFlee()
	{
		if (this.GetFact(BaseNpc.Facts.WantsToFlee) == 1 || !this.IsNavRunning())
		{
			return;
		}
		this.SetFact(BaseNpc.Facts.WantsToFlee, 1, true, true);
		this.maxFleeTime = UnityEngine.Time.realtimeSinceStartup + this.Stats.MaxFleeTime;
	}

	private static bool WithinVisionCone(BaseNpc npc, BaseEntity other)
	{
		if (Mathf.Approximately(npc.Stats.VisionCone, -1f))
		{
			return true;
		}
		Vector3 serverPosition = (other.ServerPosition - npc.ServerPosition).normalized;
		if (Vector3.Dot(npc.transform.forward, serverPosition) < npc.Stats.VisionCone)
		{
			return false;
		}
		return true;
	}

	public enum AfraidRangeEnum : byte
	{
		InAfraidRange,
		OutOfRange
	}

	[Flags]
	public enum AiFlags
	{
		Sitting = 2,
		Chasing = 4,
		Sleeping = 8
	}

	[Serializable]
	public struct AiStatistics
	{
		[Range(0f, 1f)]
		[Tooltip("Ai will be less likely to fight animals that are larger than them, and more likely to flee from them.")]
		public float Size;

		[Tooltip("How fast we can move")]
		public float Speed;

		[Tooltip("How fast can we accelerate")]
		public float Acceleration;

		[Tooltip("How fast can we turn around")]
		public float TurnSpeed;

		[Range(0f, 1f)]
		[Tooltip("Determines things like how near we'll allow other species to get")]
		public float Tolerance;

		[Tooltip("How far this NPC can see")]
		public float VisionRange;

		[Tooltip("Our vision cone for dot product - a value of -1 means we can see all around us, 0 = only infront ")]
		public float VisionCone;

		[Tooltip("NPCs use distance visibility to basically make closer enemies easier to detect than enemies further away")]
		public AnimationCurve DistanceVisibility;

		[Tooltip("How likely are we to be offensive without being threatened")]
		public float Hostility;

		[Tooltip("How likely are we to defend ourselves when attacked")]
		public float Defensiveness;

		[Tooltip("The range at which we will engage targets")]
		public float AggressionRange;

		[Tooltip("The range at which an aggrified npc will disengage it's current target")]
		public float DeaggroRange;

		[Tooltip("For how long will we chase a target until we give up")]
		public float DeaggroChaseTime;

		[Tooltip("When we deaggro, how long do we wait until we can aggro again.")]
		public float DeaggroCooldown;

		[Tooltip("The threshold of our health fraction where there's a chance that we want to flee")]
		public float HealthThresholdForFleeing;

		[Tooltip("The chance that we will flee when our health threshold is triggered")]
		public float HealthThresholdFleeChance;

		[Tooltip("When we flee, what is the minimum distance we should flee?")]
		public float MinFleeRange;

		[Tooltip("When we flee, what is the maximum distance we should flee?")]
		public float MaxFleeRange;

		[Tooltip("When we flee, what is the maximum time that can pass until we stop?")]
		public float MaxFleeTime;

		[Tooltip("At what range we are afraid of a target that is in our Is Afraid Of list.")]
		public float AfraidRange;

		[Tooltip("The family this npc belong to. Npcs in the same family will not attack each other.")]
		public BaseNpc.AiStatistics.FamilyEnum Family;

		[Tooltip("List of the types of Npc that we are afraid of.")]
		public BaseNpc.AiStatistics.FamilyEnum[] IsAfraidOf;

		[Tooltip("The minimum distance this npc will wander when idle.")]
		public float MinRoamRange;

		[Tooltip("The maximum distance this npc will wander when idle.")]
		public float MaxRoamRange;

		[Tooltip("The minimum amount of time between each time we seek a new roam destination (when idle)")]
		public float MinRoamDelay;

		[Tooltip("The maximum amount of time between each time we seek a new roam destination (when idle)")]
		public float MaxRoamDelay;

		[Tooltip("If an npc is mobile, they are allowed to move when idle.")]
		public bool IsMobile;

		[Tooltip("In the range between min and max roam delay, we evaluate the random value through this curve")]
		public AnimationCurve RoamDelayDistribution;

		[Tooltip("For how long do we remember that someone attacked us")]
		public float AttackedMemoryTime;

		[Tooltip("How long should we block movement to make the wakeup animation not look whack?")]
		public float WakeupBlockMoveTime;

		[Tooltip("The maximum water depth this npc willingly will walk into.")]
		public float MaxWaterDepth;

		[Tooltip("The water depth at which they will start swimming.")]
		public float WaterLevelNeck;

		[Tooltip("The range we consider using close range weapons.")]
		public float CloseRange;

		[Tooltip("The range we consider using medium range weapons.")]
		public float MediumRange;

		[Tooltip("The range we consider using long range weapons.")]
		public float LongRange;

		[Tooltip("How long can we be out of range of our spawn point before we time out and make our way back home (when idle).")]
		public float OutOfRangeOfSpawnPointTimeout;

		[Tooltip("What is the maximum distance we are allowed to have to our spawn location before we are being encourraged to go back home.")]
		public NPCPlayerApex.EnemyRangeEnum MaxRangeToSpawnLoc;

		[Tooltip("If this is set to true, then a target must hold special markers (like IsHostile) for the target to be considered for aggressive action.")]
		public bool OnlyAggroMarkedTargets;

		public enum FamilyEnum
		{
			Bear,
			Wolf,
			Deer,
			Boar,
			Chicken,
			Horse,
			Zombie,
			Scientist,
			Murderer,
			Player
		}
	}

	public enum Behaviour
	{
		Idle,
		Wander,
		Attack,
		Flee,
		Eat,
		Sleep,
		RetreatingToCover
	}

	public enum EnemyRangeEnum : byte
	{
		AttackRange,
		AggroRange,
		AwareRange,
		OutOfRange
	}

	public enum Facts
	{
		HasEnemy,
		EnemyRange,
		CanTargetEnemies,
		Health,
		Speed,
		IsTired,
		IsSleeping,
		IsAttackReady,
		IsRoamReady,
		IsAggro,
		WantsToFlee,
		IsHungry,
		FoodRange,
		AttackedLately,
		LoudNoiseNearby,
		CanTargetFood,
		IsMoving,
		IsFleeing,
		IsEating,
		IsAfraid,
		AfraidRange,
		IsUnderHealthThreshold,
		CanNotMove,
		PathToTargetStatus
	}

	public enum FoodRangeEnum : byte
	{
		EatRange,
		AwareRange,
		OutOfRange
	}

	public enum HealthEnum : byte
	{
		Fine,
		Medium,
		Low
	}

	public enum SpeedEnum : byte
	{
		StandStill,
		Walk,
		Run
	}
}