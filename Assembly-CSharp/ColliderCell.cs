using ConVar;
using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ColliderCell
{
	public Vector3 position;

	public ColliderGrid grid;

	public bool interrupt;

	private ListDictionary<ColliderKey, ColliderGroup> batches = new ListDictionary<ColliderKey, ColliderGroup>(8);

	public ColliderCell(ColliderGrid grid, Vector3 position)
	{
		this.grid = grid;
		this.position = position;
	}

	public int BatchedMeshCount()
	{
		int num = 0;
		BufferList<ColliderGroup> values = this.batches.Values;
		for (int i = 0; i < values.Count; i++)
		{
			num += values[i].BatchedMeshCount();
		}
		return num;
	}

	private ColliderGroup CreateColliderGroup(ColliderGrid grid, ColliderCell cell, ColliderKey key)
	{
		ColliderGroup colliderGroup = Facepunch.Pool.Get<ColliderGroup>();
		colliderGroup.Initialize(grid, cell, key);
		return colliderGroup;
	}

	private void DestroyColliderGroup(ref ColliderGroup grp)
	{
		Facepunch.Pool.Free<ColliderGroup>(ref grp);
	}

	public ColliderGroup FindBatchGroup(ColliderBatch collider)
	{
		ColliderGroup colliderGroup;
		ColliderKey colliderKey = new ColliderKey(collider);
		if (!this.batches.TryGetValue(colliderKey, out colliderGroup))
		{
			colliderGroup = this.CreateColliderGroup(this.grid, this, colliderKey);
			this.batches.Add(colliderKey, colliderGroup);
		}
		return colliderGroup;
	}

	public int MeshCount()
	{
		int num = 0;
		BufferList<ColliderGroup> values = this.batches.Values;
		for (int i = 0; i < values.Count; i++)
		{
			num += values[i].MeshCount();
		}
		return num;
	}

	public bool NeedsRefresh()
	{
		BufferList<ColliderGroup> values = this.batches.Values;
		for (int i = 0; i < values.Count; i++)
		{
			if (values[i].NeedsRefresh)
			{
				return true;
			}
		}
		return false;
	}

	public void Refresh()
	{
		this.interrupt = false;
		BufferList<ColliderGroup> values = this.batches.Values;
		for (int i = 0; i < values.Count; i++)
		{
			ColliderGroup item = values[i];
			if (!item.Processing)
			{
				if (item.Count <= 0)
				{
					item.Clear();
					this.DestroyColliderGroup(ref item);
					int num = i;
					i = num - 1;
					this.batches.RemoveAt(num);
				}
				else
				{
					item.Start();
					if (item.Processing)
					{
						item.UpdateData();
						item.CreateBatches();
						item.RefreshBatches();
						item.ApplyBatches();
						item.DisplayBatches();
					}
					item.End();
				}
			}
		}
	}

	public IEnumerator RefreshAsync()
	{
		ColliderCell colliderCell = null;
		int num;
		IEnumerator enumerator;
		colliderCell.interrupt = false;
		BufferList<ColliderGroup> values = colliderCell.batches.Values;
		for (int i = 0; i < values.Count; i++)
		{
			ColliderGroup item = values[i];
			if (item.Count > 0)
			{
				item.Start();
			}
		}
		for (num = 0; num < values.Count && !colliderCell.interrupt; num++)
		{
			ColliderGroup colliderGroup = values[num];
			if (colliderGroup.Processing)
			{
				if (!Batching.collider_threading)
				{
					colliderGroup.UpdateData();
					if (colliderCell.grid.NeedsTimeout)
					{
						yield return CoroutineEx.waitForEndOfFrame;
						colliderCell.grid.ResetTimeout();
					}
				}
				else
				{
					enumerator = colliderGroup.UpdateDataAsync();
					while (enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}
					colliderCell.grid.ResetTimeout();
					enumerator = null;
				}
			}
		}
		for (num = 0; num < values.Count && !colliderCell.interrupt; num++)
		{
			ColliderGroup item1 = values[num];
			if (item1.Processing)
			{
				item1.CreateBatches();
				if (colliderCell.grid.NeedsTimeout)
				{
					yield return CoroutineEx.waitForEndOfFrame;
					colliderCell.grid.ResetTimeout();
				}
			}
		}
		for (num = 0; num < values.Count && !colliderCell.interrupt; num++)
		{
			ColliderGroup colliderGroup1 = values[num];
			if (colliderGroup1.Processing)
			{
				if (!Batching.collider_threading)
				{
					colliderGroup1.RefreshBatches();
					if (colliderCell.grid.NeedsTimeout)
					{
						yield return CoroutineEx.waitForEndOfFrame;
						colliderCell.grid.ResetTimeout();
					}
				}
				else
				{
					enumerator = colliderGroup1.RefreshBatchesAsync();
					while (enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}
					colliderCell.grid.ResetTimeout();
					enumerator = null;
				}
			}
		}
		for (num = 0; num < values.Count && !colliderCell.interrupt; num++)
		{
			ColliderGroup item2 = values[num];
			if (item2.Processing)
			{
				for (int k = 0; k < item2.TempBatches.Count && !colliderCell.interrupt; k++)
				{
					item2.TempBatches[k].Apply();
					if (colliderCell.grid.NeedsTimeout)
					{
						yield return CoroutineEx.waitForEndOfFrame;
						colliderCell.grid.ResetTimeout();
					}
				}
			}
			item2 = null;
		}
		for (int l = 0; l < values.Count && !colliderCell.interrupt; l++)
		{
			ColliderGroup colliderGroup2 = values[l];
			if (colliderGroup2.Processing)
			{
				colliderGroup2.DisplayBatches();
			}
		}
		for (int m = 0; m < values.Count; m++)
		{
			ColliderGroup item3 = values[m];
			if (item3.Processing || item3.Preserving)
			{
				item3.End();
			}
			else if (item3.Count == 0 && !colliderCell.interrupt)
			{
				item3.Clear();
				colliderCell.DestroyColliderGroup(ref item3);
				int num1 = m;
				m = num1 - 1;
				colliderCell.batches.RemoveAt(num1);
			}
		}
	}
}