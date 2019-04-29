using System;
using UnityEngine;

public class DevWeatherAdjust : MonoBehaviour
{
	public DevWeatherAdjust()
	{
	}

	protected void Awake()
	{
		SingletonComponent<Climate>.Instance.Overrides.Clouds = 0f;
		SingletonComponent<Climate>.Instance.Overrides.Fog = 0f;
		SingletonComponent<Climate>.Instance.Overrides.Wind = 0f;
		SingletonComponent<Climate>.Instance.Overrides.Rain = 0f;
	}

	protected void OnGUI()
	{
		float single = (float)Screen.width * 0.2f;
		GUILayout.BeginArea(new Rect((float)Screen.width - single - 20f, 20f, single, 400f), "", "box");
		GUILayout.Box("Weather", Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		GUILayout.Label("Clouds", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Clouds = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Clouds, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Fog", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Fog = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Fog, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Wind", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Wind = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Wind, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.Label("Rain", Array.Empty<GUILayoutOption>());
		SingletonComponent<Climate>.Instance.Overrides.Rain = GUILayout.HorizontalSlider(SingletonComponent<Climate>.Instance.Overrides.Rain, 0f, 1f, Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}
}