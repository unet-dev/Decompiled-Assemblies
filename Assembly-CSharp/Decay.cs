using ConVar;
using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Decay : PrefabAttribute, IServerComponent
{
	private const float hours = 3600f;

	protected Decay()
	{
	}

	public static void BuildingDecayTouch(BuildingBlock buildingBlock)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		List<DecayEntity> list = Facepunch.Pool.GetList<DecayEntity>();
		Vis.Entities<DecayEntity>(buildingBlock.transform.position, 40f, list, 2097408, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			DecayEntity item = list[i];
			BuildingBlock buildingBlock1 = item as BuildingBlock;
			if (!buildingBlock1 || buildingBlock1.buildingID == buildingBlock.buildingID)
			{
				item.DecayTouch();
			}
		}
		Facepunch.Pool.FreeList<DecayEntity>(ref list);
	}

	public static void EntityLinkDecayTouch(BaseEntity ent)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		ent.EntityLinkBroadcast<DecayEntity>((DecayEntity decayEnt) => decayEnt.DecayTouch());
	}

	protected float GetDecayDelay(BuildingGrade.Enum grade)
	{
		if (!ConVar.Decay.upkeep)
		{
			switch (grade)
			{
				case BuildingGrade.Enum.Wood:
				{
					return 64800f;
				}
				case BuildingGrade.Enum.Stone:
				{
					return 64800f;
				}
				case BuildingGrade.Enum.Metal:
				{
					return 64800f;
				}
				case BuildingGrade.Enum.TopTier:
				{
					return 86400f;
				}
				default:
				{
					return 3600f;
				}
			}
		}
		if (ConVar.Decay.delay_override > 0f)
		{
			return ConVar.Decay.delay_override;
		}
		switch (grade)
		{
			case BuildingGrade.Enum.Wood:
			{
				return ConVar.Decay.delay_wood * 3600f;
			}
			case BuildingGrade.Enum.Stone:
			{
				return ConVar.Decay.delay_stone * 3600f;
			}
			case BuildingGrade.Enum.Metal:
			{
				return ConVar.Decay.delay_metal * 3600f;
			}
			case BuildingGrade.Enum.TopTier:
			{
				return ConVar.Decay.delay_toptier * 3600f;
			}
			default:
			{
				return ConVar.Decay.delay_twig * 3600f;
			}
		}
	}

	public abstract float GetDecayDelay(BaseEntity entity);

	protected float GetDecayDuration(BuildingGrade.Enum grade)
	{
		if (!ConVar.Decay.upkeep)
		{
			switch (grade)
			{
				case BuildingGrade.Enum.Wood:
				{
					return 86400f;
				}
				case BuildingGrade.Enum.Stone:
				{
					return 172800f;
				}
				case BuildingGrade.Enum.Metal:
				{
					return 259200f;
				}
				case BuildingGrade.Enum.TopTier:
				{
					return 432000f;
				}
				default:
				{
					return 3600f;
				}
			}
		}
		if (ConVar.Decay.duration_override > 0f)
		{
			return ConVar.Decay.duration_override;
		}
		switch (grade)
		{
			case BuildingGrade.Enum.Wood:
			{
				return ConVar.Decay.duration_wood * 3600f;
			}
			case BuildingGrade.Enum.Stone:
			{
				return ConVar.Decay.duration_stone * 3600f;
			}
			case BuildingGrade.Enum.Metal:
			{
				return ConVar.Decay.duration_metal * 3600f;
			}
			case BuildingGrade.Enum.TopTier:
			{
				return ConVar.Decay.duration_toptier * 3600f;
			}
			default:
			{
				return ConVar.Decay.duration_twig * 3600f;
			}
		}
	}

	public abstract float GetDecayDuration(BaseEntity entity);

	protected override Type GetIndexedType()
	{
		return typeof(Decay);
	}

	public static void RadialDecayTouch(Vector3 pos, float radius, int mask)
	{
		if (ConVar.Decay.upkeep)
		{
			return;
		}
		List<DecayEntity> list = Facepunch.Pool.GetList<DecayEntity>();
		Vis.Entities<DecayEntity>(pos, radius, list, mask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].DecayTouch();
		}
		Facepunch.Pool.FreeList<DecayEntity>(ref list);
	}

	public virtual bool ShouldDecay(BaseEntity entity)
	{
		return true;
	}
}