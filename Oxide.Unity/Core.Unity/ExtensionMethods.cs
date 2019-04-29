using Oxide.Core.Logging;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oxide.Core.Unity
{
	public static class ExtensionMethods
	{
		public static Oxide.Core.Logging.LogType ToLogType(this UnityEngine.LogType logType)
		{
			switch (logType)
			{
				case UnityEngine.LogType.Error:
				case UnityEngine.LogType.Assert:
				case UnityEngine.LogType.Exception:
				{
					return 1;
				}
				case UnityEngine.LogType.Warning:
				{
					return 3;
				}
				case UnityEngine.LogType.Log:
				{
					return 2;
				}
				default:
				{
					return 2;
				}
			}
		}

		public static Vector3 ToVector3(this string vector3)
		{
			float[] array = vector3.Split(new char[] { ',' }).Select<string, float>(new Func<string, float>(Convert.ToSingle)).ToArray<float>();
			if ((int)array.Length != 3)
			{
				return Vector3.zero;
			}
			return new Vector3(array[0], array[1], array[2]);
		}
	}
}