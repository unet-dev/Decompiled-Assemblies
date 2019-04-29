using System;
using UnityEngine;

public class EntityTimedDestroy : EntityComponent<BaseEntity>
{
	public float secondsTillDestroy = 1f;

	public EntityTimedDestroy()
	{
	}

	private void OnEnable()
	{
		base.Invoke(new Action(this.TimedDestroy), this.secondsTillDestroy);
	}

	private void TimedDestroy()
	{
		if (base.baseEntity == null)
		{
			Debug.LogWarning("EntityTimedDestroy failed, baseEntity was already null!");
			return;
		}
		base.baseEntity.Kill(BaseNetworkable.DestroyMode.None);
	}
}