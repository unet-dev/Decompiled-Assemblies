using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class ProximityToPeers : WeightedScorerBase<Vector3>
	{
		[ApexSerialization(defaultValue=14f)]
		public float desiredRange = 14f;

		public ProximityToPeers()
		{
		}

		public override float GetScore(BaseContext c, Vector3 position)
		{
			float single = Single.MaxValue;
			Vector3 vector3 = Vector3.zero;
			for (int i = 0; i < c.Memory.All.Count; i++)
			{
				Memory.SeenInfo item = c.Memory.All[i];
				if (item.Entity != null)
				{
					float single1 = this.Test(item, c);
					if (single1 > 0f)
					{
						float single2 = (position - item.Position).sqrMagnitude * single1;
						if (single2 < single)
						{
							single = single2;
							vector3 = item.Position;
						}
					}
				}
			}
			if (vector3 == Vector3.zero)
			{
				return 0f;
			}
			single = Vector3.Distance(vector3, position);
			return 1f - single / this.desiredRange;
		}

		protected virtual float Test(Memory.SeenInfo memory, BaseContext c)
		{
			if (memory.Entity == null)
			{
				return 0f;
			}
			if (!(memory.Entity is BaseNpc))
			{
				return 0f;
			}
			return 1f;
		}
	}
}