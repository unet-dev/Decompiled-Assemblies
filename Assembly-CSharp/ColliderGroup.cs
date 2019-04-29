using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderGroup : Pool.IPooled
{
	public bool Invalidated;

	public bool NeedsRefresh;

	public bool Processing;

	public bool Preserving;

	public ListHashSet<ColliderBatch> Colliders = new ListHashSet<ColliderBatch>(8);

	public List<ColliderBatch> TempColliders = new List<ColliderBatch>();

	public List<MeshColliderBatch> Batches = new List<MeshColliderBatch>();

	public List<MeshColliderBatch> TempBatches = new List<MeshColliderBatch>();

	public List<MeshColliderInstance> TempInstances = new List<MeshColliderInstance>();

	private ColliderGrid grid;

	private ColliderCell cell;

	private ColliderKey key;

	private Action updateData;

	private Action refreshBatches;

	public int Count
	{
		get
		{
			return this.Colliders.Count;
		}
	}

	public Vector3 Position
	{
		get
		{
			return this.cell.position;
		}
	}

	public float Size
	{
		get
		{
			return this.grid.CellSize;
		}
	}

	public ColliderGroup()
	{
	}

	public void Add(ColliderBatch collider)
	{
		this.Colliders.Add(collider);
		this.NeedsRefresh = true;
	}

	public void Add(MeshColliderInstance instance)
	{
		this.TempInstances.Add(instance);
	}

	public void ApplyBatches()
	{
		for (int i = 0; i < this.TempBatches.Count && !this.cell.interrupt; i++)
		{
			this.TempBatches[i].Apply();
		}
	}

	public int BatchedMeshCount()
	{
		int batchedCount = 0;
		for (int i = 0; i < this.Batches.Count; i++)
		{
			batchedCount += this.Batches[i].BatchedCount;
		}
		return batchedCount;
	}

	public void Cancel()
	{
		for (int i = 0; i < this.TempBatches.Count; i++)
		{
			this.grid.RecycleInstance(this.TempBatches[i]);
		}
		this.TempBatches.Clear();
	}

	public void Clear()
	{
		for (int i = 0; i < this.Batches.Count; i++)
		{
			this.grid.RecycleInstance(this.Batches[i]);
		}
		this.Batches.Clear();
	}

	public MeshColliderBatch CreateBatch()
	{
		MeshColliderBatch meshColliderBatch = this.grid.CreateInstance();
		meshColliderBatch.Setup(this.cell.position, this.key.layer, this.key.material);
		meshColliderBatch.Alloc();
		this.TempBatches.Add(meshColliderBatch);
		return meshColliderBatch;
	}

	public void CreateBatches()
	{
		if (this.TempInstances.Count == 0)
		{
			return;
		}
		MeshColliderBatch meshColliderBatch = this.CreateBatch();
		for (int i = 0; i < this.TempInstances.Count; i++)
		{
			MeshColliderInstance item = this.TempInstances[i];
			if (meshColliderBatch.AvailableVertices < item.mesh.vertexCount)
			{
				meshColliderBatch = this.CreateBatch();
			}
			meshColliderBatch.Add(item);
		}
		this.TempInstances.Clear();
	}

	public void DisplayBatches()
	{
		for (int i = 0; i < this.TempBatches.Count && !this.cell.interrupt; i++)
		{
			this.TempBatches[i].Display();
		}
	}

	public void End()
	{
		if (!this.Processing)
		{
			this.Preserving = false;
			return;
		}
		if (this.cell.interrupt)
		{
			this.Cancel();
		}
		else
		{
			this.Clear();
			for (int i = 0; i < this.TempBatches.Count; i++)
			{
				this.TempBatches[i].Free();
			}
			List<MeshColliderBatch> batches = this.Batches;
			this.Batches = this.TempBatches;
			this.TempBatches = batches;
			this.Invalidated = false;
		}
		this.TempColliders.Clear();
		this.Processing = false;
	}

	public void EnterPool()
	{
		this.Invalidated = false;
		this.NeedsRefresh = false;
		this.Processing = false;
		this.Preserving = false;
		this.Colliders.Clear();
		this.TempColliders.Clear();
		this.Batches.Clear();
		this.TempBatches.Clear();
		this.TempInstances.Clear();
		this.grid = null;
		this.cell = null;
		this.key = new ColliderKey();
	}

	public void Initialize(ColliderGrid grid, ColliderCell cell, ColliderKey key)
	{
		this.grid = grid;
		this.cell = cell;
		this.key = key;
	}

	public void Invalidate()
	{
		if (!this.Invalidated)
		{
			for (int i = 0; i < this.Batches.Count; i++)
			{
				this.Batches[i].Invalidate();
			}
			this.Invalidated = true;
		}
		this.cell.interrupt = true;
	}

	public void LeavePool()
	{
	}

	public int MeshCount()
	{
		int count = 0;
		for (int i = 0; i < this.Batches.Count; i++)
		{
			count += this.Batches[i].Count;
		}
		return count;
	}

	public void RefreshBatches()
	{
		for (int i = 0; i < this.TempBatches.Count && !this.cell.interrupt; i++)
		{
			this.TempBatches[i].Refresh();
		}
	}

	public IEnumerator RefreshBatchesAsync()
	{
		if (this.refreshBatches == null)
		{
			this.refreshBatches = new Action(this.RefreshBatches);
		}
		return Parallel.Coroutine(this.refreshBatches);
	}

	public void Remove(ColliderBatch collider)
	{
		this.Colliders.Remove(collider);
		this.NeedsRefresh = true;
	}

	public void Start()
	{
		if (!this.NeedsRefresh)
		{
			this.Preserving = true;
			return;
		}
		this.Processing = true;
		this.TempColliders.Clear();
		this.TempColliders.AddRange(this.Colliders.Values);
		this.NeedsRefresh = false;
	}

	public void UpdateData()
	{
		this.TempInstances.Clear();
		for (int i = 0; i < this.TempColliders.Count && !this.cell.interrupt; i++)
		{
			this.TempColliders[i].AddBatch(this);
		}
	}

	public IEnumerator UpdateDataAsync()
	{
		if (this.updateData == null)
		{
			this.updateData = new Action(this.UpdateData);
		}
		return Parallel.Coroutine(this.updateData);
	}
}