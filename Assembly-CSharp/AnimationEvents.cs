using System;
using UnityEngine;

public class AnimationEvents : BaseMonoBehaviour
{
	public Transform rootObject;

	public HeldEntity targetEntity;

	[Tooltip("Path to the effect folder for these animations. Relative to this object.")]
	public string effectFolder;

	public string localFolder;

	public bool IsBusy;

	public AnimationEvents()
	{
	}

	protected void OnEnable()
	{
		if (this.rootObject == null)
		{
			this.rootObject = base.transform;
		}
	}
}