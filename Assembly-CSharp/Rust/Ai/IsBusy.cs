using System;

namespace Rust.Ai
{
	public sealed class IsBusy : BaseScorer
	{
		public IsBusy()
		{
		}

		public override float GetScore(BaseContext c)
		{
			object obj;
			if (c.AIAgent.BusyTimerActive())
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			return (float)obj;
		}
	}
}