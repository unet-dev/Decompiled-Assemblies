using Apex.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class MemorisedPlayerTarget : ActionBase<PlayerTargetContext>
	{
		public MemorisedPlayerTarget()
		{
		}

		public override void Execute(PlayerTargetContext context)
		{
			BaseContext baseContext = context.Self.GetContext(Guid.Empty) as BaseContext;
			if (baseContext != null)
			{
				float danger = 0f;
				BasePlayer basePlayer = null;
				Vector3 position = Vector3.zero;
				float aggressionRange = baseContext.AIAgent.GetStats.AggressionRange * baseContext.AIAgent.GetStats.AggressionRange;
				float deaggroRange = baseContext.AIAgent.GetStats.DeaggroRange * baseContext.AIAgent.GetStats.DeaggroRange;
				for (int i = 0; i < baseContext.Memory.All.Count; i++)
				{
					Memory.SeenInfo item = baseContext.Memory.All[i];
					BasePlayer entity = item.Entity as BasePlayer;
					if (entity != null)
					{
						float single = (item.Position - baseContext.Position).sqrMagnitude;
						if (item.Danger > danger && (single <= aggressionRange || baseContext.Entity.lastAttacker == entity && single <= deaggroRange))
						{
							danger = item.Danger;
							basePlayer = entity;
							position = item.Position;
						}
					}
				}
				if (basePlayer != null)
				{
					context.Target = basePlayer;
					context.Score = danger;
					context.LastKnownPosition = position;
				}
			}
		}
	}
}