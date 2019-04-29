using Apex.AI;
using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class BestMountedPlayerDirection : OptionScorerBase<BasePlayer>
	{
		[ApexSerialization]
		private float score = 10f;

		public BestMountedPlayerDirection()
		{
		}

		public static bool Evaluate(BasePlayer self, Vector3 optionPosition, out Vector3 dir, out float dot)
		{
			BaseMountable mounted = self.GetMounted();
			dir = (optionPosition - self.ServerPosition).normalized;
			dot = Vector3.Dot(dir, mounted.transform.forward);
			if (dot >= -0.1f)
			{
				return true;
			}
			dot = -1f;
			return false;
		}

		public override float Score(IAIContext context, BasePlayer option)
		{
			Vector3 vector3;
			float single;
			PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
			if (playerTargetContext != null)
			{
				BasePlayer self = playerTargetContext.Self as BasePlayer;
				if (self && self.isMounted && BestMountedPlayerDirection.Evaluate(self, option.ServerPosition, out vector3, out single))
				{
					playerTargetContext.Direction[playerTargetContext.CurrentOptionsIndex] = vector3;
					playerTargetContext.Dot[playerTargetContext.CurrentOptionsIndex] = single;
					return (single + 1f) * 0.5f * this.score;
				}
			}
			playerTargetContext.Direction[playerTargetContext.CurrentOptionsIndex] = Vector3.zero;
			playerTargetContext.Dot[playerTargetContext.CurrentOptionsIndex] = 0f;
			return 0f;
		}
	}
}