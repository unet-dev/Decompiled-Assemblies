using Facepunch.Extend;
using System;
using UnityEngine;

[RequireComponent(typeof(TerrainMeta))]
public abstract class TerrainExtension : MonoBehaviour
{
	[NonSerialized]
	public bool isInitialized;

	internal Terrain terrain;

	internal TerrainConfig config;

	protected TerrainExtension()
	{
	}

	public void Init(Terrain terrain, TerrainConfig config)
	{
		this.terrain = terrain;
		this.config = config;
	}

	public void LogSize(object obj, ulong size)
	{
		Debug.Log(string.Concat(obj.GetType(), " allocated: ", size.FormatBytes<ulong>(false)));
	}

	public virtual void PostSetup()
	{
	}

	public virtual void Setup()
	{
	}
}