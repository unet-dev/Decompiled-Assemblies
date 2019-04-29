using Oxide.Core;
using System;
using UnityEngine;

public class DestroyOnGroundMissing : MonoBehaviour, IServerComponent
{
	public DestroyOnGroundMissing()
	{
	}

	private void OnGroundMissing()
	{
		BaseEntity baseEntity = base.gameObject.ToBaseEntity();
		if (baseEntity != null)
		{
			if (Interface.CallHook("OnEntityGroundMissing", baseEntity) != null)
			{
				return;
			}
			BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
			if (baseCombatEntity != null)
			{
				baseCombatEntity.Die(null);
				return;
			}
			baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}
}