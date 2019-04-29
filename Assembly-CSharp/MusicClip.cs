using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicClip : ScriptableObject
{
	public AudioClip audioClip;

	public int lengthInBars = 1;

	public int lengthInBarsWithTail;

	public List<float> fadeInPoints = new List<float>();

	public MusicClip()
	{
	}

	public float GetNextFadeInPoint(float currentClipTimeBars)
	{
		if (this.fadeInPoints.Count == 0)
		{
			return currentClipTimeBars;
		}
		float single = -1f;
		float single1 = Single.PositiveInfinity;
		for (int i = 0; i < this.fadeInPoints.Count; i++)
		{
			float item = this.fadeInPoints[i];
			float single2 = item - currentClipTimeBars;
			if (item > 0.01f && single2 > 0f && single2 < single1)
			{
				single1 = single2;
				single = item;
			}
		}
		return single;
	}
}