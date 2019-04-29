using System;
using UnityEngine;

public class TriggerTemperature : TriggerBase
{
	public float Temperature = 50f;

	public float triggerSize;

	public float minSize;

	public TriggerTemperature()
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

	private void OnValidate()
	{
		this.triggerSize = base.GetComponent<SphereCollider>().radius * base.transform.localScale.y;
	}

	public float WorkoutTemperature(Vector3 position, float oldTemperature)
	{
		float single = Vector3.Distance(base.gameObject.transform.position, position);
		float single1 = Mathf.InverseLerp(this.triggerSize, this.minSize, single);
		return Mathf.Lerp(oldTemperature, this.Temperature, single1);
	}
}