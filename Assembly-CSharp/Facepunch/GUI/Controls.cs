using System;
using UnityEngine;

namespace Facepunch.GUI
{
	public static class Controls
	{
		public static float labelWidth;

		static Controls()
		{
			Controls.labelWidth = 100f;
		}

		public static bool Button(string strName)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool flag = GUILayout.Button(strName, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return flag;
		}

		public static bool Checkbox(string strName, bool value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, new GUILayoutOption[] { GUILayout.Width(Controls.labelWidth) });
			bool flag = GUILayout.Toggle(value, "", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return flag;
		}

		public static float FloatSlider(string strLabel, float value, float low, float high, string format = "0.00")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, new GUILayoutOption[] { GUILayout.Width(Controls.labelWidth) });
			float single = float.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[] { GUILayout.ExpandWidth(true) }));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			float single1 = GUILayout.HorizontalSlider(single, low, high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return single1;
		}

		public static int IntSlider(string strLabel, int value, int low, int high, string format = "0")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, new GUILayoutOption[] { GUILayout.Width(Controls.labelWidth) });
			int num = int.Parse(GUILayout.TextField(value.ToString(format), new GUILayoutOption[] { GUILayout.ExpandWidth(true) }));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			int num1 = (int)GUILayout.HorizontalSlider((float)num, (float)low, (float)high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return num1;
		}

		public static string TextArea(string strName, string value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, new GUILayoutOption[] { GUILayout.Width(Controls.labelWidth) });
			string str = GUILayout.TextArea(value, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return str;
		}
	}
}