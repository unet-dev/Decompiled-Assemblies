using System;
using UnityEngine;

public class EntityCollisionMessage : EntityComponent<BaseEntity>
{
	public EntityCollisionMessage()
	{
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		if (base.baseEntity.IsDestroyed)
		{
			return;
		}
		BaseEntity entity = collision.GetEntity();
		if (entity == base.baseEntity)
		{
			return;
		}
		if (entity != null)
		{
			if (entity.IsDestroyed)
			{
				return;
			}
			if (base.baseEntity.isServer)
			{
				entity = entity.ToServer<BaseEntity>();
			}
		}
		base.baseEntity.OnCollision(collision, entity);
	}
}