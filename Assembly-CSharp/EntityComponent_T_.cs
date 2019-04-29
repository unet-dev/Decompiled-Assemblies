using System;
using UnityEngine;

public class EntityComponent<T> : EntityComponentBase
where T : BaseEntity
{
	[NonSerialized]
	private T _baseEntity;

	protected T baseEntity
	{
		get
		{
			if (this._baseEntity == null)
			{
				this.UpdateBaseEntity();
			}
			return this._baseEntity;
		}
	}

	public EntityComponent()
	{
	}

	protected override BaseEntity GetBaseEntity()
	{
		return (object)this.baseEntity;
	}

	protected void UpdateBaseEntity()
	{
		if (!this)
		{
			return;
		}
		if (!base.gameObject)
		{
			return;
		}
		this._baseEntity = (T)(base.gameObject.ToBaseEntity() as T);
	}
}