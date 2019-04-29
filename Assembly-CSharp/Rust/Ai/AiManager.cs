using Apex.LoadBalancing;
using Rust;
using Rust.Ai.HTN;
using Rust.Ai.HTN.ScientistJunkpile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai
{
	[DefaultExecutionOrder(-103)]
	public class AiManager : SingletonComponent<AiManager>, IServerComponent, ILoadBalanced
	{
		private readonly HashSet<IAIAgent> activeAgents = new HashSet<IAIAgent>();

		private readonly List<IAIAgent> dormantAgents = new List<IAIAgent>();

		private readonly HashSet<IAIAgent> pendingAddToActive = new HashSet<IAIAgent>();

		private readonly HashSet<IAIAgent> pendingAddToDormant = new HashSet<IAIAgent>();

		private readonly HashSet<IAIAgent> pendingRemoveFromActive = new HashSet<IAIAgent>();

		private readonly HashSet<IAIAgent> pendingRemoveFromDormant = new HashSet<IAIAgent>();

		private int lastWakeUpDormantIndex;

		[Header("Cover System")]
		[SerializeField]
		public bool UseCover = true;

		public float CoverPointVolumeCellSize = 20f;

		public float CoverPointVolumeCellHeight = 8f;

		public float CoverPointRayLength = 1f;

		public CoverPointVolume cpvPrefab;

		[SerializeField]
		public LayerMask DynamicCoverPointVolumeLayerMask;

		private WorldSpaceGrid<CoverPointVolume> coverPointVolumeGrid;

		[ServerVar(Help="If true we'll wait for the navmesh to generate before completely starting the server. This might cause your server to hitch and lag as it generates in the background.")]
		public static bool nav_wait;

		[ServerVar(Help="If set to true the navmesh won't generate.. which means Ai that uses the navmesh won't be able to move")]
		public static bool nav_disable;

		[ServerVar(Help="If ai_dormant is true, any npc outside the range of players will render itself dormant and take up less resources, but wildlife won't simulate as well.")]
		public static bool ai_dormant;

		[ServerVar(Help="The maximum amount of nodes processed each frame in the asynchronous pathfinding process. Increasing this value will cause the paths to be processed faster, but can cause some hiccups in frame rate. Default value is 100, a good range for tuning is between 50 and 500.")]
		public static int pathfindingIterationsPerFrame;

		[ServerVar(Help="If an agent is beyond this distance to a player, it's flagged for becoming dormant.")]
		public static float ai_to_player_distance_wakeup_range;

		[ServerVar(Help="nav_obstacles_carve_state defines which obstacles can carve the terrain. 0 - No carving, 1 - Only player construction carves, 2 - All obstacles carve.")]
		public static int nav_obstacles_carve_state;

		[ServerVar(Help="ai_dormant_max_wakeup_per_tick defines the maximum number of dormant agents we will wake up in a single tick. (default: 30)")]
		public static int ai_dormant_max_wakeup_per_tick;

		[ServerVar(Help="ai_htn_player_tick_budget defines the maximum amount of milliseconds ticking htn player agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_player_tick_budget;

		[ServerVar(Help="ai_htn_player_junkpile_tick_budget defines the maximum amount of milliseconds ticking htn player junkpile agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_player_junkpile_tick_budget;

		[ServerVar(Help="ai_htn_animal_tick_budget defines the maximum amount of milliseconds ticking htn animal agents are allowed to consume. (default: 4 ms)")]
		public static float ai_htn_animal_tick_budget;

		[ServerVar(Help="If ai_htn_use_agency_tick is true, the ai manager's agency system will tick htn agents at the ms budgets defined in ai_htn_player_tick_budget and ai_htn_animal_tick_budget. If it's false, each agent registers with the invoke system individually, with no frame-budget restrictions. (default: true)")]
		public static bool ai_htn_use_agency_tick;

		private readonly BasePlayer[] playerVicinityQuery = new BasePlayer[1];

		private readonly Func<BasePlayer, bool> filter = new Func<BasePlayer, bool>(AiManager.InterestedInPlayersOnly);

		public AiManager.AgencyHTN HTNAgency { get; } = new AiManager.AgencyHTN();

		public bool repeat
		{
			get
			{
				return true;
			}
		}

		static AiManager()
		{
			AiManager.nav_wait = true;
			AiManager.nav_disable = false;
			AiManager.ai_dormant = true;
			AiManager.pathfindingIterationsPerFrame = 100;
			AiManager.ai_to_player_distance_wakeup_range = 160f;
			AiManager.nav_obstacles_carve_state = 2;
			AiManager.ai_dormant_max_wakeup_per_tick = 30;
			AiManager.ai_htn_player_tick_budget = 4f;
			AiManager.ai_htn_player_junkpile_tick_budget = 4f;
			AiManager.ai_htn_animal_tick_budget = 4f;
			AiManager.ai_htn_use_agency_tick = true;
		}

		public AiManager()
		{
		}

		public void Add(IAIAgent agent)
		{
			if (!AiManager.ai_dormant)
			{
				this.AddActiveAgency(agent);
				return;
			}
			if (this.IsAgentCloseToPlayers(agent))
			{
				this.AddActiveAgency(agent);
				return;
			}
			this.AddDormantAgency(agent);
		}

		internal void AddActiveAgency(IAIAgent agent)
		{
			if (this.pendingAddToActive.Contains(agent))
			{
				return;
			}
			this.pendingAddToActive.Add(agent);
		}

		internal void AddDormantAgency(IAIAgent agent)
		{
			if (this.pendingAddToDormant.Contains(agent))
			{
				return;
			}
			this.pendingAddToDormant.Add(agent);
		}

		private void AgencyAddPending()
		{
			if (AiManager.ai_dormant)
			{
				foreach (IAIAgent aIAgent in this.pendingAddToDormant)
				{
					if (aIAgent == null || aIAgent.Entity == null || aIAgent.Entity.IsDestroyed)
					{
						continue;
					}
					this.dormantAgents.Add(aIAgent);
					aIAgent.IsDormant = true;
				}
				this.pendingAddToDormant.Clear();
			}
			foreach (IAIAgent aIAgent1 in this.pendingAddToActive)
			{
				if (aIAgent1 == null || aIAgent1.Entity == null || aIAgent1.Entity.IsDestroyed || !this.activeAgents.Add(aIAgent1))
				{
					continue;
				}
				aIAgent1.IsDormant = false;
			}
			this.pendingAddToActive.Clear();
		}

		private void AgencyCleanup()
		{
			if (AiManager.ai_dormant)
			{
				foreach (IAIAgent aIAgent in this.pendingRemoveFromDormant)
				{
					if (aIAgent == null)
					{
						continue;
					}
					this.dormantAgents.Remove(aIAgent);
				}
				this.pendingRemoveFromDormant.Clear();
			}
			foreach (IAIAgent aIAgent1 in this.pendingRemoveFromActive)
			{
				if (aIAgent1 == null)
				{
					continue;
				}
				this.activeAgents.Remove(aIAgent1);
			}
			this.pendingRemoveFromActive.Clear();
		}

		public static CoverPointVolume CreateNewCoverVolume(Vector3 point, Transform coverPointGroup)
		{
			if (!(SingletonComponent<AiManager>.Instance != null) || !SingletonComponent<AiManager>.Instance.enabled || !SingletonComponent<AiManager>.Instance.UseCover)
			{
				return null;
			}
			CoverPointVolume coverVolumeContaining = SingletonComponent<AiManager>.Instance.GetCoverVolumeContaining(point);
			if (coverVolumeContaining == null)
			{
				Vector2i gridCoords = SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.WorldToGridCoords(point);
				coverVolumeContaining = (SingletonComponent<AiManager>.Instance.cpvPrefab == null ? (new GameObject("CoverPointVolume")).AddComponent<CoverPointVolume>() : UnityEngine.Object.Instantiate<CoverPointVolume>(SingletonComponent<AiManager>.Instance.cpvPrefab));
				coverVolumeContaining.transform.localPosition = new Vector3();
				coverVolumeContaining.transform.position = SingletonComponent<AiManager>.Instance.coverPointVolumeGrid.GridToWorldCoords(gridCoords) + (Vector3.up * point.y);
				coverVolumeContaining.transform.localScale = new Vector3(SingletonComponent<AiManager>.Instance.CoverPointVolumeCellSize, SingletonComponent<AiManager>.Instance.CoverPointVolumeCellHeight, SingletonComponent<AiManager>.Instance.CoverPointVolumeCellSize);
				coverVolumeContaining.CoverLayerMask = SingletonComponent<AiManager>.Instance.DynamicCoverPointVolumeLayerMask;
				coverVolumeContaining.CoverPointRayLength = SingletonComponent<AiManager>.Instance.CoverPointRayLength;
				SingletonComponent<AiManager>.Instance.coverPointVolumeGrid[gridCoords] = coverVolumeContaining;
				coverVolumeContaining.GenerateCoverPoints(coverPointGroup);
			}
			return coverVolumeContaining;
		}

		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			if (AiManager.nav_disable)
			{
				return new float?(nextInterval);
			}
			this.UpdateAgency();
			AiManager.AgencyHTN hTNAgency = this.HTNAgency;
			if (hTNAgency != null)
			{
				hTNAgency.UpdateAgency();
			}
			else
			{
			}
			return new float?(UnityEngine.Random.@value + 1f);
		}

		public CoverPointVolume GetCoverVolumeContaining(Vector3 point)
		{
			if (this.coverPointVolumeGrid == null)
			{
				return null;
			}
			Vector2i gridCoords = this.coverPointVolumeGrid.WorldToGridCoords(point);
			return this.coverPointVolumeGrid[gridCoords];
		}

		public void Initialize()
		{
			this.OnEnableAgency();
			if (this.UseCover)
			{
				this.OnEnableCover();
			}
			AiManagerLoadBalancer.aiManagerLoadBalancer.Add(this);
			if (this.HTNAgency != null)
			{
				this.HTNAgency.OnEnableAgency();
				if (AiManager.ai_htn_use_agency_tick)
				{
					InvokeHandler.InvokeRepeating(this, new Action(this.HTNAgency.InvokedTick), 0f, 0.033f);
				}
			}
		}

		private static bool InterestedInPlayersOnly(BaseEntity entity)
		{
			BasePlayer basePlayer = entity as BasePlayer;
			if (basePlayer == null)
			{
				return false;
			}
			if (basePlayer is IAIAgent)
			{
				return false;
			}
			if (!basePlayer.IsSleeping() && basePlayer.IsConnected)
			{
				return true;
			}
			return false;
		}

		private bool IsAgentCloseToPlayers(IAIAgent agent)
		{
			return BaseEntity.Query.Server.GetPlayersInSphere(agent.Entity.transform.position, AiManager.ai_to_player_distance_wakeup_range, this.playerVicinityQuery, this.filter) > 0;
		}

		private void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			this.OnDisableAgency();
			if (this.UseCover)
			{
				this.OnDisableCover();
			}
			AiManagerLoadBalancer.aiManagerLoadBalancer.Remove(this);
			if (this.HTNAgency != null)
			{
				this.HTNAgency.OnDisableAgency();
				if (AiManager.ai_htn_use_agency_tick)
				{
					InvokeHandler.CancelInvoke(this, new Action(this.HTNAgency.InvokedTick));
				}
			}
		}

		internal void OnDisableAgency()
		{
		}

		internal void OnDisableCover()
		{
			if (this.coverPointVolumeGrid == null || this.coverPointVolumeGrid.Cells == null)
			{
				return;
			}
			for (int i = 0; i < (int)this.coverPointVolumeGrid.Cells.Length; i++)
			{
				UnityEngine.Object.Destroy(this.coverPointVolumeGrid.Cells[i]);
			}
		}

		internal void OnEnableAgency()
		{
		}

		internal void OnEnableCover()
		{
			if (this.coverPointVolumeGrid == null)
			{
				Vector3 size = TerrainMeta.Size;
				this.coverPointVolumeGrid = new WorldSpaceGrid<CoverPointVolume>(size.x, this.CoverPointVolumeCellSize);
			}
		}

		public void Remove(IAIAgent agent)
		{
			this.RemoveActiveAgency(agent);
			if (AiManager.ai_dormant)
			{
				this.RemoveDormantAgency(agent);
			}
		}

		internal void RemoveActiveAgency(IAIAgent agent)
		{
			if (this.pendingRemoveFromActive.Contains(agent))
			{
				return;
			}
			this.pendingRemoveFromActive.Add(agent);
		}

		internal void RemoveDormantAgency(IAIAgent agent)
		{
			if (this.pendingRemoveFromDormant.Contains(agent))
			{
				return;
			}
			this.pendingRemoveFromDormant.Add(agent);
		}

		private void TryMakeAgentsDormant()
		{
			if (!AiManager.ai_dormant)
			{
				return;
			}
			foreach (IAIAgent activeAgent in this.activeAgents)
			{
				if (!activeAgent.Entity.IsDestroyed)
				{
					if (this.IsAgentCloseToPlayers(activeAgent))
					{
						continue;
					}
					this.AddDormantAgency(activeAgent);
					this.RemoveActiveAgency(activeAgent);
				}
				else
				{
					this.RemoveActiveAgency(activeAgent);
				}
			}
		}

		private void TryWakeUpDormantAgents()
		{
			if (!AiManager.ai_dormant || this.dormantAgents.Count == 0)
			{
				return;
			}
			if (this.lastWakeUpDormantIndex >= this.dormantAgents.Count)
			{
				this.lastWakeUpDormantIndex = 0;
			}
			int num = this.lastWakeUpDormantIndex;
			int num1 = 0;
			while (num1 < AiManager.ai_dormant_max_wakeup_per_tick)
			{
				if (this.lastWakeUpDormantIndex >= this.dormantAgents.Count)
				{
					this.lastWakeUpDormantIndex = 0;
				}
				if (this.lastWakeUpDormantIndex == num && num1 > 0)
				{
					break;
				}
				IAIAgent item = this.dormantAgents[this.lastWakeUpDormantIndex];
				this.lastWakeUpDormantIndex++;
				num1++;
				if (!item.Entity.IsDestroyed)
				{
					if (!this.IsAgentCloseToPlayers(item))
					{
						continue;
					}
					this.AddActiveAgency(item);
					this.RemoveDormantAgency(item);
				}
				else
				{
					this.RemoveDormantAgency(item);
				}
			}
		}

		internal void UpdateAgency()
		{
			this.AgencyCleanup();
			this.AgencyAddPending();
			if (AiManager.ai_dormant)
			{
				this.TryWakeUpDormantAgents();
				this.TryMakeAgentsDormant();
			}
		}

		public class AgencyHTN
		{
			private readonly HashSet<IHTNAgent> activeAgents;

			private readonly List<IHTNAgent> dormantAgents;

			private readonly HashSet<IHTNAgent> pendingAddToActive;

			private readonly HashSet<IHTNAgent> pendingAddToDormant;

			private readonly HashSet<IHTNAgent> pendingRemoveFromActive;

			private readonly HashSet<IHTNAgent> pendingRemoveFromDormant;

			private readonly List<HTNPlayer> tickingPlayers;

			private readonly List<HTNPlayer> tickingJunkpilePlayers;

			private readonly List<HTNAnimal> tickingAnimals;

			private int playerTickIndex;

			private int junkpilePlayerTickIndex;

			private int animalTickIndex;

			private Stopwatch watch;

			private int lastWakeUpDormantIndex;

			private readonly BasePlayer[] playerVicinityQuery;

			private readonly Func<BasePlayer, bool> filter;

			public AgencyHTN()
			{
			}

			public void Add(IHTNAgent agent)
			{
				if (!AiManager.ai_dormant)
				{
					this.AddActiveAgency(agent);
					return;
				}
				if (this.IsAgentCloseToPlayers(agent))
				{
					this.AddActiveAgency(agent);
					return;
				}
				this.AddDormantAgency(agent);
			}

			internal void AddActiveAgency(IHTNAgent agent)
			{
				if (this.pendingAddToActive.Contains(agent))
				{
					return;
				}
				this.pendingAddToActive.Add(agent);
			}

			internal void AddDormantAgency(IHTNAgent agent)
			{
				if (this.pendingAddToDormant.Contains(agent))
				{
					return;
				}
				this.pendingAddToDormant.Add(agent);
			}

			private void AgencyAddPending()
			{
				if (AiManager.ai_dormant)
				{
					foreach (IHTNAgent hTNAgent in this.pendingAddToDormant)
					{
						if (hTNAgent == null || hTNAgent.IsDestroyed)
						{
							continue;
						}
						this.dormantAgents.Add(hTNAgent);
						hTNAgent.IsDormant = true;
					}
					this.pendingAddToDormant.Clear();
				}
				foreach (IHTNAgent hTNAgent1 in this.pendingAddToActive)
				{
					if (hTNAgent1 == null || hTNAgent1.IsDestroyed || !this.activeAgents.Add(hTNAgent1))
					{
						continue;
					}
					hTNAgent1.IsDormant = false;
					HTNPlayer hTNPlayer = hTNAgent1 as HTNPlayer;
					if (!hTNPlayer)
					{
						HTNAnimal hTNAnimal = hTNAgent1 as HTNAnimal;
						if (!hTNAnimal)
						{
							continue;
						}
						this.tickingAnimals.Add(hTNAnimal);
					}
					else if (!(hTNPlayer.AiDomain is ScientistJunkpileDomain))
					{
						this.tickingPlayers.Add(hTNPlayer);
					}
					else
					{
						this.tickingJunkpilePlayers.Add(hTNPlayer);
					}
				}
				this.pendingAddToActive.Clear();
			}

			private void AgencyCleanup()
			{
				if (AiManager.ai_dormant)
				{
					foreach (IHTNAgent hTNAgent in this.pendingRemoveFromDormant)
					{
						if (hTNAgent == null)
						{
							continue;
						}
						this.dormantAgents.Remove(hTNAgent);
					}
					this.pendingRemoveFromDormant.Clear();
				}
				foreach (IHTNAgent hTNAgent1 in this.pendingRemoveFromActive)
				{
					if (hTNAgent1 == null)
					{
						continue;
					}
					this.activeAgents.Remove(hTNAgent1);
					HTNPlayer hTNPlayer = hTNAgent1 as HTNPlayer;
					if (!hTNPlayer)
					{
						HTNAnimal hTNAnimal = hTNAgent1 as HTNAnimal;
						if (!hTNAnimal)
						{
							continue;
						}
						this.tickingAnimals.Remove(hTNAnimal);
					}
					else if (!(hTNPlayer.AiDomain is ScientistJunkpileDomain))
					{
						this.tickingPlayers.Remove(hTNPlayer);
					}
					else
					{
						this.tickingJunkpilePlayers.Remove(hTNPlayer);
					}
				}
				this.pendingRemoveFromActive.Clear();
			}

			public void InvokedTick()
			{
				this.watch.Reset();
				this.watch.Start();
				int num = this.playerTickIndex;
				do
				{
					if (this.tickingPlayers.Count <= 0)
					{
						break;
					}
					if (this.playerTickIndex >= this.tickingPlayers.Count)
					{
						this.playerTickIndex = 0;
					}
					HTNPlayer item = this.tickingPlayers[this.playerTickIndex];
					if (item != null && item.transform != null && !item.IsDestroyed)
					{
						item.Tick();
					}
					this.playerTickIndex++;
					if (this.playerTickIndex < this.tickingPlayers.Count)
					{
						continue;
					}
					this.playerTickIndex = 0;
				}
				while (this.playerTickIndex != num && this.watch.Elapsed.TotalMilliseconds <= (double)AiManager.ai_htn_player_tick_budget);
				this.watch.Reset();
				this.watch.Start();
				num = this.junkpilePlayerTickIndex;
				do
				{
					if (this.tickingJunkpilePlayers.Count <= 0)
					{
						break;
					}
					if (this.junkpilePlayerTickIndex >= this.tickingJunkpilePlayers.Count)
					{
						this.junkpilePlayerTickIndex = 0;
					}
					HTNPlayer hTNPlayer = this.tickingJunkpilePlayers[this.junkpilePlayerTickIndex];
					if (hTNPlayer != null && hTNPlayer.transform != null && !hTNPlayer.IsDestroyed)
					{
						hTNPlayer.Tick();
					}
					this.junkpilePlayerTickIndex++;
					if (this.junkpilePlayerTickIndex < this.tickingJunkpilePlayers.Count)
					{
						continue;
					}
					this.junkpilePlayerTickIndex = 0;
				}
				while (this.junkpilePlayerTickIndex != num && this.watch.Elapsed.TotalMilliseconds <= (double)AiManager.ai_htn_player_junkpile_tick_budget);
				this.watch.Reset();
				this.watch.Start();
				num = this.animalTickIndex;
				do
				{
					if (this.tickingAnimals.Count <= 0)
					{
						break;
					}
					if (this.animalTickIndex >= this.tickingAnimals.Count)
					{
						this.animalTickIndex = 0;
					}
					HTNAnimal hTNAnimal = this.tickingAnimals[this.animalTickIndex];
					if (hTNAnimal != null && hTNAnimal.transform != null && !hTNAnimal.IsDestroyed)
					{
						hTNAnimal.Tick();
					}
					this.animalTickIndex++;
					if (this.animalTickIndex < this.tickingAnimals.Count)
					{
						continue;
					}
					this.animalTickIndex = 0;
				}
				while (this.animalTickIndex != num && this.watch.Elapsed.TotalMilliseconds <= (double)AiManager.ai_htn_animal_tick_budget);
			}

			private bool IsAgentCloseToPlayers(IHTNAgent agent)
			{
				return BaseEntity.Query.Server.GetPlayersInSphere(agent.transform.position, AiManager.ai_to_player_distance_wakeup_range, this.playerVicinityQuery, this.filter) > 0;
			}

			internal void OnDisableAgency()
			{
			}

			internal void OnEnableAgency()
			{
			}

			public void Remove(IHTNAgent agent)
			{
				this.RemoveActiveAgency(agent);
				if (AiManager.ai_dormant)
				{
					this.RemoveDormantAgency(agent);
				}
			}

			internal void RemoveActiveAgency(IHTNAgent agent)
			{
				if (this.pendingRemoveFromActive.Contains(agent))
				{
					return;
				}
				this.pendingRemoveFromActive.Add(agent);
			}

			internal void RemoveDormantAgency(IHTNAgent agent)
			{
				if (this.pendingRemoveFromDormant.Contains(agent))
				{
					return;
				}
				this.pendingRemoveFromDormant.Add(agent);
			}

			private void TryMakeAgentsDormant()
			{
				if (!AiManager.ai_dormant)
				{
					return;
				}
				foreach (IHTNAgent activeAgent in this.activeAgents)
				{
					if (!activeAgent.IsDestroyed)
					{
						if (this.IsAgentCloseToPlayers(activeAgent))
						{
							continue;
						}
						this.AddDormantAgency(activeAgent);
						this.RemoveActiveAgency(activeAgent);
					}
					else
					{
						this.RemoveActiveAgency(activeAgent);
					}
				}
			}

			private void TryWakeUpDormantAgents()
			{
				if (!AiManager.ai_dormant || this.dormantAgents.Count == 0)
				{
					return;
				}
				if (this.lastWakeUpDormantIndex >= this.dormantAgents.Count)
				{
					this.lastWakeUpDormantIndex = 0;
				}
				int num = this.lastWakeUpDormantIndex;
				int num1 = 0;
				while (num1 < AiManager.ai_dormant_max_wakeup_per_tick)
				{
					if (this.lastWakeUpDormantIndex >= this.dormantAgents.Count)
					{
						this.lastWakeUpDormantIndex = 0;
					}
					if (this.lastWakeUpDormantIndex == num && num1 > 0)
					{
						break;
					}
					IHTNAgent item = this.dormantAgents[this.lastWakeUpDormantIndex];
					this.lastWakeUpDormantIndex++;
					num1++;
					if (!item.IsDestroyed)
					{
						if (!this.IsAgentCloseToPlayers(item))
						{
							continue;
						}
						this.AddActiveAgency(item);
						this.RemoveDormantAgency(item);
					}
					else
					{
						this.RemoveDormantAgency(item);
					}
				}
			}

			internal void UpdateAgency()
			{
				this.AgencyCleanup();
				this.AgencyAddPending();
				if (AiManager.ai_dormant)
				{
					this.TryWakeUpDormantAgents();
					this.TryMakeAgentsDormant();
				}
			}
		}
	}
}