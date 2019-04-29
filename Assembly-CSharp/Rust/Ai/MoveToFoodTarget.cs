using System;
using UnityEngine;

namespace Rust.Ai
{
	public class MoveToFoodTarget : BaseAction
	{
		public MoveToFoodTarget()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			if (c.AIAgent.FoodTarget == null)
			{
				return;
			}
			c.AIAgent.UpdateDestination(c.AIAgent.FoodTarget.transform);
		}
	}
}