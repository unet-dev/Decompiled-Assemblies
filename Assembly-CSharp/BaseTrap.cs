using System;
using UnityEngine;

public class BaseTrap : DecayEntity
{
	public BaseTrap()
	{
	}

	public virtual void Arm()
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public virtual void ObjectEntered(GameObject obj)
	{
	}

	public virtual void OnEmpty()
	{
	}
}