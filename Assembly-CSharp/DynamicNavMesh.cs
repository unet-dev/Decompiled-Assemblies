using Rust;
using Rust.Ai;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class DynamicNavMesh : SingletonComponent<DynamicNavMesh>, IServerComponent
{
	public int NavMeshAgentTypeIndex;

	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "Walkable";

	public int AsyncTerrainNavMeshBakeCellSize = 80;

	public int AsyncTerrainNavMeshBakeCellHeight = 100;

	public UnityEngine.Bounds Bounds;

	public UnityEngine.AI.NavMeshData NavMeshData;

	public UnityEngine.AI.NavMeshDataInstance NavMeshDataInstance;

	public UnityEngine.LayerMask LayerMask;

	public UnityEngine.AI.NavMeshCollectGeometry NavMeshCollectGeometry;

	private List<AsyncTerrainNavMeshBake> terrainBakes;

	private List<NavMeshBuildSource> sources;

	private AsyncOperation BuildingOperation;

	private bool HasBuildOperationStarted;

	private Stopwatch BuildTimer = new Stopwatch();

	private int defaultArea;

	private int agentTypeId;

	public bool IsBuilding
	{
		get
		{
			if (this.HasBuildOperationStarted && this.BuildingOperation == null)
			{
				return false;
			}
			return true;
		}
	}

	public DynamicNavMesh()
	{
	}

	private void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
	{
		foreach (NavMeshModifierVolume activeModifier in NavMeshModifierVolume.activeModifiers)
		{
			if ((this.LayerMask & 1 << (activeModifier.gameObject.layer & 31)) == 0 || !activeModifier.AffectsAgentType(this.agentTypeId))
			{
				continue;
			}
			Vector3 vector3 = activeModifier.transform.TransformPoint(activeModifier.center);
			Vector3 vector31 = activeModifier.transform.lossyScale;
			Vector3 vector32 = new Vector3(activeModifier.size.x * Mathf.Abs(vector31.x), activeModifier.size.y * Mathf.Abs(vector31.y), activeModifier.size.z * Mathf.Abs(vector31.z));
			NavMeshBuildSource navMeshBuildSource = new NavMeshBuildSource()
			{
				shape = NavMeshBuildSourceShape.ModifierBox,
				transform = Matrix4x4.TRS(vector3, activeModifier.transform.rotation, Vector3.one),
				size = vector32,
				area = activeModifier.area
			};
			sources.Add(navMeshBuildSource);
		}
	}

	private IEnumerator CollectSourcesAsync(Action callback)
	{
		DynamicNavMesh dynamicNavMesh = null;
		float single = Time.realtimeSinceStartup;
		UnityEngine.Debug.Log("Starting Navmesh Source Collecting.");
		List<NavMeshBuildMarkup> navMeshBuildMarkups = new List<NavMeshBuildMarkup>();
		NavMeshBuilder.CollectSources(dynamicNavMesh.Bounds, dynamicNavMesh.LayerMask, dynamicNavMesh.NavMeshCollectGeometry, dynamicNavMesh.defaultArea, navMeshBuildMarkups, dynamicNavMesh.sources);
		if (TerrainMeta.HeightMap != null)
		{
			for (float i = -dynamicNavMesh.Bounds.extents.x; i < dynamicNavMesh.Bounds.extents.x; i += (float)dynamicNavMesh.AsyncTerrainNavMeshBakeCellSize)
			{
				for (float j = -dynamicNavMesh.Bounds.extents.z; j < dynamicNavMesh.Bounds.extents.z; j += (float)dynamicNavMesh.AsyncTerrainNavMeshBakeCellSize)
				{
					AsyncTerrainNavMeshBake asyncTerrainNavMeshBake = new AsyncTerrainNavMeshBake(new Vector3(i, 0f, j), dynamicNavMesh.AsyncTerrainNavMeshBakeCellSize, dynamicNavMesh.AsyncTerrainNavMeshBakeCellHeight, false, true);
					yield return asyncTerrainNavMeshBake;
					dynamicNavMesh.terrainBakes.Add(asyncTerrainNavMeshBake);
					dynamicNavMesh.sources.Add(asyncTerrainNavMeshBake.CreateNavMeshBuildSource(true));
					asyncTerrainNavMeshBake = null;
				}
			}
		}
		dynamicNavMesh.AppendModifierVolumes(ref dynamicNavMesh.sources);
		float single1 = Time.realtimeSinceStartup - single;
		if (single1 > 0.1f)
		{
			UnityEngine.Debug.LogWarning(string.Concat("Calling CollectSourcesAsync took ", single1));
		}
		if (callback != null)
		{
			callback();
		}
	}

	public void FinishBuildingNavmesh()
	{
		if (this.BuildingOperation == null)
		{
			return;
		}
		if (!this.BuildingOperation.isDone)
		{
			return;
		}
		if (!this.NavMeshDataInstance.valid)
		{
			this.NavMeshDataInstance = NavMesh.AddNavMeshData(this.NavMeshData);
		}
		TimeSpan elapsed = this.BuildTimer.Elapsed;
		UnityEngine.Debug.Log(string.Format("Navmesh Build took {0:0.00} seconds", elapsed.TotalSeconds));
		this.BuildingOperation = null;
	}

	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		base.CancelInvoke(new Action(this.FinishBuildingNavmesh));
		this.NavMeshDataInstance.Remove();
	}

	private void OnEnable()
	{
		this.agentTypeId = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex).agentTypeID;
		this.NavMeshData = new UnityEngine.AI.NavMeshData(this.agentTypeId);
		this.sources = new List<NavMeshBuildSource>();
		this.terrainBakes = new List<AsyncTerrainNavMeshBake>();
		this.defaultArea = NavMesh.GetAreaFromName(this.DefaultAreaName);
		base.InvokeRepeating(new Action(this.FinishBuildingNavmesh), 0f, 1f);
	}

	public IEnumerator UpdateNavMeshAndWait()
	{
		DynamicNavMesh size = null;
		if (size.HasBuildOperationStarted)
		{
			yield break;
		}
		if (AiManager.nav_disable)
		{
			yield break;
		}
		size.HasBuildOperationStarted = false;
		size.Bounds.size = TerrainMeta.Size;
		NavMesh.pathfindingIterationsPerFrame = AiManager.pathfindingIterationsPerFrame;
		if (!AiManager.nav_wait)
		{
			size.StartCoroutine(size.CollectSourcesAsync(new Action(size.UpdateNavMeshAsync)));
		}
		else
		{
			yield return size.CollectSourcesAsync(new Action(size.UpdateNavMeshAsync));
		}
		if (!AiManager.nav_wait)
		{
			UnityEngine.Debug.Log("nav_wait is false, so we're not waiting for the navmesh to finish generating. This might cause your server to sputter while it's generating.");
			yield break;
		}
		int num = 0;
		while (!size.HasBuildOperationStarted)
		{
			Thread.Sleep(250);
			yield return null;
		}
		while (size.BuildingOperation != null)
		{
			int buildingOperation = (int)(size.BuildingOperation.progress * 100f);
			if (num != buildingOperation)
			{
				UnityEngine.Debug.LogFormat("{0}%", new object[] { buildingOperation });
				num = buildingOperation;
			}
			Thread.Sleep(250);
			size.FinishBuildingNavmesh();
			yield return null;
		}
	}

	[ContextMenu("Update Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		if (this.HasBuildOperationStarted)
		{
			return;
		}
		if (AiManager.nav_disable)
		{
			return;
		}
		float single = Time.realtimeSinceStartup;
		UnityEngine.Debug.Log(string.Concat("Starting Navmesh Build with ", this.sources.Count, " sources"));
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex);
		settingsByIndex.overrideVoxelSize = true;
		settingsByIndex.voxelSize = settingsByIndex.voxelSize * 2f;
		this.BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(this.NavMeshData, settingsByIndex, this.sources, this.Bounds);
		this.BuildTimer.Reset();
		this.BuildTimer.Start();
		this.HasBuildOperationStarted = true;
		float single1 = Time.realtimeSinceStartup - single;
		if (single1 > 0.1f)
		{
			UnityEngine.Debug.LogWarning(string.Concat("Calling UpdateNavMesh took ", single1));
		}
	}
}