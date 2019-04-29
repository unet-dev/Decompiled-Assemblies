using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class PlayerDetectionChance : OptionScorerBase<BasePlayer>
	{
		[ApexSerialization]
		private float score = 10f;

		public PlayerDetectionChance()
		{
		}

		public static bool Evaluate(IAIAgent self, float dot, BasePlayer option)
		{
			NPCPlayerApex nPCPlayerApex = self as NPCPlayerApex;
			if (nPCPlayerApex == null)
			{
				return true;
			}
			if (Time.time > nPCPlayerApex.NextDetectionCheck)
			{
				nPCPlayerApex.NextDetectionCheck = Time.time + 2f;
				bool flag = UnityEngine.Random.@value < PlayerDetectionChance.FovDetection(dot, option);
				bool flag1 = UnityEngine.Random.@value < PlayerDetectionChance.NoiseLevel(option);
				bool flag2 = UnityEngine.Random.@value < PlayerDetectionChance.LightDetection(option);
				nPCPlayerApex.LastDetectionCheckResult = flag | flag1 | flag2;
			}
			return nPCPlayerApex.LastDetectionCheckResult;
		}

		private static float FovDetection(float dot, BasePlayer option)
		{
			return (dot >= 0.75f ? 1.5f : (dot + 1f) * 0.5f) * (option.IsRunning() ? 1.5f : 1f) * (option.IsDucked() ? 0.75f : 1f);
		}

		private static float LightDetection(BasePlayer option)
		{
			bool flag = false;
			Item activeItem = option.GetActiveItem();
			if (activeItem != null)
			{
				HeldEntity heldEntity = activeItem.GetHeldEntity() as HeldEntity;
				if (heldEntity != null)
				{
					flag = heldEntity.LightsOn();
				}
			}
			if (!flag)
			{
				return 0f;
			}
			return 0.1f;
		}

		private static float NoiseLevel(BasePlayer option)
		{
			float count = (option.IsDucked() ? 0.5f : 1f);
			count = count * (option.IsRunning() ? 1.5f : 1f);
			count = count * (option.estimatedSpeed <= 0.01f ? 0.1f : 1f);
			if (option.inventory.containerWear.itemList.Count != 0)
			{
				count = count + (float)option.inventory.containerWear.itemList.Count * 0.025f;
			}
			else
			{
				count *= 0.1f;
			}
			return count;
		}

		public override float Score(IAIContext context, BasePlayer option)
		{
			PlayerTargetContext playerTargetContext = context as PlayerTargetContext;
			if (playerTargetContext == null)
			{
				return 0f;
			}
			if (!PlayerDetectionChance.Evaluate(playerTargetContext.Self, playerTargetContext.Dot[playerTargetContext.CurrentOptionsIndex], option))
			{
				return 0f;
			}
			return this.score;
		}
	}
}