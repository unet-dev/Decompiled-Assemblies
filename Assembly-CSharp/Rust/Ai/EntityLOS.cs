using System;

namespace Rust.Ai
{
	public sealed class EntityLOS : WeightedScorerBase<BaseEntity>
	{
		public EntityLOS()
		{
		}

		public override float GetScore(BaseContext c, BaseEntity target)
		{
			if (!c.Entity.IsVisible(target.CenterPoint(), Single.PositiveInfinity))
			{
				return 0f;
			}
			return 1f;
		}
	}
}