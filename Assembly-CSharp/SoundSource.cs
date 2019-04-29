using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour, IClientComponentEx, ILOD
{
	[Header("Occlusion")]
	public bool handleOcclusionChecks;

	public LayerMask occlusionLayerMask;

	public List<SoundSource.OcclusionPoint> occlusionPoints = new List<SoundSource.OcclusionPoint>();

	public bool isOccluded;

	public float occlusionAmount;

	public float lodDistance = 100f;

	public bool inRange;

	public SoundSource()
	{
	}

	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent(this);
	}

	[Serializable]
	public class OcclusionPoint
	{
		public Vector3 offset;

		public bool isOccluded;

		public OcclusionPoint()
		{
		}
	}
}