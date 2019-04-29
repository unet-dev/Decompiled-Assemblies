using ConVar;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshRendererBatch : MeshBatch
{
	private Vector3 position;

	private UnityEngine.Mesh meshBatch;

	private MeshFilter meshFilter;

	private MeshRenderer meshRenderer;

	private MeshRendererData meshData;

	private MeshRendererGroup meshGroup;

	private MeshRendererLookup meshLookup;

	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	public MeshRendererBatch()
	{
	}

	public void Add(MeshRendererInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshLookup.Apply();
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new MeshRendererData();
		this.meshGroup = new MeshRendererGroup();
		this.meshLookup = new MeshRendererLookup();
	}

	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	protected override void OnPooled()
	{
		if (this.meshFilter)
		{
			this.meshFilter.sharedMesh = null;
		}
		if (this.meshBatch)
		{
			AssetPool.Free(ref this.meshBatch);
		}
		this.meshData.Free();
		this.meshGroup.Free();
		this.meshLookup.src.Clear();
		this.meshLookup.dst.Clear();
	}

	protected override void RefreshMesh()
	{
		this.meshLookup.dst.Clear();
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup, this.meshLookup);
	}

	public void Setup(Vector3 position, Material material, ShadowCastingMode shadows, int layer)
	{
		Vector3 vector3 = position;
		Vector3 vector31 = vector3;
		base.transform.position = vector3;
		this.position = vector31;
		base.gameObject.layer = layer;
		this.meshRenderer.sharedMaterial = material;
		this.meshRenderer.shadowCastingMode = shadows;
		if (shadows == ShadowCastingMode.ShadowsOnly)
		{
			this.meshRenderer.receiveShadows = false;
			this.meshRenderer.motionVectors = false;
			this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			return;
		}
		this.meshRenderer.receiveShadows = true;
		this.meshRenderer.motionVectors = true;
		this.meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
		this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
	}

	protected override void ToggleMesh(bool state)
	{
		List<MeshRendererLookup.LookupEntry> lookupEntries = this.meshLookup.src.data;
		for (int i = 0; i < lookupEntries.Count; i++)
		{
			Renderer item = lookupEntries[i].renderer;
			if (item)
			{
				item.enabled = !state;
			}
		}
		if (!state)
		{
			if (this.meshFilter)
			{
				this.meshFilter.sharedMesh = null;
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = false;
			}
		}
		else
		{
			if (this.meshFilter)
			{
				this.meshFilter.sharedMesh = this.meshBatch;
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = true;
				return;
			}
		}
	}
}