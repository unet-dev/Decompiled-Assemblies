using System;
using System.Collections.Generic;
using UnityEngine;

public class RHIBAIController : FacepunchBehaviour
{
	public List<Vector3> nodes = new List<Vector3>();

	public RHIBAIController()
	{
	}

	public float GetWaterDepth(Vector3 pos)
	{
		RaycastHit raycastHit;
		if (!Physics.Raycast(pos, Vector3.down, out raycastHit, 100f, 8388608))
		{
			return 100f;
		}
		return raycastHit.distance;
	}

	public void OnDrawGizmosSelected()
	{
		if (TerrainMeta.Path.OceanPatrolClose != null)
		{
			for (int i = 0; i < TerrainMeta.Path.OceanPatrolClose.Count; i++)
			{
				Vector3 item = TerrainMeta.Path.OceanPatrolClose[i];
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(item, 3f);
				Gizmos.DrawLine(item, (i + 1 == TerrainMeta.Path.OceanPatrolClose.Count ? TerrainMeta.Path.OceanPatrolClose[0] : TerrainMeta.Path.OceanPatrolClose[i + 1]));
			}
		}
	}

	[ContextMenu("Calculate Path")]
	public void SetupPatrolPath()
	{
		RaycastHit raycastHit;
		float size = TerrainMeta.Size.x;
		float single = 30f;
		int num = Mathf.CeilToInt(size * 2f * 3.14159274f / single);
		this.nodes = new List<Vector3>();
		float single1 = size;
		float single2 = 0f;
		for (int i = 0; i < num; i++)
		{
			float single3 = (float)i / (float)num * 360f;
			this.nodes.Add(new Vector3(Mathf.Sin(single3 * 0.0174532924f) * single1, single2, Mathf.Cos(single3 * 0.0174532924f) * single1));
		}
		float single4 = 2f;
		float single5 = 200f;
		float single6 = 150f;
		float single7 = 8f;
		bool flag = true;
		int num1 = 1;
		float single8 = 20f;
		Vector3[] vector3 = new Vector3[] { new Vector3(0f, 0f, 0f), new Vector3(single8, 0f, 0f), new Vector3(-single8, 0f, 0f), new Vector3(0f, 0f, single8), new Vector3(0f, 0f, -single8) };
		while (flag)
		{
			Debug.Log(string.Concat("Loop # :", num1));
			num1++;
			flag = false;
			for (int j = 0; j < num; j++)
			{
				Vector3 item = this.nodes[j];
				int num2 = (j == 0 ? num - 1 : j - 1);
				Vector3 item1 = this.nodes[(j == num - 1 ? 0 : j + 1)];
				Vector3 vector31 = this.nodes[num2];
				Vector3 vector32 = item;
				Vector3 vector33 = Vector3.zero - item;
				Vector3 vector34 = vector33.normalized;
				Vector3 vector35 = item + (vector34 * single4);
				if (Vector3.Distance(vector35, item1) <= single5 && Vector3.Distance(vector35, vector31) <= single5)
				{
					bool flag1 = true;
					for (int k = 0; k < (int)vector3.Length; k++)
					{
						Vector3 vector36 = vector35 + vector3[k];
						if (this.GetWaterDepth(vector36) < single7)
						{
							flag1 = false;
						}
						Vector3 vector37 = vector34;
						if (vector36 != Vector3.zero)
						{
							vector33 = vector36 - vector32;
							vector37 = vector33.normalized;
						}
						if (Physics.Raycast(vector32, vector37, out raycastHit, single6, 1218511105))
						{
							flag1 = false;
						}
					}
					if (flag1)
					{
						flag = true;
						this.nodes[j] = vector35;
					}
				}
			}
		}
		List<int> nums = new List<int>();
		LineUtility.Simplify(this.nodes, 15f, nums);
		List<Vector3> vector3s = this.nodes;
		this.nodes = new List<Vector3>();
		foreach (int num3 in nums)
		{
			this.nodes.Add(vector3s[num3]);
		}
	}
}