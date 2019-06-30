using Facepunch.Extend;
using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class StringExtensions
	{
		public static string BBCodeToUnity(this string x)
		{
			x = x.Replace("[", "<");
			x = x.Replace("]", ">");
			return x;
		}

		public static Color ToColor(this string str)
		{
			Color color = new Color(1f, 1f, 1f, 1f);
			string[] strArrays = str.Split(new char[] { ',' });
			if ((int)strArrays.Length != 3 && (int)strArrays.Length != 4)
			{
				return color;
			}
			color.r = strArrays[0].ToFloat(0f);
			color.g = strArrays[1].ToFloat(0f);
			color.b = strArrays[2].ToFloat(0f);
			if ((int)strArrays.Length == 4)
			{
				color.a = strArrays[3].ToFloat(0f);
			}
			return color;
		}

		public static Vector3 ToVector3(this string str)
		{
			Vector3 num = new Vector3();
			string[] strArrays = str.Trim(new char[] { '(', ')', ' ' }).Replace(",", " ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if ((int)strArrays.Length != 3)
			{
				return num;
			}
			num.x = strArrays[0].ToFloat(0f);
			num.y = strArrays[1].ToFloat(0f);
			num.z = strArrays[2].ToFloat(0f);
			return num;
		}
	}
}