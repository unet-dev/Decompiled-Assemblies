using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundDefinition : ScriptableObject
{
	public GameObjectRef template;

	[Horizontal(2, -1)]
	public List<WeightedAudioClip> weightedAudioClips = new List<WeightedAudioClip>()
	{
		new WeightedAudioClip()
	};

	public List<SoundDefinition.DistanceAudioClipList> distanceAudioClips;

	public SoundClass soundClass;

	public bool defaultToFirstPerson;

	public bool loop;

	public bool randomizeStartPosition;

	[Range(0f, 1f)]
	public float volume = 1f;

	[Range(0f, 1f)]
	public float volumeVariation;

	[Range(-3f, 3f)]
	public float pitch = 1f;

	[Range(0f, 1f)]
	public float pitchVariation;

	[Header("Voice limiting")]
	public bool dontVoiceLimit;

	public int globalVoiceMaxCount = 100;

	public int localVoiceMaxCount = 100;

	public float localVoiceRange = 10f;

	public float voiceLimitFadeOutTime = 0.05f;

	public float localVoiceDebounceTime = 0.1f;

	[Header("Occlusion Settings")]
	public bool forceOccludedPlayback;

	[Header("Custom curves")]
	public AnimationCurve falloffCurve;

	public bool useCustomFalloffCurve;

	public AnimationCurve spatialBlendCurve;

	public bool useCustomSpatialBlendCurve;

	public AnimationCurve spreadCurve;

	public bool useCustomSpreadCurve;

	public float maxDistance
	{
		get
		{
			if (this.template == null)
			{
				return 0f;
			}
			AudioSource component = this.template.Get().GetComponent<AudioSource>();
			if (component == null)
			{
				return 0f;
			}
			return component.maxDistance;
		}
	}

	public SoundDefinition()
	{
	}

	public float GetLength()
	{
		float single = 0f;
		for (int i = 0; i < this.weightedAudioClips.Count; i++)
		{
			AudioClip item = this.weightedAudioClips[i].audioClip;
			if (item)
			{
				single = Mathf.Max(item.length, single);
			}
		}
		for (int j = 0; j < this.distanceAudioClips.Count; j++)
		{
			List<WeightedAudioClip> weightedAudioClips = this.distanceAudioClips[j].audioClips;
			for (int k = 0; k < weightedAudioClips.Count; k++)
			{
				AudioClip audioClip = weightedAudioClips[k].audioClip;
				if (audioClip)
				{
					single = Mathf.Max(audioClip.length, single);
				}
			}
		}
		return single;
	}

	[Serializable]
	public class DistanceAudioClipList
	{
		public int distance;

		[Horizontal(2, -1)]
		public List<WeightedAudioClip> audioClips;

		public DistanceAudioClipList()
		{
		}
	}
}