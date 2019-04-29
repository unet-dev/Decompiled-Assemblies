using System;
using UnityEngine;

public class RandomParameterNumber : StateMachineBehaviour
{
	public string parameterName;

	public int min;

	public int max;

	public RandomParameterNumber()
	{
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetInteger(this.parameterName, UnityEngine.Random.Range(this.min, this.max));
	}
}