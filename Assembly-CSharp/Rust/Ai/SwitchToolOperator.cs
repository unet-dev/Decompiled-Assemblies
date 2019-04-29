using Apex.Serialization;
using ConVar;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class SwitchToolOperator : BaseAction
	{
		[ApexSerialization]
		private NPCPlayerApex.ToolTypeEnum ToolTypeDay;

		[ApexSerialization]
		private NPCPlayerApex.ToolTypeEnum ToolTypeNight;

		public SwitchToolOperator()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			SwitchToolOperator.TrySwitchToolTo(c as NPCHumanContext, this.ToolTypeDay, this.ToolTypeNight);
		}

		public static Item FindTool(NPCHumanContext c, NPCPlayerApex.ToolTypeEnum tool)
		{
			Item[] itemArray = c.Human.inventory.AllItems();
			for (int i = 0; i < (int)itemArray.Length; i++)
			{
				Item item = itemArray[i];
				if (item.info.category == ItemCategory.Tool)
				{
					HeldEntity heldEntity = item.GetHeldEntity() as HeldEntity;
					if (heldEntity != null && heldEntity.toolType == tool)
					{
						return item;
					}
				}
			}
			return null;
		}

		public static bool TrySwitchToolTo(NPCHumanContext c, NPCPlayerApex.ToolTypeEnum toolDay, NPCPlayerApex.ToolTypeEnum toolNight)
		{
			if (c != null)
			{
				Item item = null;
				uint human = c.Human.svActiveItemID;
				if (TOD_Sky.Instance != null)
				{
					item = (!TOD_Sky.Instance.IsDay ? SwitchToolOperator.FindTool(c, toolNight) : SwitchToolOperator.FindTool(c, toolDay));
				}
				if (item != null)
				{
					c.Human.UpdateActiveItem(item.uid);
					if (human != c.Human.svActiveItemID)
					{
						c.Human.NextToolSwitchTime = UnityEngine.Time.realtimeSinceStartup + c.Human.ToolSwitchFrequency;
						c.SetFact(NPCPlayerApex.Facts.CurrentWeaponType, 0, true, true);
						c.SetFact(NPCPlayerApex.Facts.CurrentToolType, (byte)c.Human.GetCurrentToolTypeEnum(), true, true);
					}
					return true;
				}
			}
			return false;
		}

		public class HasCurrentToolType : BaseScorer
		{
			[ApexSerialization(defaultValue=NPCPlayerApex.ToolTypeEnum.None)]
			public NPCPlayerApex.ToolTypeEnum @value;

			public HasCurrentToolType()
			{
			}

			public override float GetScore(BaseContext c)
			{
				if (c.GetFact(NPCPlayerApex.Facts.CurrentToolType) != (byte)this.@value)
				{
					return 0f;
				}
				return 1f;
			}
		}

		public class ReactiveAimsAtTarget : BaseScorer
		{
			public ReactiveAimsAtTarget()
			{
			}

			public override float GetScore(BaseContext c)
			{
				return SwitchToolOperator.ReactiveAimsAtTarget.Test(c as NPCHumanContext);
			}

			public static float Test(NPCHumanContext c)
			{
				if (c.Human == null || c.Human.transform == null || c.Human.IsDestroyed || c.Human.AttackTarget == null || c.Human.AttackTarget.transform == null || c.Human.AttackTarget.IsDestroyed)
				{
					return 0f;
				}
				Vector3 serverPosition = c.Human.AttackTarget.ServerPosition - c.Position;
				Vector3 vector3 = serverPosition.normalized;
				float single = Vector3.Dot(c.Human.eyes.BodyForward(), vector3);
				if (c.Human.isMounted)
				{
					if (single < ConVar.AI.npc_valid_mounted_aim_cone)
					{
						return 0f;
					}
					return 1f;
				}
				if (single < ConVar.AI.npc_valid_aim_cone)
				{
					return 0f;
				}
				return 1f;
			}
		}

		public class TargetVisibleFor : BaseScorer
		{
			[ApexSerialization]
			public float duration;

			public TargetVisibleFor()
			{
			}

			public override float GetScore(BaseContext c)
			{
				return SwitchToolOperator.TargetVisibleFor.Test(c as NPCHumanContext, this.duration);
			}

			public static float Test(NPCHumanContext c, float duration)
			{
				if (c == null)
				{
					return 0f;
				}
				if (c.Human.AttackTarget != null && c.Human.lastAttacker == c.Human.AttackTarget && c.Human.SecondsSinceAttacked < 10f)
				{
					return 1f;
				}
				if (c.Human.AttackTargetVisibleFor < duration)
				{
					return 0f;
				}
				return 1f;
			}
		}
	}
}