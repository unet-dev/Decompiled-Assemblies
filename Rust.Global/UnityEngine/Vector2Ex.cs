using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class Vector2Ex
	{
		public static float Length(float x, float y)
		{
			return Mathf.Sqrt(x * x + y * y);
		}

		public static float Length(Vector2 vec)
		{
			return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y);
		}

		public static Vector2 Parse(string p)
		{
			string[] strArrays = p.Split(new char[] { ' ' });
			if ((int)strArrays.Length != 2)
			{
				return Vector2.zero;
			}
			return new Vector2(float.Parse(strArrays[0]), float.Parse(strArrays[1]));
		}

		public static Vector2 Rotate(this Vector2 v, float degrees)
		{
			float single = degrees * 0.0174532924f;
			float single1 = Mathf.Sin(single);
			float single2 = Mathf.Cos(single);
			return new Vector2(v.x * single2 - v.y * single1, v.y * single2 + v.x * single1);
		}
	}
}