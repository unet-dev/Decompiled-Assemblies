using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class ProximityToDanger : WeightedScorerBase<Vector3>
	{
		[ApexSerialization]
		public float Range = 20f;

		public ProximityToDanger()
		{
		}

		public override float GetScore(BaseContext c, Vector3 position)
		{
			float danger = 0f;
			for (int i = 0; i < c.Memory.All.Count; i++)
			{
				float single = Vector3.Distance(position, c.Memory.All[i].Position) / this.Range;
				single = 1f - single;
				if (single >= 0f)
				{
					danger = danger + c.Memory.All[i].Danger * single;
				}
			}
			return danger;
		}
	}
}