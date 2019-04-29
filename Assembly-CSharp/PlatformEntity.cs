using System;
using UnityEngine;

public class PlatformEntity : BaseEntity
{
	private const float movementSpeed = 1f;

	private const float rotationSpeed = 10f;

	private const float radius = 10f;

	private Vector3 targetPosition = Vector3.zero;

	private Quaternion targetRotation = Quaternion.identity;

	public PlatformEntity()
	{
	}

	protected void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (this.targetPosition == Vector3.zero || Vector3.Distance(base.transform.position, this.targetPosition) < 0.01f)
		{
			Vector2 vector2 = UnityEngine.Random.insideUnitCircle * 10f;
			this.targetPosition = base.transform.position + new Vector3(vector2.x, 0f, vector2.y);
			if (TerrainMeta.HeightMap != null && TerrainMeta.WaterMap != null)
			{
				float height = TerrainMeta.HeightMap.GetHeight(this.targetPosition);
				float single = TerrainMeta.WaterMap.GetHeight(this.targetPosition);
				this.targetPosition.y = Mathf.Max(height, single) + 1f;
			}
			this.targetRotation = Quaternion.LookRotation(this.targetPosition - base.transform.position);
		}
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, Time.fixedDeltaTime * 1f);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, Time.fixedDeltaTime * 10f);
	}

	public override bool PhysicsDriven()
	{
		return true;
	}
}