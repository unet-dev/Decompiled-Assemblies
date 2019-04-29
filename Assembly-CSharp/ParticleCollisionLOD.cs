using System;

public class ParticleCollisionLOD : LODComponentParticleSystem
{
	[Horizontal(1, 0)]
	public ParticleCollisionLOD.State[] States;

	public ParticleCollisionLOD()
	{
	}

	public enum QualityLevel
	{
		Disabled = -1,
		HighQuality = 0,
		MediumQuality = 1,
		LowQuality = 2
	}

	[Serializable]
	public class State
	{
		public float distance;

		public ParticleCollisionLOD.QualityLevel quality;

		public State()
		{
		}
	}
}