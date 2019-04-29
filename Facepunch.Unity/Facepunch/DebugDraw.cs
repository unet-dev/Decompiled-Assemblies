using System;
using UnityEngine;

namespace Facepunch
{
	public static class DebugDraw
	{
		public static void Arrow(Vector3 position, Vector3 direction, Color color, float duration = 0f, bool depthTest = true)
		{
			Debug.DrawRay(position, direction, color, duration, depthTest);
			DebugDraw.Cone(position + direction, -direction * 0.333f, color, 15f, duration, depthTest);
		}

		public static void Arrow(Vector3 position, Vector3 direction, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Arrow(position, direction, Color.white, duration, depthTest);
		}

		public static void Bounds(Bounds bounds, Color color, float duration = 0f, bool depthTest = true)
		{
			Vector3 vector3 = bounds.center;
			float single = bounds.extents.x;
			float single1 = bounds.extents.y;
			float single2 = bounds.extents.z;
			Vector3 vector31 = vector3 + new Vector3(single, single1, single2);
			Vector3 vector32 = vector3 + new Vector3(single, single1, -single2);
			Vector3 vector33 = vector3 + new Vector3(-single, single1, single2);
			Vector3 vector34 = vector3 + new Vector3(-single, single1, -single2);
			Vector3 vector35 = vector3 + new Vector3(single, -single1, single2);
			Vector3 vector36 = vector3 + new Vector3(single, -single1, -single2);
			Vector3 vector37 = vector3 + new Vector3(-single, -single1, single2);
			Vector3 vector38 = vector3 + new Vector3(-single, -single1, -single2);
			Debug.DrawLine(vector31, vector33, color, duration, depthTest);
			Debug.DrawLine(vector31, vector32, color, duration, depthTest);
			Debug.DrawLine(vector33, vector34, color, duration, depthTest);
			Debug.DrawLine(vector32, vector34, color, duration, depthTest);
			Debug.DrawLine(vector31, vector35, color, duration, depthTest);
			Debug.DrawLine(vector32, vector36, color, duration, depthTest);
			Debug.DrawLine(vector33, vector37, color, duration, depthTest);
			Debug.DrawLine(vector34, vector38, color, duration, depthTest);
			Debug.DrawLine(vector35, vector37, color, duration, depthTest);
			Debug.DrawLine(vector35, vector36, color, duration, depthTest);
			Debug.DrawLine(vector37, vector38, color, duration, depthTest);
			Debug.DrawLine(vector38, vector36, color, duration, depthTest);
		}

		public static void Bounds(Bounds bounds, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Bounds(bounds, Color.white, duration, depthTest);
		}

		public static void Capsule(Vector3 start, Vector3 end, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
		{
			Vector3 vector3 = end - start;
			Vector3 vector31 = vector3.normalized * radius;
			Vector3 vector32 = Vector3.Slerp(vector31, -vector31, 0.5f);
			vector3 = Vector3.Cross(vector31, vector32);
			Vector3 vector33 = vector3.normalized * radius;
			vector3 = start - end;
			float single = vector3.magnitude;
			float single1 = Mathf.Max(0f, single * 0.5f - radius);
			Vector3 vector34 = (end + start) * 0.5f;
			vector3 = start - vector34;
			start = vector34 + (vector3.normalized * single1);
			vector3 = end - vector34;
			end = vector34 + (vector3.normalized * single1);
			DebugDraw.Circle(start, vector31, color, radius, duration, depthTest);
			DebugDraw.Circle(end, -vector31, color, radius, duration, depthTest);
			Debug.DrawLine(start + vector33, end + vector33, color, duration, depthTest);
			Debug.DrawLine(start - vector33, end - vector33, color, duration, depthTest);
			Debug.DrawLine(start + vector32, end + vector32, color, duration, depthTest);
			Debug.DrawLine(start - vector32, end - vector32, color, duration, depthTest);
			for (int i = 1; i < 26; i++)
			{
				Debug.DrawLine(Vector3.Slerp(vector33, -vector31, (float)i / 25f) + start, Vector3.Slerp(vector33, -vector31, (float)(i - 1) / 25f) + start, color, duration, depthTest);
				Debug.DrawLine(Vector3.Slerp(-vector33, -vector31, (float)i / 25f) + start, Vector3.Slerp(-vector33, -vector31, (float)(i - 1) / 25f) + start, color, duration, depthTest);
				Debug.DrawLine(Vector3.Slerp(vector32, -vector31, (float)i / 25f) + start, Vector3.Slerp(vector32, -vector31, (float)(i - 1) / 25f) + start, color, duration, depthTest);
				Debug.DrawLine(Vector3.Slerp(-vector32, -vector31, (float)i / 25f) + start, Vector3.Slerp(-vector32, -vector31, (float)(i - 1) / 25f) + start, color, duration, depthTest);
				Debug.DrawLine(Vector3.Slerp(vector33, vector31, (float)i / 25f) + end, Vector3.Slerp(vector33, vector31, (float)(i - 1) / 25f) + end, color, duration, depthTest);
				Debug.DrawLine(Vector3.Slerp(-vector33, vector31, (float)i / 25f) + end, Vector3.Slerp(-vector33, vector31, (float)(i - 1) / 25f) + end, color, duration, depthTest);
				Debug.DrawLine(Vector3.Slerp(vector32, vector31, (float)i / 25f) + end, Vector3.Slerp(vector32, vector31, (float)(i - 1) / 25f) + end, color, duration, depthTest);
				Debug.DrawLine(Vector3.Slerp(-vector32, vector31, (float)i / 25f) + end, Vector3.Slerp(-vector32, vector31, (float)(i - 1) / 25f) + end, color, duration, depthTest);
			}
		}

		public static void Capsule(Vector3 start, Vector3 end, float radius = 1f, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Capsule(start, end, Color.white, radius, duration, depthTest);
		}

		public static void Circle(Vector3 position, Vector3 up, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
		{
			Vector3 vector3 = up.normalized * radius;
			Vector3 vector31 = Vector3.Slerp(vector3, -vector3, 0.5f);
			Vector3 vector32 = Vector3.Cross(vector3, vector31);
			Vector3 vector33 = vector32.normalized * radius;
			Matrix4x4 matrix4x4 = new Matrix4x4();
			matrix4x4[0] = vector33.x;
			matrix4x4[1] = vector33.y;
			matrix4x4[2] = vector33.z;
			matrix4x4[4] = vector3.x;
			matrix4x4[5] = vector3.y;
			matrix4x4[6] = vector3.z;
			matrix4x4[8] = vector31.x;
			matrix4x4[9] = vector31.y;
			matrix4x4[10] = vector31.z;
			Vector3 vector34 = position + matrix4x4.MultiplyPoint3x4(new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)));
			Vector3 vector35 = Vector3.zero;
			Color color1 = new Color();
			color = (color == color1 ? Color.white : color);
			for (int i = 0; i < 91; i++)
			{
				vector35.x = Mathf.Cos((float)(i * 4) * 0.0174532924f);
				vector35.z = Mathf.Sin((float)(i * 4) * 0.0174532924f);
				vector35.y = 0f;
				vector35 = position + matrix4x4.MultiplyPoint3x4(vector35);
				Debug.DrawLine(vector34, vector35, color, duration, depthTest);
				vector34 = vector35;
			}
		}

		public static void Circle(Vector3 position, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Circle(position, Vector3.up, color, radius, duration, depthTest);
		}

		public static void Circle(Vector3 position, Vector3 up, float radius = 1f, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Circle(position, up, Color.white, radius, duration, depthTest);
		}

		public static void Circle(Vector3 position, float radius = 1f, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Circle(position, Vector3.up, Color.white, radius, duration, depthTest);
		}

		public static void Cone(Vector3 position, Vector3 direction, Color color, float angle = 45f, float duration = 0f, bool depthTest = true)
		{
			float single;
			float single1 = direction.magnitude;
			Vector3 vector3 = direction;
			Vector3 vector31 = Vector3.Slerp(vector3, -vector3, 0.5f);
			Vector3 vector32 = Vector3.Cross(vector3, vector31);
			Vector3 vector33 = vector32.normalized * single1;
			direction = direction.normalized;
			Vector3 vector34 = Vector3.Slerp(vector3, vector31, angle / 90f);
			Plane plane = new Plane(-direction, position + vector3);
			Ray ray = new Ray(position, vector34);
			plane.Raycast(ray, out single);
			Debug.DrawRay(position, vector34.normalized * single, color);
			vector32 = Vector3.Slerp(vector3, -vector31, angle / 90f);
			Debug.DrawRay(position, vector32.normalized * single, color, duration, depthTest);
			vector32 = Vector3.Slerp(vector3, vector33, angle / 90f);
			Debug.DrawRay(position, vector32.normalized * single, color, duration, depthTest);
			vector32 = Vector3.Slerp(vector3, -vector33, angle / 90f);
			Debug.DrawRay(position, vector32.normalized * single, color, duration, depthTest);
			Vector3 vector35 = position + vector3;
			vector32 = vector3 - (vector34.normalized * single);
			DebugDraw.Circle(vector35, direction, color, vector32.magnitude, duration, depthTest);
			Vector3 vector36 = position + (vector3 * 0.5f);
			vector32 = (vector3 * 0.5f) - (vector34.normalized * (single * 0.5f));
			DebugDraw.Circle(vector36, direction, color, vector32.magnitude, duration, depthTest);
		}

		public static void Cone(Vector3 position, Vector3 direction, float angle = 45f, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Cone(position, direction, Color.white, angle, duration, depthTest);
		}

		public static void Cone(Vector3 position, Color color, float angle = 45f, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Cone(position, Vector3.up, color, angle, duration, depthTest);
		}

		public static void Cone(Vector3 position, float angle = 45f, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Cone(position, Vector3.up, Color.white, angle, duration, depthTest);
		}

		public static void Cylinder(Vector3 start, Vector3 end, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
		{
			Vector3 vector3 = end - start;
			Vector3 vector31 = vector3.normalized * radius;
			Vector3 vector32 = Vector3.Slerp(vector31, -vector31, 0.5f);
			vector3 = Vector3.Cross(vector31, vector32);
			Vector3 vector33 = vector3.normalized * radius;
			DebugDraw.Circle(start, vector31, color, radius, duration, depthTest);
			DebugDraw.Circle(end, -vector31, color, radius, duration, depthTest);
			DebugDraw.Circle((start + end) * 0.5f, vector31, color, radius, duration, depthTest);
			Debug.DrawLine(start + vector33, end + vector33, color, duration, depthTest);
			Debug.DrawLine(start - vector33, end - vector33, color, duration, depthTest);
			Debug.DrawLine(start + vector32, end + vector32, color, duration, depthTest);
			Debug.DrawLine(start - vector32, end - vector32, color, duration, depthTest);
			Debug.DrawLine(start - vector33, start + vector33, color, duration, depthTest);
			Debug.DrawLine(start - vector32, start + vector32, color, duration, depthTest);
			Debug.DrawLine(end - vector33, end + vector33, color, duration, depthTest);
			Debug.DrawLine(end - vector32, end + vector32, color, duration, depthTest);
		}

		public static void Cylinder(Vector3 start, Vector3 end, float radius = 1f, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Cylinder(start, end, Color.white, radius, duration, depthTest);
		}

		public static void Line(Vector3 position, Vector3 endposition, Color color, float duration = 0f, bool depthTest = true)
		{
			Debug.DrawLine(position, endposition, color, duration, depthTest);
		}

		public static void LocalCube(Transform transform, Vector3 size, Color color, Vector3 center = null, float duration = 0f, bool depthTest = true)
		{
			Vector3 vector3 = transform.TransformPoint(center + (-size * 0.5f));
			Vector3 vector31 = transform.TransformPoint(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));
			Vector3 vector32 = transform.TransformPoint(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
			Vector3 vector33 = transform.TransformPoint(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));
			Vector3 vector34 = transform.TransformPoint(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
			Vector3 vector35 = transform.TransformPoint(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));
			Vector3 vector36 = transform.TransformPoint(center + (size * 0.5f));
			Vector3 vector37 = transform.TransformPoint(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));
			Debug.DrawLine(vector3, vector31, color, duration, depthTest);
			Debug.DrawLine(vector31, vector32, color, duration, depthTest);
			Debug.DrawLine(vector32, vector33, color, duration, depthTest);
			Debug.DrawLine(vector33, vector3, color, duration, depthTest);
			Debug.DrawLine(vector34, vector35, color, duration, depthTest);
			Debug.DrawLine(vector35, vector36, color, duration, depthTest);
			Debug.DrawLine(vector36, vector37, color, duration, depthTest);
			Debug.DrawLine(vector37, vector34, color, duration, depthTest);
			Debug.DrawLine(vector3, vector34, color, duration, depthTest);
			Debug.DrawLine(vector31, vector35, color, duration, depthTest);
			Debug.DrawLine(vector32, vector36, color, duration, depthTest);
			Debug.DrawLine(vector33, vector37, color, duration, depthTest);
		}

		public static void LocalCube(Transform transform, Vector3 size, Vector3 center = null, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.LocalCube(transform, size, Color.white, center, duration, depthTest);
		}

		public static void LocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = null, float duration = 0f, bool depthTest = true)
		{
			Color color1 = new Color();
			color = (color == color1 ? Color.white : color);
			Vector3 vector3 = space.MultiplyPoint3x4(center + (-size * 0.5f));
			Vector3 vector31 = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));
			Vector3 vector32 = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
			Vector3 vector33 = space.MultiplyPoint3x4(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));
			Vector3 vector34 = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
			Vector3 vector35 = space.MultiplyPoint3x4(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));
			Vector3 vector36 = space.MultiplyPoint3x4(center + (size * 0.5f));
			Vector3 vector37 = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));
			Debug.DrawLine(vector3, vector31, color, duration, depthTest);
			Debug.DrawLine(vector31, vector32, color, duration, depthTest);
			Debug.DrawLine(vector32, vector33, color, duration, depthTest);
			Debug.DrawLine(vector33, vector3, color, duration, depthTest);
			Debug.DrawLine(vector34, vector35, color, duration, depthTest);
			Debug.DrawLine(vector35, vector36, color, duration, depthTest);
			Debug.DrawLine(vector36, vector37, color, duration, depthTest);
			Debug.DrawLine(vector37, vector34, color, duration, depthTest);
			Debug.DrawLine(vector3, vector34, color, duration, depthTest);
			Debug.DrawLine(vector31, vector35, color, duration, depthTest);
			Debug.DrawLine(vector32, vector36, color, duration, depthTest);
			Debug.DrawLine(vector33, vector37, color, duration, depthTest);
		}

		public static void LocalCube(Matrix4x4 space, Vector3 size, Vector3 center = null, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.LocalCube(space, size, Color.white, center, duration, depthTest);
		}

		public static void Normal(Vector3 point, Vector3 normal, float size, Color color, float duration = 0f, bool depthtest = true)
		{
			DebugDraw.Line(point, point + (normal.normalized * size), color, duration, depthtest);
			DebugDraw.Circle(point, normal.normalized, color, size * 0.5f, duration, depthtest);
		}

		public static void Point(Vector3 position, Color color, float scale = 1f, float duration = 0f, bool depthTest = true)
		{
			Color color1 = new Color();
			color = (color == color1 ? Color.white : color);
			Debug.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale, color, duration, depthTest);
			Debug.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale, color, duration, depthTest);
			Debug.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale, color, duration, depthTest);
		}

		public static void Point(Vector3 position, float scale = 1f, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Point(position, Color.white, scale, duration, depthTest);
		}

		public static void Sphere(Vector3 position, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
		{
			float single = 10f;
			Vector3 vector3 = new Vector3(position.x, position.y + radius * Mathf.Sin(0f), position.z + radius * Mathf.Cos(0f));
			Vector3 vector31 = new Vector3(position.x + radius * Mathf.Cos(0f), position.y, position.z + radius * Mathf.Sin(0f));
			Vector3 vector32 = new Vector3(position.x + radius * Mathf.Cos(0f), position.y + radius * Mathf.Sin(0f), position.z);
			for (int i = 1; i < 37; i++)
			{
				Vector3 vector33 = new Vector3(position.x, position.y + radius * Mathf.Sin(single * (float)i * 0.0174532924f), position.z + radius * Mathf.Cos(single * (float)i * 0.0174532924f));
				Vector3 vector34 = new Vector3(position.x + radius * Mathf.Cos(single * (float)i * 0.0174532924f), position.y, position.z + radius * Mathf.Sin(single * (float)i * 0.0174532924f));
				Vector3 vector35 = new Vector3(position.x + radius * Mathf.Cos(single * (float)i * 0.0174532924f), position.y + radius * Mathf.Sin(single * (float)i * 0.0174532924f), position.z);
				Debug.DrawLine(vector3, vector33, color, duration, depthTest);
				Debug.DrawLine(vector31, vector34, color, duration, depthTest);
				Debug.DrawLine(vector32, vector35, color, duration, depthTest);
				vector3 = vector33;
				vector31 = vector34;
				vector32 = vector35;
			}
		}

		public static void Sphere(Vector3 position, float radius = 1f, float duration = 0f, bool depthTest = true)
		{
			DebugDraw.Sphere(position, Color.white, radius, duration, depthTest);
		}
	}
}