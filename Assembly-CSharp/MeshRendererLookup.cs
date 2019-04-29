using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererLookup
{
	public MeshRendererLookup.LookupGroup src = new MeshRendererLookup.LookupGroup();

	public MeshRendererLookup.LookupGroup dst = new MeshRendererLookup.LookupGroup();

	public MeshRendererLookup()
	{
	}

	public void Add(MeshRendererInstance instance)
	{
		this.dst.Add(instance);
	}

	public void Apply()
	{
		MeshRendererLookup.LookupGroup lookupGroup = this.src;
		this.src = this.dst;
		this.dst = lookupGroup;
		this.dst.Clear();
	}

	public void Clear()
	{
		this.dst.Clear();
	}

	public MeshRendererLookup.LookupEntry Get(int index)
	{
		return this.src.Get(index);
	}

	public struct LookupEntry
	{
		public Renderer renderer;

		public LookupEntry(MeshRendererInstance instance)
		{
			this.renderer = instance.renderer;
		}
	}

	public class LookupGroup
	{
		public List<MeshRendererLookup.LookupEntry> data;

		public LookupGroup()
		{
		}

		public void Add(MeshRendererInstance instance)
		{
			this.data.Add(new MeshRendererLookup.LookupEntry(instance));
		}

		public void Clear()
		{
			this.data.Clear();
		}

		public MeshRendererLookup.LookupEntry Get(int index)
		{
			return this.data[index];
		}
	}
}