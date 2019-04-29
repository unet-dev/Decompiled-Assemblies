using System;
using UnityEngine;

[RequireComponent(typeof(OnePoleLowpassFilter))]
public class SoundOcclusion : MonoBehaviour
{
	public LayerMask occlusionLayerMask;

	public SoundOcclusion()
	{
	}
}