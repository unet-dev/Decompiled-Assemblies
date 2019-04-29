using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColliderGrid : SingletonComponent<ColliderGrid>, IServerComponent
{
	public static bool Paused;

	public GameObjectRef BatchPrefab;

	public float CellSize = 50f;

	public float MaxMilliseconds = 0.1f;

	private WorldSpaceGrid<ColliderCell> grid;

	private Stopwatch watch = Stopwatch.StartNew();

	public ColliderCell this[Vector3 worldPos]
	{
		get
		{
			if (this.grid == null)
			{
				this.Init();
			}
			return this.grid[worldPos];
		}
	}

	public bool NeedsTimeout
	{
		get
		{
			return this.watch.Elapsed.TotalMilliseconds > (double)this.MaxMilliseconds;
		}
	}

	static ColliderGrid()
	{
	}

	public ColliderGrid()
	{
	}

	public int BatchedMeshCount()
	{
		if (this.grid == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				num += this.grid[i, j].BatchedMeshCount();
			}
		}
		return num;
	}

	public MeshColliderBatch CreateInstance()
	{
		GameObject gameObject = GameManager.server.CreatePrefab(this.BatchPrefab.resourcePath, true);
		SceneManager.MoveGameObjectToScene(gameObject, Generic.BatchingScene);
		return gameObject.GetComponent<MeshColliderBatch>();
	}

	private void Init()
	{
		this.grid = new WorldSpaceGrid<ColliderCell>(TerrainMeta.Size.x * 2f, this.CellSize);
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				this.grid[i, j] = new ColliderCell(this, this.grid.GridToWorldCoords(new Vector2i(i, j)));
			}
		}
	}

	public int MeshCount()
	{
		if (this.grid == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				num += this.grid[i, j].MeshCount();
			}
		}
		return num;
	}

	protected void OnEnable()
	{
		base.StartCoroutine(this.UpdateCoroutine());
	}

	public void RecycleInstance(MeshColliderBatch instance)
	{
		GameManager.server.Retire(instance.gameObject);
	}

	public void Refresh()
	{
		if (this.grid == null)
		{
			this.Init();
		}
		for (int i = 0; i < this.grid.CellCount; i++)
		{
			for (int j = 0; j < this.grid.CellCount; j++)
			{
				this.grid[i, j].Refresh();
			}
		}
	}

	public static void RefreshAll()
	{
		if (SingletonComponent<ColliderGrid>.Instance)
		{
			SingletonComponent<ColliderGrid>.Instance.Refresh();
		}
	}

	public void ResetTimeout()
	{
		this.watch.Reset();
		this.watch.Start();
	}

	private IEnumerator UpdateCoroutine()
	{
		ColliderGrid colliderGrid = null;
		while (true)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (!Rust.Application.isReceiving && !Rust.Application.isLoading && !ColliderGrid.Paused && colliderGrid.grid != null)
			{
				colliderGrid.ResetTimeout();
				for (int i = 0; i < colliderGrid.grid.CellCount; i++)
				{
					for (int j = 0; j < colliderGrid.grid.CellCount; j++)
					{
						ColliderCell item = colliderGrid.grid[i, j];
						if (item.NeedsRefresh())
						{
							IEnumerator enumerator = item.RefreshAsync();
							while (enumerator.MoveNext())
							{
								yield return enumerator.Current;
							}
							enumerator = null;
						}
						if (colliderGrid.NeedsTimeout)
						{
							yield return CoroutineEx.waitForEndOfFrame;
							colliderGrid.ResetTimeout();
						}
					}
				}
			}
		}
	}
}