using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class HumanAttackOperator : BaseAction
	{
		[ApexSerialization]
		public AttackOperator.AttackType Type;

		[ApexSerialization]
		public AttackOperator.AttackTargetType Target;

		public HumanAttackOperator()
		{
		}

		public static void AttackEnemy(NPCHumanContext c, AttackOperator.AttackType type)
		{
			if (c.GetFact(NPCPlayerApex.Facts.IsWeaponAttackReady) == 0)
			{
				return;
			}
			BaseCombatEntity enemyNpc = null;
			if (c.EnemyNpc != null)
			{
				enemyNpc = c.EnemyNpc;
			}
			if (c.EnemyPlayer != null)
			{
				enemyNpc = c.EnemyPlayer;
			}
			if (enemyNpc == null)
			{
				return;
			}
			c.AIAgent.StartAttack(type, enemyNpc);
			c.SetFact(NPCPlayerApex.Facts.IsWeaponAttackReady, 0, true, true);
			if (UnityEngine.Random.@value < 0.1f && c.Human.OnAggro != null)
			{
				c.Human.OnAggro();
			}
		}

		public override void DoExecute(BaseContext c)
		{
			if (this.Target == AttackOperator.AttackTargetType.Enemy)
			{
				HumanAttackOperator.AttackEnemy(c as NPCHumanContext, this.Type);
			}
		}
	}
}