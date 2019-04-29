using System;
using UnityEngine;

public class RandomParameterNumberFloat : StateMachineBehaviour
{
	public string parameterName;

	public int min;

	public int max;

	public RandomParameterNumberFloat()
	{
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (string.IsNullOrEmpty(this.parameterName))
		{
			return;
		}
		animator.SetFloat(this.parameterName, Mathf.Floor(UnityEngine.Random.Range((float)this.min, (float)this.max + 0.5f)));
	}
}