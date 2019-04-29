using System;

namespace Rust.Ai
{
	public sealed class EntitySizeDifference : WeightedScorerBase<BaseEntity>
	{
		public EntitySizeDifference()
		{
		}

		public override float GetScore(BaseContext c, BaseEntity target)
		{
			float size = 1f;
			BaseNpc aIAgent = c.AIAgent as BaseNpc;
			if (aIAgent != null)
			{
				size = aIAgent.Stats.Size;
			}
			if (target is BasePlayer)
			{
				return 1f / size;
			}
			BaseNpc baseNpc = target as BaseNpc;
			if (baseNpc == null)
			{
				return 0f;
			}
			return baseNpc.Stats.Size / size;
		}
	}
}