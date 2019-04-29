using System;
using UnityEngine;

public class DecorScale : DecorComponent
{
	public Vector3 MinScale = new Vector3(1f, 1f, 1f);

	public Vector3 MaxScale = new Vector3(2f, 2f, 2f);

	public DecorScale()
	{
	}

	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 3;
		float single = SeedRandom.Value(ref num);
		scale.x *= Mathf.Lerp(this.MinScale.x, this.MaxScale.x, single);
		scale.y *= Mathf.Lerp(this.MinScale.y, this.MaxScale.y, single);
		scale.z *= Mathf.Lerp(this.MinScale.z, this.MaxScale.z, single);
	}
}