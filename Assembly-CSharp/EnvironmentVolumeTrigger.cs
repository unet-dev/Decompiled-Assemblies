using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnvironmentVolumeTrigger : MonoBehaviour
{
	[HideInInspector]
	public Vector3 Center = Vector3.zero;

	[HideInInspector]
	public Vector3 Size = Vector3.one;

	public EnvironmentVolume volume
	{
		get;
		private set;
	}

	public EnvironmentVolumeTrigger()
	{
	}

	protected void Awake()
	{
		this.volume = base.gameObject.GetComponent<EnvironmentVolume>();
		if (this.volume == null)
		{
			this.volume = base.gameObject.AddComponent<EnvironmentVolume>();
			this.volume.Center = this.Center;
			this.volume.Size = this.Size;
			this.volume.UpdateTrigger();
		}
	}
}