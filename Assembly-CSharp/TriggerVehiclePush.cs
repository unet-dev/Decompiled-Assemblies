using System;
using System.Linq;
using UnityEngine;

public class TriggerVehiclePush : TriggerBase, IServerComponent
{
	public BaseEntity thisEntity;

	public float maxPushVelocity = 10f;

	public float minRadius;

	public float maxRadius;

	public TriggerVehiclePush()
	{
	}

	public void FixedUpdate()
	{
		if (this.thisEntity == null)
		{
			return;
		}
		if (this.entityContents == null)
		{
			return;
		}
		BaseEntity[] array = this.entityContents.ToArray<BaseEntity>();
		for (int i = 0; i < (int)array.Length; i++)
		{
			BaseEntity baseEntity = array[i];
			if (baseEntity.IsValid() && !baseEntity.EqualNetID(this.thisEntity))
			{
				Rigidbody component = baseEntity.GetComponent<Rigidbody>();
				if (component && !component.isKinematic)
				{
					float single = Vector3Ex.Distance2D(baseEntity.transform.position, base.transform.position);
					float single1 = 1f - Mathf.InverseLerp(this.minRadius, this.maxRadius, single);
					float single2 = 1f - Mathf.InverseLerp(this.minRadius - 1f, this.minRadius, single);
					Vector3 vector3 = baseEntity.ClosestPoint(base.transform.position);
					Vector3 vector31 = Vector3Ex.Direction2D(vector3, base.transform.position);
					component.AddForceAtPosition((vector31 * this.maxPushVelocity) * single1, vector3, ForceMode.Acceleration);
					if (single2 > 0f)
					{
						component.AddForceAtPosition((vector31 * 1f) * single2, vector3, ForceMode.VelocityChange);
					}
				}
			}
		}
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

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.minRadius);
		Gizmos.color = new Color(0.5f, 0f, 0f, 1f);
		Gizmos.DrawWireSphere(base.transform.position, this.maxRadius);
	}
}