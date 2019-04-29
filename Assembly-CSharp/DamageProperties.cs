using Rust;
using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Damage Properties")]
public class DamageProperties : ScriptableObject
{
	public DamageProperties fallback;

	[Horizontal(1, 0)]
	public DamageProperties.HitAreaProperty[] bones;

	public DamageProperties()
	{
	}

	public float GetMultiplier(HitArea area)
	{
		for (int i = 0; i < (int)this.bones.Length; i++)
		{
			DamageProperties.HitAreaProperty hitAreaProperty = this.bones[i];
			if (hitAreaProperty.area == area)
			{
				return hitAreaProperty.damage;
			}
		}
		if (!this.fallback)
		{
			return 1f;
		}
		return this.fallback.GetMultiplier(area);
	}

	public void ScaleDamage(HitInfo info)
	{
		HitArea hitArea = info.boneArea;
		if (hitArea == (HitArea.Head | HitArea.Chest | HitArea.Stomach | HitArea.Arm | HitArea.Hand | HitArea.Leg | HitArea.Foot) || (int)hitArea == 0)
		{
			return;
		}
		info.damageTypes.ScaleAll(this.GetMultiplier(hitArea));
	}

	[Serializable]
	public class HitAreaProperty
	{
		public HitArea area;

		public float damage;

		public HitAreaProperty()
		{
		}
	}
}