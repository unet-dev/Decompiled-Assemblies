using System;

namespace Rust.Ai
{
	public class StaminaLevel : BaseScorer
	{
		public StaminaLevel()
		{
		}

		public override float GetScore(BaseContext c)
		{
			return c.AIAgent.GetStamina;
		}
	}
}