using System;
using System.Collections.Generic;
using UnityEngine;

public class RainSurfaceAmbience : MonoBehaviour
{
	public float tickRate = 1f;

	public float gridSize = 20f;

	public float gridSamples = 10f;

	public float startHeight = 100f;

	public float rayLength = 250f;

	public LayerMask layerMask;

	public float spreadScale = 8f;

	public float maxDistance = 10f;

	public float lerpSpeed = 5f;

	public List<RainSurfaceAmbience.SurfaceSound> surfaces = new List<RainSurfaceAmbience.SurfaceSound>();

	public RainSurfaceAmbience()
	{
	}

	[Serializable]
	public class SurfaceSound
	{
		public SoundDefinition soundDef;

		public List<PhysicMaterial> materials;

		[HideInInspector]
		public Sound sound;

		[HideInInspector]
		public float amount;

		[HideInInspector]
		public Vector3 position;

		[HideInInspector]
		public Bounds bounds;

		[HideInInspector]
		public List<Vector3> points;

		[HideInInspector]
		public SoundModulation.Modulator gainMod;

		[HideInInspector]
		public SoundModulation.Modulator spreadMod;

		public SurfaceSound()
		{
		}
	}
}