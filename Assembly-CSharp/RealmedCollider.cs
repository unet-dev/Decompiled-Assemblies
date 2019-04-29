using System;
using UnityEngine;

public class RealmedCollider : BasePrefab
{
	public Collider ServerCollider;

	public Collider ClientCollider;

	public RealmedCollider()
	{
	}

	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		if (this.ServerCollider != this.ClientCollider)
		{
			if (clientside)
			{
				if (this.ServerCollider)
				{
					process.RemoveComponent(this.ServerCollider);
					this.ServerCollider = null;
				}
			}
			else if (this.ClientCollider)
			{
				process.RemoveComponent(this.ClientCollider);
				this.ClientCollider = null;
			}
		}
		process.RemoveComponent(this);
	}
}