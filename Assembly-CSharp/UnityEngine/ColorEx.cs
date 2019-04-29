using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class ColorEx
	{
		public static Color Parse(string str)
		{
			string[] strArrays = str.Split(new char[] { ' ' });
			if ((int)strArrays.Length == 3)
			{
				return new Color(float.Parse(strArrays[0]), float.Parse(strArrays[1]), float.Parse(strArrays[2]));
			}
			if ((int)strArrays.Length != 4)
			{
				return Color.white;
			}
			return new Color(float.Parse(strArrays[0]), float.Parse(strArrays[1]), float.Parse(strArrays[2]), float.Parse(strArrays[3]));
		}

		public static string ToHex(this Color32 color)
		{
			return string.Concat(color.r.ToString("X2"), color.g.ToString("X2"), color.b.ToString("X2"));
		}
	}
}