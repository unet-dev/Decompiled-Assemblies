using System;
using UnityEngine;

public class TimeBasedSoundSpread : SoundModifier
{
	public AnimationCurve spreadCurve;

	public AnimationCurve wanderIntensityCurve;

	public TimeBasedSoundSpread()
	{
	}
}