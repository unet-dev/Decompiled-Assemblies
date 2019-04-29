using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Environment Volume Properties")]
public class EnvironmentVolumeProperties : ScriptableObject
{
	public int ReflectionQuality;

	public LayerMask ReflectionCullingFlags;

	[Horizontal(1, 0)]
	public EnvironmentMultiplier[] ReflectionMultipliers;

	[Horizontal(1, 0)]
	public EnvironmentMultiplier[] AmbientMultipliers;

	public EnvironmentVolumeProperties()
	{
	}

	public float FindAmbientMultiplier(EnvironmentType type)
	{
		EnvironmentMultiplier[] ambientMultipliers = this.AmbientMultipliers;
		for (int i = 0; i < (int)ambientMultipliers.Length; i++)
		{
			EnvironmentMultiplier environmentMultiplier = ambientMultipliers[i];
			if ((int)(type & environmentMultiplier.Type) != 0)
			{
				return environmentMultiplier.Multiplier;
			}
		}
		return 1f;
	}

	public float FindReflectionMultiplier(EnvironmentType type)
	{
		EnvironmentMultiplier[] reflectionMultipliers = this.ReflectionMultipliers;
		for (int i = 0; i < (int)reflectionMultipliers.Length; i++)
		{
			EnvironmentMultiplier environmentMultiplier = reflectionMultipliers[i];
			if ((int)(type & environmentMultiplier.Type) != 0)
			{
				return environmentMultiplier.Multiplier;
			}
		}
		return 1f;
	}
}