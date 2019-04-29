using System;
using UnityEngine;

namespace Rust.Ai
{
	public class ChaseTarget : BaseAction
	{
		public ChaseTarget()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			if (c.AIAgent.AttackTarget == null)
			{
				return;
			}
			if (c.AIAgent is BaseNpc)
			{
				c.AIAgent.UpdateDestination(c.AIAgent.AttackTarget.transform);
			}
		}
	}
}