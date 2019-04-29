using System;

namespace Rust.Ai
{
	public sealed class Eat : BaseAction
	{
		public Eat()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			c.AIAgent.Eat();
		}
	}
}