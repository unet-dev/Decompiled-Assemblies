using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SocketMod_Attraction : SocketMod
{
	public float outerRadius = 1f;

	public float innerRadius = 0.1f;

	public string groupName = "wallbottom";

	public SocketMod_Attraction()
	{
	}

	public override bool DoCheck(Construction.Placement place)
	{
		return true;
	}

	public override void ModifyPlacement(Construction.Placement place)
	{
		Vector3 vector3 = place.position + (place.rotation * this.worldPosition);
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(vector3, this.outerRadius * 2f, list, -1, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			if (baseEntity.isServer != this.isServer)
			{
				continue;
			}
			AttractionPoint[] attractionPointArray = this.prefabAttribute.FindAll<AttractionPoint>(baseEntity.prefabID);
			if (attractionPointArray == null)
			{
				continue;
			}
			AttractionPoint[] attractionPointArray1 = attractionPointArray;
			for (int i = 0; i < (int)attractionPointArray1.Length; i++)
			{
				AttractionPoint attractionPoint = attractionPointArray1[i];
				if (attractionPoint.groupName == this.groupName)
				{
					Vector3 vector31 = baseEntity.transform.position + (baseEntity.transform.rotation * attractionPoint.worldPosition);
					Vector3 vector32 = vector31 - vector3;
					float single = vector32.magnitude;
					if (single <= this.outerRadius)
					{
						Quaternion quaternion = QuaternionEx.LookRotationWithOffset(this.worldPosition, vector31 - place.position, Vector3.up);
						float single1 = Mathf.InverseLerp(this.outerRadius, this.innerRadius, single);
						place.rotation = Quaternion.Lerp(place.rotation, quaternion, single1);
						vector3 = place.position + (place.rotation * this.worldPosition);
						vector32 = vector31 - vector3;
						place.position = place.position + (vector32 * single1);
					}
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
		Gizmos.DrawSphere(Vector3.zero, this.outerRadius);
		Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
		Gizmos.DrawSphere(Vector3.zero, this.innerRadius);
	}
}