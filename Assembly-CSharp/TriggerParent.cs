using System;
using UnityEngine;

public class TriggerParent : TriggerBase, IServerComponent
{
	public TriggerParent()
	{
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		BasePlayer player = ent.ToPlayer();
		if (player != null && player.isMounted)
		{
			return;
		}
		if (ent.HasParent())
		{
			return;
		}
		ent.SetParent(base.gameObject.ToBaseEntity(), true, true);
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		BasePlayer player = ent.ToPlayer();
		if (player != null && player.IsSleeping())
		{
			return;
		}
		if (ent.GetParentEntity() != base.gameObject.ToBaseEntity())
		{
			return;
		}
		ent.SetParent(null, true, true);
		if (player != null)
		{
			player.PauseFlyHackDetection(5f);
			player.PauseSpeedHackDetection(5f);
			player.PauseVehicleNoClipDetection(5f);
		}
	}
}