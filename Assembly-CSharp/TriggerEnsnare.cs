using System;
using UnityEngine;

public class TriggerEnsnare : TriggerBase
{
	public bool blockHands = true;

	public TriggerEnsnare()
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
		return baseEntity.gameObject;
	}
}