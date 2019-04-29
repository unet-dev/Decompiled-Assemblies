using Oxide.Core;
using System;
using UnityEngine;

public class BaseTrapTrigger : TriggerBase
{
	public BaseTrap _trap;

	public BaseTrapTrigger()
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

	internal override void OnEmpty()
	{
		base.OnEmpty();
		this._trap.OnEmpty();
	}

	internal override void OnObjectAdded(GameObject obj)
	{
		Interface.CallHook("OnTrapSnapped", this, obj);
		base.OnObjectAdded(obj);
		this._trap.ObjectEntered(obj);
	}
}