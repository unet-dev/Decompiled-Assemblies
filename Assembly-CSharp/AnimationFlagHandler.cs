using System;
using UnityEngine;

public class AnimationFlagHandler : MonoBehaviour
{
	public Animator animator;

	public AnimationFlagHandler()
	{
	}

	public void SetBoolFalse(string name)
	{
		this.animator.SetBool(name, false);
	}

	public void SetBoolTrue(string name)
	{
		this.animator.SetBool(name, true);
	}
}