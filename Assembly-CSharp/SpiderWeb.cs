using System;
using UnityEngine;

public class SpiderWeb : BaseCombatEntity
{
	public SpiderWeb()
	{
	}

	public bool Fresh()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved1) || base.HasFlag(BaseEntity.Flags.Reserved2) || base.HasFlag(BaseEntity.Flags.Reserved3))
		{
			return false;
		}
		return !base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.Fresh())
		{
			int num = UnityEngine.Random.Range(0, 4);
			BaseEntity.Flags flag = BaseEntity.Flags.Reserved1;
			if (num == 0)
			{
				flag = BaseEntity.Flags.Reserved1;
			}
			else if (num == 1)
			{
				flag = BaseEntity.Flags.Reserved2;
			}
			else if (num == 2)
			{
				flag = BaseEntity.Flags.Reserved3;
			}
			else if (num == 3)
			{
				flag = BaseEntity.Flags.Reserved4;
			}
			base.SetFlag(flag, true, false, true);
		}
	}
}