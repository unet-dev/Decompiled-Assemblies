using System;
using UnityEngine;

public class BasePrefab : BaseMonoBehaviour, IPrefabPreProcess
{
	[HideInInspector]
	public uint prefabID;

	[HideInInspector]
	public bool isClient;

	public bool isServer
	{
		get
		{
			return !this.isClient;
		}
	}

	public BasePrefab()
	{
	}

	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.prefabID = StringPool.Get(name);
		this.isClient = clientside;
	}
}