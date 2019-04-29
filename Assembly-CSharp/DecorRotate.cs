using System;
using UnityEngine;

public class DecorRotate : DecorComponent
{
	public Vector3 MinRotation = new Vector3(0f, -180f, 0f);

	public Vector3 MaxRotation = new Vector3(0f, 180f, 0f);

	public DecorRotate()
	{
	}

	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 2;
		float single = SeedRandom.Range(ref num, this.MinRotation.x, this.MaxRotation.x);
		float single1 = SeedRandom.Range(ref num, this.MinRotation.y, this.MaxRotation.y);
		float single2 = SeedRandom.Range(ref num, this.MinRotation.z, this.MaxRotation.z);
		rot = Quaternion.Euler(single, single1, single2) * rot;
	}
}