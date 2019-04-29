using Apex.Serialization;
using ConVar;
using System;

namespace Rust.Ai
{
	public class MountOperator : BaseAction
	{
		[ApexSerialization]
		public MountOperator.MountOperationType Type;

		public MountOperator()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			MountOperator.MountOperation(c as NPCHumanContext, this.Type);
		}

		public static void MountOperation(NPCHumanContext c, MountOperator.MountOperationType type)
		{
			if (type != MountOperator.MountOperationType.Mount)
			{
				if (type != MountOperator.MountOperationType.Dismount)
				{
					return;
				}
				if (c.GetFact(NPCPlayerApex.Facts.IsMounted) == 1)
				{
					c.Human.Dismount();
				}
			}
			else if (c.GetFact(NPCPlayerApex.Facts.IsMounted) == 0 && !ConVar.AI.npc_ignore_chairs)
			{
				BaseChair chairTarget = c.ChairTarget;
				if (chairTarget != null)
				{
					c.Human.Mount(chairTarget);
					return;
				}
			}
		}

		public enum MountOperationType
		{
			Mount,
			Dismount
		}
	}
}