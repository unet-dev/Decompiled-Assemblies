using System;
using UnityEngine;

public class Socket_Terrain : Socket_Base
{
	public float placementHeight;

	public bool alignToNormal;

	public Socket_Terrain()
	{
	}

	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Vector3 vector3 = this.rotation.eulerAngles;
		vector3.x = 0f;
		vector3.z = 0f;
		Vector3 vector31 = target.ray.direction;
		vector31.y = 0f;
		vector31.Normalize();
		Vector3 vector32 = Vector3.up;
		if (this.alignToNormal)
		{
			vector32 = target.normal;
		}
		Quaternion quaternion = (Quaternion.LookRotation(vector31, vector32) * Quaternion.Euler(0f, vector3.y, 0f)) * Quaternion.Euler(target.rotation);
		Vector3 vector33 = target.position;
		vector33 = vector33 - (quaternion * this.position);
		return new Construction.Placement()
		{
			rotation = quaternion,
			position = vector33
		};
	}

	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
		Gizmos.DrawCube(Vector3.zero, new Vector3(0.1f, 0.1f, this.placementHeight));
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.1f, 0.1f, this.placementHeight));
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	public override bool TestTarget(Construction.Target target)
	{
		return target.onTerrain;
	}
}