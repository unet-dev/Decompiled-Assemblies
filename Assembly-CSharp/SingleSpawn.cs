using System;

public class SingleSpawn : SpawnGroup
{
	public SingleSpawn()
	{
	}

	public void FillDelay(float delay)
	{
		SingleSpawn singleSpawn = this;
		base.Invoke(new Action(singleSpawn.Fill), delay);
	}

	public override bool WantsInitialSpawn()
	{
		return false;
	}
}