using System;

public class WaterOverlay : ImageEffectLayer, IClientComponent
{
	public static bool goggles;

	public WaterOverlay.EffectParams gogglesParams = WaterOverlay.EffectParams.DefaultGoggles;

	static WaterOverlay()
	{
	}

	public WaterOverlay()
	{
	}

	[Serializable]
	public struct EffectParams
	{
		public float scatterCoefficient;

		public bool blur;

		public float blurDistance;

		public bool wiggle;

		public float doubleVisionAmount;

		public float photoFilterDensity;

		public static WaterOverlay.EffectParams DefaultGoggles;

		static EffectParams()
		{
			WaterOverlay.EffectParams effectParam = new WaterOverlay.EffectParams()
			{
				scatterCoefficient = 0.1f,
				blur = false,
				blurDistance = 10f,
				wiggle = false,
				doubleVisionAmount = 0.753f,
				photoFilterDensity = 1f
			};
			WaterOverlay.EffectParams.DefaultGoggles = effectParam;
		}
	}
}