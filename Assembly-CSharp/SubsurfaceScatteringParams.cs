using System;

[Serializable]
public struct SubsurfaceScatteringParams
{
	public bool enabled;

	public SubsurfaceScatteringParams.Quality quality;

	public bool halfResolution;

	public float radiusScale;

	public static SubsurfaceScatteringParams Default;

	static SubsurfaceScatteringParams()
	{
		SubsurfaceScatteringParams subsurfaceScatteringParam = new SubsurfaceScatteringParams()
		{
			enabled = true,
			quality = SubsurfaceScatteringParams.Quality.Medium,
			halfResolution = true,
			radiusScale = 1f
		};
		SubsurfaceScatteringParams.Default = subsurfaceScatteringParam;
	}

	public enum Quality
	{
		Low,
		Medium,
		High
	}
}