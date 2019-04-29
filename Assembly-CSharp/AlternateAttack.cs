using System;
using UnityEngine;

public class AlternateAttack : StateMachineBehaviour
{
	public bool random;

	public bool dontIncrement;

	public string[] targetTransitions;

	public AlternateAttack()
	{
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.random)
		{
			string str = this.targetTransitions[UnityEngine.Random.Range(0, (int)this.targetTransitions.Length)];
			animator.Play(str, layerIndex, 0f);
			return;
		}
		int integer = animator.GetInteger("lastAttack");
		string str1 = this.targetTransitions[integer % (int)this.targetTransitions.Length];
		animator.Play(str1, layerIndex, 0f);
		if (!this.dontIncrement)
		{
			animator.SetInteger("lastAttack", integer + 1);
		}
	}
}