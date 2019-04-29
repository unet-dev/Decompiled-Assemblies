using System;
using UnityEngine;

public class TriggerRadiation : TriggerBase
{
	public TriggerRadiation.RadiationTier radiationTier = TriggerRadiation.RadiationTier.LOW;

	public float RadiationAmountOverride;

	public float radiationSize;

	public float falloff = 0.1f;

	public TriggerRadiation()
	{
	}

	public float GetRadiation(Vector3 position, float radProtection)
	{
		float radiationAmount = this.GetRadiationAmount();
		float single = Vector3.Distance(base.gameObject.transform.position, position);
		float single1 = Mathf.InverseLerp(this.radiationSize, this.radiationSize * (1f - this.falloff), single);
		return Mathf.Clamp(radiationAmount - radProtection, 0f, radiationAmount) * single1;
	}

	public float GetRadiationAmount()
	{
		if (this.RadiationAmountOverride > 0f)
		{
			return this.RadiationAmountOverride;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.MINIMAL)
		{
			return 2f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.LOW)
		{
			return 10f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.MEDIUM)
		{
			return 25f;
		}
		if (this.radiationTier == TriggerRadiation.RadiationTier.HIGH)
		{
			return 51f;
		}
		return 1f;
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
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, this.radiationSize);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radiationSize * (1f - this.falloff));
	}

	private void OnValidate()
	{
		this.radiationSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
	}

	public enum RadiationTier
	{
		MINIMAL,
		LOW,
		MEDIUM,
		HIGH
	}
}