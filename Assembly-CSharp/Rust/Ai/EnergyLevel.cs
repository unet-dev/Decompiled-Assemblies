using System;

namespace Rust.Ai
{
	public class EnergyLevel : BaseScorer
	{
		public EnergyLevel()
		{
		}

		public override float GetScore(BaseContext c)
		{
			return c.AIAgent.GetEnergy;
		}
	}
}