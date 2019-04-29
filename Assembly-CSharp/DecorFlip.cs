using System;
using UnityEngine;

public class DecorFlip : DecorComponent
{
	public DecorFlip.AxisType FlipAxis = DecorFlip.AxisType.Y;

	public DecorFlip()
	{
	}

	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 4;
		if (SeedRandom.Value(ref num) > 0.5f)
		{
			return;
		}
		switch (this.FlipAxis)
		{
			case DecorFlip.AxisType.X:
			case DecorFlip.AxisType.Z:
			{
				rot = Quaternion.AngleAxis(180f, rot * Vector3.up) * rot;
				return;
			}
			case DecorFlip.AxisType.Y:
			{
				rot = Quaternion.AngleAxis(180f, rot * Vector3.forward) * rot;
				return;
			}
			default:
			{
				return;
			}
		}
	}

	public enum AxisType
	{
		X,
		Y,
		Z
	}
}