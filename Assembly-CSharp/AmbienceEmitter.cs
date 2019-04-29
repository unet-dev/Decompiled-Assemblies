using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AmbienceEmitter : MonoBehaviour, IClientComponent, IComparable<AmbienceEmitter>
{
	public AmbienceDefinitionList baseAmbience;

	public AmbienceDefinitionList stings;

	public bool isStatic = true;

	public bool followCamera;

	public bool isBaseEmitter;

	public bool active;

	public float cameraDistance = Single.PositiveInfinity;

	public BoundingSphere boundingSphere;

	public float crossfadeTime = 2f;

	public Dictionary<AmbienceDefinition, float> nextStingTime = new Dictionary<AmbienceDefinition, float>();

	public float deactivateTime = Single.PositiveInfinity;

	public TerrainBiome.Enum currentBiome
	{
		get;
		private set;
	}

	public TerrainTopology.Enum currentTopology
	{
		get;
		private set;
	}

	public AmbienceEmitter()
	{
	}

	public int CompareTo(AmbienceEmitter other)
	{
		return this.cameraDistance.CompareTo(other.cameraDistance);
	}
}