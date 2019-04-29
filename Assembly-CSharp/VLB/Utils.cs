using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace VLB
{
	public static class Utils
	{
		private static Utils.FloatPackingPrecision ms_FloatPackingPrecision;

		private const int kFloatPackingHighMinShaderLevel = 35;

		static Utils()
		{
		}

		public static Utils.FloatPackingPrecision GetFloatPackingPrecision()
		{
			if (Utils.ms_FloatPackingPrecision == Utils.FloatPackingPrecision.Undef)
			{
				Utils.ms_FloatPackingPrecision = (SystemInfo.graphicsShaderLevel >= 35 ? Utils.FloatPackingPrecision.High : Utils.FloatPackingPrecision.Low);
			}
			return Utils.ms_FloatPackingPrecision;
		}

		public static float GetMaxArea2D(this Bounds self)
		{
			return Mathf.Max(Mathf.Max(self.size.x * self.size.y, self.size.y * self.size.z), self.size.x * self.size.z);
		}

		public static T GetOrAddComponent<T>(this GameObject self)
		where T : Component
		{
			T component = self.GetComponent<T>();
			if (component == null)
			{
				component = self.AddComponent<T>();
			}
			return component;
		}

		public static T GetOrAddComponent<T>(this MonoBehaviour self)
		where T : Component
		{
			return self.gameObject.GetOrAddComponent<T>();
		}

		public static string GetPath(Transform current)
		{
			if (current.parent == null)
			{
				return string.Concat("/", current.name);
			}
			return string.Concat(Utils.GetPath(current.parent), "/", current.name);
		}

		public static float GetVolumeCubic(this Bounds self)
		{
			return self.size.x * self.size.y * self.size.z;
		}

		public static void GizmosDrawPlane(Vector3 normal, Vector3 position, Color color, float size = 1f)
		{
			Vector3 vector3 = Vector3.Cross(normal, (Mathf.Abs(Vector3.Dot(normal, Vector3.forward)) < 0.999f ? Vector3.forward : Vector3.up));
			Vector3 vector31 = vector3.normalized * size;
			Vector3 vector32 = position + vector31;
			Vector3 vector33 = position - vector31;
			vector31 = Quaternion.AngleAxis(90f, normal) * vector31;
			Vector3 vector34 = position + vector31;
			Vector3 vector35 = position - vector31;
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = color;
			Gizmos.DrawLine(vector32, vector33);
			Gizmos.DrawLine(vector34, vector35);
			Gizmos.DrawLine(vector32, vector34);
			Gizmos.DrawLine(vector34, vector33);
			Gizmos.DrawLine(vector33, vector35);
			Gizmos.DrawLine(vector35, vector32);
		}

		public static bool HasFlag(this Enum mask, Enum flags)
		{
			return ((int)mask & (int)flags) == (int)flags;
		}

		public static bool IsValid(this Plane plane)
		{
			return plane.normal.sqrMagnitude > 0.5f;
		}

		public static void MarkCurrentSceneDirty()
		{
		}

		public static T NewWithComponent<T>(string name)
		where T : Component
		{
			return (new GameObject(name, new Type[] { typeof(T) })).GetComponent<T>();
		}

		public static Color Opaque(this Color self)
		{
			return new Color(self.r, self.g, self.b, 1f);
		}

		public static float PackToFloat(this Color color, int floatPackingPrecision)
		{
			Vector4 vector4 = Utils.Vector4_Floor(color * (float)(floatPackingPrecision - 1));
			return 0f + vector4.x * (float)floatPackingPrecision * (float)floatPackingPrecision * (float)floatPackingPrecision + vector4.y * (float)floatPackingPrecision * (float)floatPackingPrecision + vector4.z * (float)floatPackingPrecision + vector4.w;
		}

		public static Color[] SampleInArray(this Gradient self, int samplesCount)
		{
			Color[] colorArray = new Color[samplesCount];
			for (int i = 0; i < samplesCount; i++)
			{
				colorArray[i] = self.Evaluate(Mathf.Clamp01((float)i / (float)(samplesCount - 1)));
			}
			return colorArray;
		}

		public static Matrix4x4 SampleInMatrix(this Gradient self, int floatPackingPrecision)
		{
			Matrix4x4 num = new Matrix4x4();
			for (int i = 0; i < 16; i++)
			{
				Color color = self.Evaluate(Mathf.Clamp01((float)i / 15f));
				num[i] = color.PackToFloat(floatPackingPrecision);
			}
			return num;
		}

		public static Plane TranslateCustom(this Plane plane, Vector3 translation)
		{
			plane.distance = plane.distance + Vector3.Dot(translation.normalized, plane.normal) * translation.magnitude;
			return plane;
		}

		private static Vector4 Vector4_Floor(Vector4 vec)
		{
			return new Vector4(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z), Mathf.Floor(vec.w));
		}

		public static Vector2 xy(this Vector3 aVector)
		{
			return new Vector2(aVector.x, aVector.y);
		}

		public static Vector2 xz(this Vector3 aVector)
		{
			return new Vector2(aVector.x, aVector.z);
		}

		public static Vector2 yx(this Vector3 aVector)
		{
			return new Vector2(aVector.y, aVector.x);
		}

		public static Vector2 yz(this Vector3 aVector)
		{
			return new Vector2(aVector.y, aVector.z);
		}

		public static Vector2 zx(this Vector3 aVector)
		{
			return new Vector2(aVector.z, aVector.x);
		}

		public static Vector2 zy(this Vector3 aVector)
		{
			return new Vector2(aVector.z, aVector.y);
		}

		public enum FloatPackingPrecision
		{
			Undef = 0,
			Low = 8,
			High = 64
		}
	}
}