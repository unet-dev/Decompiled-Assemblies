using System;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class AimingAtPoint : WeightedScorerBase<Vector3>
	{
		public AimingAtPoint()
		{
		}

		public override float GetScore(BaseContext context, Vector3 position)
		{
			BaseContext baseContext = context;
			Vector3 entity = baseContext.Entity.transform.forward;
			Vector3 vector3 = position - baseContext.Position;
			return Vector3.Dot(entity, vector3.normalized);
		}
	}
}