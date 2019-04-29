using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class AttackOperator : BaseAction
	{
		[ApexSerialization]
		public AttackOperator.AttackType Type;

		[ApexSerialization]
		public AttackOperator.AttackTargetType Target;

		public AttackOperator()
		{
		}

		public static void AttackEnemy(BaseContext c, AttackOperator.AttackType type)
		{
			if (c.GetFact(BaseNpc.Facts.IsAttackReady) == 0)
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
			c.SetFact(BaseNpc.Facts.IsAttackReady, 0);
		}

		public override void DoExecute(BaseContext c)
		{
			if (this.Target == AttackOperator.AttackTargetType.Enemy)
			{
				AttackOperator.AttackEnemy(c, this.Type);
			}
		}

		public enum AttackTargetType
		{
			Enemy
		}

		public enum AttackType
		{
			CloseRange,
			MediumRange,
			LongRange
		}
	}
}