using System;
using UnityEngine;

public abstract class BaseFootstepEffect : MonoBehaviour, IClientComponent
{
	public LayerMask validImpactLayers = -1;

	protected BaseFootstepEffect()
	{
	}
}