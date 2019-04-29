using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeIntensity : MonoBehaviour
{
	public float raiseTo;

	public List<MusicChangeIntensity.DistanceIntensity> distanceIntensities = new List<MusicChangeIntensity.DistanceIntensity>();

	public float tickInterval = 0.2f;

	public MusicChangeIntensity()
	{
	}

	[Serializable]
	public class DistanceIntensity
	{
		public float distance;

		public float raiseTo;

		public bool forceStartMusicInSuppressedMusicZones;

		public DistanceIntensity()
		{
		}
	}
}