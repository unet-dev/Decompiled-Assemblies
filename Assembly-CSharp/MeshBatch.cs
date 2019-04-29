using Rust;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class MeshBatch : MonoBehaviour
{
	public int AvailableVertices
	{
		get
		{
			return Mathf.Clamp(this.VertexCapacity, this.VertexCutoff, 65534) - this.VertexCount;
		}
	}

	public int BatchedCount
	{
		get;
		private set;
	}

	public int Count
	{
		get;
		private set;
	}

	public bool NeedsRefresh
	{
		get;
		private set;
	}

	public abstract int VertexCapacity
	{
		get;
	}

	public int VertexCount
	{
		get;
		private set;
	}

	public abstract int VertexCutoff
	{
		get;
	}

	protected MeshBatch()
	{
	}

	protected void AddVertices(int vertices)
	{
		this.NeedsRefresh = true;
		this.Count = this.Count + 1;
		this.VertexCount = this.VertexCount + vertices;
	}

	public void Alloc()
	{
		this.AllocMemory();
	}

	protected abstract void AllocMemory();

	public void Apply()
	{
		this.NeedsRefresh = false;
		this.ApplyMesh();
	}

	protected abstract void ApplyMesh();

	public void Display()
	{
		this.ToggleMesh(true);
		this.BatchedCount = this.Count;
	}

	public void Free()
	{
		this.FreeMemory();
	}

	protected abstract void FreeMemory();

	public void Invalidate()
	{
		this.ToggleMesh(false);
		this.BatchedCount = 0;
	}

	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.NeedsRefresh = false;
		this.Count = 0;
		this.BatchedCount = 0;
		this.VertexCount = 0;
		this.OnPooled();
	}

	protected void OnEnable()
	{
		this.NeedsRefresh = false;
		this.Count = 0;
		this.BatchedCount = 0;
		this.VertexCount = 0;
	}

	protected abstract void OnPooled();

	public void Refresh()
	{
		this.RefreshMesh();
	}

	protected abstract void RefreshMesh();

	protected abstract void ToggleMesh(bool state);
}