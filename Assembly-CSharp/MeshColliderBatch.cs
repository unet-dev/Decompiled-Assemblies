using ConVar;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderBatch : MeshBatch
{
	private Vector3 position;

	private UnityEngine.Mesh meshBatch;

	private MeshCollider meshCollider;

	private MeshColliderData meshData;

	private MeshColliderGroup meshGroup;

	public MeshColliderLookup meshLookup;

	public override int VertexCapacity
	{
		get
		{
			return Batching.collider_capacity;
		}
	}

	public override int VertexCutoff
	{
		get
		{
			return Batching.collider_vertices;
		}
	}

	public MeshColliderBatch()
	{
	}

	public void Add(MeshColliderInstance instance)
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
	}

	protected void Awake()
	{
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.meshData = new MeshColliderData();
		this.meshGroup = new MeshColliderGroup();
		this.meshLookup = new MeshColliderLookup();
	}

	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	public Collider LookupCollider(int triangleIndex)
	{
		return this.meshLookup.Get(triangleIndex).collider;
	}

	public void LookupColliders<T>(Vector3 position, float distance, List<T> list)
	where T : Collider
	{
		List<MeshColliderLookup.LookupEntry> lookupEntries = this.meshLookup.src.data;
		float single = distance * distance;
		for (int i = 0; i < lookupEntries.Count; i++)
		{
			MeshColliderLookup.LookupEntry item = lookupEntries[i];
			if (item.collider && (item.bounds.ClosestPoint(position) - position).sqrMagnitude <= single)
			{
				list.Add((T)(item.collider as T));
			}
		}
	}

	public void LookupColliders<T>(OBB bounds, List<T> list)
	where T : Collider
	{
		List<MeshColliderLookup.LookupEntry> lookupEntries = this.meshLookup.src.data;
		for (int i = 0; i < lookupEntries.Count; i++)
		{
			MeshColliderLookup.LookupEntry item = lookupEntries[i];
			if (item.collider && item.bounds.Intersects(bounds))
			{
				list.Add((T)(item.collider as T));
			}
		}
	}

	public Rigidbody LookupRigidbody(int triangleIndex)
	{
		return this.meshLookup.Get(triangleIndex).rigidbody;
	}

	public Transform LookupTransform(int triangleIndex)
	{
		return this.meshLookup.Get(triangleIndex).transform;
	}

	protected override void OnPooled()
	{
		if (this.meshCollider)
		{
			this.meshCollider.sharedMesh = null;
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

	public void Setup(Vector3 position, LayerMask layer, PhysicMaterial material)
	{
		Vector3 vector3 = position;
		Vector3 vector31 = vector3;
		base.transform.position = vector3;
		this.position = vector31;
		base.gameObject.layer = layer;
		this.meshCollider.sharedMaterial = material;
	}

	protected override void ToggleMesh(bool state)
	{
		if (!Rust.Application.isLoading)
		{
			List<MeshColliderLookup.LookupEntry> lookupEntries = this.meshLookup.src.data;
			for (int i = 0; i < lookupEntries.Count; i++)
			{
				Collider item = lookupEntries[i].collider;
				if (item)
				{
					item.enabled = !state;
				}
			}
		}
		if (state)
		{
			if (this.meshCollider)
			{
				this.meshCollider.sharedMesh = this.meshBatch;
				this.meshCollider.enabled = false;
				this.meshCollider.enabled = true;
				return;
			}
		}
		else if (this.meshCollider)
		{
			this.meshCollider.sharedMesh = null;
			this.meshCollider.enabled = false;
		}
	}
}