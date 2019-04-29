using System;
using UnityEngine;

public class SoundModulation : MonoBehaviour, IClientComponent
{
	private const int parameterCount = 4;

	public SoundModulation()
	{
	}

	[Serializable]
	public class Modulator
	{
		public SoundModulation.Parameter param;

		public float @value;

		public Modulator()
		{
		}
	}

	public enum Parameter
	{
		Gain,
		Pitch,
		Spread,
		MaxDistance
	}
}