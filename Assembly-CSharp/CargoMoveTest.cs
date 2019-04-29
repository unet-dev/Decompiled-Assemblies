using System;
using System.Collections.Generic;
using UnityEngine;

public class CargoMoveTest : FacepunchBehaviour
{
	public int targetNodeIndex = -1;

	private float currentThrottle;

	private float turnScale;

	public CargoMoveTest()
	{
	}

	private void Awake()
	{
		base.Invoke(new Action(this.FindInitialNode), 2f);
	}

	public void FindInitialNode()
	{
		this.targetNodeIndex = this.GetClosestNodeToUs();
	}

	public int GetClosestNodeToUs()
	{
		int num = 0;
		float single = Single.PositiveInfinity;
		for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
		{
			Vector3 item = TerrainMeta.Path.OceanPatrolFar[i];
			float single1 = Vector3.Distance(base.transform.position, item);
			if (single1 < single)
			{
				num = i;
				single = single1;
			}
		}
		return num;
	}

	public void OnDrawGizmosSelected()
	{
		if (TerrainMeta.Path.OceanPatrolFar != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex], 10f);
			for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
			{
				Vector3 item = TerrainMeta.Path.OceanPatrolFar[i];
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(item, 3f);
				Gizmos.DrawLine(item, (i + 1 == TerrainMeta.Path.OceanPatrolFar.Count ? TerrainMeta.Path.OceanPatrolFar[0] : TerrainMeta.Path.OceanPatrolFar[i + 1]));
			}
		}
	}

	private void Update()
	{
		this.UpdateMovement();
	}

	public void UpdateMovement()
	{
		if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0)
		{
			return;
		}
		if (this.targetNodeIndex == -1)
		{
			return;
		}
		Vector3 item = TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex];
		float single = 0f;
		Vector3 vector3 = (item - base.transform.position).normalized;
		float single1 = Vector3.Dot(base.transform.forward, vector3);
		single = Mathf.InverseLerp(0.5f, 1f, single1);
		float single2 = Vector3.Dot(base.transform.right, vector3);
		float single3 = 5f;
		float single4 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(single2));
		this.turnScale = Mathf.Lerp(this.turnScale, single4, Time.deltaTime * 0.2f);
		float single5 = (float)((single2 < 0f ? -1 : 1));
		base.transform.Rotate(Vector3.up, single3 * Time.deltaTime * this.turnScale * single5, Space.World);
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, single, Time.deltaTime * 0.2f);
		Transform transforms = base.transform;
		transforms.position = transforms.position + (((base.transform.forward * 5f) * Time.deltaTime) * this.currentThrottle);
		if (Vector3.Distance(base.transform.position, item) < 60f)
		{
			this.targetNodeIndex++;
			if (this.targetNodeIndex >= TerrainMeta.Path.OceanPatrolFar.Count)
			{
				this.targetNodeIndex = 0;
			}
		}
	}
}