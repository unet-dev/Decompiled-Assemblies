using System;
using UnityEngine;

public static class GizmosUtil
{
	public static void DrawBounds(Transform transform)
	{
		Bounds bounds = transform.GetBounds(true, false, true);
		Vector3 vector3 = transform.lossyScale;
		Quaternion quaternion = transform.rotation;
		Vector3 vector31 = transform.position + (quaternion * Vector3.Scale(vector3, bounds.center));
		Vector3 vector32 = Vector3.Scale(vector3, bounds.size);
		GizmosUtil.DrawCube(vector31, vector32, quaternion);
		GizmosUtil.DrawWireCube(vector31, vector32, quaternion);
	}

	public static void DrawCapsuleX(Vector3 pos, float radius, float height)
	{
		Vector3 vector3 = pos - new Vector3(0.5f * height, 0f, 0f);
		Vector3 vector31 = pos + new Vector3(0.5f * height, 0f, 0f);
		Gizmos.DrawSphere(vector3, radius);
		Gizmos.DrawSphere(vector31, radius);
	}

	public static void DrawCapsuleY(Vector3 pos, float radius, float height)
	{
		Vector3 vector3 = pos - new Vector3(0f, 0.5f * height, 0f);
		Vector3 vector31 = pos + new Vector3(0f, 0.5f * height, 0f);
		Gizmos.DrawSphere(vector3, radius);
		Gizmos.DrawSphere(vector31, radius);
	}

	public static void DrawCapsuleZ(Vector3 pos, float radius, float height)
	{
		Vector3 vector3 = pos - new Vector3(0f, 0f, 0.5f * height);
		Vector3 vector31 = pos + new Vector3(0f, 0f, 0.5f * height);
		Gizmos.DrawSphere(vector3, radius);
		Gizmos.DrawSphere(vector31, radius);
	}

	public static void DrawCircleX(Vector3 pos, float radius)
	{
		Matrix4x4 matrix4x4 = Gizmos.matrix;
		Gizmos.matrix = Gizmos.matrix * Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(0f, 1f, 1f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix4x4;
	}

	public static void DrawCircleY(Vector3 pos, float radius)
	{
		Matrix4x4 matrix4x4 = Gizmos.matrix;
		Gizmos.matrix = Gizmos.matrix * Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 0f, 1f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix4x4;
	}

	public static void DrawCircleZ(Vector3 pos, float radius)
	{
		Matrix4x4 matrix4x4 = Gizmos.matrix;
		Gizmos.matrix = Gizmos.matrix * Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 1f, 0f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix4x4;
	}

	public static void DrawCube(Vector3 pos, Vector3 size, Quaternion rot)
	{
		Matrix4x4 matrix4x4 = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(pos, rot, size);
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
		Gizmos.matrix = matrix4x4;
	}

	public static void DrawCylinderX(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleX(pos - new Vector3(0.5f * height, 0f, 0f), radius);
		GizmosUtil.DrawCircleX(pos + new Vector3(0.5f * height, 0f, 0f), radius);
	}

	public static void DrawCylinderY(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleY(pos - new Vector3(0f, 0.5f * height, 0f), radius);
		GizmosUtil.DrawCircleY(pos + new Vector3(0f, 0.5f * height, 0f), radius);
	}

	public static void DrawCylinderZ(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleZ(pos - new Vector3(0f, 0f, 0.5f * height), radius);
		GizmosUtil.DrawCircleZ(pos + new Vector3(0f, 0f, 0.5f * height), radius);
	}

	public static void DrawMeshes(Transform transform)
	{
		MeshRenderer[] componentsInChildren = transform.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			MeshRenderer meshRenderer = componentsInChildren[i];
			if (meshRenderer.enabled)
			{
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (component)
				{
					Transform transforms = meshRenderer.transform;
					Gizmos.DrawMesh(component.sharedMesh, transforms.position, transforms.rotation, transforms.lossyScale);
				}
			}
		}
	}

	public static void DrawSemiCircle(float radius)
	{
		float single = radius * 0.0174532924f * 0.5f;
		Vector3 vector3 = (Mathf.Cos(single) * Vector3.forward) + (Mathf.Sin(single) * Vector3.right);
		Gizmos.DrawLine(Vector3.zero, vector3);
		Vector3 vector31 = (Mathf.Cos(-single) * Vector3.forward) + (Mathf.Sin(-single) * Vector3.right);
		Gizmos.DrawLine(Vector3.zero, vector31);
		float single1 = Mathf.Clamp(radius / 16f, 4f, 64f);
		float single2 = single / single1;
		for (float i = single; i > 0f; i -= single2)
		{
			Vector3 vector32 = (Mathf.Cos(i) * Vector3.forward) + (Mathf.Sin(i) * Vector3.right);
			Gizmos.DrawLine(Vector3.zero, vector32);
			if (vector3 != Vector3.zero)
			{
				Gizmos.DrawLine(vector32, vector3);
			}
			vector3 = vector32;
			Vector3 vector33 = (Mathf.Cos(-i) * Vector3.forward) + (Mathf.Sin(-i) * Vector3.right);
			Gizmos.DrawLine(Vector3.zero, vector33);
			if (vector31 != Vector3.zero)
			{
				Gizmos.DrawLine(vector33, vector31);
			}
			vector31 = vector33;
		}
		Gizmos.DrawLine(vector3, vector31);
	}

	public static void DrawWireCapsuleX(Vector3 pos, float radius, float height)
	{
		Vector3 vector3 = pos - new Vector3(0.5f * height, 0f, 0f);
		Vector3 vector31 = pos + new Vector3(0.5f * height, 0f, 0f);
		Gizmos.DrawWireSphere(vector3, radius);
		Gizmos.DrawWireSphere(vector31, radius);
		Gizmos.DrawLine(vector3 + (Vector3.forward * radius), vector31 + (Vector3.forward * radius));
		Gizmos.DrawLine(vector3 + (Vector3.up * radius), vector31 + (Vector3.up * radius));
		Gizmos.DrawLine(vector3 + (Vector3.back * radius), vector31 + (Vector3.back * radius));
		Gizmos.DrawLine(vector3 + (Vector3.down * radius), vector31 + (Vector3.down * radius));
	}

	public static void DrawWireCapsuleY(Vector3 pos, float radius, float height)
	{
		Vector3 vector3 = pos - new Vector3(0f, 0.5f * height, 0f);
		Vector3 vector31 = pos + new Vector3(0f, 0.5f * height, 0f);
		Gizmos.DrawWireSphere(vector3, radius);
		Gizmos.DrawWireSphere(vector31, radius);
		Gizmos.DrawLine(vector3 + (Vector3.forward * radius), vector31 + (Vector3.forward * radius));
		Gizmos.DrawLine(vector3 + (Vector3.right * radius), vector31 + (Vector3.right * radius));
		Gizmos.DrawLine(vector3 + (Vector3.back * radius), vector31 + (Vector3.back * radius));
		Gizmos.DrawLine(vector3 + (Vector3.left * radius), vector31 + (Vector3.left * radius));
	}

	public static void DrawWireCapsuleZ(Vector3 pos, float radius, float height)
	{
		Vector3 vector3 = pos - new Vector3(0f, 0f, 0.5f * height);
		Vector3 vector31 = pos + new Vector3(0f, 0f, 0.5f * height);
		Gizmos.DrawWireSphere(vector3, radius);
		Gizmos.DrawWireSphere(vector31, radius);
		Gizmos.DrawLine(vector3 + (Vector3.up * radius), vector31 + (Vector3.up * radius));
		Gizmos.DrawLine(vector3 + (Vector3.right * radius), vector31 + (Vector3.right * radius));
		Gizmos.DrawLine(vector3 + (Vector3.down * radius), vector31 + (Vector3.down * radius));
		Gizmos.DrawLine(vector3 + (Vector3.left * radius), vector31 + (Vector3.left * radius));
	}

	public static void DrawWireCircleX(Vector3 pos, float radius)
	{
		Matrix4x4 matrix4x4 = Gizmos.matrix;
		Gizmos.matrix = Gizmos.matrix * Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(0f, 1f, 1f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix4x4;
	}

	public static void DrawWireCircleY(Vector3 pos, float radius)
	{
		Matrix4x4 matrix4x4 = Gizmos.matrix;
		Gizmos.matrix = Gizmos.matrix * Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 0f, 1f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix4x4;
	}

	public static void DrawWireCircleZ(Vector3 pos, float radius)
	{
		Matrix4x4 matrix4x4 = Gizmos.matrix;
		Gizmos.matrix = Gizmos.matrix * Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 1f, 0f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix4x4;
	}

	public static void DrawWireCube(Vector3 pos, Vector3 size, Quaternion rot)
	{
		Matrix4x4 matrix4x4 = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(pos, rot, size);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		Gizmos.matrix = matrix4x4;
	}

	public static void DrawWireCylinderX(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleX(pos - new Vector3(0.5f * height, 0f, 0f), radius);
		GizmosUtil.DrawWireCircleX(pos + new Vector3(0.5f * height, 0f, 0f), radius);
	}

	public static void DrawWireCylinderY(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleY(pos - new Vector3(0f, 0.5f * height, 0f), radius);
		GizmosUtil.DrawWireCircleY(pos + new Vector3(0f, 0.5f * height, 0f), radius);
	}

	public static void DrawWireCylinderZ(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleZ(pos - new Vector3(0f, 0f, 0.5f * height), radius);
		GizmosUtil.DrawWireCircleZ(pos + new Vector3(0f, 0f, 0.5f * height), radius);
	}

	public static void DrawWirePath(Vector3 a, Vector3 b, float thickness)
	{
		GizmosUtil.DrawWireCircleY(a, thickness);
		GizmosUtil.DrawWireCircleY(b, thickness);
		Vector3 vector3 = (b - a).normalized;
		Vector3 vector31 = Quaternion.Euler(0f, 90f, 0f) * vector3;
		Gizmos.DrawLine(b + (vector31 * thickness), a + (vector31 * thickness));
		Gizmos.DrawLine(b - (vector31 * thickness), a - (vector31 * thickness));
	}
}