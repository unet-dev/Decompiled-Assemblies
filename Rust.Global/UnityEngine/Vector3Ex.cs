using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class Vector3Ex
	{
		public static Vector4 Abs(this Vector4 v)
		{
			return new Vector4(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z), Mathf.Abs(v.w));
		}

		public static Vector3 Abs(this Vector3 v)
		{
			return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
		}

		public static Vector2 Abs(this Vector2 v)
		{
			return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
		}

		public static Vector3 BlendNormals(Vector3 n1, Vector3 n2)
		{
			return Vector3.Normalize(new Vector3(n1.x + n2.x, n1.y * n2.y, n1.z + n2.z));
		}

		public static Vector3 Direction(Vector3 aimAt, Vector3 aimFrom)
		{
			return (aimAt - aimFrom).normalized;
		}

		public static Vector3 Direction2D(Vector3 aimAt, Vector3 aimFrom)
		{
			Vector3 vector3 = new Vector3(aimAt.x, 0f, aimAt.z) - new Vector3(aimFrom.x, 0f, aimFrom.z);
			return vector3.normalized;
		}

		public static float Distance2D(Vector3 a, Vector3 b)
		{
			return Vector3.Distance(new Vector3(a.x, 0f, a.z), new Vector3(b.x, 0f, b.z));
		}

		public static float DotDegrees(this Vector3 a, Vector3 b)
		{
			return a.DotRadians(b) * 57.29578f;
		}

		public static float DotRadians(this Vector3 a, Vector3 b)
		{
			float single = Vector3.Dot(a.normalized, b.normalized);
			if (float.IsNaN(single))
			{
				Debug.LogWarning(string.Concat(new object[] { "DotRadians NAN: ", a, " -> ", b }));
			}
			return Mathf.Acos(Mathf.Clamp(single, -1f, 1f));
		}

		public static void FastRenormalize(this Vector3 n, float scale = 1f)
		{
			float single = n.x * n.x + n.y * n.y + n.z * n.z;
			float single1 = (1.875f + -1.25f * single + 0.375f * single * single) * scale;
			n.x *= single1;
			n.y *= single1;
			n.z *= single1;
		}

		public static Vector3 Inverse(this Vector3 v)
		{
			return new Vector3(1f / v.x, 1f / v.y, 1f / v.z);
		}

		public static bool IsNaNOrInfinity(this Vector3 v)
		{
			if (float.IsNaN(v.x))
			{
				return true;
			}
			if (float.IsNaN(v.y))
			{
				return true;
			}
			if (float.IsNaN(v.z))
			{
				return true;
			}
			if (float.IsInfinity(v.x))
			{
				return true;
			}
			if (float.IsInfinity(v.y))
			{
				return true;
			}
			if (float.IsInfinity(v.z))
			{
				return true;
			}
			return false;
		}

		public static float Magnitude2D(this Vector3 v)
		{
			return v.MagnitudeXZ();
		}

		public static float MagnitudeXY(this Vector3 v)
		{
			return Mathf.Sqrt(v.x * v.x + v.y * v.y);
		}

		public static float MagnitudeXZ(this Vector3 v)
		{
			return Mathf.Sqrt(v.x * v.x + v.z * v.z);
		}

		public static float MagnitudeYZ(this Vector3 v)
		{
			return Mathf.Sqrt(v.y * v.y + v.z * v.z);
		}

		public static float Max(this Vector4 v)
		{
			return Mathf.Max(Mathf.Max(v.x, v.y), Mathf.Max(v.z, v.w));
		}

		public static float Max(this Vector3 v)
		{
			return Mathf.Max(Mathf.Max(v.x, v.y), v.z);
		}

		public static float Max(this Vector2 v)
		{
			return Mathf.Max(v.x, v.y);
		}

		public static Vector3 Parse(string p)
		{
			string[] strArrays = p.Split(new char[] { ' ' });
			if ((int)strArrays.Length != 3)
			{
				return Vector3.zero;
			}
			return new Vector3(float.Parse(strArrays[0]), float.Parse(strArrays[1]), float.Parse(strArrays[2]));
		}

		public static Vector3 Range(float x, float y)
		{
			return new Vector3(UnityEngine.Random.Range(x, y), UnityEngine.Random.Range(x, y), UnityEngine.Random.Range(x, y));
		}

		public static Vector3 Scale(this Vector3 vector, float x, float y, float z)
		{
			return new Vector3(vector.x * x, vector.y * y, vector.z * z);
		}

		public static float SignedAngle(this Vector3 v1, Vector3 v2, Vector3 n)
		{
			float single = Vector3.Angle(v1, v2);
			float single1 = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(v1, v2)));
			return single * single1;
		}

		public static Vector3 SnapTo(this Vector3 vector, float snapValue)
		{
			return new Vector3(vector.x.SnapTo(snapValue), vector.y.SnapTo(snapValue), vector.z.SnapTo(snapValue));
		}

		public static float SqrMagnitude2D(this Vector3 v)
		{
			return v.SqrMagnitudeXZ();
		}

		public static float SqrMagnitudeXY(this Vector3 v)
		{
			return v.x * v.x + v.y * v.y;
		}

		public static float SqrMagnitudeXZ(this Vector3 v)
		{
			return v.x * v.x + v.z * v.z;
		}

		public static float SqrMagnitudeYZ(this Vector3 v)
		{
			return v.y * v.y + v.z * v.z;
		}

		public static Vector2 XY2D(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		public static Vector3 XY3D(this Vector3 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		public static Vector3 XZ(Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		public static Vector2 XZ2D(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static Vector3 XZ3D(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		public static Vector2 YX2D(this Vector3 v)
		{
			return new Vector2(v.y, v.x);
		}

		public static Vector2 YZ2D(this Vector3 v)
		{
			return new Vector2(v.y, v.z);
		}

		public static Vector3 YZ3D(this Vector3 v)
		{
			return new Vector3(0f, v.y, v.z);
		}

		public static Vector2 ZX2D(this Vector3 v)
		{
			return new Vector2(v.z, v.x);
		}

		public static Vector2 ZY2D(this Vector3 v)
		{
			return new Vector2(v.z, v.y);
		}
	}
}