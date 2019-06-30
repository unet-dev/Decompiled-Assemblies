using System;
using UnityEngine;

public class AnimalFootIK : MonoBehaviour
{
	public Transform[] Feet;

	public Animator animator;

	public float maxWeightDistance = 0.1f;

	public float minWeightDistance = 0.025f;

	public float actualFootOffset = 0.01f;

	public AnimalFootIK()
	{
	}

	public AvatarIKGoal GoalFromIndex(int index)
	{
		if (index == 0)
		{
			return AvatarIKGoal.LeftHand;
		}
		if (index == 1)
		{
			return AvatarIKGoal.RightHand;
		}
		if (index == 2)
		{
			return AvatarIKGoal.LeftFoot;
		}
		if (index == 3)
		{
			return AvatarIKGoal.RightFoot;
		}
		return AvatarIKGoal.LeftHand;
	}

	public bool GroundSample(Vector3 origin, out RaycastHit hit)
	{
		if (Physics.Raycast(origin + (Vector3.up * 0.5f), Vector3.down, out hit, 1f, 455155969))
		{
			return true;
		}
		return false;
	}

	private void OnAnimatorIK(int layerIndex)
	{
		RaycastHit raycastHit;
		Debug.Log("animal ik!");
		for (int i = 0; i < 4; i++)
		{
			Transform feet = this.Feet[i];
			AvatarIKGoal avatarIKGoal = this.GoalFromIndex(i);
			Vector3 vector3 = Vector3.up;
			Vector3 vector31 = feet.transform.position;
			float kPositionWeight = this.animator.GetIKPositionWeight(avatarIKGoal);
			if (!this.GroundSample(feet.transform.position - (Vector3.down * this.actualFootOffset), out raycastHit))
			{
				kPositionWeight = 0f;
			}
			else
			{
				Vector3 vector32 = raycastHit.normal;
				vector31 = raycastHit.point;
				float single = Vector3.Distance(feet.transform.position - (Vector3.down * this.actualFootOffset), vector31);
				kPositionWeight = 1f - Mathf.InverseLerp(this.minWeightDistance, this.maxWeightDistance, single);
				this.animator.SetIKPosition(avatarIKGoal, vector31 + (Vector3.up * this.actualFootOffset));
			}
			this.animator.SetIKPositionWeight(avatarIKGoal, kPositionWeight);
		}
	}

	public void Start()
	{
	}
}