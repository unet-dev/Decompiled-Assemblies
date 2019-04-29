using System;
using UnityEngine;

public class SkeletonScale : MonoBehaviour
{
	protected BoneInfoComponent[] bones;

	public int seed;

	public GameObject leftShoulder;

	public GameObject rightShoulder;

	public GameObject spine;

	public SkeletonScale()
	{
	}

	protected void Awake()
	{
		this.bones = base.GetComponentsInChildren<BoneInfoComponent>(true);
	}

	public void Reset()
	{
		BoneInfoComponent[] boneInfoComponentArray = this.bones;
		for (int i = 0; i < (int)boneInfoComponentArray.Length; i++)
		{
			BoneInfoComponent boneInfoComponent = boneInfoComponentArray[i];
			if (boneInfoComponent.sizeVariation != Vector3.zero)
			{
				boneInfoComponent.transform.localScale = Vector3.one;
			}
		}
	}

	public void UpdateBones(int seedNumber)
	{
		this.seed = seedNumber;
		BoneInfoComponent[] boneInfoComponentArray = this.bones;
		for (int i = 0; i < (int)boneInfoComponentArray.Length; i++)
		{
			BoneInfoComponent boneInfoComponent = boneInfoComponentArray[i];
			if (boneInfoComponent.sizeVariation != Vector3.zero)
			{
				UnityEngine.Random.State state = UnityEngine.Random.state;
				UnityEngine.Random.InitState(boneInfoComponent.sizeVariationSeed + this.seed);
				boneInfoComponent.transform.localScale = Vector3.one + (boneInfoComponent.sizeVariation * UnityEngine.Random.Range(-1f, 1f));
				UnityEngine.Random.state = state;
			}
		}
		if (this.spine != null)
		{
			Transform transforms = this.rightShoulder.transform;
			Transform transforms1 = this.leftShoulder.transform;
			Vector3 vector3 = new Vector3(1f / this.spine.transform.localScale.x, 1f / this.spine.transform.localScale.y, 1f / this.spine.transform.localScale.z);
			transforms1.localScale = vector3;
			transforms.localScale = vector3;
		}
	}
}