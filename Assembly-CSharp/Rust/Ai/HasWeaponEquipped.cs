using System;

namespace Rust.Ai
{
	public sealed class HasWeaponEquipped : BaseScorer
	{
		public HasWeaponEquipped()
		{
		}

		public override float GetScore(BaseContext c)
		{
			BasePlayer aIAgent = c.AIAgent as BasePlayer;
			if (aIAgent != null && aIAgent.GetHeldEntity() is AttackEntity)
			{
				return 1f;
			}
			return 0f;
		}
	}
}