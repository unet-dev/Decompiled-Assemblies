using System;

public class BaseDetector : IOEntity
{
	public PlayerDetectionTrigger myTrigger;

	public const BaseEntity.Flags Flag_HasContents = BaseEntity.Flags.Reserved1;

	public BaseDetector()
	{
	}

	public override int ConsumptionAmount()
	{
		return base.ConsumptionAmount();
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			return 0;
		}
		return base.GetPassthroughAmount(0);
	}

	public virtual void OnEmpty()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		if (this.IsPowered())
		{
			this.MarkDirty();
		}
	}

	public virtual void OnObjects()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		if (this.IsPowered())
		{
			this.MarkDirty();
		}
	}
}