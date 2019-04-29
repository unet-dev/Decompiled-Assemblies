using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshColliderLookup
{
	public MeshColliderLookup.LookupGroup src = new MeshColliderLookup.LookupGroup();

	public MeshColliderLookup.LookupGroup dst = new MeshColliderLookup.LookupGroup();

	public MeshColliderLookup()
	{
	}

	public void Add(MeshColliderInstance instance)
	{
		this.dst.Add(instance);
	}

	public void Apply()
	{
		MeshColliderLookup.LookupGroup lookupGroup = this.src;
		this.src = this.dst;
		this.dst = lookupGroup;
		this.dst.Clear();
	}

	public MeshColliderLookup.LookupEntry Get(int index)
	{
		return this.src.Get(index);
	}

	public struct LookupEntry
	{
		public Transform transform;

		public Rigidbody rigidbody;

		public Collider collider;

		public OBB bounds;

		public LookupEntry(MeshColliderInstance instance)
		{
			this.transform = instance.transform;
			this.rigidbody = instance.rigidbody;
			this.collider = instance.collider;
			this.bounds = instance.bounds;
		}
	}

	public class LookupGroup
	{
		public List<MeshColliderLookup.LookupEntry> data;

		public List<int> indices;

		public LookupGroup()
		{
		}

		public void Add(MeshColliderInstance instance)
		{
			this.data.Add(new MeshColliderLookup.LookupEntry(instance));
			int count = this.data.Count - 1;
			int length = (int)instance.data.triangles.Length / 3;
			for (int i = 0; i < length; i++)
			{
				this.indices.Add(count);
			}
		}

		public void Clear()
		{
			this.data.Clear();
			this.indices.Clear();
		}

		public MeshColliderLookup.LookupEntry Get(int index)
		{
			return this.data[this.indices[index]];
		}
	}
}