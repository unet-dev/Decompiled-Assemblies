using System;
using UnityEngine;

[Serializable]
public struct FogSettings
{
	public Gradient ColorOverDaytime;

	public float Density;

	public float StartDistance;

	public float Height;

	public float HeightDensity;

	public static FogSettings Default;

	static FogSettings()
	{
		FogSettings fogSetting = new FogSettings();
		Gradient gradient = new Gradient()
		{
			colorKeys = new GradientColorKey[] { new GradientColorKey(Color.gray, 0f), new GradientColorKey(Color.gray, 1f) },
			alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
		};
		fogSetting.ColorOverDaytime = gradient;
		fogSetting.Density = 0.001f;
		fogSetting.StartDistance = 0f;
		fogSetting.Height = 0f;
		fogSetting.HeightDensity = 0.5f;
		FogSettings.Default = fogSetting;
	}

	public static FogSettings Lerp(FogSettings source, FogSettings target, float t)
	{
		FogSettings fogSetting = new FogSettings()
		{
			Density = Mathf.Lerp(source.Density, target.Density, t),
			StartDistance = Mathf.Lerp(source.StartDistance, target.StartDistance, t),
			Height = Mathf.Lerp(source.Height, target.Height, t),
			HeightDensity = Mathf.Lerp(source.HeightDensity, target.HeightDensity, t)
		};
		return fogSetting;
	}
}