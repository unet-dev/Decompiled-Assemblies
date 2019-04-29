using System;

public class NPCPlayerCorpse : PlayerCorpse
{
	private bool lootEnabled;

	public NPCPlayerCorpse()
	{
	}

	public override bool CanLoot()
	{
		return this.lootEnabled;
	}

	public void EnableLooting()
	{
		this.lootEnabled = true;
	}

	public override float GetRemovalTime()
	{
		return 60f;
	}

	public void SetLootableIn(float when)
	{
		base.Invoke(new Action(this.EnableLooting), when);
	}
}