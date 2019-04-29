using Apex.AI;
using Apex.AI.Components;
using Apex.AI.Serialization;
using Apex.LoadBalancing;
using ConVar;
using Network;
using Oxide.Core;
using Rust;
using Rust.Ai;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class NPCPlayerApex : NPCPlayer, IContextProvider, IAIAgent, ILoadBalanced
{
	public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

	public GameObjectRef RadioEffect;

	public GameObjectRef DeathEffect;

	public int agentTypeIndex;

	private Vector3 lastStuckPos;

	public float stuckDuration;

	public float lastStuckTime;

	private float timeAtDestination;

	public const float TickRate = 0.1f;

	public bool IsInvinsible;

	public float lastInvinsibleStartTime;

	public float InvinsibleTime = 2f;

	public readonly static HashSet<NPCPlayerApex> AllJunkpileNPCs;

	public readonly static HashSet<NPCPlayerApex> AllBanditCampNPCs;

	private float nextSensorySystemTick;

	private float nextReasoningSystemTick;

	private float attackTargetVisibleFor;

	private BaseEntity lastAttackTarget;

	public BaseNpc.AiStatistics Stats;

	[SerializeField]
	public UtilityAIComponent utilityAiComponent;

	public bool NewAI;

	public bool NeverMove;

	public bool IsMountableAgent;

	public float WeaponSwitchFrequency = 5f;

	public float ToolSwitchFrequency = 5f;

	public Rust.Ai.WaypointSet WaypointSet;

	[NonSerialized]
	public Transform[] LookAtInterestPointsStationary;

	private NPCHumanContext _aiContext;

	public StateTimer BusyTimer;

	private float maxFleeTime;

	private float fleeHealthThresholdPercentage = 1f;

	private float aggroTimeout = Single.NegativeInfinity;

	private float lastAggroChanceResult;

	private float lastAggroChanceCalcTime;

	private const float aggroChanceRecalcTimeout = 5f;

	private BaseEntity blockTargetingThisEnemy;

	[ReadOnly]
	public float NextWeaponSwitchTime;

	[ReadOnly]
	public float NextToolSwitchTime;

	[ReadOnly]
	public float NextDetectionCheck;

	private bool wasAggro;

	[NonSerialized]
	public float TimeLastMoved;

	[NonSerialized]
	public float TimeLastMovedToCover;

	[NonSerialized]
	public float AllyAttackedRecentlyTimeout;

	[NonSerialized]
	public float LastHasEnemyTime;

	[NonSerialized]
	public bool LastDetectionCheckResult;

	public BaseNpc.Behaviour _currentBehavior;

	protected float lastInRangeOfSpawnPositionTime = Single.NegativeInfinity;

	private static Vector3[] pathCornerCache;

	private static NavMeshPath _pathCache;

	private float nextLookAtPointTime;

	[NonSerialized]
	public Transform LookAtPoint;

	[NonSerialized]
	public PlayerEyes LookAtEyes;

	[Header("Npc Communication")]
	public float CommunicationRadius = -1f;

	[NonSerialized]
	public byte[] CurrentFacts = new byte[Enum.GetValues(typeof(NPCPlayerApex.Facts)).Length];

	[Header("NPC Player Senses")]
	public int ForgetUnseenEntityTime = 10;

	public float SensesTickRate = 0.5f;

	public float MaxDistanceToCover = 15f;

	public float MinDistanceToRetreatCover = 6f;

	[Header("NPC Player Senses Target Scoring")]
	public float VisionRangeScore = 1f;

	public float AggroRangeScore = 5f;

	public float LongRangeScore = 1f;

	public float MediumRangeScore = 5f;

	public float CloseRangeScore = 10f;

	[NonSerialized]
	public BaseEntity[] SensesResults = new BaseEntity[128];

	private List<NavPointSample> navPointSamples = new List<NavPointSample>(8);

	private NPCPlayerApex.CoverPointComparer coverPointComparer;

	private float lastTickTime;

	private const int sensesTicksPerCoverSweep = 5;

	private int sensesTicksSinceLastCoverSweep = 5;

	private float alertness;

	protected float lastSeenPlayerTime = Single.NegativeInfinity;

	private bool isAlreadyCheckingPathPending;

	private int numPathPendingAttempts;

	private float accumPathPendingDelay;

	[Header("Sensory")]
	[Tooltip("Only care about sensations from our active enemy target, and nobody else.")]
	public bool OnlyTargetSensations;

	private const int MaxPlayers = 128;

	public static BasePlayer[] PlayerQueryResults;

	public static int PlayerQueryResultCount;

	private static NavMeshPath PathToPlayerTarget;

	private static Rust.Ai.PlayerTargetContext _playerTargetContext;

	public static BaseEntity[] EntityQueryResults;

	public static int EntityQueryResultCount;

	private static Rust.Ai.EntityTargetContext _entityTargetContext;

	private static Rust.Ai.CoverContext _coverContext;

	private static BaseAiUtilityClient _selectPlayerTargetAI;

	private static BaseAiUtilityClient _selectPlayerTargetMountedAI;

	private static BaseAiUtilityClient _selectEntityTargetAI;

	private static BaseAiUtilityClient _selectCoverTargetsAI;

	private static BaseAiUtilityClient _selectEnemyHideoutAI;

	[Header("Sensory System")]
	public AIStorage SelectPlayerTargetUtility;

	public AIStorage SelectPlayerTargetMountedUtility;

	public AIStorage SelectEntityTargetsUtility;

	public AIStorage SelectCoverTargetsUtility;

	public AIStorage SelectEnemyHideoutUtility;

	private float playerTargetDecisionStartTime;

	private float animalTargetDecisionStartTime;

	private float nextCoverInfoTick;

	private float nextCoverPosInfoTick;

	private float _lastHeardGunshotTime = Single.NegativeInfinity;

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

	public NPCHumanContext AiContext
	{
		get
		{
			if (this._aiContext == null)
			{
				this.SetupAiContext();
			}
			return this._aiContext;
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
			return this.eyes.position;
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
			return this.attackTargetVisibleFor;
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

	private static Rust.Ai.CoverContext CoverContext
	{
		get
		{
			if (NPCPlayerApex._coverContext == null)
			{
				NPCPlayerApex._coverContext = new Rust.Ai.CoverContext();
			}
			return NPCPlayerApex._coverContext;
		}
	}

	public Vector3 CrouchedAttackPosition
	{
		get
		{
			if (base.IsDucked())
			{
				return this.AttackPosition;
			}
			return this.AttackPosition - (Vector3.down * 1f);
		}
	}

	public Vector3 CurrentAimAngles
	{
		get
		{
			return this.eyes.BodyForward();
		}
	}

	public float currentBehaviorDuration
	{
		get
		{
			return JustDecompileGenerated_get_currentBehaviorDuration();
		}
		set
		{
			JustDecompileGenerated_set_currentBehaviorDuration(value);
		}
	}

	private float JustDecompileGenerated_currentBehaviorDuration_k__BackingField;

	public float JustDecompileGenerated_get_currentBehaviorDuration()
	{
		return this.JustDecompileGenerated_currentBehaviorDuration_k__BackingField;
	}

	public void JustDecompileGenerated_set_currentBehaviorDuration(float value)
	{
		this.JustDecompileGenerated_currentBehaviorDuration_k__BackingField = value;
	}

	public BaseNpc.Behaviour CurrentBehaviour
	{
		get
		{
			return this._currentBehavior;
		}
		set
		{
			this._currentBehavior = value;
			this.BehaviourChanged();
		}
	}

	public int CurrentWaypointIndex
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
				this.IsStopped = false;
				this.GetNavAgent.destination = value;
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

	private static Rust.Ai.EntityTargetContext EntityTargetContext
	{
		get
		{
			if (NPCPlayerApex._entityTargetContext == null)
			{
				NPCPlayerApex._entityTargetContext = new Rust.Ai.EntityTargetContext();
			}
			return NPCPlayerApex._entityTargetContext;
		}
	}

	public override BaseNpc.AiStatistics.FamilyEnum Family
	{
		get
		{
			return BaseNpc.AiStatistics.FamilyEnum.Scientist;
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
			return 0f;
		}
	}

	public Vector3 GetAttackOffset
	{
		get
		{
			return new Vector3(0f, 1.8f, 0f);
		}
	}

	public float GetAttackRange
	{
		get
		{
			return this.WeaponAttackRange();
		}
	}

	public float GetAttackRate
	{
		get
		{
			return 0f;
		}
	}

	public float GetEnergy
	{
		get
		{
			return 1f;
		}
	}

	public float GetLastStuckTime
	{
		get
		{
			return 0f;
		}
	}

	public NavMeshAgent GetNavAgent
	{
		get
		{
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
			return 1f;
		}
	}

	public float GetStamina
	{
		get
		{
			return 1f;
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
			return 0f;
		}
	}

	public override bool IsDormant
	{
		get
		{
			return base.IsDormant;
		}
		set
		{
			base.IsDormant = value;
			if (value)
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

	public bool IsWaitingAtWaypoint
	{
		get;
		set;
	}

	public Vector3 LastHeardGunshotDirection
	{
		get;
		set;
	}

	public NPCPlayerApex.ActionCallback OnAggro
	{
		get;
		set;
	}

	public NPCPlayerApex.ActionCallback OnChatter
	{
		get;
		set;
	}

	public NPCPlayerApex.ActionCallback OnDeath
	{
		get;
		set;
	}

	public NPCPlayerApex.ActionCallback OnFleeExplosive
	{
		get;
		set;
	}

	public NPCPlayerApex.ActionCallback OnReload
	{
		get;
		set;
	}

	public NPCPlayerApex.ActionCallback OnTakeCover
	{
		get;
		set;
	}

	private static Rust.Ai.PlayerTargetContext PlayerTargetContext
	{
		get
		{
			if (NPCPlayerApex._playerTargetContext == null)
			{
				NPCPlayerApex._playerTargetContext = new Rust.Ai.PlayerTargetContext()
				{
					Direction = new Vector3[128],
					Dot = new float[128],
					DistanceSqr = new float[128],
					LineOfSight = new byte[128]
				};
			}
			return NPCPlayerApex._playerTargetContext;
		}
	}

	public float SecondsSinceLastHeardGunshot
	{
		get
		{
			return UnityEngine.Time.time - this._lastHeardGunshotTime;
		}
	}

	public float SecondsSinceLastInRangeOfSpawnPosition
	{
		get
		{
			return UnityEngine.Time.time - this.lastInRangeOfSpawnPositionTime;
		}
	}

	public float SecondsSinceSeenPlayer
	{
		get
		{
			return UnityEngine.Time.time - this.lastSeenPlayerTime;
		}
	}

	private BaseAiUtilityClient SelectCoverTargetsAI
	{
		get
		{
			if (NPCPlayerApex._selectCoverTargetsAI == null && this.SelectCoverTargetsUtility != null)
			{
				NPCPlayerApex._selectCoverTargetsAI = new BaseAiUtilityClient(AIManager.GetAI(new Guid(this.SelectCoverTargetsUtility.aiId)), this);
				NPCPlayerApex._selectCoverTargetsAI.Initialize();
			}
			return NPCPlayerApex._selectCoverTargetsAI;
		}
	}

	private BaseAiUtilityClient SelectEnemyHideoutAI
	{
		get
		{
			if (NPCPlayerApex._selectEnemyHideoutAI == null && this.SelectEnemyHideoutUtility != null)
			{
				NPCPlayerApex._selectEnemyHideoutAI = new BaseAiUtilityClient(AIManager.GetAI(new Guid(this.SelectEnemyHideoutUtility.aiId)), this);
				NPCPlayerApex._selectEnemyHideoutAI.Initialize();
			}
			return NPCPlayerApex._selectEnemyHideoutAI;
		}
	}

	private BaseAiUtilityClient SelectEntityTargetAI
	{
		get
		{
			if (NPCPlayerApex._selectEntityTargetAI == null && this.SelectEntityTargetsUtility != null)
			{
				NPCPlayerApex._selectEntityTargetAI = new BaseAiUtilityClient(AIManager.GetAI(new Guid(this.SelectEntityTargetsUtility.aiId)), this);
				NPCPlayerApex._selectEntityTargetAI.Initialize();
			}
			return NPCPlayerApex._selectEntityTargetAI;
		}
	}

	private BaseAiUtilityClient SelectPlayerTargetAI
	{
		get
		{
			if (NPCPlayerApex._selectPlayerTargetAI == null && this.SelectPlayerTargetUtility != null)
			{
				NPCPlayerApex._selectPlayerTargetAI = new BaseAiUtilityClient(AIManager.GetAI(new Guid(this.SelectPlayerTargetUtility.aiId)), this);
				NPCPlayerApex._selectPlayerTargetAI.Initialize();
			}
			return NPCPlayerApex._selectPlayerTargetAI;
		}
	}

	private BaseAiUtilityClient SelectPlayerTargetMountedAI
	{
		get
		{
			if (NPCPlayerApex._selectPlayerTargetMountedAI == null && this.SelectPlayerTargetMountedUtility != null)
			{
				NPCPlayerApex._selectPlayerTargetMountedAI = new BaseAiUtilityClient(AIManager.GetAI(new Guid(this.SelectPlayerTargetMountedUtility.aiId)), this);
				NPCPlayerApex._selectPlayerTargetMountedAI.Initialize();
			}
			return NPCPlayerApex._selectPlayerTargetMountedAI;
		}
	}

	public Vector3 SpawnPosition
	{
		get;
		set;
	}

	public float SqrStoppingDistance
	{
		get
		{
			if (!this.IsNavRunning())
			{
				return 0f;
			}
			return this.GetNavAgent.stoppingDistance * this.GetNavAgent.stoppingDistance;
		}
	}

	public float StoppingDistance
	{
		get
		{
			if (!this.IsNavRunning())
			{
				return 0f;
			}
			return this.GetNavAgent.stoppingDistance;
		}
		set
		{
			if (this.IsNavRunning())
			{
				this.GetNavAgent.stoppingDistance = value;
			}
		}
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
			return this.timeAtDestination;
		}
	}

	public float WaypointDelayTime
	{
		get;
		set;
	}

	public int WaypointDirection
	{
		get;
		set;
	}

	static NPCPlayerApex()
	{
		NPCPlayerApex.AllJunkpileNPCs = new HashSet<NPCPlayerApex>();
		NPCPlayerApex.AllBanditCampNPCs = new HashSet<NPCPlayerApex>();
		NPCPlayerApex.pathCornerCache = new Vector3[128];
		NPCPlayerApex._pathCache = null;
		NPCPlayerApex.PlayerQueryResults = new BasePlayer[128];
		NPCPlayerApex.PlayerQueryResultCount = 0;
		NPCPlayerApex.EntityQueryResults = new BaseEntity[128];
		NPCPlayerApex.EntityQueryResultCount = 0;
	}

	public NPCPlayerApex()
	{
	}

	private bool _FindCoverPointsInVolume()
	{
		CoverPointVolume currentCoverVolume = this.AiContext.CurrentCoverVolume;
		return this._FindCoverPointsInVolume(this.AiContext.Position, this.AiContext.sampledCoverPoints, ref currentCoverVolume, ref this.nextCoverInfoTick);
	}

	private bool _FindCoverPointsInVolume(Vector3 position)
	{
		CoverPointVolume coverPointVolume = null;
		return this._FindCoverPointsInVolume(position, this.AiContext.EnemyCoverPoints, ref coverPointVolume, ref this.nextCoverPosInfoTick);
	}

	private bool _FindCoverPointsInVolume(Vector3 position, List<CoverPoint> coverPoints, ref CoverPointVolume volume, ref float timer)
	{
		Transform coverPointGroup;
		if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || !SingletonComponent<AiManager>.Instance.UseCover)
		{
			return false;
		}
		if (UnityEngine.Time.time > timer)
		{
			timer = UnityEngine.Time.time + 0.1f * AI.npc_cover_info_tick_rate_multiplier;
			if (volume == null || !volume.Contains(position))
			{
				volume = SingletonComponent<AiManager>.Instance.GetCoverVolumeContaining(position);
				if (volume == null)
				{
					Vector3 vector3 = position;
					if (this.AiContext.AiLocationManager != null)
					{
						coverPointGroup = this.AiContext.AiLocationManager.CoverPointGroup;
					}
					else
					{
						coverPointGroup = null;
					}
					volume = AiManager.CreateNewCoverVolume(vector3, coverPointGroup);
				}
			}
		}
		if (volume == null)
		{
			return false;
		}
		if (coverPoints.Count > 0)
		{
			coverPoints.Clear();
		}
		float maxDistanceToCover = this.MaxDistanceToCover * this.MaxDistanceToCover;
		foreach (CoverPoint coverPoint in volume.CoverPoints)
		{
			if (coverPoint.IsReserved || coverPoint.IsCompromised || (position - coverPoint.Position).sqrMagnitude > maxDistanceToCover)
			{
				continue;
			}
			coverPoints.Add(coverPoint);
		}
		if (coverPoints.Count > 1)
		{
			coverPoints.Sort(this.coverPointComparer);
		}
		return true;
	}

	private void _FindEntitiesInCloseRange()
	{
		if (Interface.CallHook("IOnNpcPlayerSenseClose", this) != null)
		{
			return;
		}
		NPCPlayerApex.EntityQueryResultCount = BaseEntity.Query.Server.GetInSphere(base.transform.position, this.Stats.CloseRange, NPCPlayerApex.EntityQueryResults, (BaseEntity entity) => {
			if (entity == null || !entity.isServer || entity.IsDestroyed)
			{
				return false;
			}
			if (!(entity is BaseNpc) && !(entity is TimedExplosive))
			{
				return false;
			}
			return true;
		});
	}

	private void _FindPlayersInVisionRange()
	{
		if (AI.ignoreplayers || base.transform == null)
		{
			return;
		}
		if (Interface.CallHook("IOnNpcPlayerSenseVision", this) != null)
		{
			return;
		}
		NPCPlayerApex.PlayerQueryResultCount = BaseEntity.Query.Server.GetPlayersInSphere(base.transform.position, this.Stats.VisionRange, NPCPlayerApex.PlayerQueryResults, (BasePlayer player) => {
			if (player == null || !player.isServer || player.IsDead())
			{
				return false;
			}
			if (player.IsSleeping() && player.secondsSleeping < NPCAutoTurret.sleeperhostiledelay)
			{
				return false;
			}
			float visionRange = this.Stats.VisionRange * this.Stats.VisionRange;
			if ((player.ServerPosition - this.ServerPosition).sqrMagnitude > visionRange)
			{
				return false;
			}
			return true;
		});
	}

	private void _GatherPlayerTargetFacts()
	{
		object obj;
		object obj1;
		object obj2;
		if (this.AttackTarget == null)
		{
			this._NoEnemyFacts();
			return;
		}
		float distanceSqr = Single.MaxValue;
		byte lineOfSight = 0;
		if (NPCPlayerApex.PlayerTargetContext.Index < 0)
		{
			Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo(this.AttackTarget);
			if (extendedInfo.Entity == null)
			{
				this._NoEnemyFacts();
				return;
			}
			distanceSqr = extendedInfo.DistanceSqr;
			lineOfSight = extendedInfo.LineOfSight;
		}
		else
		{
			int index = NPCPlayerApex.PlayerTargetContext.Index;
			distanceSqr = NPCPlayerApex.PlayerTargetContext.DistanceSqr[index];
			lineOfSight = NPCPlayerApex.PlayerTargetContext.LineOfSight[index];
			this.lastSeenPlayerTime = UnityEngine.Time.time;
		}
		this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte)this.ToEnemyRangeEnum(distanceSqr), true, true);
		this.SetFact(NPCPlayerApex.Facts.EnemyEngagementRange, (byte)this.ToEnemyEngagementRangeEnum(distanceSqr), true, true);
		this.SetFact(NPCPlayerApex.Facts.AfraidRange, (byte)this.ToAfraidRangeEnum(distanceSqr), true, true);
		if (lineOfSight > 0)
		{
			obj = 1;
		}
		else
		{
			obj = null;
		}
		this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, (byte)obj, true, true);
		if (lineOfSight == 1 || lineOfSight == 3)
		{
			obj1 = 1;
		}
		else
		{
			obj1 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, (byte)obj1, true, true);
		if (lineOfSight == 2 || lineOfSight == 3)
		{
			obj2 = 1;
		}
		else
		{
			obj2 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, (byte)obj2, true, true);
	}

	private void _NoEnemyFacts()
	{
		this.SetFact(NPCPlayerApex.Facts.EnemyRange, 3, true, true);
		this.SetFact(NPCPlayerApex.Facts.EnemyEngagementRange, 1, true, true);
		this.SetFact(NPCPlayerApex.Facts.AfraidRange, 1, true, true);
		this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, 0, true, true);
		this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, 0, true, true);
		this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, 0, true, true);
	}

	private void _UpdateCoverFacts()
	{
		object obj;
		object obj1;
		object obj2;
		object obj3;
		object obj4;
		object obj5;
		object obj6;
		object obj7;
		object obj8;
		if (this.GetFact(NPCPlayerApex.Facts.HasEnemy) != 1)
		{
			this.SetFact(NPCPlayerApex.Facts.RetreatCoverInRange, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.FlankCoverInRange, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.AdvanceCoverInRange, 0, true, true);
			if (this.AiContext.CoverSet.Closest.ReservedCoverPoint != null)
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			this.SetFact(NPCPlayerApex.Facts.CoverInRange, (byte)obj, true, true);
			this.SetFact(NPCPlayerApex.Facts.IsMovingToCover, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, 0, true, true);
		}
		else
		{
			if (this.AiContext.CoverSet.Retreat.ReservedCoverPoint != null)
			{
				obj3 = 1;
			}
			else
			{
				obj3 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.RetreatCoverInRange, (byte)obj3, true, true);
			if (this.AiContext.CoverSet.Flank.ReservedCoverPoint != null)
			{
				obj4 = 1;
			}
			else
			{
				obj4 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.FlankCoverInRange, (byte)obj4, true, true);
			if (this.AiContext.CoverSet.Advance.ReservedCoverPoint != null)
			{
				obj5 = 1;
			}
			else
			{
				obj5 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.AdvanceCoverInRange, (byte)obj5, true, true);
			if (this.AiContext.CoverSet.Closest.ReservedCoverPoint != null)
			{
				obj6 = 1;
			}
			else
			{
				obj6 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.CoverInRange, (byte)obj6, true, true);
			if (this.GetFact(NPCPlayerApex.Facts.IsMovingToCover) == 1)
			{
				this.SetFact(NPCPlayerApex.Facts.IsMovingToCover, this.IsMoving(), true, true);
			}
			Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo(this.AttackTarget);
			if (extendedInfo.Entity != null)
			{
				if (!base.isMounted)
				{
					if (extendedInfo.Dot > AI.npc_valid_aim_cone)
					{
						obj7 = 1;
					}
					else
					{
						obj7 = null;
					}
					this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, (byte)obj7, true, true);
				}
				else
				{
					if (extendedInfo.Dot > AI.npc_valid_mounted_aim_cone)
					{
						obj8 = 1;
					}
					else
					{
						obj8 = null;
					}
					this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, (byte)obj8, true, true);
				}
			}
		}
		if (this.AiContext.CoverSet.Closest.ReservedCoverPoint != null)
		{
			if ((this.AiContext.CoverSet.Closest.ReservedCoverPoint.Position - this.ServerPosition).sqrMagnitude < 0.5625f)
			{
				obj1 = 1;
			}
			else
			{
				obj1 = null;
			}
			byte num = (byte)obj1;
			this.SetFact(NPCPlayerApex.Facts.IsInCover, num, true, true);
			if (num == 1)
			{
				if (this.AiContext.CoverSet.Closest.ReservedCoverPoint.IsCompromised)
				{
					obj2 = 1;
				}
				else
				{
					obj2 = null;
				}
				this.SetFact(NPCPlayerApex.Facts.IsCoverCompromised, (byte)obj2, true, true);
			}
		}
		if (this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) == 1)
		{
			this.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, this.IsMoving(), true, true);
		}
	}

	private void _UpdateGroundedSelfFacts()
	{
		object obj;
		object obj1;
		object obj2;
		object obj3;
		object obj4;
		object obj5;
		object obj6;
		object obj7;
		object obj8;
		object obj9;
		this.SetFact(NPCPlayerApex.Facts.Health, (byte)this.ToHealthEnum(base.healthFraction), true, true);
		if (UnityEngine.Time.realtimeSinceStartup >= this.NextAttackTime())
		{
			obj = 1;
		}
		else
		{
			obj = null;
		}
		this.SetFact(NPCPlayerApex.Facts.IsWeaponAttackReady, (byte)obj, true, true);
		if (UnityEngine.Time.realtimeSinceStartup < this.AiContext.NextRoamTime || !this.IsNavRunning())
		{
			obj1 = null;
		}
		else
		{
			obj1 = 1;
		}
		this.SetFact(NPCPlayerApex.Facts.IsRoamReady, (byte)obj1, true, true);
		this.SetFact(NPCPlayerApex.Facts.Speed, (byte)this.ToSpeedEnum(this.TargetSpeed / this.Stats.Speed), true, true);
		if (base.SecondsSinceAttacked < this.Stats.AttackedMemoryTime)
		{
			obj2 = 1;
		}
		else
		{
			obj2 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedLately, (byte)obj2, true, true);
		if (base.SecondsSinceAttacked < 2f)
		{
			obj3 = 1;
		}
		else
		{
			obj3 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedVeryRecently, (byte)obj3, true, true);
		if (base.SecondsSinceAttacked < 7f)
		{
			obj4 = 1;
		}
		else
		{
			obj4 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedRecently, (byte)obj4, true, true);
		this.SetFact(NPCPlayerApex.Facts.IsMoving, this.IsMoving(), true, false);
		if (UnityEngine.Time.realtimeSinceStartup > this.NextWeaponSwitchTime)
		{
			obj5 = 1;
		}
		else
		{
			obj5 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.CanSwitchWeapon, (byte)obj5, true, true);
		if (UnityEngine.Time.realtimeSinceStartup > this.NextToolSwitchTime)
		{
			obj6 = 1;
		}
		else
		{
			obj6 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.CanSwitchTool, (byte)obj6, true, true);
		this.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte)this.GetCurrentAmmoStateEnum(), true, true);
		this.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte)this.GetCurrentWeaponTypeEnum(), true, true);
		this.SetFact(NPCPlayerApex.Facts.CurrentToolType, (byte)this.GetCurrentToolTypeEnum(), true, true);
		if (this.AiContext.DeployedExplosives.Count > 0)
		{
			obj7 = 1;
		}
		else
		{
			obj7 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.ExplosiveInRange, (byte)obj7, true, true);
		if (this.Stats.IsMobile)
		{
			obj8 = 1;
		}
		else
		{
			obj8 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.IsMobile, (byte)obj8, true, true);
		if (!(this.WaypointSet != null) || this.WaypointSet.Points.Count <= 0)
		{
			obj9 = null;
		}
		else
		{
			obj9 = 1;
		}
		this.SetFact(NPCPlayerApex.Facts.HasWaypoints, (byte)obj9, true, true);
		NPCPlayerApex.EnemyRangeEnum rangeToSpawnPoint = this.GetRangeToSpawnPoint();
		this.SetFact(NPCPlayerApex.Facts.RangeToSpawnLocation, (byte)rangeToSpawnPoint, true, true);
		if (rangeToSpawnPoint < this.Stats.MaxRangeToSpawnLoc || this.Stats.MaxRangeToSpawnLoc == NPCPlayerApex.EnemyRangeEnum.CloseAttackRange && rangeToSpawnPoint == NPCPlayerApex.EnemyRangeEnum.CloseAttackRange)
		{
			this.lastInRangeOfSpawnPositionTime = UnityEngine.Time.time;
		}
		if (this.CheckHealthThresholdToFlee())
		{
			this.WantsToFlee();
		}
	}

	private void _UpdateMountedSelfFacts()
	{
		object obj;
		object obj1;
		object obj2;
		object obj3;
		object obj4;
		this.SetFact(NPCPlayerApex.Facts.Health, (byte)this.ToHealthEnum(base.healthFraction), true, true);
		if (UnityEngine.Time.realtimeSinceStartup >= this.NextAttackTime())
		{
			obj = 1;
		}
		else
		{
			obj = null;
		}
		this.SetFact(NPCPlayerApex.Facts.IsWeaponAttackReady, (byte)obj, true, true);
		if (base.SecondsSinceAttacked < this.Stats.AttackedMemoryTime)
		{
			obj1 = 1;
		}
		else
		{
			obj1 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedLately, (byte)obj1, true, true);
		if (base.SecondsSinceAttacked < 2f)
		{
			obj2 = 1;
		}
		else
		{
			obj2 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedVeryRecently, (byte)obj2, true, true);
		if (base.SecondsSinceAttacked < 7f)
		{
			obj3 = 1;
		}
		else
		{
			obj3 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedRecently, (byte)obj3, true, true);
		if (UnityEngine.Time.realtimeSinceStartup > this.NextWeaponSwitchTime)
		{
			obj4 = 1;
		}
		else
		{
			obj4 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.CanSwitchWeapon, (byte)obj4, true, true);
		this.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte)this.GetCurrentAmmoStateEnum(), true, true);
		this.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte)this.GetCurrentWeaponTypeEnum(), true, true);
	}

	private void AggroBestScorePlayerOrClosestAnimal()
	{
		List<AiAnswer_ShareEnemyTarget> aiAnswerShareEnemyTargets;
		object obj;
		object obj1;
		object obj2;
		float single = Single.MaxValue;
		float single1 = 0f;
		bool flag = false;
		bool flag1 = false;
		BasePlayer playerTarget = null;
		BaseNpc baseNpc = null;
		this.AiContext.AIAgent.AttackTarget = null;
		Vector3 value = Vector3.zero;
		float single2 = Single.MaxValue;
		foreach (BasePlayer player in this.AiContext.Players)
		{
			if (player.IsDead() || player.IsDestroyed || this.blockTargetingThisEnemy != null && player.net != null && this.blockTargetingThisEnemy.net != null && player.net.ID == this.blockTargetingThisEnemy.net.ID || this.Stats.OnlyAggroMarkedTargets && !this.HostilityConsideration(player))
			{
				continue;
			}
			NPCPlayerApex nPCPlayerApex = player as NPCPlayerApex;
			if (nPCPlayerApex != null && this.Stats.Family == nPCPlayerApex.Stats.Family)
			{
				continue;
			}
			float visionRangeScore = 0f;
			Vector3 serverPosition = player.ServerPosition - this.ServerPosition;
			float single3 = serverPosition.sqrMagnitude;
			if (single3 < single)
			{
				single = single3;
			}
			if (single3 < this.Stats.VisionRange * this.Stats.VisionRange)
			{
				visionRangeScore += this.VisionRangeScore;
			}
			if (single3 < this.Stats.AggressionRange * this.Stats.AggressionRange)
			{
				visionRangeScore += this.AggroRangeScore;
			}
			NPCPlayerApex.EnemyRangeEnum enemyRangeEnum = this.ToEnemyRangeEnum(single3);
			if (enemyRangeEnum == NPCPlayerApex.EnemyRangeEnum.LongAttackRange)
			{
				visionRangeScore += this.LongRangeScore;
			}
			else if (enemyRangeEnum == NPCPlayerApex.EnemyRangeEnum.MediumAttackRange)
			{
				visionRangeScore += this.MediumRangeScore;
			}
			else if (enemyRangeEnum == NPCPlayerApex.EnemyRangeEnum.CloseAttackRange)
			{
				visionRangeScore += this.CloseRangeScore;
			}
			bool flag2 = this.IsVisibleStanding(player);
			bool flag3 = false;
			if (!flag2)
			{
				flag3 = this.IsVisibleCrouched(player);
			}
			if (flag2 || flag3)
			{
				this.AiContext.Memory.Update(player, 0f);
			}
			else
			{
				if (this.AiContext.Memory.GetInfo(player).Entity == null || !this.IsWithinAggroRange(single3))
				{
					continue;
				}
				visionRangeScore *= 0.75f;
			}
			float single4 = Mathf.Sqrt(single3);
			visionRangeScore *= this.VisibilityScoreModifier(player, serverPosition, single4, flag2, flag3);
			if (visionRangeScore <= single1)
			{
				continue;
			}
			playerTarget = player;
			baseNpc = null;
			value = serverPosition;
			single2 = single3;
			single1 = visionRangeScore;
			flag = flag2;
			flag1 = flag3;
		}
		if (!base.isMounted && playerTarget == null)
		{
			if (this.AskQuestion(new AiQuestion_ShareEnemyTarget(), out aiAnswerShareEnemyTargets) > 0)
			{
				foreach (AiAnswer_ShareEnemyTarget aiAnswerShareEnemyTarget in aiAnswerShareEnemyTargets)
				{
					if (!(aiAnswerShareEnemyTarget.PlayerTarget != null) || !aiAnswerShareEnemyTarget.LastKnownPosition.HasValue || !this.HostilityConsideration(aiAnswerShareEnemyTarget.PlayerTarget))
					{
						continue;
					}
					playerTarget = aiAnswerShareEnemyTarget.PlayerTarget;
					baseNpc = null;
					value = aiAnswerShareEnemyTarget.LastKnownPosition.Value - this.ServerPosition;
					single2 = value.sqrMagnitude;
					single1 = 100f;
					single = value.sqrMagnitude;
					flag = this.IsVisibleStanding(playerTarget);
					flag1 = false;
					if (!flag)
					{
						flag1 = this.IsVisibleCrouched(playerTarget);
					}
					this.AiContext.Memory.Update(playerTarget, aiAnswerShareEnemyTarget.LastKnownPosition.Value, 0f);
					goto Label0;
				}
			}
		}
	Label0:
		if (single > 0.1f && single1 < 10f)
		{
			bool flag4 = (playerTarget == null ? false : single <= this.Stats.AggressionRange);
			foreach (BaseNpc npc in this.AiContext.Npcs)
			{
				if (npc.IsDead() || npc.IsDestroyed || this.Stats.Family == npc.Stats.Family)
				{
					continue;
				}
				Vector3 vector3 = npc.ServerPosition - this.ServerPosition;
				float single5 = vector3.sqrMagnitude;
				if (single5 >= single)
				{
					continue;
				}
				NPCPlayerApex.EnemyRangeEnum enemyRangeEnum1 = this.ToEnemyRangeEnum(single5);
				if (flag4 && enemyRangeEnum1 > NPCPlayerApex.EnemyRangeEnum.CloseAttackRange || enemyRangeEnum1 > NPCPlayerApex.EnemyRangeEnum.MediumAttackRange)
				{
					continue;
				}
				single = single5;
				baseNpc = npc;
				playerTarget = null;
				value = vector3;
				single2 = single5;
				flag1 = false;
				flag = this.IsVisibleStanding(npc);
				if (!flag)
				{
					flag1 = this.IsVisibleCrouched(npc);
				}
				if (flag | flag1)
				{
					this.AiContext.Memory.Update(npc, 0f);
				}
				if (single >= 0.1f)
				{
					continue;
				}
				goto Label1;
			}
		}
	Label1:
		this.AiContext.EnemyPlayer = playerTarget;
		this.AiContext.EnemyNpc = baseNpc;
		this.AiContext.LastTargetScore = single1;
		if ((!(playerTarget != null) || playerTarget.IsDestroyed || playerTarget.IsDead()) && !(baseNpc != null))
		{
			this.SetFact(NPCPlayerApex.Facts.HasEnemy, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.EnemyRange, 3, true, true);
			this.SetFact(NPCPlayerApex.Facts.AfraidRange, 1, true, true);
			this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, 0, true, true);
			return;
		}
		this.SetFact(NPCPlayerApex.Facts.HasEnemy, 1, true, false);
		if (playerTarget == null)
		{
			this.AiContext.AIAgent.AttackTarget = baseNpc;
		}
		else
		{
			this.AiContext.AIAgent.AttackTarget = playerTarget;
		}
		NPCPlayerApex.EnemyRangeEnum enemyRangeEnum2 = this.ToEnemyRangeEnum(single2);
		NPCPlayerApex.AfraidRangeEnum afraidRangeEnum = this.ToAfraidRangeEnum(single2);
		this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte)enemyRangeEnum2, true, true);
		this.SetFact(NPCPlayerApex.Facts.AfraidRange, (byte)afraidRangeEnum, true, true);
		bool flag5 = flag | flag1;
		if (flag5)
		{
			obj = 1;
		}
		else
		{
			obj = null;
		}
		this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, (byte)obj, true, true);
		if (flag1)
		{
			obj1 = 1;
		}
		else
		{
			obj1 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, (byte)obj1, true, true);
		if (flag)
		{
			obj2 = 1;
		}
		else
		{
			obj2 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, (byte)obj2, true, true);
		if ((playerTarget != null) & flag5)
		{
			this.lastSeenPlayerTime = UnityEngine.Time.time;
		}
		this.TryAggro(enemyRangeEnum2);
	}

	private void AggroClosestPlayerMounted()
	{
		float single = Single.MaxValue;
		bool flag = false;
		bool flag1 = false;
		BasePlayer basePlayer = null;
		foreach (BasePlayer player in this.AiContext.Players)
		{
			if (player.IsDead() || player.IsDestroyed || this.Stats.OnlyAggroMarkedTargets && !this.HostilityConsideration(player))
			{
				continue;
			}
			bool flag2 = this.IsVisibleMounted(player);
			if (flag2)
			{
				this.AiContext.Memory.Update(player, 0f);
			}
			Vector3 serverPosition = player.ServerPosition - this.ServerPosition;
			BaseMountable mounted = base.GetMounted();
			if (Vector3.Dot(serverPosition.normalized, mounted.transform.forward) < -0.1f)
			{
				continue;
			}
			float single1 = serverPosition.sqrMagnitude;
			if (single1 >= single)
			{
				continue;
			}
			single = single1;
			basePlayer = player;
			flag = flag2;
			flag1 = flag2;
		}
		this.SetAttackTarget(basePlayer, 1f, single, flag, flag1, true);
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
		if (ent is WorldItem)
		{
			return true;
		}
		if (ent is BaseCorpse)
		{
			return true;
		}
		if (ent is TimedExplosive)
		{
			return true;
		}
		if (ent is BaseChair)
		{
			return true;
		}
		return false;
	}

	private static bool AiCaresAboutIgnoreChairs(BaseEntity ent)
	{
		if (ent is BasePlayer)
		{
			return true;
		}
		if (ent is BaseNpc)
		{
			return true;
		}
		if (ent is WorldItem)
		{
			return true;
		}
		if (ent is BaseCorpse)
		{
			return true;
		}
		if (ent is TimedExplosive)
		{
			return true;
		}
		return false;
	}

	float? Apex.LoadBalancing.ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
	{
		float single = UnityEngine.Time.time;
		this.IsInvinsible = single - this.lastInvinsibleStartTime < this.InvinsibleTime;
		if (single > this.nextSensorySystemTick)
		{
			using (TimeWarning timeWarning = TimeWarning.New("NPC.TickSensorySystem", 0.1f))
			{
				this.TickSensorySystem();
			}
			this.nextSensorySystemTick = single + 0.1f * AI.npc_sensory_system_tick_rate_multiplier + UnityEngine.Random.@value * 0.1f;
		}
		if (single > this.nextReasoningSystemTick)
		{
			using (timeWarning = TimeWarning.New("NPC.TickReasoningSystem", 0.1f))
			{
				this.TickReasoningSystem();
			}
			this.nextReasoningSystemTick = single + 0.1f * AI.npc_reasoning_system_tick_rate_multiplier + UnityEngine.Random.@value * 0.1f;
		}
		using (timeWarning = TimeWarning.New("NPC.TickBehaviourState", 0.1f))
		{
			this.TickBehaviourState();
		}
		return new float?(UnityEngine.Random.@value * 0.1f + 0.1f);
	}

	public virtual int AskQuestion(AiQuestion_ShareEnemyTarget question, out List<AiAnswer_ShareEnemyTarget> answers)
	{
		answers = null;
		return 0;
	}

	public bool AttackReady()
	{
		return true;
	}

	public void BehaviourChanged()
	{
		this.currentBehaviorDuration = 0f;
	}

	public bool BusyTimerActive()
	{
		return this.BusyTimer.IsActive;
	}

	public override string Categorize()
	{
		return "scientist";
	}

	private bool CheckHealthThresholdToFlee()
	{
		object obj;
		if (base.healthFraction > this.Stats.HealthThresholdForFleeing)
		{
			if (this.Stats.HealthThresholdForFleeing < 1f)
			{
				this.SetFact(NPCPlayerApex.Facts.IsUnderHealthThreshold, 0, true, true);
				return false;
			}
			if (this.GetFact(NPCPlayerApex.Facts.HasEnemy) == 1)
			{
				this.SetFact(NPCPlayerApex.Facts.IsUnderHealthThreshold, 0, true, true);
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
		this.SetFact(NPCPlayerApex.Facts.IsUnderHealthThreshold, (byte)obj, true, true);
		return healthThresholdFleeChance;
	}

	public NPCPlayerApex.EnemyRangeEnum CurrentWeaponToEnemyRange()
	{
		return this.WeaponToEnemyRange(this.GetCurrentWeaponTypeEnum());
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

	private void DelayedReloadOnInit()
	{
		ReloadOperator.Reload(this.AiContext);
	}

	private void DelayedSpawnPosition()
	{
		this.SpawnPosition = base.GetPosition();
	}

	private void DelayedTargetPathStatus()
	{
		this.numPathPendingAttempts++;
		this.accumPathPendingDelay += 0.1f;
		this.isAlreadyCheckingPathPending = false;
		this.SetTargetPathStatus(this.accumPathPendingDelay);
	}

	public override float DesiredMoveSpeed()
	{
		float single = 0f;
		float single1 = (this.modelState.ducked ? 1f : 0f);
		float npcSpeedWalk = 1f;
		if (this.CurrentBehaviour != BaseNpc.Behaviour.Wander)
		{
			npcSpeedWalk = 1f;
			Vector3 navAgent = this.NavAgent.desiredVelocity;
			float single2 = Vector3.Dot(navAgent.normalized, this.eyes.BodyForward());
			single2 = (single2 <= 0.75f ? 0f : Mathf.Clamp01((single2 - 0.75f) / 0.25f));
			single = single2;
		}
		else
		{
			npcSpeedWalk = AI.npc_speed_walk * 3f;
		}
		return base.GetSpeed(single, single1) * npcSpeedWalk;
	}

	public void Dismount()
	{
		BaseMountable mounted = base.GetMounted();
		if (mounted != null && mounted.AttemptDismount(this))
		{
			this.SetFact(NPCPlayerApex.Facts.IsMounted, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.WantsToDismount, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.CanNotWieldWeapon, 0, true, true);
			this.Resume();
		}
	}

	public override void DismountObject()
	{
		base.DismountObject();
		this.SetFact(NPCPlayerApex.Facts.WantsToDismount, 1, true, true);
	}

	public static float Distance2DSqr(Vector3 a, Vector3 b)
	{
		Vector2 vector2 = new Vector2(a.x, a.z) - new Vector2(b.x, b.z);
		return vector2.sqrMagnitude;
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.NewAI)
		{
			if (this.AiContext != null && this.AiContext.AiLocationManager != null)
			{
				if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
				{
					NPCPlayerApex.AllJunkpileNPCs.Remove(this);
				}
				else if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.BanditTown)
				{
					NPCPlayerApex.AllBanditCampNPCs.Remove(this);
				}
			}
			NPCSensesLoadBalancer.NpcSensesLoadBalancer.Remove(this);
		}
		base.CancelInvoke(new Action(this.RadioChatter));
	}

	public void Eat()
	{
	}

	public float FearLevel(BaseEntity ent)
	{
		return 0f;
	}

	private void FindClosestCoverToUs()
	{
		float single = Single.MaxValue;
		CoverPoint coverPoint = null;
		this.AiContext.CoverSet.Reset();
		foreach (CoverPoint sampledCoverPoint in this.AiContext.sampledCoverPoints)
		{
			if (sampledCoverPoint.IsReserved || sampledCoverPoint.IsCompromised)
			{
				continue;
			}
			float position = (sampledCoverPoint.Position - this.ServerPosition).sqrMagnitude;
			if (position >= single)
			{
				continue;
			}
			single = position;
			coverPoint = sampledCoverPoint;
		}
		if (coverPoint != null)
		{
			this.AiContext.CoverSet.Closest.ReservedCoverPoint = coverPoint;
		}
	}

	private void FindCoverFromEnemy()
	{
		this.AiContext.CoverSet.Reset();
		if (this.AttackTarget != null)
		{
			this.FindCoverFromPosition(this.AiContext.EnemyPosition);
		}
	}

	private void FindCoverFromPosition(Vector3 position)
	{
		float single = 0f;
		float single1 = 0f;
		float single2 = 0f;
		CoverPoint coverPoint = null;
		CoverPoint coverPoint1 = null;
		CoverPoint coverPoint2 = null;
		this.AiContext.CoverSet.Reset();
		foreach (CoverPoint sampledCoverPoint in this.AiContext.sampledCoverPoints)
		{
			if (sampledCoverPoint.IsReserved || sampledCoverPoint.IsCompromised || !sampledCoverPoint.ProvidesCoverFromPoint(position, -0.8f))
			{
				continue;
			}
			Vector3 vector3 = sampledCoverPoint.Position - this.ServerPosition;
			Vector3 vector31 = position - this.ServerPosition;
			float single3 = Vector3.Dot(vector3.normalized, vector31.normalized);
			if (single3 > 0.5f && vector3.sqrMagnitude > vector31.sqrMagnitude)
			{
				continue;
			}
			if (single3 <= -0.5f)
			{
				if (vector3.sqrMagnitude >= this.MinDistanceToRetreatCover * this.MinDistanceToRetreatCover)
				{
					float single4 = single3 * -1f;
					if (single4 > single)
					{
						single = single4;
						coverPoint = sampledCoverPoint;
					}
				}
				else
				{
					single3 = 0.1f;
				}
			}
			if (single3 >= 0.5f)
			{
				float single5 = vector3.sqrMagnitude;
				if (single5 > vector31.sqrMagnitude)
				{
					continue;
				}
				float single6 = single3;
				if (single6 > single2)
				{
					if (AI.npc_cover_use_path_distance && this.IsNavRunning() && this.AttackTarget != null && !this.PathDistanceIsValid(this.AttackTarget.ServerPosition, sampledCoverPoint.Position, false))
					{
						continue;
					}
					if ((sampledCoverPoint.Position - position).sqrMagnitude < single5)
					{
						single6 *= 0.9f;
					}
					single2 = single6;
					coverPoint2 = sampledCoverPoint;
				}
			}
			if (single3 < -0.1f || single3 > 0.1f)
			{
				continue;
			}
			float single7 = 1f - Mathf.Abs(single3);
			if (single7 <= single1 || AI.npc_cover_use_path_distance && this.IsNavRunning() && this.AttackTarget != null && !this.PathDistanceIsValid(this.AttackTarget.ServerPosition, sampledCoverPoint.Position, false))
			{
				continue;
			}
			single1 = 0.1f - Mathf.Abs(single7);
			coverPoint1 = sampledCoverPoint;
		}
		this.AiContext.CoverSet.Update(coverPoint, coverPoint1, coverPoint2);
	}

	private void FindCoverPoints()
	{
		if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || !SingletonComponent<AiManager>.Instance.UseCover)
		{
			return;
		}
		if (this.AiContext.sampledCoverPoints.Count > 0)
		{
			this.AiContext.sampledCoverPoints.Clear();
		}
		if (this.AiContext.CurrentCoverVolume == null || !this.AiContext.CurrentCoverVolume.Contains(this.AiContext.Position))
		{
			this.AiContext.CurrentCoverVolume = SingletonComponent<AiManager>.Instance.GetCoverVolumeContaining(this.AiContext.Position);
		}
		if (this.AiContext.CurrentCoverVolume != null)
		{
			foreach (CoverPoint coverPoint in this.AiContext.CurrentCoverVolume.CoverPoints)
			{
				if (coverPoint.IsReserved)
				{
					continue;
				}
				Vector3 position = coverPoint.Position;
				if ((this.AiContext.Position - position).sqrMagnitude > this.MaxDistanceToCover * this.MaxDistanceToCover)
				{
					continue;
				}
				this.AiContext.sampledCoverPoints.Add(coverPoint);
			}
			if (this.AiContext.sampledCoverPoints.Count > 0)
			{
				this.AiContext.sampledCoverPoints.Sort(this.coverPointComparer);
			}
		}
	}

	public float GetActiveAggressionRangeSqr()
	{
		if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == 1)
		{
			return this.Stats.DeaggroRange * this.Stats.DeaggroRange;
		}
		return this.Stats.AggressionRange * this.Stats.AggressionRange;
	}

	public override Vector3 GetAimDirection()
	{
		Vector3 serverPosition;
		if (base.isMounted)
		{
			BaseMountable mounted = base.GetMounted();
			if (this.CurrentBehaviour != BaseNpc.Behaviour.Attack || !(this.AttackTarget != null))
			{
				return mounted.transform.forward;
			}
			Vector3 vector3 = Vector3.zero;
			BasePlayer attackTarget = this.AttackTarget as BasePlayer;
			if (attackTarget == null)
			{
				if (this.AttackTarget is BaseNpc)
				{
					vector3 = new Vector3(0f, -0.5f, 0f);
				}
			}
			else if (attackTarget.IsDucked())
			{
				vector3 = PlayerEyes.DuckOffset;
			}
			else if (attackTarget.IsSleeping())
			{
				vector3 = new Vector3(0f, -1f, 0f);
			}
			Vector3 vector31 = base.CenterPoint() + new Vector3(0f, 0f, 0f);
			Vector3 position = this.AttackTarget.CenterPoint();
			if (!this.AttackTarget.IsVisible(this.eyes.position, this.AttackTarget.CenterPoint(), Single.PositiveInfinity))
			{
				Memory.SeenInfo info = this.AiContext.Memory.GetInfo(this.AttackTarget);
				if (!(info.Entity != null) || (info.Position - this.ServerPosition).sqrMagnitude <= 4f)
				{
					return mounted.transform.forward;
				}
				position = info.Position;
			}
			return ((position + vector3) - vector31).normalized;
		}
		if (this.LookAtEyes != null && this.LookAtEyes.transform != null && (this.CurrentBehaviour == BaseNpc.Behaviour.Wander || this.CurrentBehaviour == BaseNpc.Behaviour.Idle))
		{
			Vector3 vector32 = base.CenterPoint();
			Vector3 lookAtEyes = (this.LookAtEyes.position + PlayerEyes.DuckOffset) - vector32;
			return lookAtEyes.normalized;
		}
		if (this.LookAtPoint != null && (this.CurrentBehaviour == BaseNpc.Behaviour.Wander || this.CurrentBehaviour == BaseNpc.Behaviour.Idle))
		{
			Vector3 vector33 = base.CenterPoint();
			return (this.LookAtPoint.position - vector33).normalized;
		}
		if (this._traversingNavMeshLink)
		{
			serverPosition = (this.AttackTarget == null ? this.NavAgent.destination - this.ServerPosition : this.AttackTarget.ServerPosition - this.ServerPosition);
			if (serverPosition.sqrMagnitude > 1f)
			{
				serverPosition = this._currentNavMeshLinkEndPos - this.ServerPosition;
			}
			if (serverPosition.sqrMagnitude > 0.001f)
			{
				return this._currentNavMeshLinkOrientation * Vector3.forward;
			}
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Wander || this.CurrentBehaviour == BaseNpc.Behaviour.RetreatingToCover)
		{
			if (!this.IsNavRunning() || this.NavAgent.desiredVelocity.sqrMagnitude <= 0.01f)
			{
				return base.transform.rotation * Vector3.forward;
			}
			return this.NavAgent.desiredVelocity.normalized;
		}
		if (this.CurrentBehaviour != BaseNpc.Behaviour.Attack || !(this.AttackTarget != null))
		{
			if (!this.IsNavRunning() || this.NavAgent.desiredVelocity.sqrMagnitude <= 0.01f)
			{
				return base.transform.rotation * Vector3.forward;
			}
			return this.NavAgent.desiredVelocity.normalized;
		}
		Vector3 duckOffset = Vector3.zero;
		BasePlayer basePlayer = this.AttackTarget as BasePlayer;
		if (basePlayer == null)
		{
			if (this.AttackTarget is BaseNpc)
			{
				duckOffset = new Vector3(0f, -0.5f, 0f);
			}
		}
		else if (basePlayer.IsDucked())
		{
			duckOffset = PlayerEyes.DuckOffset;
		}
		else if (basePlayer.IsSleeping())
		{
			duckOffset = new Vector3(0f, -1f, 0f);
		}
		Vector3 vector34 = base.CenterPoint() + new Vector3(0f, 0f, 0f);
		Vector3 vector35 = this.AttackTarget.CenterPoint();
		Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo(this.AttackTarget);
		if (!(extendedInfo.Entity == null) && extendedInfo.LineOfSight != 0)
		{
			return ((vector35 + duckOffset) - vector34).normalized;
		}
		if (!this.IsNavRunning() || this.NavAgent.desiredVelocity.sqrMagnitude <= 0.01f || this.IsMoving() <= 0)
		{
			return base.transform.rotation * Vector3.forward;
		}
		return this.NavAgent.desiredVelocity.normalized;
	}

	public virtual int GetAlliesInRange(out List<Scientist> allies)
	{
		allies = null;
		return 0;
	}

	public IAIContext GetContext(Guid aiId)
	{
		if (this.SelectPlayerTargetAI != null && aiId == this.SelectPlayerTargetAI.ai.id || this.SelectPlayerTargetMountedAI != null && aiId == this.SelectPlayerTargetMountedAI.ai.id)
		{
			return NPCPlayerApex.PlayerTargetContext;
		}
		if (this.SelectEntityTargetAI != null && aiId == this.SelectEntityTargetAI.ai.id)
		{
			return NPCPlayerApex.EntityTargetContext;
		}
		if (this.SelectCoverTargetsAI != null && aiId == this.SelectCoverTargetsAI.ai.id || this.SelectEnemyHideoutAI != null && aiId == this.SelectEnemyHideoutAI.ai.id)
		{
			return NPCPlayerApex.CoverContext;
		}
		return this.AiContext;
	}

	public NPCPlayerApex.AmmoStateEnum GetCurrentAmmoStateEnum()
	{
		AttackEntity heldEntity = base.GetHeldEntity() as AttackEntity;
		if (heldEntity == null)
		{
			return NPCPlayerApex.AmmoStateEnum.Empty;
		}
		BaseProjectile baseProjectile = heldEntity as BaseProjectile;
		if (!baseProjectile)
		{
			return NPCPlayerApex.AmmoStateEnum.Full;
		}
		if (baseProjectile.primaryMagazine.contents == 0)
		{
			return NPCPlayerApex.AmmoStateEnum.Empty;
		}
		float single = (float)baseProjectile.primaryMagazine.contents / (float)baseProjectile.primaryMagazine.capacity;
		if (single < 0.3f)
		{
			return NPCPlayerApex.AmmoStateEnum.Low;
		}
		if (single < 0.65f)
		{
			return NPCPlayerApex.AmmoStateEnum.Medium;
		}
		if (single < 1f)
		{
			return NPCPlayerApex.AmmoStateEnum.High;
		}
		return NPCPlayerApex.AmmoStateEnum.Full;
	}

	public NPCPlayerApex.ToolTypeEnum GetCurrentToolTypeEnum()
	{
		HeldEntity heldEntity = base.GetHeldEntity();
		if (heldEntity == null)
		{
			return NPCPlayerApex.ToolTypeEnum.None;
		}
		return heldEntity.toolType;
	}

	public NPCPlayerApex.WeaponTypeEnum GetCurrentWeaponTypeEnum()
	{
		HeldEntity heldEntity = base.GetHeldEntity();
		if (heldEntity == null)
		{
			return NPCPlayerApex.WeaponTypeEnum.None;
		}
		AttackEntity attackEntity = heldEntity as AttackEntity;
		if (attackEntity == null)
		{
			return NPCPlayerApex.WeaponTypeEnum.None;
		}
		return attackEntity.effectiveRangeType;
	}

	public byte GetFact(BaseNpc.Facts fact)
	{
		return (byte)0;
	}

	public byte GetFact(NPCPlayerApex.Facts fact)
	{
		return this.CurrentFacts[(int)fact];
	}

	private Transform GetLookatPoint(ref Transform[] points)
	{
		this.LookAtEyes = null;
		if (points == null || points.Length == 0)
		{
			return null;
		}
		return points[UnityEngine.Random.Range(0, (int)points.Length)];
	}

	public Transform GetLookatPointFromWaypoints()
	{
		this.LookAtEyes = null;
		if (this.WaypointSet == null || this.WaypointSet.Points.Count == 0)
		{
			return null;
		}
		Rust.Ai.WaypointSet.Waypoint item = this.WaypointSet.Points[this.CurrentWaypointIndex];
		if (item.LookatPoints == null || item.LookatPoints.Length == 0)
		{
			return null;
		}
		return item.LookatPoints[UnityEngine.Random.Range(0, (int)item.LookatPoints.Length)];
	}

	public int GetNextWaypointIndex()
	{
		if (this.WaypointSet == null || this.WaypointSet.Points.Count == 0 || this.WaypointSet.Points[this.PeekNextWaypointIndex()].IsOccupied)
		{
			return this.CurrentWaypointIndex;
		}
		int currentWaypointIndex = this.CurrentWaypointIndex;
		if (currentWaypointIndex >= 0 && currentWaypointIndex < this.WaypointSet.Points.Count)
		{
			Rust.Ai.WaypointSet.Waypoint item = this.WaypointSet.Points[currentWaypointIndex];
			item.IsOccupied = false;
			this.WaypointSet.Points[currentWaypointIndex] = item;
		}
		Rust.Ai.WaypointSet.NavModes navMode = this.WaypointSet.NavMode;
		if (navMode == Rust.Ai.WaypointSet.NavModes.Loop)
		{
			currentWaypointIndex++;
			if (currentWaypointIndex >= this.WaypointSet.Points.Count)
			{
				currentWaypointIndex = 0;
			}
			else if (currentWaypointIndex < 0)
			{
				currentWaypointIndex = this.WaypointSet.Points.Count - 1;
			}
		}
		else
		{
			if (navMode != Rust.Ai.WaypointSet.NavModes.PingPong)
			{
				throw new ArgumentOutOfRangeException();
			}
			currentWaypointIndex += this.WaypointDirection;
			if (currentWaypointIndex >= this.WaypointSet.Points.Count)
			{
				currentWaypointIndex = this.CurrentWaypointIndex - 1;
				this.WaypointDirection = -1;
			}
			else if (currentWaypointIndex < 0)
			{
				currentWaypointIndex = 0;
				this.WaypointDirection = 1;
			}
		}
		if (currentWaypointIndex >= 0 && currentWaypointIndex < this.WaypointSet.Points.Count)
		{
			Rust.Ai.WaypointSet.Waypoint waypoint = this.WaypointSet.Points[currentWaypointIndex];
			waypoint.IsOccupied = true;
			this.WaypointSet.Points[currentWaypointIndex] = waypoint;
		}
		return currentWaypointIndex;
	}

	public byte GetPathStatus()
	{
		if (!this.IsNavRunning())
		{
			return (byte)2;
		}
		return (byte)this.NavAgent.pathStatus;
	}

	private NPCPlayerApex.EnemyRangeEnum GetRangeToSpawnPoint()
	{
		float sqrRange = this.ToSqrRange(this.Stats.MaxRangeToSpawnLoc) * 2f;
		float serverPosition = (this.ServerPosition - this.SpawnPosition).sqrMagnitude;
		if (serverPosition > sqrRange)
		{
			return NPCPlayerApex.EnemyRangeEnum.OutOfRange;
		}
		return this.ToEnemyRangeEnum(serverPosition);
	}

	public float GetWantsToAttack(BaseEntity target)
	{
		if (target == null)
		{
			return 0f;
		}
		object obj = Interface.CallHook("IOnNpcPlayerTarget", this, target);
		if (obj is float)
		{
			return (float)obj;
		}
		if (!target.HasAnyTrait(BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Human))
		{
			return 0f;
		}
		if (target.GetType() == base.GetType())
		{
			return 0f;
		}
		if (target.Health() <= 0f)
		{
			return 0f;
		}
		return 1f;
	}

	public NPCPlayerApex.WeaponTypeEnum GetWeaponTypeEnum(BaseProjectile proj)
	{
		if (!proj)
		{
			return NPCPlayerApex.WeaponTypeEnum.None;
		}
		return proj.effectiveRangeType;
	}

	public bool HostilityConsideration(BasePlayer target)
	{
		if (target == null || target.transform == null || target.IsDestroyed || target.IsDead())
		{
			return true;
		}
		if (this.Stats.OnlyAggroMarkedTargets && target.HasPlayerFlag(BasePlayer.PlayerFlags.SafeZone))
		{
			if (target.IsSleeping() && target.secondsSleeping >= NPCAutoTurret.sleeperhostiledelay)
			{
				return true;
			}
			return target.IsHostile();
		}
		if (this.Stats.Hostility > 0f)
		{
			return true;
		}
		if (this.Stats.Defensiveness > 0f && this.AiContext.LastAttacker == target && this.Stats.AttackedMemoryTime > base.SecondsSinceAttacked)
		{
			return true;
		}
		if (this.AiContext.AiLocationManager != null && this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.BanditTown)
		{
			if (target.IsHostile())
			{
				return true;
			}
			if (target.IsSleeping() && target.secondsSleeping >= NPCAutoTurret.sleeperhostiledelay)
			{
				return true;
			}
		}
		return false;
	}

	public override void Hurt(HitInfo info)
	{
		Memory.ExtendedInfo extendedInfo;
		if (this.IsInvinsible)
		{
			return;
		}
		if (AI.npc_families_no_hurt)
		{
			NPCPlayerApex initiator = info.Initiator as NPCPlayerApex;
			if (initiator != null && initiator.Family == this.Family)
			{
				return;
			}
		}
		base.Hurt(info);
		if (info.Initiator != null && this.AiContext != null)
		{
			float single = info.damageTypes.Total();
			if (info.InitiatorPlayer != null && this.AiContext.EnemyPlayer == null)
			{
				this.AiContext.EnemyPlayer = info.InitiatorPlayer;
			}
			else if (info.Initiator is BaseNpc)
			{
				this.AiContext.EnemyNpc = (BaseNpc)info.Initiator;
			}
			this.UpdateTargetMemory(info.Initiator, single, out extendedInfo);
			this.AiContext.LastAttacker = info.Initiator;
			if (this.AiContext.CoverSet.Closest.ReservedCoverPoint != null && this.GetFact(NPCPlayerApex.Facts.IsInCover) > 0)
			{
				this.AiContext.CoverSet.Closest.ReservedCoverPoint.CoverIsCompromised(AI.npc_cover_compromised_cooldown);
			}
			if (this.TryAggro(extendedInfo.DistanceSqr) && this.AiContext.EnemyPlayer != null)
			{
				this.SetAttackTarget(this.AiContext.EnemyPlayer, 1f, extendedInfo.DistanceSqr, (extendedInfo.LineOfSight & 1) != 0, (extendedInfo.LineOfSight & 2) != 0, false);
			}
		}
	}

	public void InitFacts()
	{
		this.SetFact(NPCPlayerApex.Facts.CanTargetEnemies, 1, true, true);
	}

	public bool IsBeyondDeaggroRange(NPCPlayerApex.EnemyRangeEnum range)
	{
		return this.ToEnemyEngagementRangeEnum(this.ToSqrRange(range)) == NPCPlayerApex.EnemyEngagementRangeEnum.DeaggroRange;
	}

	public bool IsInCommunicationRange(NPCPlayerApex npc)
	{
		if (!(npc != null) || npc.IsDestroyed || !(npc.transform != null) || npc.Health() <= 0f)
		{
			return false;
		}
		Vector3 serverPosition = npc.ServerPosition - this.ServerPosition;
		return serverPosition.sqrMagnitude <= this.CommunicationRadius * this.CommunicationRadius;
	}

	private byte IsMoving()
	{
		object obj;
		if (!this.IsNavRunning() || !this.NavAgent.hasPath || this.NavAgent.remainingDistance <= this.NavAgent.stoppingDistance || this.IsStuck || this.IsStopped)
		{
			obj = null;
		}
		else
		{
			obj = 1;
		}
		return (byte)obj;
	}

	public override bool IsNavRunning()
	{
		if (!base.isServer || AiManager.nav_disable || base.isMounted || !(this.GetNavAgent != null) || !this.GetNavAgent.enabled)
		{
			return false;
		}
		return this.GetNavAgent.isOnNavMesh;
	}

	public bool IsVisibleCrouched(BasePlayer player)
	{
		Vector3 vector3 = this.eyes.worldCrouchedPosition;
		if (!player.IsVisible(vector3, player.CenterPoint(), Single.PositiveInfinity) && !player.IsVisible(vector3, player.transform.position, Single.PositiveInfinity) && !player.IsVisible(vector3, player.eyes.position, Single.PositiveInfinity))
		{
			return false;
		}
		if (!base.IsVisible(player.CenterPoint(), vector3, Single.PositiveInfinity) && !base.IsVisible(player.transform.position, vector3, Single.PositiveInfinity) && !base.IsVisible(player.eyes.position, vector3, Single.PositiveInfinity))
		{
			return false;
		}
		return true;
	}

	public bool IsVisibleCrouched(BaseNpc npc)
	{
		Vector3 eyeOffset = this.eyes.transform.position + (this.eyes.transform.up * (PlayerEyes.EyeOffset.y + PlayerEyes.DuckOffset.y));
		if (!npc.IsVisible(eyeOffset, npc.CenterPoint(), Single.PositiveInfinity))
		{
			return false;
		}
		if (!base.IsVisible(npc.CenterPoint(), eyeOffset, Single.PositiveInfinity))
		{
			return false;
		}
		return true;
	}

	public bool IsVisibleMounted(BasePlayer player)
	{
		Vector3 vector3 = this.eyes.worldMountedPosition;
		if (!player.IsVisible(vector3, player.CenterPoint(), Single.PositiveInfinity) && !player.IsVisible(vector3, player.transform.position, Single.PositiveInfinity) && !player.IsVisible(vector3, player.eyes.position, Single.PositiveInfinity))
		{
			return false;
		}
		if (!base.IsVisible(player.CenterPoint(), vector3, Single.PositiveInfinity) && !base.IsVisible(player.transform.position, vector3, Single.PositiveInfinity) && !base.IsVisible(player.eyes.position, vector3, Single.PositiveInfinity))
		{
			return false;
		}
		return true;
	}

	public bool IsVisibleStanding(BasePlayer player)
	{
		Vector3 vector3 = this.eyes.worldStandingPosition;
		if (!player.IsVisible(vector3, player.CenterPoint(), Single.PositiveInfinity) && !player.IsVisible(vector3, player.transform.position, Single.PositiveInfinity) && !player.IsVisible(vector3, player.eyes.position, Single.PositiveInfinity))
		{
			return false;
		}
		if (!base.IsVisible(player.CenterPoint(), vector3, Single.PositiveInfinity) && !base.IsVisible(player.transform.position, vector3, Single.PositiveInfinity) && !base.IsVisible(player.eyes.position, vector3, Single.PositiveInfinity))
		{
			return false;
		}
		return true;
	}

	public bool IsVisibleStanding(BaseNpc npc)
	{
		Vector3 eyeOffset = this.eyes.transform.position + (this.eyes.transform.up * PlayerEyes.EyeOffset.y);
		if (!npc.IsVisible(eyeOffset, npc.CenterPoint(), Single.PositiveInfinity))
		{
			return false;
		}
		if (!base.IsVisible(npc.CenterPoint(), eyeOffset, Single.PositiveInfinity))
		{
			return false;
		}
		return true;
	}

	public bool IsWithinAggroRange(NPCPlayerApex.EnemyRangeEnum range)
	{
		NPCPlayerApex.EnemyEngagementRangeEnum enemyEngagementRangeEnum = this.ToEnemyEngagementRangeEnum(this.ToSqrRange(range));
		if (enemyEngagementRangeEnum == NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange)
		{
			return true;
		}
		if (this.GetFact(NPCPlayerApex.Facts.IsAggro) != 1)
		{
			return false;
		}
		return enemyEngagementRangeEnum == NPCPlayerApex.EnemyEngagementRangeEnum.NeutralRange;
	}

	public bool IsWithinAggroRange(float sqrRange)
	{
		NPCPlayerApex.EnemyEngagementRangeEnum enemyEngagementRangeEnum = this.ToEnemyEngagementRangeEnum(sqrRange);
		if (enemyEngagementRangeEnum == NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange)
		{
			return true;
		}
		if (this.GetFact(NPCPlayerApex.Facts.IsAggro) != 1)
		{
			return false;
		}
		return enemyEngagementRangeEnum == NPCPlayerApex.EnemyEngagementRangeEnum.NeutralRange;
	}

	public void LookAtRandomPoint(float nextTimeAddition = 5f)
	{
		if (UnityEngine.Time.realtimeSinceStartup > this.nextLookAtPointTime)
		{
			this.LookAtEyes = null;
			this.nextLookAtPointTime = UnityEngine.Time.realtimeSinceStartup + nextTimeAddition;
			this.LookAtPoint = this.GetLookatPointFromWaypoints();
			if (this.LookAtPoint == null && this.LookAtInterestPointsStationary != null)
			{
				this.LookAtPoint = this.GetLookatPoint(ref this.LookAtInterestPointsStationary);
			}
		}
	}

	public void Mount(BaseMountable mountable)
	{
		if (mountable.GetMounted() == null)
		{
			mountable.AttemptMount(this);
			mountable = base.GetMounted();
			if (mountable)
			{
				this.NavAgent.enabled = false;
				this.SetFact(NPCPlayerApex.Facts.IsMounted, 1, true, true);
				if (!mountable.canWieldItems)
				{
					this.SetFact(NPCPlayerApex.Facts.CanNotWieldWeapon, 1, true, true);
				}
				base.CancelInvoke(new Action(this.RadioChatter));
			}
		}
	}

	public override void MovementUpdate(float delta)
	{
		int num;
		BaseMountable mounted = base.GetMounted();
		this.modelState.mounted = mounted != null;
		ModelState modelState = this.modelState;
		if (this.modelState.mounted)
		{
			num = (int)mounted.mountPose;
		}
		else
		{
			num = 0;
		}
		modelState.poseType = num;
		if (!AI.move)
		{
			return;
		}
		if (!base.isMounted && !this.IsNavRunning())
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			this.StopMoving();
			return;
		}
		base.MovementUpdate(delta);
		if (base.isMounted)
		{
			this.timeAtDestination += delta;
		}
		else if ((!this.IsNavRunning() || this.NavAgent.hasPath) && Vector3Ex.Distance2D(this.NavAgent.destination, base.GetPosition()) >= 1f)
		{
			this.timeAtDestination = 0f;
		}
		else
		{
			this.timeAtDestination += delta;
		}
		this.modelState.aiming = (this.timeAtDestination <= 0.25f || !(this.AttackTarget != null) || this.GetFact(NPCPlayerApex.Facts.HasLineOfSight) <= 0 ? false : this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) == 0);
		this.TickStuck(delta);
	}

	private float NextAttackTime()
	{
		AttackEntity heldEntity = base.GetHeldEntity() as AttackEntity;
		if (heldEntity == null)
		{
			return Single.PositiveInfinity;
		}
		return heldEntity.NextAttackTime;
	}

	public AiAnswer_ShareEnemyTarget OnAiQuestion(NPCPlayerApex source, AiQuestion_ShareEnemyTarget question)
	{
		AiLocationSpawner.SquadSpawnerLocation? nullable;
		BasePlayer enemyPlayer;
		UnityEngine.Object obj;
		AiLocationSpawner.SquadSpawnerLocation? nullable1;
		AiAnswer_ShareEnemyTarget aiAnswerShareEnemyTarget = new AiAnswer_ShareEnemyTarget()
		{
			Source = this
		};
		NPCHumanContext aiContext = this.AiContext;
		if (aiContext != null)
		{
			enemyPlayer = aiContext.EnemyPlayer;
		}
		else
		{
			enemyPlayer = null;
		}
		aiAnswerShareEnemyTarget.PlayerTarget = enemyPlayer;
		AiAnswer_ShareEnemyTarget aiAnswerShareEnemyTarget1 = aiAnswerShareEnemyTarget;
		NPCHumanContext nPCHumanContext = this.AiContext;
		if (nPCHumanContext != null)
		{
			obj = nPCHumanContext.EnemyPlayer;
		}
		else
		{
			obj = null;
		}
		if (obj != null)
		{
			Memory.SeenInfo info = this.AiContext.Memory.GetInfo(this.AiContext.EnemyPlayer);
			if (!(info.Entity != null) || info.Entity.IsDestroyed || this.AiContext.EnemyPlayer.IsDead())
			{
				aiAnswerShareEnemyTarget1.PlayerTarget = null;
			}
			else
			{
				aiAnswerShareEnemyTarget1.LastKnownPosition = new Vector3?(info.Position);
				if (source != null)
				{
					NPCHumanContext aiContext1 = source.AiContext;
					if (aiContext1 != null)
					{
						AiLocationManager aiLocationManager = aiContext1.AiLocationManager;
						if (aiLocationManager != null)
						{
							nullable1 = new AiLocationSpawner.SquadSpawnerLocation?(aiLocationManager.LocationType);
						}
						else
						{
							nullable = null;
							nullable1 = nullable;
						}
					}
					else
					{
						nullable = null;
						nullable1 = nullable;
					}
					AiLocationSpawner.SquadSpawnerLocation? nullable2 = nullable1;
					if (nullable2.GetValueOrDefault() == AiLocationSpawner.SquadSpawnerLocation.BanditTown & nullable2.HasValue)
					{
						source.AiContext.LastAttacker = aiAnswerShareEnemyTarget1.PlayerTarget;
						source.lastAttackedTime = this.lastAttackedTime;
					}
				}
			}
		}
		return aiAnswerShareEnemyTarget1;
	}

	public virtual void OnAiStatement(NPCPlayerApex source, AiStatement_EnemyEngaged statement)
	{
	}

	public virtual void OnAiStatement(NPCPlayerApex source, AiStatement_EnemySeen statement)
	{
	}

	public void OnBecomeStuck()
	{
		this.IsStuck = true;
	}

	public void OnBecomeUnStuck()
	{
		this.IsStuck = false;
	}

	private void OnFactChanged(NPCPlayerApex.Facts fact, byte oldValue, byte newValue)
	{
		if (fact <= NPCPlayerApex.Facts.IsAggro)
		{
			if (fact > NPCPlayerApex.Facts.CanTargetEnemies)
			{
				if (fact == NPCPlayerApex.Facts.Speed)
				{
					NPCPlayerApex.SpeedEnum speedEnum = (NPCPlayerApex.SpeedEnum)newValue;
					if (speedEnum == NPCPlayerApex.SpeedEnum.StandStill)
					{
						this.StopMoving();
						if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == 0 && this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) == 0)
						{
							this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
							if (newValue != oldValue)
							{
								base.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
								return;
							}
						}
					}
					else if (speedEnum == NPCPlayerApex.SpeedEnum.Walk)
					{
						this.IsStopped = false;
						if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == 0 && this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) == 0)
						{
							this.CurrentBehaviour = BaseNpc.Behaviour.Wander;
							if (newValue != oldValue)
							{
								base.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
								return;
							}
						}
					}
					else
					{
						this.IsStopped = false;
						if (this.GetFact(NPCPlayerApex.Facts.IsAggro) > 0)
						{
							if (newValue != oldValue)
							{
								base.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
								return;
							}
						}
						else if (newValue != oldValue)
						{
							base.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
							return;
						}
					}
				}
				else
				{
					if (fact != NPCPlayerApex.Facts.IsAggro)
					{
						return;
					}
					if (newValue <= 0 || this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) != 0)
					{
						base.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
						this.SetFact(NPCPlayerApex.Facts.Speed, 0, true, true);
						return;
					}
					this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
					if (newValue != oldValue)
					{
						base.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
						return;
					}
				}
			}
			else if (fact != NPCPlayerApex.Facts.HasEnemy)
			{
				if (fact != NPCPlayerApex.Facts.CanTargetEnemies)
				{
					return;
				}
				if (newValue == 1)
				{
					this.blockTargetingThisEnemy = null;
					return;
				}
			}
			else if (newValue == 1)
			{
				this.LastHasEnemyTime = UnityEngine.Time.time;
				if (this.GetFact(NPCPlayerApex.Facts.HasLineOfSight) > 0)
				{
					this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
				}
			}
		}
		else if (fact <= NPCPlayerApex.Facts.BodyState)
		{
			if (fact != NPCPlayerApex.Facts.IsMoving)
			{
				return;
			}
			if (newValue == 1)
			{
				this.TimeLastMoved = UnityEngine.Time.realtimeSinceStartup;
				return;
			}
		}
		else if (fact != NPCPlayerApex.Facts.IsRetreatingToCover)
		{
			if (fact != NPCPlayerApex.Facts.IsMounted)
			{
				if (fact != NPCPlayerApex.Facts.IsSearchingForEnemy)
				{
					return;
				}
				if (newValue > 0)
				{
					this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
					return;
				}
			}
		}
		else if (newValue == 1)
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.RetreatingToCover;
			if (newValue != oldValue)
			{
				base.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
				return;
			}
		}
		else if (this.GetFact(NPCPlayerApex.Facts.IsAggro) <= 0)
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
			if (newValue != oldValue)
			{
				base.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
				return;
			}
		}
		else
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
			if (newValue != oldValue)
			{
				base.SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
				return;
			}
		}
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (this.OnDeath != null)
		{
			this.OnDeath();
		}
		if (this.NewAI)
		{
			if (this.AiContext != null && this.AiContext.AiLocationManager != null)
			{
				if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
				{
					NPCPlayerApex.AllJunkpileNPCs.Remove(this);
				}
				else if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.BanditTown)
				{
					NPCPlayerApex.AllBanditCampNPCs.Remove(this);
				}
			}
			NPCSensesLoadBalancer.NpcSensesLoadBalancer.Remove(this);
			this.ShutdownSensorySystem();
		}
		base.CancelInvoke(new Action(this.RadioChatter));
		if (this.DeathEffect.isValid)
		{
			Effect.server.Run(this.DeathEffect.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
		}
	}

	public override void OnSensation(Sensation sensation)
	{
		if (this.AiContext == null || this is NPCMurderer)
		{
			return;
		}
		BasePlayer initiatorPlayer = sensation.InitiatorPlayer;
		if (this.OnlyTargetSensations && (initiatorPlayer == null || initiatorPlayer != this.AiContext.EnemyPlayer))
		{
			return;
		}
		SensationType type = sensation.Type;
		if (type == SensationType.Gunshot)
		{
			if (sensation.DamagePotential <= 0f)
			{
				this.OnSenseItemOfInterest(sensation);
				return;
			}
			this.OnSenseGunshot(sensation, initiatorPlayer);
			return;
		}
		if (type != SensationType.ThrownWeapon)
		{
			return;
		}
		if (sensation.DamagePotential <= 0f)
		{
			this.OnSenseItemOfInterest(sensation);
			return;
		}
		this.OnSenseThrownThreat(sensation, initiatorPlayer);
	}

	protected virtual void OnSenseGunshot(Sensation sensation, BasePlayer invoker)
	{
		Memory.ExtendedInfo extendedInfo;
		Memory.ExtendedInfo extendedInfo1;
		if (this.AiContext.Memory.GetInfo(sensation.Position).Entity != null)
		{
			this.UpdateTargetMemory(invoker, 1f, sensation.Position, out extendedInfo1);
		}
		else if (invoker == null)
		{
			this.AiContext.Memory.AddDanger(sensation.Position, 1f);
		}
		else
		{
			this.UpdateTargetMemory(invoker, 1f, sensation.Position, out extendedInfo);
		}
		this._lastHeardGunshotTime = UnityEngine.Time.time;
		Vector3 position = sensation.Position - base.transform.localPosition;
		this.LastHeardGunshotDirection = position.normalized;
	}

	protected virtual void OnSenseItemOfInterest(Sensation sensation)
	{
		if (sensation.InitiatorPlayer != null && this.AiContext.AiLocationManager != null && this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.BanditTown && this.Family != sensation.InitiatorPlayer.Family && base.InSafeZone())
		{
			sensation.InitiatorPlayer.MarkHostileFor(30f);
		}
	}

	protected virtual void OnSenseThrownThreat(Sensation sensation, BasePlayer invoker)
	{
		Memory.ExtendedInfo extendedInfo;
		Memory.ExtendedInfo extendedInfo1;
		if (this.AiContext.Memory.GetInfo(sensation.Position).Entity != null)
		{
			this.UpdateTargetMemory(invoker, 1f, sensation.Position, out extendedInfo1);
		}
		else if (invoker == null)
		{
			this.AiContext.Memory.AddDanger(sensation.Position, 1f);
		}
		else
		{
			this.UpdateTargetMemory(invoker, 1f, sensation.Position, out extendedInfo);
		}
		this._lastHeardGunshotTime = UnityEngine.Time.time;
		Vector3 position = sensation.Position - base.transform.localPosition;
		this.LastHeardGunshotDirection = position.normalized;
		if (invoker != null && this.AiContext.AiLocationManager != null && this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.BanditTown && this.Family != invoker.Family && base.InSafeZone())
		{
			invoker.MarkHostileFor(30f);
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
		if (single > this.Stats.MediumRange * this.Stats.MediumRange || !allowCloseRange && single < this.Stats.CloseRange * this.Stats.CloseRange)
		{
			return true;
		}
		float single1 = Mathf.Sqrt(single);
		if (NPCPlayerApex._pathCache == null)
		{
			NPCPlayerApex._pathCache = new NavMeshPath();
		}
		if (NavMesh.CalculatePath(from, to, this.GetNavAgent.areaMask, NPCPlayerApex._pathCache))
		{
			int cornersNonAlloc = NPCPlayerApex._pathCache.GetCornersNonAlloc(NPCPlayerApex.pathCornerCache);
			if (NPCPlayerApex._pathCache.status == NavMeshPathStatus.PathComplete && cornersNonAlloc > 1 && Mathf.Abs(single1 - this.PathDistance(cornersNonAlloc, ref NPCPlayerApex.pathCornerCache, single1 + AI.npc_cover_path_vs_straight_dist_max_diff)) > AI.npc_cover_path_vs_straight_dist_max_diff)
			{
				return false;
			}
		}
		return true;
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
		base.CancelInvoke(new Action(this.RadioChatter));
	}

	public int PeekNextWaypointIndex()
	{
		if (this.WaypointSet == null || this.WaypointSet.Points.Count == 0)
		{
			return this.CurrentWaypointIndex;
		}
		int currentWaypointIndex = this.CurrentWaypointIndex;
		Rust.Ai.WaypointSet.NavModes navMode = this.WaypointSet.NavMode;
		if (navMode == Rust.Ai.WaypointSet.NavModes.Loop)
		{
			currentWaypointIndex++;
			if (currentWaypointIndex >= this.WaypointSet.Points.Count)
			{
				currentWaypointIndex = 0;
			}
			else if (currentWaypointIndex < 0)
			{
				currentWaypointIndex = this.WaypointSet.Points.Count - 1;
			}
		}
		else
		{
			if (navMode != Rust.Ai.WaypointSet.NavModes.PingPong)
			{
				throw new ArgumentOutOfRangeException();
			}
			currentWaypointIndex += this.WaypointDirection;
			if (currentWaypointIndex >= this.WaypointSet.Points.Count)
			{
				currentWaypointIndex = this.CurrentWaypointIndex - 1;
			}
			else if (currentWaypointIndex < 0)
			{
				currentWaypointIndex = 0;
			}
		}
		return currentWaypointIndex;
	}

	public void RadioChatter()
	{
		if (base.IsDestroyed || base.transform == null)
		{
			base.CancelInvoke(new Action(this.RadioChatter));
			return;
		}
		if (this.RadioEffect.isValid)
		{
			Effect.server.Run(this.RadioEffect.resourcePath, this, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
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

	public override void Resume()
	{
		if (base.isMounted)
		{
			if (this.utilityAiComponent == null)
			{
				this.utilityAiComponent = this.Entity.GetComponent<UtilityAIComponent>();
			}
			if (this.utilityAiComponent != null)
			{
				this.utilityAiComponent.enabled = true;
				this.utilityAiComponent.Resume();
			}
			base.SendNetworkUpdateImmediate(false);
			return;
		}
		if (!this.GetNavAgent.isOnNavMesh)
		{
			if (Interface.CallHook("OnNpcPlayerResume", this) != null)
			{
				return;
			}
			base.StartCoroutine(this.TryForceToNavmesh());
			return;
		}
		this.GetNavAgent.enabled = true;
		this.StoppingDistance = 1f;
		if (this.utilityAiComponent == null)
		{
			this.utilityAiComponent = this.Entity.GetComponent<UtilityAIComponent>();
		}
		if (this.utilityAiComponent != null)
		{
			this.utilityAiComponent.enabled = true;
			this.utilityAiComponent.Resume();
		}
		base.InvokeRandomized(new Action(this.RadioChatter), this.RadioEffectRepeatRange.x, this.RadioEffectRepeatRange.x, this.RadioEffectRepeatRange.y - this.RadioEffectRepeatRange.x);
	}

	private void SelectEnemy()
	{
		if (this.AiContext.Players.Count != 0 || this.AiContext.Npcs.Count != 0)
		{
			if (base.isMounted)
			{
				this.AggroClosestPlayerMounted();
				return;
			}
			this.AggroBestScorePlayerOrClosestAnimal();
			return;
		}
		this.AiContext.EnemyNpc = null;
		this.AiContext.EnemyPlayer = null;
		this.SetFact(NPCPlayerApex.Facts.HasEnemy, 0, true, true);
		this.SetFact(NPCPlayerApex.Facts.EnemyRange, 3, true, true);
		this.SetFact(NPCPlayerApex.Facts.IsAggro, 0, false, true);
	}

	private void SelectMountable()
	{
		if (this.AiContext.Chairs.Count != 0 || base.isMounted)
		{
			this.TargetClosestChair();
			return;
		}
		this.AiContext.ChairTarget = null;
		this.SetFact(NPCPlayerApex.Facts.IsMounted, 0, true, true);
	}

	public virtual void SendStatement(AiStatement_EnemyEngaged statement)
	{
	}

	public virtual void SendStatement(AiStatement_EnemySeen statement)
	{
	}

	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		base.ServerInit();
		this.SpawnPosition = base.GetPosition();
		if (this.SpawnPosition.sqrMagnitude < 0.01f)
		{
			base.Invoke(new Action(this.DelayedSpawnPosition), 1f);
		}
		this.IsStuck = false;
		if (this.NewAI)
		{
			this.InitFacts();
			this.CurrentWaypointIndex = 0;
			this.IsWaitingAtWaypoint = false;
			this.WaypointDirection = 1;
			this.fleeHealthThresholdPercentage = this.Stats.HealthThresholdForFleeing;
			this.coverPointComparer = new NPCPlayerApex.CoverPointComparer(this);
			SwitchWeaponOperator.TrySwitchWeaponTo(this.AiContext, NPCPlayerApex.WeaponTypeEnum.MediumRange);
			this.DelayedReloadOnInit();
			NPCSensesLoadBalancer.NpcSensesLoadBalancer.Add(this);
			this.lastInvinsibleStartTime = UnityEngine.Time.time;
			if (this.AiContext.AiLocationManager != null)
			{
				if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
				{
					NPCPlayerApex.AllJunkpileNPCs.Add(this);
					return;
				}
				if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.BanditTown)
				{
					NPCPlayerApex.AllBanditCampNPCs.Add(this);
				}
			}
			else
			{
				float single = Single.PositiveInfinity;
				AiLocationManager aiLocationManager = null;
				if (AiLocationManager.Managers != null && AiLocationManager.Managers.Count > 0)
				{
					foreach (AiLocationManager manager in AiLocationManager.Managers)
					{
						float serverPosition = (manager.transform.position - this.ServerPosition).sqrMagnitude;
						if (serverPosition >= single)
						{
							continue;
						}
						single = serverPosition;
						aiLocationManager = manager;
					}
				}
				if (aiLocationManager != null && single <= this.Stats.DeaggroRange * this.Stats.DeaggroRange)
				{
					this.AiContext.AiLocationManager = aiLocationManager;
					if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
					{
						NPCPlayerApex.AllJunkpileNPCs.Add(this);
						return;
					}
					if (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.BanditTown)
					{
						NPCPlayerApex.AllBanditCampNPCs.Add(this);
						return;
					}
				}
			}
		}
	}

	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		this.currentBehaviorDuration = this.currentBehaviorDuration + delta;
		this.UpdateAttackTargetVisibility(delta);
		base.SetFlag(BaseEntity.Flags.Reserved3, (this.AttackTarget == null ? false : this.IsAlive()), false, true);
	}

	public override void SetAimDirection(Vector3 newAim)
	{
		Quaternion quaternion;
		if (newAim == Vector3.zero)
		{
			return;
		}
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity && this.AttackTarget && this.GetFact(NPCPlayerApex.Facts.HasLineOfSight) > 0 && this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
		{
			newAim = attackEntity.ModifyAIAim(newAim, 1f);
		}
		if (base.isMounted)
		{
			BaseMountable mounted = base.GetMounted();
			Vector3 vector3 = mounted.transform.eulerAngles;
			quaternion = Quaternion.LookRotation(newAim, mounted.transform.up);
			Quaternion quaternion1 = Quaternion.Euler(quaternion.eulerAngles);
			Quaternion quaternion2 = Quaternion.LookRotation(base.transform.InverseTransformDirection(quaternion1 * Vector3.forward), base.transform.up);
			Vector3 vector31 = quaternion2.eulerAngles;
			vector31 = BaseMountable.ConvertVector(vector31);
			Quaternion quaternion3 = Quaternion.Euler(Mathf.Clamp(vector31.x, mounted.pitchClamp.x, mounted.pitchClamp.y), Mathf.Clamp(vector31.y, mounted.yawClamp.x, mounted.yawClamp.y), vector3.z);
			Quaternion quaternion4 = Quaternion.LookRotation(base.transform.TransformDirection(quaternion3 * Vector3.forward), base.transform.up);
			newAim = BaseMountable.ConvertVector(quaternion4.eulerAngles);
		}
		this.eyes.rotation = (base.isMounted ? Quaternion.Slerp(this.eyes.rotation, Quaternion.Euler(newAim), UnityEngine.Time.smoothDeltaTime * 70f) : Quaternion.LookRotation(newAim, base.transform.up));
		quaternion = this.eyes.rotation;
		this.viewAngles = quaternion.eulerAngles;
		this.ServerRotation = this.eyes.rotation;
	}

	protected void SetAttackTarget(BasePlayer player, float score, float sqrDistance, bool lineOfSightStanding, bool lineOfSightCrouched, bool tryAggro = true)
	{
		object obj;
		object obj1;
		object obj2;
		if (!(player != null) || player.IsDestroyed || player.IsDead())
		{
			this.SetFact(NPCPlayerApex.Facts.HasEnemy, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.EnemyRange, 3, true, true);
			this.SetFact(NPCPlayerApex.Facts.AfraidRange, 1, true, true);
			this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, 0, true, true);
		}
		else
		{
			this.AiContext.EnemyPlayer = player;
			this.AiContext.EnemyNpc = null;
			this.AiContext.LastTargetScore = score;
			this.SetFact(NPCPlayerApex.Facts.HasEnemy, 1, true, false);
			this.AiContext.AIAgent.AttackTarget = player;
			NPCPlayerApex.EnemyRangeEnum enemyRangeEnum = this.ToEnemyRangeEnum(sqrDistance);
			NPCPlayerApex.AfraidRangeEnum afraidRangeEnum = this.ToAfraidRangeEnum(sqrDistance);
			this.SetFact(NPCPlayerApex.Facts.EnemyRange, (byte)enemyRangeEnum, true, true);
			this.SetFact(NPCPlayerApex.Facts.AfraidRange, (byte)afraidRangeEnum, true, true);
			bool flag = lineOfSightStanding | lineOfSightCrouched;
			if (flag)
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, (byte)obj, true, true);
			if (lineOfSightCrouched)
			{
				obj1 = 1;
			}
			else
			{
				obj1 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.HasLineOfSightCrouched, (byte)obj1, true, true);
			if (lineOfSightStanding)
			{
				obj2 = 1;
			}
			else
			{
				obj2 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.HasLineOfSightStanding, (byte)obj2, true, true);
			if (flag)
			{
				this.lastSeenPlayerTime = UnityEngine.Time.time;
			}
			if (tryAggro)
			{
				this.TryAggro(enemyRangeEnum);
				return;
			}
		}
	}

	public void SetBusyFor(float dur)
	{
		this.BusyTimer.Activate(dur, null);
	}

	public override void SetDestination(Vector3 newDestination)
	{
		if (Interface.CallHook("OnNpcDestinationSet", this, newDestination) != null)
		{
			return;
		}
		base.SetDestination(newDestination);
		this.Destination = newDestination;
	}

	public void SetFact(BaseNpc.Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true)
	{
	}

	public void SetFact(NPCPlayerApex.Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true)
	{
		byte currentFacts = this.CurrentFacts[(int)fact];
		this.CurrentFacts[(int)fact] = value;
		if (triggerCallback && (!onlyTriggerCallbackOnDiffValue || value != currentFacts))
		{
			this.OnFactChanged(fact, currentFacts, value);
		}
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
		this.SetFact(NPCPlayerApex.Facts.PathToTargetStatus, this.GetPathStatus(), true, true);
	}

	protected virtual void SetupAiContext()
	{
		this._aiContext = new NPCHumanContext(this);
	}

	public override bool ShotTest()
	{
		if (!base.ShotTest())
		{
			return false;
		}
		this.lastInvinsibleStartTime = 0f;
		return true;
	}

	private void ShutdownSensorySystem()
	{
		NPCPlayerApex._selectPlayerTargetAI.Kill();
		NPCPlayerApex._selectPlayerTargetMountedAI.Kill();
		NPCPlayerApex._selectEntityTargetAI.Kill();
		NPCPlayerApex._selectCoverTargetsAI.Kill();
		NPCPlayerApex._selectEnemyHideoutAI.Kill();
	}

	public bool StartAggro(float timeout, bool broadcastEvent = true)
	{
		if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == 1)
		{
			this.wasAggro = true;
			return false;
		}
		this.SetFact(NPCPlayerApex.Facts.IsAggro, 1, true, true);
		this.aggroTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		if (!this.wasAggro & broadcastEvent && this.OnAggro != null && this.GetFact(NPCPlayerApex.Facts.HasLineOfSight) > 0)
		{
			this.OnAggro();
		}
		this.wasAggro = true;
		return true;
	}

	public void StartAttack()
	{
		if (!this.IsAlive())
		{
			return;
		}
		this.ShotTest();
		base.MeleeAttack();
	}

	public void StartAttack(AttackOperator.AttackType type, BaseCombatEntity target)
	{
		Memory.ExtendedInfo extendedInfo;
		if (!this.IsAlive())
		{
			return;
		}
		this.AttackTarget = target;
		this.UpdateTargetMemory(this.AttackTarget, 0.1f, out extendedInfo);
		if (type != AttackOperator.AttackType.CloseRange)
		{
			this.ShotTest();
		}
		else if (!base.MeleeAttack())
		{
			this.ShotTest();
			return;
		}
	}

	public void StopMoving()
	{
		if (Interface.CallHook("OnNpcStopMoving", this) != null)
		{
			return;
		}
		this.IsStopped = true;
		this.finalDestination = base.GetPosition();
	}

	private void TargetClosestChair()
	{
		float single = Single.MaxValue;
		foreach (BaseChair chair in this.AiContext.Chairs)
		{
			if (chair.IsMounted())
			{
				continue;
			}
			float serverPosition = (chair.ServerPosition - this.ServerPosition).sqrMagnitude;
			if (serverPosition >= single)
			{
				continue;
			}
			single = serverPosition;
			this.AiContext.ChairTarget = chair;
		}
	}

	private void TickAggro()
	{
		bool flag = false;
		bool flag1 = true;
		if (float.IsInfinity(base.SecondsSinceDealtDamage) || float.IsNegativeInfinity(base.SecondsSinceDealtDamage) || float.IsNaN(base.SecondsSinceDealtDamage))
		{
			flag = UnityEngine.Time.realtimeSinceStartup > this.aggroTimeout;
		}
		else
		{
			BaseCombatEntity attackTarget = this.AttackTarget as BaseCombatEntity;
			if (!(attackTarget != null) || !(attackTarget.lastAttacker != null) || this.net == null || attackTarget.lastAttacker.net == null || base.isMounted)
			{
				flag = UnityEngine.Time.realtimeSinceStartup > this.aggroTimeout;
			}
			else
			{
				flag = (attackTarget.lastAttacker.net.ID != this.net.ID ? false : base.SecondsSinceDealtDamage > this.Stats.DeaggroChaseTime);
			}
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
			this.SetFact(NPCPlayerApex.Facts.IsAggro, 0, flag1, true);
		}
	}

	public override void TickAi(float delta)
	{
		this.MovementUpdate(delta);
	}

	private void TickBehaviourState()
	{
		if (this.GetFact(NPCPlayerApex.Facts.WantsToFlee) == 1 && this.ToPathStatus(this.GetPathStatus()) == NavMeshPathStatus.PathComplete && UnityEngine.Time.realtimeSinceStartup - (this.maxFleeTime - this.Stats.MaxFleeTime) > 0.5f)
		{
			this.TickFlee();
		}
		if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == 1)
		{
			this.TickAggro();
		}
		if (this.GetFact(NPCPlayerApex.Facts.AllyAttackedRecently) == 1 && UnityEngine.Time.realtimeSinceStartup >= this.AllyAttackedRecentlyTimeout)
		{
			this.SetFact(NPCPlayerApex.Facts.AllyAttackedRecently, 0, true, true);
		}
	}

	private void TickEnemyAwareness()
	{
		if (this.GetFact(NPCPlayerApex.Facts.CanTargetEnemies) != 0 || !(this.blockTargetingThisEnemy == null))
		{
			this.SelectEnemy();
			return;
		}
		this.AiContext.EnemyNpc = null;
		this.AiContext.EnemyPlayer = null;
		this.SetFact(NPCPlayerApex.Facts.HasEnemy, 0, true, true);
		this.SetFact(NPCPlayerApex.Facts.EnemyRange, 3, true, true);
		this.SetFact(NPCPlayerApex.Facts.IsAggro, 0, false, true);
	}

	private void TickFlee()
	{
		if (UnityEngine.Time.realtimeSinceStartup > this.maxFleeTime || this.IsNavRunning() && this.NavAgent.remainingDistance <= this.NavAgent.stoppingDistance + 1f)
		{
			this.SetFact(NPCPlayerApex.Facts.WantsToFlee, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.IsFleeing, 0, true, true);
			this.Stats.HealthThresholdForFleeing = base.healthFraction * this.fleeHealthThresholdPercentage;
		}
	}

	private void TickHearing()
	{
		this.SetFact(NPCPlayerApex.Facts.LoudNoiseNearby, 0, true, true);
	}

	private void TickMountableAwareness()
	{
		this.SelectMountable();
	}

	public void TickReasoningSystem()
	{
		object obj;
		if (this.AttackTarget != null)
		{
			obj = 1;
		}
		else
		{
			obj = null;
		}
		this.SetFact(NPCPlayerApex.Facts.HasEnemy, (byte)obj, true, true);
		this._GatherPlayerTargetFacts();
		if (!base.isMounted)
		{
			this._UpdateGroundedSelfFacts();
			this._UpdateCoverFacts();
		}
		else
		{
			this._UpdateMountedSelfFacts();
		}
		if (this.AttackTarget != null)
		{
			Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo(this.AttackTarget);
			if (extendedInfo.Entity != null)
			{
				this.TryAggro(extendedInfo.DistanceSqr);
			}
		}
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
		if (base.isMounted)
		{
			this.UpdateMountedSelfFacts();
			return;
		}
		this.UpdateSelfFacts();
	}

	public void TickSensorySystem()
	{
		object obj;
		if (BaseEntity.Query.Server == null || this.AiContext == null || this.IsDormant)
		{
			return;
		}
		this.AiContext.Players.Clear();
		this.AiContext.Npcs.Clear();
		this.AiContext.DeployedExplosives.Clear();
		this._FindPlayersInVisionRange();
		NPCPlayerApex.PlayerTargetContext.Refresh(this, NPCPlayerApex.PlayerQueryResults, NPCPlayerApex.PlayerQueryResultCount);
		if (!base.isMounted)
		{
			BaseAiUtilityClient selectPlayerTargetAI = this.SelectPlayerTargetAI;
			if (selectPlayerTargetAI != null)
			{
				selectPlayerTargetAI.Execute();
			}
			else
			{
			}
			this._FindEntitiesInCloseRange();
			NPCPlayerApex.EntityTargetContext.Refresh(this, NPCPlayerApex.EntityQueryResults, NPCPlayerApex.EntityQueryResultCount);
			BaseAiUtilityClient selectEntityTargetAI = this.SelectEntityTargetAI;
			if (selectEntityTargetAI != null)
			{
				selectEntityTargetAI.Execute();
			}
			else
			{
			}
			byte lineOfSight = 0;
			if (this.AiContext.EnemyPlayer == null || this.AiContext.EnemyPlayer.IsDestroyed || this.AiContext.EnemyPlayer.IsDead() || NPCPlayerApex.PlayerTargetContext.Score > this.AiContext.LastEnemyPlayerScore + this.DecisionMomentumPlayerTarget())
			{
				this.AiContext.EnemyPlayer = NPCPlayerApex.PlayerTargetContext.Target;
				this.AiContext.LastEnemyPlayerScore = NPCPlayerApex.PlayerTargetContext.Score;
				this.playerTargetDecisionStartTime = UnityEngine.Time.time;
				if (NPCPlayerApex.PlayerTargetContext.Index < 0 || NPCPlayerApex.PlayerTargetContext.Index >= (int)NPCPlayerApex.PlayerTargetContext.LineOfSight.Length)
				{
					Memory.ExtendedInfo extendedInfo = this.AiContext.Memory.GetExtendedInfo(this.AiContext.EnemyPlayer);
					if (extendedInfo.Entity)
					{
						lineOfSight = extendedInfo.LineOfSight;
					}
				}
				else
				{
					lineOfSight = NPCPlayerApex.PlayerTargetContext.LineOfSight[NPCPlayerApex.PlayerTargetContext.Index];
				}
			}
			else if (!(NPCPlayerApex.PlayerTargetContext.Target == null) || this.DecisionMomentumPlayerTarget() >= 0.01f)
			{
				Memory.ExtendedInfo extendedInfo1 = this.AiContext.Memory.GetExtendedInfo(this.AiContext.EnemyPlayer);
				if (extendedInfo1.Entity)
				{
					lineOfSight = extendedInfo1.LineOfSight;
				}
			}
			else
			{
				this.AiContext.EnemyPlayer = NPCPlayerApex.PlayerTargetContext.Target;
				this.AiContext.LastEnemyPlayerScore = 0f;
				this.playerTargetDecisionStartTime = 0f;
			}
			this.AiContext.ClosestPlayer = NPCPlayerApex.PlayerTargetContext.Target;
			if (this.AiContext.ClosestPlayer == null)
			{
				this.AiContext.ClosestPlayer = this.AiContext.EnemyPlayer;
			}
			if (this.AiContext.EnemyNpc == null || this.AiContext.EnemyNpc.IsDestroyed || this.AiContext.EnemyNpc.IsDead() || NPCPlayerApex.EntityTargetContext.AnimalScore > this.AiContext.LastEnemyNpcScore + this.DecisionMomentumAnimalTarget())
			{
				this.AiContext.EnemyNpc = NPCPlayerApex.EntityTargetContext.AnimalTarget;
				this.AiContext.LastEnemyNpcScore = NPCPlayerApex.EntityTargetContext.AnimalScore;
				this.animalTargetDecisionStartTime = UnityEngine.Time.time;
			}
			else if (NPCPlayerApex.EntityTargetContext.AnimalTarget == null && this.DecisionMomentumAnimalTarget() < 0.01f)
			{
				this.AiContext.EnemyNpc = NPCPlayerApex.EntityTargetContext.AnimalTarget;
				this.AiContext.LastEnemyNpcScore = 0f;
				this.animalTargetDecisionStartTime = 0f;
			}
			this.AiContext.DeployedExplosives.Clear();
			if (NPCPlayerApex.EntityTargetContext.ExplosiveTarget != null)
			{
				this.AiContext.DeployedExplosives.Add(NPCPlayerApex.EntityTargetContext.ExplosiveTarget);
			}
			this.AttackTarget = this.AiContext.EnemyPlayer;
			if (this.AttackTarget == null)
			{
				this.AttackTarget = this.AiContext.EnemyNpc;
			}
			if (this.AiContext.EnemyPlayer == null)
			{
				this.AiContext.EnemyHideoutGuess = null;
			}
			else
			{
				Memory.SeenInfo info = this.AiContext.Memory.GetInfo(this.AiContext.EnemyPlayer);
				bool flag = false;
				if (this.GetFact(NPCPlayerApex.Facts.IsMilitaryTunnelLab) > 0)
				{
					if (NPCPlayerApex.PathToPlayerTarget == null)
					{
						NPCPlayerApex.PathToPlayerTarget = new NavMeshPath();
					}
					flag = (!(this.NavAgent != null) || !this.NavAgent.isOnNavMesh || this.NavAgent.CalculatePath(this.AiContext.EnemyPlayer.ServerPosition, NPCPlayerApex.PathToPlayerTarget) ? NPCPlayerApex.PathToPlayerTarget.status != NavMeshPathStatus.PathComplete : true);
					if (flag)
					{
						obj = 1;
					}
					else
					{
						obj = null;
					}
					this.SetFact(NPCPlayerApex.Facts.IncompletePathToTarget, (byte)obj, true, true);
				}
				if (!flag)
				{
					this._FindCoverPointsInVolume();
					NPCPlayerApex.CoverContext.Refresh(this, info.Position, this.AiContext.sampledCoverPoints);
					BaseAiUtilityClient selectCoverTargetsAI = this.SelectCoverTargetsAI;
					if (selectCoverTargetsAI != null)
					{
						selectCoverTargetsAI.Execute();
					}
					else
					{
					}
					this.AiContext.CoverSet.Reset();
					this.AiContext.CoverSet.Update(NPCPlayerApex.CoverContext.BestRetreatCP, NPCPlayerApex.CoverContext.BestFlankCP, NPCPlayerApex.CoverContext.BestAdvanceCP);
					if (lineOfSight != 0)
					{
						this.AiContext.EnemyHideoutGuess = null;
					}
					else if (this._FindCoverPointsInVolume(info.Position))
					{
						NPCPlayerApex.CoverContext.Refresh(this, info.Position, this.AiContext.EnemyCoverPoints);
						BaseAiUtilityClient selectEnemyHideoutAI = this.SelectEnemyHideoutAI;
						if (selectEnemyHideoutAI != null)
						{
							selectEnemyHideoutAI.Execute();
						}
						else
						{
						}
						this.AiContext.EnemyHideoutGuess = NPCPlayerApex.CoverContext.HideoutCP;
					}
				}
			}
		}
		else
		{
			BaseAiUtilityClient selectPlayerTargetMountedAI = this.SelectPlayerTargetMountedAI;
			if (selectPlayerTargetMountedAI != null)
			{
				selectPlayerTargetMountedAI.Execute();
			}
			else
			{
			}
			this.AiContext.EnemyPlayer = NPCPlayerApex.PlayerTargetContext.Target;
			this.AiContext.EnemyNpc = null;
			this.AttackTarget = NPCPlayerApex.PlayerTargetContext.Target;
		}
		this.AiContext.Memory.Forget((float)this.ForgetUnseenEntityTime);
		this.AiContext.ForgetCheckedHideouts((float)this.ForgetUnseenEntityTime * 0.5f);
	}

	private void TickSmell()
	{
	}

	public void TickStuck(float delta)
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
			this.stuckDuration += delta;
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
		this.AiContext.DeployedExplosives.Clear();
		if (this.IsMountableAgent)
		{
			this.AiContext.Chairs.Clear();
		}
		if (base.isMounted)
		{
			for (int i = 0; i < BasePlayer.activePlayerList.Count; i++)
			{
				BasePlayer item = BasePlayer.activePlayerList[i];
				if (!(item == null) && item.isServer && !AI.ignoreplayers && !item.IsSleeping() && !item.IsDead() && NPCPlayerApex.Distance2DSqr(item.ServerPosition, this.ServerPosition) <= this.Stats.VisionRange * this.Stats.VisionRange)
				{
					this.AiContext.Players.Add(item);
				}
			}
			return;
		}
		if (BaseEntity.Query.Server == null)
		{
			return;
		}
		int num = 0;
		num = (!AI.npc_ignore_chairs ? BaseEntity.Query.Server.GetInSphere(base.transform.position, this.Stats.VisionRange, this.SensesResults, new Func<BaseEntity, bool>(NPCPlayerApex.AiCaresAbout)) : BaseEntity.Query.Server.GetInSphere(base.transform.position, this.Stats.VisionRange, this.SensesResults, new Func<BaseEntity, bool>(NPCPlayerApex.AiCaresAboutIgnoreChairs)));
		if (num == 0)
		{
			return;
		}
		for (int j = 0; j < num; j++)
		{
			BaseEntity sensesResults = this.SensesResults[j];
			if (!(sensesResults == null) && !(sensesResults == this) && sensesResults.isServer)
			{
				BasePlayer basePlayer = sensesResults as BasePlayer;
				if (basePlayer != null)
				{
					if (!AI.ignoreplayers && !basePlayer.IsSleeping() && !basePlayer.IsDead())
					{
						this.AiContext.Players.Add(sensesResults as BasePlayer);
					}
				}
				else if (sensesResults is BaseNpc)
				{
					this.AiContext.Npcs.Add(sensesResults as BaseNpc);
				}
				else if (sensesResults is TimedExplosive)
				{
					TimedExplosive timedExplosive = sensesResults as TimedExplosive;
					if ((this.ServerPosition - timedExplosive.ServerPosition).sqrMagnitude < (timedExplosive.explosionRadius + 2f) * (timedExplosive.explosionRadius + 2f))
					{
						this.AiContext.DeployedExplosives.Add(timedExplosive);
					}
				}
				else if (this.IsMountableAgent && !AI.npc_ignore_chairs && sensesResults is BaseChair)
				{
					this.AiContext.Chairs.Add(sensesResults as BaseChair);
				}
			}
		}
		float single = Single.MaxValue;
		foreach (BasePlayer player in this.AiContext.Players)
		{
			float serverPosition = (player.ServerPosition - this.ServerPosition).sqrMagnitude;
			if (serverPosition >= single || player.IsDead() || player.IsDestroyed)
			{
				continue;
			}
			single = serverPosition;
			this.AiContext.ClosestPlayer = player;
		}
		this.sensesTicksSinceLastCoverSweep++;
		if (this.sensesTicksSinceLastCoverSweep > 5)
		{
			this.FindCoverPoints();
			this.sensesTicksSinceLastCoverSweep = 0;
		}
	}

	public NPCPlayerApex.AfraidRangeEnum ToAfraidRangeEnum(float sqrRange)
	{
		if (sqrRange <= this.Stats.AfraidRange * this.Stats.AfraidRange)
		{
			return NPCPlayerApex.AfraidRangeEnum.InAfraidRange;
		}
		return NPCPlayerApex.AfraidRangeEnum.OutOfRange;
	}

	public NPCPlayerApex.EnemyEngagementRangeEnum ToEnemyEngagementRangeEnum(float sqrRange)
	{
		if (sqrRange <= this.ToSqrRange(NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange))
		{
			return NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange;
		}
		if (sqrRange > this.ToSqrRange(NPCPlayerApex.EnemyEngagementRangeEnum.DeaggroRange))
		{
			return NPCPlayerApex.EnemyEngagementRangeEnum.DeaggroRange;
		}
		return NPCPlayerApex.EnemyEngagementRangeEnum.NeutralRange;
	}

	public NPCPlayerApex.EnemyRangeEnum ToEnemyRangeEnum(float sqrRange)
	{
		if (sqrRange <= this.ToSqrRange(NPCPlayerApex.EnemyRangeEnum.CloseAttackRange))
		{
			return NPCPlayerApex.EnemyRangeEnum.CloseAttackRange;
		}
		if (sqrRange <= this.ToSqrRange(NPCPlayerApex.EnemyRangeEnum.MediumAttackRange))
		{
			return NPCPlayerApex.EnemyRangeEnum.MediumAttackRange;
		}
		if (sqrRange <= this.ToSqrRange(NPCPlayerApex.EnemyRangeEnum.LongAttackRange))
		{
			return NPCPlayerApex.EnemyRangeEnum.LongAttackRange;
		}
		return NPCPlayerApex.EnemyRangeEnum.OutOfRange;
	}

	public NPCPlayerApex.HealthEnum ToHealthEnum(float healthNormalized)
	{
		if (healthNormalized >= 0.75f)
		{
			return NPCPlayerApex.HealthEnum.Fine;
		}
		if (healthNormalized >= 0.25f)
		{
			return NPCPlayerApex.HealthEnum.Medium;
		}
		return NPCPlayerApex.HealthEnum.Low;
	}

	public NavMeshPathStatus ToPathStatus(byte value)
	{
		return (NavMeshPathStatus)value;
	}

	public int TopologyPreference()
	{
		return -1;
	}

	public float ToSpeed(BaseNpc.SpeedEnum speed)
	{
		return 0f;
	}

	public float ToSpeed(NPCPlayerApex.SpeedEnum speed)
	{
		switch (speed)
		{
			case NPCPlayerApex.SpeedEnum.StandStill:
			{
				return 0f;
			}
			case NPCPlayerApex.SpeedEnum.CrouchWalk:
			{
				return AI.npc_speed_crouch_walk * this.Stats.Speed;
			}
			case NPCPlayerApex.SpeedEnum.Walk:
			{
				return AI.npc_speed_walk * this.Stats.Speed;
			}
			case NPCPlayerApex.SpeedEnum.Run:
			{
				return AI.npc_speed_run * this.Stats.Speed;
			}
			case NPCPlayerApex.SpeedEnum.CrouchRun:
			{
				return AI.npc_speed_crouch_run * this.Stats.Speed;
			}
		}
		return AI.npc_speed_sprint * this.Stats.Speed;
	}

	public NPCPlayerApex.SpeedEnum ToSpeedEnum(float speed)
	{
		if (speed <= 0.01f)
		{
			return NPCPlayerApex.SpeedEnum.StandStill;
		}
		if (speed <= AI.npc_speed_crouch_walk)
		{
			return NPCPlayerApex.SpeedEnum.CrouchWalk;
		}
		if (speed <= AI.npc_speed_walk)
		{
			return NPCPlayerApex.SpeedEnum.Walk;
		}
		if (speed <= AI.npc_speed_crouch_run)
		{
			return NPCPlayerApex.SpeedEnum.CrouchRun;
		}
		if (speed <= AI.npc_speed_run)
		{
			return NPCPlayerApex.SpeedEnum.Run;
		}
		return NPCPlayerApex.SpeedEnum.Sprint;
	}

	public float ToSqrRange(NPCPlayerApex.EnemyRangeEnum range)
	{
		if (range == NPCPlayerApex.EnemyRangeEnum.CloseAttackRange)
		{
			return this.Stats.CloseRange * this.Stats.CloseRange;
		}
		if (range == NPCPlayerApex.EnemyRangeEnum.MediumAttackRange)
		{
			return this.Stats.MediumRange * this.Stats.MediumRange;
		}
		if (range != NPCPlayerApex.EnemyRangeEnum.LongAttackRange)
		{
			return Single.PositiveInfinity;
		}
		return this.Stats.LongRange * this.Stats.LongRange;
	}

	public float ToSqrRange(NPCPlayerApex.EnemyEngagementRangeEnum range)
	{
		if (range == NPCPlayerApex.EnemyEngagementRangeEnum.AggroRange)
		{
			return this.Stats.AggressionRange * this.Stats.AggressionRange;
		}
		if (range != NPCPlayerApex.EnemyEngagementRangeEnum.DeaggroRange)
		{
			return Single.PositiveInfinity;
		}
		return this.Stats.DeaggroRange * this.Stats.DeaggroRange;
	}

	public override void TriggerDown()
	{
		if (!(this.AttackTarget == null) && (int)SwitchToolOperator.ReactiveAimsAtTarget.Test(this.AiContext) != 0)
		{
			base.TriggerDown();
			return;
		}
		NPCPlayerApex nPCPlayerApex = this;
		base.CancelInvoke(new Action(nPCPlayerApex.TriggerDown));
		AttackEntity heldEntity = base.GetHeldEntity() as AttackEntity;
		this.nextTriggerTime = UnityEngine.Time.time + (heldEntity != null ? heldEntity.attackSpacing : 1f);
	}

	public bool TryAggro(float sqrRange)
	{
		if (!this.HostilityConsideration(this.AiContext.EnemyPlayer))
		{
			this.wasAggro = false;
			return false;
		}
		bool flag = this.IsWithinAggroRange(sqrRange);
		if (!(this.GetFact(NPCPlayerApex.Facts.IsAggro) == 0 & flag))
		{
			this.wasAggro = flag;
			return false;
		}
		return this.StartAggro(this.Stats.DeaggroChaseTime, true);
	}

	public bool TryAggro(NPCPlayerApex.EnemyRangeEnum range)
	{
		if (!this.HostilityConsideration(this.AiContext.EnemyPlayer))
		{
			this.wasAggro = false;
			return false;
		}
		if (this.GetFact(NPCPlayerApex.Facts.IsAggro) == 0 && this.IsWithinAggroRange(range))
		{
			float single = (range <= NPCPlayerApex.EnemyRangeEnum.MediumAttackRange ? 1f : this.Stats.Defensiveness);
			single = Mathf.Max(single, this.Stats.Hostility);
			if (UnityEngine.Time.realtimeSinceStartup > this.lastAggroChanceCalcTime + 5f)
			{
				this.lastAggroChanceResult = UnityEngine.Random.@value;
				this.lastAggroChanceCalcTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (this.lastAggroChanceResult < single)
			{
				return this.StartAggro(this.Stats.DeaggroChaseTime, true);
			}
		}
		this.wasAggro = this.IsWithinAggroRange(range);
		return false;
	}

	private IEnumerator TryForceToNavmesh()
	{
		NPCPlayerApex component = null;
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
			if (component.GetNavAgent.isOnNavMesh)
			{
				component.GetNavAgent.enabled = true;
				component.StoppingDistance = 1f;
				if (component.utilityAiComponent == null)
				{
					component.utilityAiComponent = component.Entity.GetComponent<UtilityAIComponent>();
				}
				if (component.utilityAiComponent != null)
				{
					component.utilityAiComponent.enabled = true;
					component.utilityAiComponent.Resume();
				}
				component.InvokeRandomized(new Action(component.RadioChatter), component.RadioEffectRepeatRange.x, component.RadioEffectRepeatRange.x, component.RadioEffectRepeatRange.y - component.RadioEffectRepeatRange.x);
				yield break;
			}
			if (NavMesh.SamplePosition(component.ServerPosition, out navMeshHit, component.GetNavAgent.height * single1, component.GetNavAgent.areaMask))
			{
				component.ServerPosition = navMeshHit.position;
				component.GetNavAgent.Warp(component.ServerPosition);
				component.GetNavAgent.enabled = true;
				float spawnPosition = component.SpawnPosition.y - component.ServerPosition.y;
				if (spawnPosition < 0f)
				{
					spawnPosition = Mathf.Max(spawnPosition, -0.25f);
					component.GetNavAgent.baseOffset = spawnPosition;
				}
				component.StoppingDistance = 1f;
				if (component.utilityAiComponent == null)
				{
					component.utilityAiComponent = component.Entity.GetComponent<UtilityAIComponent>();
				}
				if (component.utilityAiComponent != null)
				{
					component.utilityAiComponent.enabled = true;
					component.utilityAiComponent.Resume();
				}
				component.InvokeRandomized(new Action(component.RadioChatter), component.RadioEffectRepeatRange.x, component.RadioEffectRepeatRange.x, component.RadioEffectRepeatRange.y - component.RadioEffectRepeatRange.x);
				yield break;
			}
			yield return CoroutineEx.waitForSecondsRealtime(single);
			single1 *= 1.5f;
			num++;
		}
		int areaFromName = NavMesh.GetAreaFromName("Walkable");
		if ((component.GetNavAgent.areaMask & 1 << (areaFromName & 31)) != 0)
		{
			if (component.transform != null && !component.IsDestroyed)
			{
				UnityEngine.Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[] { component.name });
				component.Kill(BaseNetworkable.DestroyMode.None);
			}
			yield break;
		}
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
		component.GetNavAgent.agentTypeID = settingsByIndex.agentTypeID;
		component.GetNavAgent.areaMask = 1 << (areaFromName & 31);
		yield return component.TryForceToNavmesh();
	}

	public void UpdateAttackTargetVisibility(float delta)
	{
		if (this.AttackTarget == null || this.lastAttackTarget != null && this.lastAttackTarget != this.AttackTarget || this.GetFact(NPCPlayerApex.Facts.HasLineOfSight) == 0)
		{
			this.attackTargetVisibleFor = 0f;
		}
		else
		{
			this.attackTargetVisibleFor += delta;
		}
		this.lastAttackTarget = this.AttackTarget;
	}

	public void UpdateDestination(Vector3 newDest)
	{
		this.SetDestination(newDest);
	}

	public void UpdateDestination(Transform tx)
	{
		this.SetDestination(tx.position);
	}

	private void UpdateMountedSelfFacts()
	{
		object obj;
		object obj1;
		object obj2;
		object obj3;
		object obj4;
		this.SetFact(NPCPlayerApex.Facts.Health, (byte)this.ToHealthEnum(base.healthFraction), true, true);
		if (UnityEngine.Time.realtimeSinceStartup >= this.NextAttackTime())
		{
			obj = 1;
		}
		else
		{
			obj = null;
		}
		this.SetFact(NPCPlayerApex.Facts.IsWeaponAttackReady, (byte)obj, true, true);
		if (base.SecondsSinceAttacked < this.Stats.AttackedMemoryTime)
		{
			obj1 = 1;
		}
		else
		{
			obj1 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedLately, (byte)obj1, true, true);
		if (base.SecondsSinceAttacked < 2f)
		{
			obj2 = 1;
		}
		else
		{
			obj2 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedVeryRecently, (byte)obj2, true, true);
		if (base.SecondsSinceAttacked < 7f)
		{
			obj3 = 1;
		}
		else
		{
			obj3 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedRecently, (byte)obj3, true, true);
		if (UnityEngine.Time.realtimeSinceStartup > this.NextWeaponSwitchTime)
		{
			obj4 = 1;
		}
		else
		{
			obj4 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.CanSwitchWeapon, (byte)obj4, true, true);
		this.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte)this.GetCurrentAmmoStateEnum(), true, true);
		this.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte)this.GetCurrentWeaponTypeEnum(), true, true);
	}

	protected override void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		if (TerrainMeta.HeightMap != null && this.AiContext.AiLocationManager != null && (this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileA || this.AiContext.AiLocationManager.LocationType == AiLocationSpawner.SquadSpawnerLocation.JunkpileG))
		{
			float height = TerrainMeta.HeightMap.GetHeight(moveToPosition);
			float single = moveToPosition.y - height;
			if (single > 0f)
			{
				moveToPosition.y = height;
			}
			else if (single < 0.5f)
			{
				moveToPosition.y = height;
			}
		}
		base.UpdatePositionAndRotation(moveToPosition);
	}

	private void UpdateSelfFacts()
	{
		object obj;
		object obj1;
		object obj2;
		object obj3;
		object obj4;
		object obj5;
		object obj6;
		object obj7;
		object obj8;
		object obj9;
		object obj10;
		object obj11;
		object obj12;
		object obj13;
		object obj14;
		object obj15;
		object obj16;
		object obj17;
		object obj18;
		if (!float.IsNegativeInfinity(base.SecondsSinceAttacked) && base.SecondsSinceAttacked < this.Stats.AttackedMemoryTime || !float.IsNegativeInfinity(this.SecondsSinceSeenPlayer) && this.SecondsSinceSeenPlayer < this.Stats.AttackedMemoryTime)
		{
			this.alertness = 1f;
		}
		else if (this.alertness > 0f)
		{
			this.alertness = Mathf.Clamp01(this.alertness - AI.npc_alertness_drain_rate);
		}
		this.SetFact(NPCPlayerApex.Facts.Health, (byte)this.ToHealthEnum(base.healthFraction), true, true);
		if (UnityEngine.Time.realtimeSinceStartup >= this.NextAttackTime())
		{
			obj = 1;
		}
		else
		{
			obj = null;
		}
		this.SetFact(NPCPlayerApex.Facts.IsWeaponAttackReady, (byte)obj, true, true);
		if (UnityEngine.Time.realtimeSinceStartup < this.AiContext.NextRoamTime || !this.IsNavRunning())
		{
			obj1 = null;
		}
		else
		{
			obj1 = 1;
		}
		this.SetFact(NPCPlayerApex.Facts.IsRoamReady, (byte)obj1, true, true);
		this.SetFact(NPCPlayerApex.Facts.Speed, (byte)this.ToSpeedEnum(this.TargetSpeed / this.Stats.Speed), true, true);
		if (base.SecondsSinceAttacked < this.Stats.AttackedMemoryTime)
		{
			obj2 = 1;
		}
		else
		{
			obj2 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedLately, (byte)obj2, true, true);
		if (base.SecondsSinceAttacked < 2f)
		{
			obj3 = 1;
		}
		else
		{
			obj3 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedVeryRecently, (byte)obj3, true, true);
		if (base.SecondsSinceAttacked < 7f)
		{
			obj4 = 1;
		}
		else
		{
			obj4 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.AttackedRecently, (byte)obj4, true, true);
		this.SetFact(NPCPlayerApex.Facts.IsMoving, this.IsMoving(), true, false);
		if (UnityEngine.Time.realtimeSinceStartup > this.NextWeaponSwitchTime)
		{
			obj5 = 1;
		}
		else
		{
			obj5 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.CanSwitchWeapon, (byte)obj5, true, true);
		if (UnityEngine.Time.realtimeSinceStartup > this.NextToolSwitchTime)
		{
			obj6 = 1;
		}
		else
		{
			obj6 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.CanSwitchTool, (byte)obj6, true, true);
		this.SetFact(NPCPlayerApex.Facts.CurrentAmmoState, (byte)this.GetCurrentAmmoStateEnum(), true, true);
		this.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, (byte)this.GetCurrentWeaponTypeEnum(), true, true);
		this.SetFact(NPCPlayerApex.Facts.CurrentToolType, (byte)this.GetCurrentToolTypeEnum(), true, true);
		if (this.AiContext.DeployedExplosives.Count > 0)
		{
			obj7 = 1;
		}
		else
		{
			obj7 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.ExplosiveInRange, (byte)obj7, true, true);
		if (this.Stats.IsMobile)
		{
			obj8 = 1;
		}
		else
		{
			obj8 = null;
		}
		this.SetFact(NPCPlayerApex.Facts.IsMobile, (byte)obj8, true, true);
		if (!(this.WaypointSet != null) || this.WaypointSet.Points.Count <= 0)
		{
			obj9 = null;
		}
		else
		{
			obj9 = 1;
		}
		this.SetFact(NPCPlayerApex.Facts.HasWaypoints, (byte)obj9, true, true);
		NPCPlayerApex.EnemyRangeEnum rangeToSpawnPoint = this.GetRangeToSpawnPoint();
		this.SetFact(NPCPlayerApex.Facts.RangeToSpawnLocation, (byte)rangeToSpawnPoint, true, true);
		if (rangeToSpawnPoint < this.Stats.MaxRangeToSpawnLoc)
		{
			this.lastInRangeOfSpawnPositionTime = UnityEngine.Time.time;
		}
		if (this.CheckHealthThresholdToFlee())
		{
			this.WantsToFlee();
		}
		if (this.GetFact(NPCPlayerApex.Facts.HasEnemy) != 1)
		{
			this.FindClosestCoverToUs();
			this.SetFact(NPCPlayerApex.Facts.RetreatCoverInRange, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.FlankCoverInRange, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.AdvanceCoverInRange, 0, true, true);
			if (this.AiContext.CoverSet.Closest.ReservedCoverPoint != null)
			{
				obj10 = 1;
			}
			else
			{
				obj10 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.CoverInRange, (byte)obj10, true, true);
			this.SetFact(NPCPlayerApex.Facts.IsMovingToCover, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, 0, true, true);
		}
		else
		{
			this.FindCoverFromEnemy();
			if (this.AiContext.CoverSet.Retreat.ReservedCoverPoint != null)
			{
				obj13 = 1;
			}
			else
			{
				obj13 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.RetreatCoverInRange, (byte)obj13, true, true);
			if (this.AiContext.CoverSet.Flank.ReservedCoverPoint != null)
			{
				obj14 = 1;
			}
			else
			{
				obj14 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.FlankCoverInRange, (byte)obj14, true, true);
			if (this.AiContext.CoverSet.Advance.ReservedCoverPoint != null)
			{
				obj15 = 1;
			}
			else
			{
				obj15 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.AdvanceCoverInRange, (byte)obj15, true, true);
			if (this.AiContext.CoverSet.Closest.ReservedCoverPoint != null)
			{
				obj16 = 1;
			}
			else
			{
				obj16 = null;
			}
			this.SetFact(NPCPlayerApex.Facts.CoverInRange, (byte)obj16, true, true);
			if (this.GetFact(NPCPlayerApex.Facts.IsMovingToCover) == 1)
			{
				this.SetFact(NPCPlayerApex.Facts.IsMovingToCover, this.IsMoving(), true, true);
			}
			Vector3 serverPosition = (this.AttackTarget.ServerPosition - this.ServerPosition).normalized;
			float single = Vector3.Dot(this.eyes.BodyForward(), serverPosition);
			if (!base.isMounted)
			{
				if (single > AI.npc_valid_aim_cone)
				{
					obj17 = 1;
				}
				else
				{
					obj17 = null;
				}
				this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, (byte)obj17, true, true);
			}
			else
			{
				if (single > AI.npc_valid_mounted_aim_cone)
				{
					obj18 = 1;
				}
				else
				{
					obj18 = null;
				}
				this.SetFact(NPCPlayerApex.Facts.AimsAtTarget, (byte)obj18, true, true);
			}
		}
		if (this.AiContext.CoverSet.Closest.ReservedCoverPoint != null)
		{
			if ((this.AiContext.CoverSet.Closest.ReservedCoverPoint.Position - this.ServerPosition).sqrMagnitude < 0.5625f)
			{
				obj11 = 1;
			}
			else
			{
				obj11 = null;
			}
			byte num = (byte)obj11;
			this.SetFact(NPCPlayerApex.Facts.IsInCover, num, true, true);
			if (num == 1)
			{
				if (this.AiContext.CoverSet.Closest.ReservedCoverPoint.IsCompromised)
				{
					obj12 = 1;
				}
				else
				{
					obj12 = null;
				}
				this.SetFact(NPCPlayerApex.Facts.IsCoverCompromised, (byte)obj12, true, true);
			}
		}
		if (this.GetFact(NPCPlayerApex.Facts.IsRetreatingToCover) == 1)
		{
			this.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, this.IsMoving(), true, true);
		}
	}

	public Memory.SeenInfo UpdateTargetMemory(BaseEntity target, float dmg, out Memory.ExtendedInfo extendedInfo)
	{
		return this.UpdateTargetMemory(target, dmg, target.ServerPosition, out extendedInfo);
	}

	public Memory.SeenInfo UpdateTargetMemory(BaseEntity target, float dmg, Vector3 lastKnownPosition, out Memory.ExtendedInfo extendedInfo)
	{
		Vector3 vector3;
		float single;
		float single1;
		float single2;
		int num;
		int num1;
		byte num2;
		if (target == null)
		{
			extendedInfo = new Memory.ExtendedInfo();
			return new Memory.SeenInfo();
		}
		if (!base.isMounted)
		{
			BestPlayerDirection.Evaluate(this, lastKnownPosition, out vector3, out single);
		}
		else
		{
			BestMountedPlayerDirection.Evaluate(this, lastKnownPosition, out vector3, out single);
		}
		BestPlayerDistance.Evaluate(this, lastKnownPosition, out single1, out single2);
		BasePlayer player = target.ToPlayer();
		if (!player)
		{
			num2 = 1;
		}
		else
		{
			num2 = (!base.isMounted ? BestLineOfSight.Evaluate(this, player, out num, out num1) : BestMountedLineOfSight.Evaluate(this, player));
		}
		this.SetFact(NPCPlayerApex.Facts.HasLineOfSight, num2, true, true);
		return this.AiContext.Memory.Update(target, lastKnownPosition, dmg, vector3, single, single1, num2, this.lastAttacker == target, this.lastAttackedTime, out extendedInfo);
	}

	private float VisibilityScoreModifier(BasePlayer target, Vector3 dir, float dist, bool losStand, bool losCrouch)
	{
		if (base.isMounted)
		{
			BaseMountable mounted = base.GetMounted();
			if (Vector3.Dot(dir.normalized, mounted.transform.forward) > -0.1f)
			{
				return 1f;
			}
			this.SetFact(NPCPlayerApex.Facts.HasEnemy, 0, true, true);
			this.SetFact(NPCPlayerApex.Facts.IsAggro, 0, true, true);
			this.AiContext.EnemyNpc = null;
			this.AiContext.EnemyPlayer = null;
			this.AttackTarget = null;
			return 0f;
		}
		float single = (target.IsDucked() ? 0.5f : 1f);
		single = single * (target.IsRunning() ? 1.5f : 1f);
		single = single * (target.estimatedSpeed <= 0.01f ? 0.5f : 1f);
		float single1 = 1f;
		bool flag = false;
		Item activeItem = target.GetActiveItem();
		if (activeItem != null)
		{
			HeldEntity heldEntity = activeItem.GetHeldEntity() as HeldEntity;
			if (heldEntity != null)
			{
				flag = heldEntity.LightsOn();
			}
		}
		if (!flag)
		{
			single1 = this.Stats.DistanceVisibility.Evaluate(Mathf.Clamp01(dist / this.Stats.VisionRange));
			if (!losStand & losCrouch)
			{
				single1 *= 0.75f;
			}
			else if (losStand && !losCrouch)
			{
				single1 *= 0.9f;
			}
			if (single >= 1f)
			{
				single1 *= single;
			}
			else
			{
				Vector3 vector3 = dir.normalized;
				Vector3 vector31 = this.eyes.HeadForward();
				float single2 = Vector3.Dot(vector3, vector31.normalized);
				if (single2 > Mathf.Abs(this.Stats.VisionCone))
				{
					single1 *= 1.5f;
				}
				else if (single2 <= 0f)
				{
					single1 = single1 * (0.25f * single);
				}
				else
				{
					single1 *= Mathf.Clamp01(single2 + single);
				}
			}
		}
		single1 = Mathf.Clamp01(single1);
		float single3 = 0f;
		if (this.alertness > 0.5f)
		{
			single3 = (UnityEngine.Random.@value < single1 ? single1 : 0f);
		}
		else if (this.alertness <= 0.01f)
		{
			single3 = (single1 > AI.npc_alertness_zero_detection_mod ? single1 : 0f);
		}
		else
		{
			single3 = (UnityEngine.Random.@value < single1 * this.alertness ? single1 : 0f);
		}
		return single3;
	}

	public bool WantsToEat(BaseEntity ent)
	{
		return false;
	}

	private void WantsToFlee()
	{
		if (this.GetFact(NPCPlayerApex.Facts.WantsToFlee) == 1 || !this.IsNavRunning())
		{
			return;
		}
		this.SetFact(NPCPlayerApex.Facts.WantsToFlee, 1, true, true);
		this.maxFleeTime = UnityEngine.Time.realtimeSinceStartup + this.Stats.MaxFleeTime;
	}

	public float WeaponAttackRange()
	{
		AttackEntity heldEntity = base.GetHeldEntity() as AttackEntity;
		if (heldEntity == null)
		{
			return 0f;
		}
		return heldEntity.effectiveRange;
	}

	public NPCPlayerApex.EnemyRangeEnum WeaponToEnemyRange(NPCPlayerApex.WeaponTypeEnum weapon)
	{
		switch (weapon)
		{
			case NPCPlayerApex.WeaponTypeEnum.None:
			case NPCPlayerApex.WeaponTypeEnum.CloseRange:
			{
				return NPCPlayerApex.EnemyRangeEnum.CloseAttackRange;
			}
			case NPCPlayerApex.WeaponTypeEnum.MediumRange:
			{
				return NPCPlayerApex.EnemyRangeEnum.MediumAttackRange;
			}
			case NPCPlayerApex.WeaponTypeEnum.LongRange:
			{
				return NPCPlayerApex.EnemyRangeEnum.LongAttackRange;
			}
		}
		return NPCPlayerApex.EnemyRangeEnum.OutOfRange;
	}

	private static bool WithinVisionCone(NPCPlayerApex npc, BaseEntity other)
	{
		if (Mathf.Approximately(npc.Stats.VisionCone, -1f))
		{
			return true;
		}
		Vector3 serverPosition = (other.ServerPosition - npc.ServerPosition).normalized;
		if (Vector3.Dot(npc.ServerRotation * Vector3.forward, serverPosition) < npc.Stats.VisionCone)
		{
			return false;
		}
		return true;
	}

	public delegate void ActionCallback();

	public enum AfraidRangeEnum : byte
	{
		InAfraidRange,
		OutOfRange
	}

	public enum AmmoStateEnum : byte
	{
		Full,
		High,
		Medium,
		Low,
		Empty
	}

	public enum BodyState : byte
	{
		StandingTall,
		Crouched
	}

	public class CoverPointComparer : IComparer<CoverPoint>
	{
		private readonly BaseEntity compareTo;

		public CoverPointComparer(BaseEntity compareTo)
		{
			this.compareTo = compareTo;
		}

		public int Compare(CoverPoint a, CoverPoint b)
		{
			if (this.compareTo == null || a == null || b == null)
			{
				return 0;
			}
			Vector3 serverPosition = this.compareTo.ServerPosition - a.Position;
			float single = serverPosition.sqrMagnitude;
			if (single < 0.01f)
			{
				return -1;
			}
			serverPosition = this.compareTo.ServerPosition - b.Position;
			float single1 = serverPosition.sqrMagnitude;
			if (single < single1)
			{
				return -1;
			}
			if (single > single1)
			{
				return 1;
			}
			return 0;
		}
	}

	public enum EnemyEngagementRangeEnum : byte
	{
		AggroRange,
		DeaggroRange,
		NeutralRange
	}

	public enum EnemyRangeEnum : byte
	{
		CloseAttackRange,
		MediumAttackRange,
		LongAttackRange,
		OutOfRange
	}

	public enum Facts
	{
		HasEnemy,
		HasSecondaryEnemies,
		EnemyRange,
		CanTargetEnemies,
		Health,
		Speed,
		IsWeaponAttackReady,
		CanReload,
		IsRoamReady,
		IsAggro,
		WantsToFlee,
		AttackedLately,
		LoudNoiseNearby,
		IsMoving,
		IsFleeing,
		IsAfraid,
		AfraidRange,
		IsUnderHealthThreshold,
		CanNotMove,
		SeekingCover,
		IsInCover,
		IsCrouched,
		CurrentAmmoState,
		CurrentWeaponType,
		BodyState,
		HasLineOfSight,
		CanSwitchWeapon,
		CoverInRange,
		IsMovingToCover,
		ExplosiveInRange,
		HasLineOfSightCrouched,
		HasLineOfSightStanding,
		PathToTargetStatus,
		AimsAtTarget,
		RetreatCoverInRange,
		FlankCoverInRange,
		AdvanceCoverInRange,
		IsRetreatingToCover,
		SidesteppedOutOfCover,
		IsCoverCompromised,
		AttackedVeryRecently,
		RangeToSpawnLocation,
		AttackedRecently,
		CurrentToolType,
		CanSwitchTool,
		AllyAttackedRecently,
		IsMounted,
		WantsToDismount,
		CanNotWieldWeapon,
		IsMobile,
		HasWaypoints,
		IsPeacekeeper,
		IsSearchingForEnemy,
		EnemyEngagementRange,
		IsMovingTowardWaypoint,
		IsMilitaryTunnelLab,
		IncompletePathToTarget,
		IsBandit
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
		CrouchWalk,
		Walk,
		Run,
		CrouchRun,
		Sprint
	}

	public enum ToolTypeEnum : byte
	{
		None,
		Research,
		Lightsource
	}

	public enum WeaponTypeEnum : byte
	{
		None,
		CloseRange,
		MediumRange,
		LongRange
	}
}