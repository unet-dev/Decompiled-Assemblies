using System;
using UnityEngine;

public class JunkPileWater : JunkPile
{
	public Transform[] buoyancyPoints;

	public bool debugDraw;

	private Quaternion baseRotation = Quaternion.identity;

	private bool first = true;

	public JunkPileWater()
	{
	}

	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		this.UpdateMovement();
	}

	public override void Spawn()
	{
		Vector3 height = base.transform.position;
		height.y = TerrainMeta.WaterMap.GetHeight(base.transform.position);
		base.transform.position = height;
		base.Spawn();
		Quaternion quaternion = base.transform.rotation;
		this.baseRotation = Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f);
	}

	public void UpdateMovement()
	{
		if (!this.isSinking)
		{
			float height = WaterSystem.GetHeight(base.transform.position);
			base.transform.position = new Vector3(base.transform.position.x, height, base.transform.position.z);
			if (this.buoyancyPoints != null && (int)this.buoyancyPoints.Length >= 3)
			{
				Vector3 vector3 = base.transform.position;
				Vector3 vector31 = this.buoyancyPoints[0].localPosition;
				Vector3 vector32 = this.buoyancyPoints[1].localPosition;
				Vector3 vector33 = this.buoyancyPoints[2].localPosition;
				Vector3 height1 = vector31 + vector3;
				Vector3 height2 = vector32 + vector3;
				Vector3 height3 = vector33 + vector3;
				height1.y = WaterSystem.GetHeight(height1);
				height2.y = WaterSystem.GetHeight(height2);
				height3.y = WaterSystem.GetHeight(height3);
				base.transform.position = new Vector3(vector3.x, height1.y - vector31.y, vector3.z);
				Vector3 vector34 = height2 - height1;
				Vector3 vector35 = Vector3.Cross(height3 - height1, vector34);
				Quaternion quaternion = Quaternion.LookRotation(new Vector3(vector35.x, vector35.z, vector35.y));
				Vector3 vector36 = quaternion.eulerAngles;
				quaternion = Quaternion.Euler(-vector36.x, 0f, -vector36.y);
				if (this.first)
				{
					Quaternion quaternion1 = base.transform.rotation;
					this.baseRotation = Quaternion.Euler(0f, quaternion1.eulerAngles.y, 0f);
					this.first = false;
				}
				base.transform.rotation = quaternion * this.baseRotation;
			}
		}
	}
}