using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAchievement : TriggerBase
{
	public string statToIncrease = "";

	public string achievementOnEnter = "";

	public string requiredVehicleName = "";

	[Tooltip("Always set to true, clientside does not work, currently")]
	public bool serverSide = true;

	[NonSerialized]
	private List<ulong> triggeredPlayers = new List<ulong>();

	public TriggerAchievement()
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
		if (baseEntity.isClient && this.serverSide)
		{
			return null;
		}
		if (baseEntity.isServer && !this.serverSide)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent == null)
		{
			return;
		}
		BasePlayer component = ent.GetComponent<BasePlayer>();
		if (component == null || !component.IsAlive() || component.IsSleeping() || component.IsNpc)
		{
			return;
		}
		if (this.triggeredPlayers.Contains(component.userID))
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.requiredVehicleName))
		{
			BaseVehicle mountedVehicle = component.GetMountedVehicle();
			if (mountedVehicle == null)
			{
				return;
			}
			if (!mountedVehicle.ShortPrefabName.Contains(this.requiredVehicleName))
			{
				return;
			}
		}
		if (this.serverSide)
		{
			if (!string.IsNullOrEmpty(this.achievementOnEnter))
			{
				component.GiveAchievement(this.achievementOnEnter);
			}
			if (!string.IsNullOrEmpty(this.statToIncrease))
			{
				component.stats.Add(this.statToIncrease, 1, Stats.Steam);
				component.stats.Save();
			}
			this.triggeredPlayers.Add(component.userID);
		}
	}

	public void OnPuzzleReset()
	{
		this.Reset();
	}

	public void Reset()
	{
		this.triggeredPlayers.Clear();
	}
}