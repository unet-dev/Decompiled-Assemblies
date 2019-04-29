using System;
using UnityEngine;

public class DecorOffset : DecorComponent
{
	public Vector3 MinOffset = new Vector3(0f, 0f, 0f);

	public Vector3 MaxOffset = new Vector3(0f, 0f, 0f);

	public DecorOffset()
	{
	}

	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 1;
		ref float singlePointer = ref pos.x;
		singlePointer = singlePointer + scale.x * SeedRandom.Range(ref num, this.MinOffset.x, this.MaxOffset.x);
		ref float singlePointer1 = ref pos.y;
		singlePointer1 = singlePointer1 + scale.y * SeedRandom.Range(ref num, this.MinOffset.y, this.MaxOffset.y);
		ref float singlePointer2 = ref pos.z;
		singlePointer2 = singlePointer2 + scale.z * SeedRandom.Range(ref num, this.MinOffset.z, this.MaxOffset.z);
	}
}