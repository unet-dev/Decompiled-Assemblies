using Network;
using System;
using UnityEngine;

public struct EntityRef
{
	internal BaseEntity ent_cached;

	internal uint id_cached;

	public uint uid
	{
		get
		{
			if (this.ent_cached.IsValid())
			{
				this.id_cached = this.ent_cached.net.ID;
			}
			return this.id_cached;
		}
		set
		{
			this.id_cached = value;
			if (this.id_cached == 0)
			{
				this.ent_cached = null;
				return;
			}
			if (this.ent_cached.IsValid() && this.ent_cached.net.ID == this.id_cached)
			{
				return;
			}
			this.ent_cached = null;
		}
	}

	public BaseEntity Get(bool serverside)
	{
		if (this.ent_cached == null && this.id_cached > 0)
		{
			if (!serverside)
			{
				Debug.LogWarning("EntityRef: Looking for clientside entities on pure server!");
			}
			else
			{
				this.ent_cached = BaseNetworkable.serverEntities.Find(this.id_cached) as BaseEntity;
			}
		}
		if (!this.ent_cached.IsValid())
		{
			this.ent_cached = null;
		}
		return this.ent_cached;
	}

	public bool IsSet()
	{
		return this.id_cached != 0;
	}

	public bool IsValid(bool serverside)
	{
		return this.Get(serverside).IsValid();
	}

	public void Set(BaseEntity ent)
	{
		this.ent_cached = ent;
		this.id_cached = 0;
		if (this.ent_cached.IsValid())
		{
			this.id_cached = this.ent_cached.net.ID;
		}
	}
}