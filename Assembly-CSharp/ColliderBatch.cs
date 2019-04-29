using ConVar;
using Rust;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ColliderBatch : MonoBehaviour, IServerComponent
{
	private ColliderGroup batchGroup;

	private MeshColliderInstance batchInstance;

	public MeshCollider BatchCollider
	{
		get;
		set;
	}

	public Rigidbody BatchRigidbody
	{
		get;
		set;
	}

	public Transform BatchTransform
	{
		get;
		set;
	}

	public ColliderBatch()
	{
	}

	public void Add()
	{
		if (this.batchGroup != null)
		{
			this.Remove();
		}
		if (!Batching.colliders)
		{
			return;
		}
		if (this.BatchTransform == null)
		{
			return;
		}
		if (this.BatchCollider == null)
		{
			return;
		}
		if (this.BatchCollider.sharedMesh.subMeshCount > Batching.collider_submeshes)
		{
			return;
		}
		if (this.BatchCollider.sharedMesh.vertexCount > Batching.collider_vertices)
		{
			return;
		}
		ColliderCell item = SingletonComponent<ColliderGrid>.Instance[this.BatchTransform.position];
		this.batchGroup = item.FindBatchGroup(this);
		this.batchGroup.Add(this);
		this.batchInstance.mesh = this.BatchCollider.sharedMesh;
		this.batchInstance.position = this.BatchTransform.position;
		this.batchInstance.rotation = this.BatchTransform.rotation;
		this.batchInstance.scale = this.BatchTransform.lossyScale;
		this.batchInstance.transform = this.BatchTransform;
		this.batchInstance.rigidbody = this.BatchRigidbody;
		this.batchInstance.collider = this.BatchCollider;
		this.batchInstance.bounds = new OBB(this.BatchTransform, this.BatchCollider.sharedMesh.bounds);
		if (Rust.Application.isLoading)
		{
			this.BatchCollider.enabled = false;
		}
	}

	public void AddBatch(ColliderGroup batchGroup)
	{
		batchGroup.Add(this.batchInstance);
	}

	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Remove();
	}

	protected void OnEnable()
	{
		this.BatchTransform = base.transform;
		this.BatchCollider = base.GetComponent<MeshCollider>();
		this.BatchRigidbody = base.GetComponent<Rigidbody>();
		this.Add();
	}

	public void Refresh()
	{
		this.Remove();
		this.Add();
	}

	public void Remove()
	{
		if (this.batchGroup == null)
		{
			return;
		}
		this.batchGroup.Invalidate();
		this.batchGroup.Remove(this);
		this.batchGroup = null;
	}
}