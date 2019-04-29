using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ImpostorBatch
{
	public SimpleList<Vector4> Positions;

	private uint[] args = new uint[5];

	private Queue<int> recycle = new Queue<int>(32);

	public ComputeBuffer ArgsBuffer
	{
		get;
		private set;
	}

	public int Count
	{
		get
		{
			return this.Positions.Count;
		}
	}

	public int DeferredPass { get; private set; } = -1;

	public UnityEngine.Material Material
	{
		get;
		private set;
	}

	public UnityEngine.Mesh Mesh
	{
		get;
		private set;
	}

	public ComputeBuffer PositionBuffer
	{
		get;
		private set;
	}

	public int ShadowPass { get; private set; } = -1;

	public bool Visible
	{
		get
		{
			return this.Positions.Count - this.recycle.Count > 0;
		}
	}

	public ImpostorBatch()
	{
	}

	public void AddInstance(ImpostorInstanceData data)
	{
		data.Batch = this;
		if (this.recycle.Count <= 0)
		{
			data.BatchIndex = this.Positions.Count;
			this.Positions.Add(data.PositionAndScale());
			return;
		}
		data.BatchIndex = this.recycle.Dequeue();
		this.Positions[data.BatchIndex] = data.PositionAndScale();
	}

	public void Initialize(UnityEngine.Mesh mesh, UnityEngine.Material material)
	{
		this.Mesh = mesh;
		this.Material = material;
		this.DeferredPass = material.FindPass("DEFERRED");
		this.ShadowPass = material.FindPass("SHADOWCASTER");
		this.Positions = Pool.Get<SimpleList<Vector4>>();
		this.Positions.Clear();
		this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
		this.ArgsBuffer = new ComputeBuffer(1, (int)this.args.Length * 4, ComputeBufferType.DrawIndirect);
		this.args[0] = this.Mesh.GetIndexCount(0);
		this.args[2] = this.Mesh.GetIndexStart(0);
		this.args[3] = this.Mesh.GetBaseVertex(0);
	}

	public void Release()
	{
		this.recycle.Clear();
		Pool.Free<SimpleList<Vector4>>(ref this.Positions);
		this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
		this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
	}

	public void RemoveInstance(ImpostorInstanceData data)
	{
		this.Positions[data.BatchIndex] = new Vector4(0f, 0f, 0f, -1f);
		this.recycle.Enqueue(data.BatchIndex);
		data.BatchIndex = 0;
		data.Batch = null;
	}

	private ComputeBuffer SafeRelease(ComputeBuffer buffer)
	{
		if (buffer != null)
		{
			buffer.Release();
		}
		return null;
	}

	public void UpdateBuffers()
	{
		bool flag = false;
		if (this.PositionBuffer == null || this.PositionBuffer.count != this.Positions.Count)
		{
			this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
			this.PositionBuffer = new ComputeBuffer(this.Positions.Count, 16);
			flag = true;
		}
		if (this.PositionBuffer != null)
		{
			this.PositionBuffer.SetData(this.Positions.Array, 0, 0, this.Positions.Count);
		}
		if (this.ArgsBuffer != null & flag)
		{
			this.args[1] = (uint)this.Positions.Count;
			this.ArgsBuffer.SetData(this.args);
		}
	}
}