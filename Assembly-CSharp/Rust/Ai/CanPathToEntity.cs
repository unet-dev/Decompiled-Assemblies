using System;
using UnityEngine.AI;

namespace Rust.Ai
{
	public sealed class CanPathToEntity : WeightedScorerBase<BaseEntity>
	{
		private readonly static NavMeshPath pathToEntity;

		static CanPathToEntity()
		{
			CanPathToEntity.pathToEntity = new NavMeshPath();
		}

		public CanPathToEntity()
		{
		}

		public override float GetScore(BaseContext c, BaseEntity target)
		{
			if (c.AIAgent.IsNavRunning() && c.AIAgent.GetNavAgent.CalculatePath(target.ServerPosition, CanPathToEntity.pathToEntity) && CanPathToEntity.pathToEntity.status == NavMeshPathStatus.PathComplete)
			{
				return 1f;
			}
			return 0f;
		}
	}
}