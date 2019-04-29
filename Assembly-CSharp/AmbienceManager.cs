using System;
using System.Collections.Generic;

public class AmbienceManager : SingletonComponent<AmbienceManager>, IClientComponent
{
	public List<AmbienceManager.EmitterTypeLimit> localEmitterLimits = new List<AmbienceManager.EmitterTypeLimit>();

	public AmbienceManager.EmitterTypeLimit catchallEmitterLimit = new AmbienceManager.EmitterTypeLimit();

	public int maxActiveLocalEmitters = 5;

	public int activeLocalEmitters;

	public List<AmbienceEmitter> cameraEmitters = new List<AmbienceEmitter>();

	public List<AmbienceEmitter> emittersInRange = new List<AmbienceEmitter>();

	public List<AmbienceEmitter> activeEmitters = new List<AmbienceEmitter>();

	public float localEmitterRange = 30f;

	public List<AmbienceZone> currentAmbienceZones = new List<AmbienceZone>();

	public AmbienceManager()
	{
	}

	[Serializable]
	public class EmitterTypeLimit
	{
		public List<AmbienceDefinitionList> ambience;

		public int limit;

		public int active;

		public EmitterTypeLimit()
		{
		}
	}
}