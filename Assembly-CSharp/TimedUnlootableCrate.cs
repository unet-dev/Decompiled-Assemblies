using System;

public class TimedUnlootableCrate : LootContainer
{
	public bool unlootableOnSpawn = true;

	public float unlootableDuration = 300f;

	public TimedUnlootableCrate()
	{
	}

	public void MakeLootable()
	{
		base.SetFlag(BaseEntity.Flags.OnFire, false, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (this.unlootableOnSpawn)
		{
			this.SetUnlootableFor(this.unlootableDuration);
		}
	}

	public void SetUnlootableFor(float duration)
	{
		base.SetFlag(BaseEntity.Flags.OnFire, true, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, true, false, true);
		this.unlootableDuration = duration;
		base.Invoke(new Action(this.MakeLootable), duration);
	}
}