using System;
using UnityEngine;

public abstract class ProceduralComponent : MonoBehaviour
{
	[InspectorFlags]
	public ProceduralComponent.Realm Mode = ProceduralComponent.Realm.Client | ProceduralComponent.Realm.Server;

	public string Description = "Procedural Component";

	public virtual bool RunOnCache
	{
		get
		{
			return false;
		}
	}

	protected ProceduralComponent()
	{
	}

	public abstract void Process(uint seed);

	public bool ShouldRun()
	{
		if (World.Cached && !this.RunOnCache)
		{
			return false;
		}
		if ((int)(this.Mode & ProceduralComponent.Realm.Server) != 0)
		{
			return true;
		}
		return false;
	}

	public enum Realm
	{
		Client = 1,
		Server = 2
	}
}