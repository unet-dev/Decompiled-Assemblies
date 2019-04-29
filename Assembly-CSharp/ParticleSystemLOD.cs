using System;
using UnityEngine;

public class ParticleSystemLOD : LODComponentParticleSystem
{
	[Horizontal(1, 0)]
	public ParticleSystemLOD.State[] States;

	public ParticleSystemLOD()
	{
	}

	[Serializable]
	public class State
	{
		public float distance;

		[Range(0f, 1f)]
		public float emission;

		public State()
		{
		}
	}
}