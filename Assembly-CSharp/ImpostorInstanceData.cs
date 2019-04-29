using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ImpostorInstanceData
{
	public ImpostorBatch Batch;

	public int BatchIndex;

	private int hash;

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

	public UnityEngine.Renderer Renderer
	{
		get;
		private set;
	}

	public ImpostorInstanceData(UnityEngine.Renderer renderer, UnityEngine.Mesh mesh, UnityEngine.Material material)
	{
		this.Renderer = renderer;
		this.Mesh = mesh;
		this.Material = material;
		this.hash = this.GenerateHashCode();
		this.Update();
	}

	public override bool Equals(object obj)
	{
		ImpostorInstanceData impostorInstanceDatum = obj as ImpostorInstanceData;
		if (impostorInstanceDatum.Material != this.Material)
		{
			return false;
		}
		return impostorInstanceDatum.Mesh == this.Mesh;
	}

	private int GenerateHashCode()
	{
		return (17 * 31 + this.Material.GetHashCode()) * 31 + this.Mesh.GetHashCode();
	}

	public override int GetHashCode()
	{
		return this.hash;
	}

	public Vector4 PositionAndScale()
	{
		float single;
		Transform renderer = this.Renderer.transform;
		Vector3 vector3 = renderer.position;
		Vector3 vector31 = renderer.lossyScale;
		single = (this.Renderer.enabled ? vector31.x : -vector31.x);
		return new Vector4(vector3.x, vector3.y, vector3.z, single);
	}

	public void Update()
	{
		if (this.Batch != null)
		{
			this.Batch.Positions[this.BatchIndex] = this.PositionAndScale();
		}
	}
}