using System;

namespace Rust.Ai
{
	public class ProximityToDangers : ProximityToPeers
	{
		public ProximityToDangers()
		{
		}

		protected override float Test(Memory.SeenInfo memory, BaseContext c)
		{
			if (memory.Entity == null)
			{
				return 0f;
			}
			return c.AIAgent.FearLevel(memory.Entity);
		}
	}
}