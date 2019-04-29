using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Environment Volume Properties Collection")]
public class EnvironmentVolumePropertiesCollection : ScriptableObject
{
	public float TransitionSpeed = 1f;

	public EnvironmentVolumeProperties[] Properties;

	public EnvironmentVolumePropertiesCollection()
	{
	}

	public EnvironmentVolumeProperties FindQuality(int quality)
	{
		EnvironmentVolumeProperties[] properties = this.Properties;
		for (int i = 0; i < (int)properties.Length; i++)
		{
			EnvironmentVolumeProperties environmentVolumeProperty = properties[i];
			if (environmentVolumeProperty.ReflectionQuality == quality)
			{
				return environmentVolumeProperty;
			}
		}
		return null;
	}
}