using Apex.AI;
using Apex.Serialization;
using ConVar;
using System;
using UnityEngine;

namespace Rust.Ai
{
	[FriendlyName("Scan for Entities", "Update Context.Entities")]
	public sealed class ScanForEntities : BaseAction
	{
		public BaseEntity[] Results = new BaseEntity[64];

		[ApexSerialization]
		public int forgetTime = 10;

		public ScanForEntities()
		{
		}

		private static bool AiCaresAbout(BaseEntity ent)
		{
			if (ent is BasePlayer)
			{
				return true;
			}
			if (ent is BaseNpc)
			{
				return true;
			}
			if (ent is WorldItem)
			{
				return true;
			}
			if (ent is BaseCorpse)
			{
				return true;
			}
			return false;
		}

		public override void DoExecute(BaseContext c)
		{
			if (BaseEntity.Query.Server == null)
			{
				return;
			}
			int inSphere = BaseEntity.Query.Server.GetInSphere(c.Position, c.AIAgent.GetStats.VisionRange, this.Results, new Func<BaseEntity, bool>(ScanForEntities.AiCaresAbout));
			if (inSphere == 0)
			{
				return;
			}
			for (int i = 0; i < inSphere; i++)
			{
				BaseEntity results = this.Results[i];
				if (!(results == null) && !(results == c.Entity) && results.isServer && ScanForEntities.WithinVisionCone(c.AIAgent, results))
				{
					BasePlayer basePlayer = results as BasePlayer;
					if (basePlayer != null && !results.IsNpc)
					{
						if (!ConVar.AI.ignoreplayers)
						{
							Vector3 attackPosition = c.AIAgent.AttackPosition;
							if ((basePlayer.IsVisible(attackPosition, basePlayer.CenterPoint(), Single.PositiveInfinity) || basePlayer.IsVisible(attackPosition, basePlayer.eyes.position, Single.PositiveInfinity) ? false : !basePlayer.IsVisible(attackPosition, basePlayer.transform.position, Single.PositiveInfinity)))
							{
								goto Label0;
							}
						}
						else
						{
							goto Label0;
						}
					}
					c.Memory.Update(results, 0f);
				}
			Label0:
			}
			c.Memory.Forget((float)this.forgetTime);
		}

		private static bool WithinVisionCone(IAIAgent agent, BaseEntity other)
		{
			if (agent.GetStats.VisionCone == -1f)
			{
				return true;
			}
			BaseCombatEntity entity = agent.Entity;
			Vector3 vector3 = entity.transform.forward;
			BasePlayer basePlayer = entity as BasePlayer;
			if (basePlayer != null)
			{
				basePlayer.eyes.BodyForward();
			}
			Vector3 vector31 = other.transform.position - entity.transform.position;
			Vector3 vector32 = vector31.normalized;
			if (Vector3.Dot(entity.transform.forward, vector32) < agent.GetStats.VisionCone)
			{
				return false;
			}
			return true;
		}
	}
}