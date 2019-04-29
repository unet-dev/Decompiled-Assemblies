using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class MemorisedHostilePlayerTarget : ActionBase<PlayerTargetContext>
	{
		[ApexSerialization]
		public float HostilityTimeout = 10f;

		public MemorisedHostilePlayerTarget()
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
				for (int i = 0; i < baseContext.Memory.All.Count; i++)
				{
					Memory.SeenInfo item = baseContext.Memory.All[i];
					BasePlayer entity = item.Entity as BasePlayer;
					if (entity != null)
					{
						Memory.ExtendedInfo extendedInfo = baseContext.Memory.GetExtendedInfo(item.Entity);
						if (Time.time < extendedInfo.LastHurtUsTime + this.HostilityTimeout && item.Danger > danger && (item.Position - baseContext.Position).sqrMagnitude <= aggressionRange)
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