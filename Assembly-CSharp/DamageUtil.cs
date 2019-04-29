using Facepunch;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class DamageUtil
{
	public static void RadiusDamage(BaseEntity attackingPlayer, BaseEntity weaponPrefab, Vector3 pos, float minradius, float radius, List<DamageTypeEntry> damage, int layers, bool useLineOfSight)
	{
		using (TimeWarning timeWarning = TimeWarning.New("DamageUtil.RadiusDamage", 0.1f))
		{
			List<HitInfo> list = Pool.GetList<HitInfo>();
			List<BaseEntity> baseEntities = Pool.GetList<BaseEntity>();
			List<BaseEntity> list1 = Pool.GetList<BaseEntity>();
			Vis.Entities<BaseEntity>(pos, radius, list1, layers, QueryTriggerInteraction.Collide);
			for (int i = 0; i < list1.Count; i++)
			{
				BaseEntity item = list1[i];
				if (item.isServer && !baseEntities.Contains(item))
				{
					Vector3 vector3 = item.ClosestPoint(pos);
					float single = Mathf.Clamp01((Vector3.Distance(vector3, pos) - minradius) / (radius - minradius));
					if (single <= 1f)
					{
						float single1 = 1f - single;
						if (!useLineOfSight || item.IsVisible(pos, Single.PositiveInfinity))
						{
							HitInfo hitInfo = new HitInfo()
							{
								Initiator = attackingPlayer,
								WeaponPrefab = weaponPrefab
							};
							hitInfo.damageTypes.Add(damage);
							hitInfo.damageTypes.ScaleAll(single1);
							hitInfo.HitPositionWorld = vector3;
							hitInfo.HitNormalWorld = (pos - vector3).normalized;
							hitInfo.PointStart = pos;
							hitInfo.PointEnd = hitInfo.HitPositionWorld;
							list.Add(hitInfo);
							baseEntities.Add(item);
						}
					}
				}
			}
			for (int j = 0; j < baseEntities.Count; j++)
			{
				baseEntities[j].OnAttacked(list[j]);
			}
			Pool.FreeList<BaseEntity>(ref baseEntities);
			Pool.FreeList<BaseEntity>(ref list1);
		}
	}
}