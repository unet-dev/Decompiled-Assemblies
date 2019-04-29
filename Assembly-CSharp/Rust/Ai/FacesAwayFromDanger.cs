using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class FacesAwayFromDanger : WeightedScorerBase<Vector3>
	{
		public FacesAwayFromDanger()
		{
		}

		public override float GetScore(BaseContext c, Vector3 position)
		{
			float single = 0f;
			Vector3 entity = c.Entity.transform.position;
			Vector3 vector3 = position - entity.normalized;
			for (int i = 0; i < c.Memory.All.Count; i++)
			{
				entity = c.Memory.All[i].Position - c.Entity.transform.position;
				single += -Vector3.Dot(vector3, entity.normalized);
			}
			return single;
		}
	}
}