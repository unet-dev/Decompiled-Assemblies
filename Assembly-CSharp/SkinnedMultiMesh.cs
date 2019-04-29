using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMultiMesh : MonoBehaviour
{
	public bool shadowOnly;

	public List<SkinnedMultiMesh.Part> parts = new List<SkinnedMultiMesh.Part>();

	public BoneDictionary boneDict;

	[NonSerialized]
	public List<SkinnedMultiMesh.Part> createdParts = new List<SkinnedMultiMesh.Part>();

	[NonSerialized]
	public long lastBuildHash;

	[NonSerialized]
	public MaterialPropertyBlock sharedPropertyBlock;

	[NonSerialized]
	public MaterialPropertyBlock hairPropertyBlock;

	public float skinNumber;

	public float meshNumber;

	public float hairNumber;

	public int skinType;

	public SkinSetCollection SkinCollection;

	private ArticulatedOccludee articulatedOccludee;

	private LODGroup lodGroup;

	private List<Renderer> renderers = new List<Renderer>(32);

	private List<Animator> animators = new List<Animator>(8);

	public List<Animator> Animators
	{
		get
		{
			return this.animators;
		}
	}

	public List<Renderer> Renderers
	{
		get
		{
			return this.renderers;
		}
	}

	public SkinnedMultiMesh()
	{
	}

	public struct Part
	{
		public GameObject gameObject;

		public string name;

		public Item item;
	}
}