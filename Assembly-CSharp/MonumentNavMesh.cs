using ConVar;
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

public class MonumentNavMesh : FacepunchBehaviour, IServerComponent
{
	public int NavMeshAgentTypeIndex;

	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "HumanNPC";

	[Tooltip("How many cells to use squared")]
	public int CellCount = 1;

	[Tooltip("The size of each cell for async object gathering")]
	public int CellSize = 80;

	public int Height = 100;

	public float NavmeshResolutionModifier = 0.5f;

	[Tooltip("Use the bounds specified in editor instead of generating it from cellsize * cellcount")]
	public bool overrideAutoBounds;

	[Tooltip("Bounds which are auto calculated from CellSize * CellCount")]
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

	public MonumentNavMesh()
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
			if (!this.Bounds.Contains(vector3))
			{
				continue;
			}
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
		MonumentNavMesh monumentNavMesh = null;
		float single = UnityEngine.Time.realtimeSinceStartup;
		UnityEngine.Debug.Log("Starting Navmesh Source Collecting.");
		List<NavMeshBuildMarkup> navMeshBuildMarkups = new List<NavMeshBuildMarkup>();
		NavMeshBuilder.CollectSources(monumentNavMesh.Bounds, monumentNavMesh.LayerMask, monumentNavMesh.NavMeshCollectGeometry, monumentNavMesh.defaultArea, navMeshBuildMarkups, monumentNavMesh.sources);
		if (TerrainMeta.HeightMap != null)
		{
			for (float i = -monumentNavMesh.Bounds.extents.x; i < monumentNavMesh.Bounds.extents.x; i += (float)monumentNavMesh.CellSize)
			{
				for (float j = -monumentNavMesh.Bounds.extents.z; j < monumentNavMesh.Bounds.extents.z; j += (float)monumentNavMesh.CellSize)
				{
					AsyncTerrainNavMeshBake asyncTerrainNavMeshBake = new AsyncTerrainNavMeshBake(monumentNavMesh.Bounds.center + new Vector3(i, 0f, j), monumentNavMesh.CellSize, monumentNavMesh.Height, false, true);
					yield return asyncTerrainNavMeshBake;
					monumentNavMesh.terrainBakes.Add(asyncTerrainNavMeshBake);
					NavMeshBuildSource navMeshBuildSource = asyncTerrainNavMeshBake.CreateNavMeshBuildSource(true);
					navMeshBuildSource.area = monumentNavMesh.defaultArea;
					monumentNavMesh.sources.Add(navMeshBuildSource);
					asyncTerrainNavMeshBake = null;
				}
			}
		}
		monumentNavMesh.AppendModifierVolumes(ref monumentNavMesh.sources);
		float single1 = UnityEngine.Time.realtimeSinceStartup - single;
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
		UnityEngine.Debug.Log(string.Format("Monument Navmesh Build took {0:0.00} seconds", elapsed.TotalSeconds));
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

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawCube(base.transform.position, this.Bounds.size);
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
		MonumentNavMesh vector3 = null;
		if (vector3.HasBuildOperationStarted)
		{
			yield break;
		}
		if (AiManager.nav_disable || !AI.npc_enable)
		{
			yield break;
		}
		vector3.HasBuildOperationStarted = false;
		vector3.Bounds.center = vector3.transform.position;
		if (!vector3.overrideAutoBounds)
		{
			vector3.Bounds.size = new Vector3((float)(vector3.CellSize * vector3.CellCount), (float)vector3.Height, (float)(vector3.CellSize * vector3.CellCount));
		}
		if (!AiManager.nav_wait)
		{
			vector3.StartCoroutine(vector3.CollectSourcesAsync(new Action(vector3.UpdateNavMeshAsync)));
		}
		else
		{
			yield return vector3.CollectSourcesAsync(new Action(vector3.UpdateNavMeshAsync));
		}
		if (!AiManager.nav_wait)
		{
			UnityEngine.Debug.Log("nav_wait is false, so we're not waiting for the navmesh to finish generating. This might cause your server to sputter while it's generating.");
			yield break;
		}
		int num = 0;
		while (!vector3.HasBuildOperationStarted)
		{
			Thread.Sleep(250);
			yield return null;
		}
		while (vector3.BuildingOperation != null)
		{
			int buildingOperation = (int)(vector3.BuildingOperation.progress * 100f);
			if (num != buildingOperation)
			{
				UnityEngine.Debug.LogFormat("{0}%", new object[] { buildingOperation });
				num = buildingOperation;
			}
			Thread.Sleep(250);
			vector3.FinishBuildingNavmesh();
			yield return null;
		}
	}

	[ContextMenu("Update Monument Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		if (this.HasBuildOperationStarted)
		{
			return;
		}
		if (AiManager.nav_disable || !AI.npc_enable)
		{
			return;
		}
		float single = UnityEngine.Time.realtimeSinceStartup;
		UnityEngine.Debug.Log(string.Concat("Starting Monument Navmesh Build with ", this.sources.Count, " sources"));
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(this.NavMeshAgentTypeIndex);
		settingsByIndex.overrideVoxelSize = true;
		settingsByIndex.voxelSize = settingsByIndex.voxelSize * this.NavmeshResolutionModifier;
		this.BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(this.NavMeshData, settingsByIndex, this.sources, this.Bounds);
		this.BuildTimer.Reset();
		this.BuildTimer.Start();
		this.HasBuildOperationStarted = true;
		float single1 = UnityEngine.Time.realtimeSinceStartup - single;
		if (single1 > 0.1f)
		{
			UnityEngine.Debug.LogWarning(string.Concat("Calling UpdateNavMesh took ", single1));
		}
	}
}